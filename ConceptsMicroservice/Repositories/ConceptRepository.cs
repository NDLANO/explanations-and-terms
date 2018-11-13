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
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        public double GetNextVersionNumber(double current, bool isMajorVersion, Guid groupId)
        {
            var highestVersion = _context.Concepts
                .Where(x => x.GroupId == groupId)
                .ToList()
                .Aggregate((i1, i2) => i1.VersionNumber > i2.VersionNumber ? i1 : i2);

            

            var newVersionNumber =  highestVersion.VersionNumber + 0.01;
            if (isMajorVersion)
            {
                newVersionNumber =  Math.Floor(highestVersion.VersionNumber + 1);
            }

            return Math.Round(newVersionNumber, 2, MidpointRounding.AwayFromZero);
        }

        public Concept Update(Concept updated, bool isMajorVersion=false)
        {
            updated.VersionNumber = GetNextVersionNumber(updated.VersionNumber, isMajorVersion, updated.GroupId);

            var concept = _context.Concepts.Update(updated);
            concept.Entity.Updated = DateTime.Now;

            _context.SaveChanges();
            return concept.Entity;
        }
        
        public Concept Insert(Concept inserted, bool isMajorVersion=false)
        {
            if (inserted.Id > 0)
            {
                inserted.VersionNumber = GetNextVersionNumber(inserted.VersionNumber, isMajorVersion, inserted.GroupId);
                inserted.Id = default(int);
            }
            else if (inserted.Id == 0)
            {
                inserted.VersionNumber = 0.1;
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
