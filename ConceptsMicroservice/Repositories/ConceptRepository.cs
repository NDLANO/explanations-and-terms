/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace ConceptsMicroservice.Repositories
{
    public class ConceptRepository : IConceptRepository
    {
        private readonly Context.ConceptsContext _context;
        private readonly DatabaseConfig _databaseConfig;
        private readonly LanguageConfig _languageConfig;

        private readonly Func<NpgsqlDataReader, List<Concept>> _sqlResultToListOfConceptsFunc;
        private readonly Func<NpgsqlDataReader, List<string>> _sqlResultToListOfConceptTitlesFunc;

        public ConceptRepository(Context.ConceptsContext context, IOptions<DatabaseConfig> config, IOptions<LanguageConfig> language)
        {
            _context = context;
            _databaseConfig = config.Value;
            _languageConfig = language.Value;
            _sqlResultToListOfConceptsFunc = reader =>
            {
                var concepts = new List<Concept>();
                if (reader == null)
                    return concepts;
                while (reader.Read())
                {
                    concepts.Add(Concept.DataReaderToConcept(reader));
                }

                return concepts;
            };

            _sqlResultToListOfConceptTitlesFunc = reader =>
            {
                var titles = new List<string>();
                if (reader == null)
                    return titles;
                while (reader.Read())
                {
                    titles.Add(reader.GetString(0));
                }

                return titles;
            };
        }
        #region Search helpers

        private T RunStoredFunction<T>(string procedure, Func<NpgsqlDataReader, T> mapColumns, List<Npgsql.NpgsqlParameter> paramsNpgsqlParameters = null)
        {
            if (paramsNpgsqlParameters == null)
                paramsNpgsqlParameters = new List<NpgsqlParameter>();

            using (var connection = new Npgsql.NpgsqlConnection(_databaseConfig.ConnectionString))
            {
                connection.Open();

                using (var command = new Npgsql.NpgsqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in paramsNpgsqlParameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    command.Prepare();
                    return mapColumns(command.ExecuteReader());
                }
            }
        }

        private List<Concept> GetConceptsByStoredProcedure(string procedure, List<Npgsql.NpgsqlParameter> paramsNpgsqlParameters = null)
        {
            return RunStoredFunction(procedure, _sqlResultToListOfConceptsFunc, paramsNpgsqlParameters);
        }
        #endregion

        public List<Concept> SearchForConcepts(ConceptSearchQuery searchParam)
        {

            if (searchParam == null)
            {
                return GetAll(BaseListQuery.DefaultValues(_languageConfig.Default));
            }
            var queryHasTitle = !string.IsNullOrWhiteSpace(searchParam.Title);
            var queryHasMetaIds = searchParam.MetaIds != null &&
                                  searchParam.MetaIds.Count > 0;
            var sqlParameters = searchParam.GetSqlParameters();
            if (queryHasTitle && !queryHasMetaIds)
            {
                return GetConceptsByStoredProcedure("get_concepts_by_title", sqlParameters);
            }

            if (!queryHasTitle && queryHasMetaIds)
            {
                return GetConceptsByStoredProcedure("get_concepts_by_list_of_meta_id", sqlParameters);
            }
            if (!queryHasMetaIds && !queryHasTitle)
                return GetAll(BaseListQuery.DefaultValues(_languageConfig.Default));

            // Has metaIds and title
            var result = GetConceptsByStoredProcedure("get_concepts_by_title_and_meta_id", sqlParameters);

            // Did not find any results with metaIds. Tries with title only
            if (result == null || result.Count == 0)
            {
                sqlParameters.ForEach(x => x.Collection = null);
                sqlParameters.RemoveAll(x => x.ParameterName == "list_of_meta_id");
                return GetConceptsByStoredProcedure("get_concepts_by_title", sqlParameters);
            }
            return result;
        }

        public Concept GetById(int id)
        {
            var sqlParameters = new List<NpgsqlParameter>();
            sqlParameters.Add(new NpgsqlParameter("concept_id", NpgsqlDbType.Integer)
            {
                Value = id
            });
            return GetConceptsByStoredProcedure("get_concepts_by_id", sqlParameters).FirstOrDefault();
        }

        public List<Concept> GetAll(BaseListQuery query)
        {
            var sqlParameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("number_of_record_to_show", NpgsqlDbType.Integer) {Value = query.PageSize},
                new NpgsqlParameter("page_number", NpgsqlDbType.Integer) {Value = query.Page},
                new NpgsqlParameter("language_param", NpgsqlDbType.Varchar) {Value = query.Language},
                new NpgsqlParameter("default_language_param", NpgsqlDbType.Varchar)
                {
                    Value = _languageConfig.Default
                }
            };
            return GetConceptsByStoredProcedure("get_concepts", sqlParameters);
        }

        public Concept Update(Concept updatedConcept)
        {

            if (_context.Entry(updatedConcept).State == EntityState.Detached)
                _context.Concepts.Update(updatedConcept);
            else
                _context.Entry(updatedConcept).State = EntityState.Modified;

            updatedConcept.Updated = DateTime.Now;

            _context.SaveChanges();
            return updatedConcept;
        }

        public Concept Insert(Concept inserted)
        {
            inserted.Id = 0;
            var concept = _context.Concepts.Add(inserted);
            concept.Entity.Created = DateTime.Now;
            concept.Entity.Updated = concept.Entity.Created;
            _context.SaveChanges();
            return GetById(concept.Entity.Id);
        }
    }
}
