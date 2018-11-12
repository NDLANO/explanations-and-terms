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
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using NpgsqlTypes;

namespace ConceptsMicroservice.Repositories
{
    public class ConceptRepository : IConceptRepository
    {
        private readonly ConceptsContext _context;
        private readonly IDatabaseConfig _databaseConfig;

        private readonly Func<NpgsqlDataReader, List<Concept>> _sqlResultToListOfConceptsFunc;
        private readonly Func<NpgsqlDataReader, List<string>> _sqlResultToListOfConceptTitlesFunc;

        public ConceptRepository(ConceptsContext context, IDatabaseConfig config)
        {
            _context = context;
            _databaseConfig = config;

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

            using (var connection = new Npgsql.NpgsqlConnection(_databaseConfig.GetConnectionString()))
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
            if (searchParam == null || !searchParam.HasQuery())
            {
                return GetAll();
            }

            var queryHasTitle = !string.IsNullOrWhiteSpace(searchParam.Title);
            var queryHasMetaIds = searchParam.MetaIds != null &&
                                  searchParam.MetaIds.Count > 0;
            
            if (queryHasTitle && !queryHasMetaIds)
            {
                return GetConceptsByStoredProcedure("get_concepts_by_title", searchParam.GetSqlParameters());
            }

            if (!queryHasTitle && queryHasMetaIds)
            {
                return GetConceptsByStoredProcedure("get_concepts_by_list_of_meta_id", searchParam.GetSqlParameters());
            }

            var result = GetConceptsByStoredProcedure("get_concepts_by_title_and_meta_id", searchParam.GetSqlParameters());
            if (result == null || result.Count == 0)
            {
                result = GetConceptsByStoredProcedure("get_concepts_by_title", searchParam.GetSqlParameters());
            }

            return result;
        }

        public Concept GetById(int id)
        {
            return _context.Concepts
                .AsNoTracking()
                .Include(x => x.Status)
                .FirstOrDefaultWithMetadata(x => x.Id == id, _context.MetaData);
        }

        public List<Concept> GetAll()
        {
            return GetConceptsByStoredProcedure("get_concepts");
        }

        public Concept Update(Concept updated, bool isMajorVersion=false)
        {
            var nextVersionNumber = GetHighestVersionsNumber("get_next_version_number", updated.GroupId, isMajorVersion);
            updated.VersionNumber = nextVersionNumber;
            var concept = _context.Concepts.Update(updated);
            concept.Entity.Updated = DateTime.Now;

            _context.SaveChanges();
            return concept.Entity;
        }

        private string GetHighestVersionsNumber(string procedureName, Guid groupByConceptsId, bool nextMajor)
        {
            var result = "";
            using (var connection = new Npgsql.NpgsqlConnection(_databaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var command = new Npgsql.NpgsqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new NpgsqlParameter("grouped_by", NpgsqlDbType.Uuid)).Value = groupByConceptsId;
                    command.Parameters.Add(new NpgsqlParameter("next_major", NpgsqlDbType.Boolean)).Value = nextMajor;
                    command.Prepare();
                    result = (string)command.ExecuteScalar();
                }
            }
            return result;
        }
        public Concept Insert(Concept inserted, bool isMajorVersion=false)
        {
            if (inserted.Id > 0)
            {
                var nextVersionNumber = GetHighestVersionsNumber("get_next_version_number", inserted.GroupId, isMajorVersion);
                inserted.VersionNumber = nextVersionNumber;
                inserted.Id = -1;
            }
            else if (inserted.Id == 0)
            {
                inserted.VersionNumber = "0.1";
            }

            var concept = _context.Concepts.Add(inserted);
            concept.Entity.Updated = DateTime.Now;
            _context.SaveChanges();
            return concept.Entity;
        }

        public List<string> GetAllTitles()
        {
            return RunStoredFunction("get_all_concept_titles", _sqlResultToListOfConceptTitlesFunc);
        }
    }
}
