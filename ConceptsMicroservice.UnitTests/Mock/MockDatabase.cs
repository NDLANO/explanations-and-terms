/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ConceptsMicroservice.UnitTests.Mock
{
    public class MockDatabase : IMockDatabase
    {
        public ConceptsContext Context { get; set; }
        public IDatabaseConfig DatabaseConfig { get; set; }

        public MockDatabase()
        {
            DatabaseConfig = new DatabaseConfig();
            var options = new DbContextOptionsBuilder<ConceptsContext>()
                .UseNpgsql(DatabaseConfig.GetConnectionString())
                .Options;

            Context = new ConceptsContext(options);
        }

        public MetaCategory InsertCategory(MetaCategory mc)
        {
            var cat = Context.Categories.Add(mc).Entity;
            Context.SaveChanges();
            return cat;
        }

        public Status InsertStatus(Status ms)
        {
            var status = Context.Status.Add(ms).Entity;
            Context.SaveChanges();
            return status;
        }


        public MetaData InsertMeta(MetaData m)
        {
            var meta = Context.MetaData.Add(m).Entity;
            Context.SaveChanges();
            return meta;
        }
        public Concept InsertConcept(Concept c)
        {
            var concept = Context.Concepts.Add(c).Entity;
            Context.SaveChanges();
            return concept;
        }

        public Concept CreateAndInsertAConcept()
        {
            var category = new MetaCategory
            {
                Name = "Name",
                Description = "Description"
            };

            var status = new Status
            {
                Name = "Name",
                Description = "Description"
            };

            var meta = new MetaData
            {
                Name = "Name",
                Abbreviation = "Abb",
                Description = "Description",
                Category = category,
                Status = status
            };

            var concept = new Concept
            {
                Author = "Author",
                Content = "Content",
                Source = "Source",
                Title = "Title",
            };

            category = InsertCategory(category);
            status = InsertStatus(status);
            meta.Category = category;
            meta.Status = status;
            meta = InsertMeta(meta);

            concept.Meta = new List<MetaData> { meta };
            concept.MetaIds = new List<int> { meta.Id };
            concept.Status = status;
            concept = InsertConcept(concept);

            return concept;
        }

        public void DeleteAllRowsInAllTables()
        {
            var conceptTableName = typeof(Concept).GetClassAttributeValue((TableAttribute table) => table.Name);
            var metaTableName = typeof(MetaData).GetClassAttributeValue((TableAttribute table) => table.Name);
            var categoryTableName = typeof(MetaCategory).GetClassAttributeValue((TableAttribute table) => table.Name);
            var statusTableName = typeof(Status).GetClassAttributeValue((TableAttribute table) => table.Name);

            using (var conn = new NpgsqlConnection(DatabaseConfig.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand($"DELETE FROM {conceptTableName}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand($"DELETE FROM {metaTableName}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand($"DELETE FROM {categoryTableName}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand($"DELETE FROM {statusTableName}", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }


        public void Dispose()
        {
            DeleteAllRowsInAllTables();

            Context?.Dispose();
        }
    }
}