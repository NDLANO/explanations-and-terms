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
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Search;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace ConceptsMicroservice.Repositories
{
    public class ConceptRepository : IConceptRepository
    {
        private readonly ConceptsContext _context;
        private readonly DatabaseConfig _databaseConfig;

        private readonly Func<NpgsqlDataReader, List<Concept>> _sqlResultToListOfConceptsFunc;
        private readonly Func<NpgsqlDataReader, List<string>> _sqlResultToListOfConceptTitlesFunc;

        public ConceptRepository(ConceptsContext context, IOptions<DatabaseConfig> config)
        {
            _context = context;
            _databaseConfig = config.Value;

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
            List<Concept> result = null;
            if (searchParam == null || !searchParam.HasQuery())
            {
                result = GetAll();
                return result;
            }

            var queryHasTitle = !string.IsNullOrWhiteSpace(searchParam.Title);
            var queryHasMetaIds = searchParam.MetaIds != null &&
                                  searchParam.MetaIds.Count > 0;
            
            if (queryHasTitle && !queryHasMetaIds)
            {
                result = GetConceptsByStoredProcedure("get_concepts_by_title", searchParam.GetSqlParameters());
                return result;
            }

            if (!queryHasTitle && queryHasMetaIds)
            {
                return GetConceptsByStoredProcedure("get_concepts_by_list_of_meta_id", searchParam.GetSqlParameters());
            }
            result = GetConceptsByStoredProcedure("get_concepts_by_title_and_meta_id", searchParam.GetSqlParameters());
            if (result == null || result.Count == 0)
            {
                var sqlParameters = new List<NpgsqlParameter>();
                sqlParameters.Add(new NpgsqlParameter("concept_title", NpgsqlDbType.Varchar)
                {
                    Value = searchParam.Title
                });
                result = GetConceptsByStoredProcedure("get_concepts_by_title", sqlParameters);

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
            //RunStoredFunction("get_concepts_by_id", _sqlResultToListOfConceptTitlesFunc, sqlParameters);
            //return _context.Concepts
            //    .AsNoTracking()
            //    .Include(x => x.Status)
            //    .FirstOrDefaultWithMetadata(x => x.Id == id, _context.MetaData);
        }

        public List<Concept> GetAll()
        {
            return GetConceptsByStoredProcedure("get_concepts");
        }

        public Concept Update(Concept updated)
        {
           var concept = _context.Concepts.Update(updated);
            concept.Entity.Updated = DateTime.Now;

            _context.SaveChanges();
            return concept.Entity;
        }

        public Concept Insert(Concept inserted)
        {
            inserted.Id = 0;
            var concept = _context.Concepts.Add(inserted);
            concept.Entity.Created = DateTime.Now;
            concept.Entity.Updated = concept.Entity.Created;
            _context.SaveChanges();
            return concept.Entity;
        }

        public List<string> GetAllTitles(string language)
        {
            var sqlParameters = new List<NpgsqlParameter>();
            sqlParameters.Add(new NpgsqlParameter("languagecode", NpgsqlDbType.Varchar)
            {
                Value = language
            });
            return RunStoredFunction("get_all_concept_titles", _sqlResultToListOfConceptTitlesFunc, sqlParameters);
        }
    }
}
