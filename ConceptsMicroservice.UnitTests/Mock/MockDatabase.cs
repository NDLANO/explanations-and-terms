/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ConceptsMicroservice.UnitTests.Mock
{
    public class MockDatabase : IMockDatabase
    {
        public ConceptsContext Context { get; set; }
        public DatabaseConfig DatabaseConfig { get; set; }

        public MockDatabase()
        {
            DatabaseConfig = ConfigHelper.GetDatabaseConfiguration();
            var options = new DbContextOptionsBuilder<ConceptsContext>()
                .UseNpgsql(DatabaseConfig.ConnectionString)
                .Options;

            Context = new ConceptsContext(options);
        }

        public MetaCategory InsertCategory(MetaCategory mc)
        {
            if (mc.LanguageId == 0)
                mc.LanguageId = InsertLanguage().Id;
            

            var cat = Context.Categories.Add(mc).Entity;
            Context.SaveChanges();
            return cat;
        }

        public Status InsertStatus(Status ms)
        {
            if (ms.LanguageId == 0)
                ms.LanguageId = InsertLanguage().Id;
            var status = Context.Status.Add(ms).Entity;
            Context.SaveChanges();
            return status;
        }


        public MetaData InsertMeta(MetaData m)
        {
            if (m.LanguageId == 0)
                m.LanguageId = InsertLanguage().Id;

            if (m.Category != null && m.Category.LanguageId == 0)
                m.Category.LanguageId = InsertLanguage().Id;


            if (m.Status != null && m.Status.LanguageId == 0)
                m.Status.LanguageId = InsertLanguage().Id;

            var meta = Context.MetaData.Add(m).Entity;
            Context.SaveChanges();
            return meta;
        }
        public Concept InsertConcept(Concept c)
        {
            if (c.LanguageId == 0)
                c.LanguageId = InsertLanguage().Id;


            if (c.Status != null && c.Status.LanguageId == 0)
                c.Status.LanguageId = InsertLanguage().Id;
            var concept = Context.Concepts.Add(c).Entity;
            Context.SaveChanges();
            return concept;
        }
        public Language InsertLanguage(Language l = null)
        {
            if (l == null)
                l = new Language
                {
                    Name = $"Bokmål {Guid.NewGuid()}",
                    Description = "Description",
                    Abbreviation = "nb"
                };

            var lang = Context.Languages.Add(l).Entity;
            Context.SaveChanges();
            return lang;
        }

        public Concept CreateAndInsertAConcept()
        {
            var language = new Language
            {
                Name = $"Bokmål {Guid.NewGuid()}",
                Description = "Description",
                Abbreviation = "nb"
            };

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
                AuthorEmail = "AuthorEmail",
                SourceAuthor = "SourceAuthor",
                AuthorName = "AuthorName",
                Content = "Content",
                Source = "Source",
                Title = "Title",
                MediaIds = new List<int>()
            };

            language = InsertLanguage(language);
            category.LanguageId = language.Id;
            status.LanguageId = language.Id;
            meta.LanguageId = language.Id;
            concept.LanguageId = language.Id;

            category = InsertCategory(category);

            status = InsertStatus(status);

            meta.Category = category;
            meta.Status = status;
            meta = InsertMeta(meta);

            concept.MediaIds = new List<int>();
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

            using (var conn = new NpgsqlConnection(DatabaseConfig.ConnectionString))
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