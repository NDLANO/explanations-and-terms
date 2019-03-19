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
using System.Linq;
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
            mc.LanguageId = InsertLanguage(mc.Language).Id;
            
            mc.TypeGroupId = InsertTypeGroup(mc.TypeGroup).Id;

            var cat = Context.Categories.FirstOrDefault(x => x.Name == mc.Name);
            if (cat == null)
            {
                cat = Context.Categories.Add(mc).Entity;
                Context.SaveChanges();
            }
            return cat;
        }

        public Status InsertStatus(Status ms)
        {
            if (ms.LanguageId == 0)
                ms.LanguageId = InsertLanguage().Id;


            ms.TypeGroupId = InsertTypeGroup(ms.TypeGroup).Id;

            var status = Context.Status.Add(ms).Entity;
            Context.SaveChanges();
            return status;
        }


        public MetaData InsertMeta(MetaData m)
        {
            if (m.LanguageId == 0)
                m.LanguageId = InsertLanguage().Id;
            
            m.Category = InsertCategory(m.Category);
            m.CategoryId = m.Category.Id;


            if (m.Status != null && m.Status.LanguageId == 0)
                m.Status.LanguageId = InsertLanguage().Id;

            var meta = Context.MetaData.Add(m).Entity;
            Context.SaveChanges();
            return meta;
        }
        public Concept InsertConcept(Concept c)
        {
            //Nasser 26.02.2019, it seems that the languageId is equal to 1
            //if (c.LanguageId == 0)
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
                    Name = "Bokmål",
                    Description = "Description",
                    Abbreviation = "nb"
                };
            var existLanguage = Context.Languages.FirstOrDefault(lan => lan.Name == "Bokmål");
            if (existLanguage == null)
            {
                var lang = Context.Languages.Add(l).Entity;
                Context.SaveChanges();
                return lang;
            }
            else
            {
                return existLanguage;
            }
        }

        public TypeGroup InsertTypeGroup(TypeGroup tg = null)
        {
            if (tg == null)
                tg = new TypeGroup
                {
                    Name = "TypeGroup",
                    Description = "Description",
                };

            var typeGroup = Context.TypeGroups.FirstOrDefault(x => x.Name == "TypeGroup");
            if (typeGroup == null)
            {
                typeGroup = Context.TypeGroups.Add(tg).Entity;
                Context.SaveChanges();
            }

            return typeGroup;
        }

        public Concept CreateAndInsertAConcept()
        {
            Language language = null;
            var guid = Guid.Parse("C56A4180-65AA-42EC-A945-5FD21DEC0538");
            
            var category = new MetaCategory
            {
                Name = "Name",
                Description = "Description"
            };

            var status = new Status
            {
                Name = "Name",
                Description = "Description",
                LanguageVariation = Guid.NewGuid()
            };

            var meta = new MetaData
            {
                Name = "Name",
                Abbreviation = "Abb",
                Description = "Description",
                Category = category,
                Status = status,
                LanguageVariation = Guid.NewGuid()
            };

            var concept = new Concept
            {
                AuthorEmail = "AuthorEmail",
                SourceAuthor = "SourceAuthor",
                AuthorName = "AuthorName",
                Content = "Content",
                Source = "Source",
                Title = "Title",
                ExternalId = "ExternalID",
                GroupId = guid,
                LanguageVariation = guid,
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
            var tables = new List<string>
            {
                typeof(ConceptMedia).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(Concept).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(MetaData).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(MetaCategory).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(Status).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(Media).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(MediaType).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(Language).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(MetaCategory).GetClassAttributeValue((TableAttribute table) => table.Name),
                typeof(TypeGroup).GetClassAttributeValue((TableAttribute table) => table.Name)

            };

            using (var conn = new NpgsqlConnection(DatabaseConfig.ConnectionString))
            {
                conn.Open();
                tables.ForEach(table =>
                {
                    using (var cmd = new NpgsqlCommand($"DELETE FROM {table}", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                });
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