/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Utilities;

namespace ConceptsMicroservice.Models.Domain
{
    [Table("meta", Schema = "public")]
    public class MetaData : Paging
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("categoryId")]
        public int CategoryId { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("abbreviation")]
        public string Abbreviation { get; set; }
        [Column("status_id")]
        public int StatusId { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("updated")]
        public DateTime Updated { get; set; }
        [Column("language_id")] public int LanguageId { get; set; }



        public virtual Language Language { get; set; }
        public virtual MetaCategory Category { get; set; }
        public virtual Status Status { get; set; }

       

        public static MetaData DataReaderToConcept(Npgsql.NpgsqlDataReader reader)
        {
            //Get column names
            var idColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, int, ColumnAttribute, string>(prop => prop.Id, attr => attr.Name));
            var statusIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, int, ColumnAttribute, string>(prop => prop.StatusId, attr => attr.Name));
            var categoryIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, int, ColumnAttribute, string>(prop => prop.CategoryId, attr => attr.Name));
            var nameColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, string, ColumnAttribute, string>(prop => prop.Name, attr => attr.Name));
            var descriptionColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, string, ColumnAttribute, string>(prop => prop.Description, attr => attr.Name));
            var abbreviationColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<MetaData, string, ColumnAttribute, string>(prop => prop.Abbreviation, attr => attr.Name));
            var createdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Created, attr => attr.Name));
            var updatedColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Updated, attr => attr.Name));
            var languageIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.LanguageId, attr => attr.Name));
            var statusObjectColumn = reader.GetOrdinal("status_object");
            var languageObjectColumn = reader.GetOrdinal("language_object");
            var categoryObjectColumn = reader.GetOrdinal("category_object");
            

            var category = JsonHelper.GetJson<MetaCategory>(reader, categoryObjectColumn);
            var status = JsonHelper.GetJson<Status>(reader, statusObjectColumn);
            var language = JsonHelper.GetJson<Language>(reader, languageObjectColumn);


            var meta = new MetaData
            {
                Id = reader.GetInt32(idColumn),
                Created = reader.GetDateTime(createdColumn),
                Updated = reader.GetDateTime(updatedColumn),
                StatusId = reader.GetInt32(statusIdColumn),
                CategoryId = reader.GetInt32(categoryIdColumn),
                LanguageId = reader.GetInt16(languageIdColumn),
                Name = reader.SafeGetString(nameColumn),
                Description = reader.SafeGetString(descriptionColumn),
                Abbreviation = reader.SafeGetString(abbreviationColumn),
                Status = status ?? new Status(),
                Language = language ?? new Language(),
                Category = category ?? new MetaCategory(),
            };


            try
            {
                var numberOfPages = reader.GetOrdinal("page_count");
                meta.NumberOfPages = reader.GetInt16(numberOfPages);
            }
            catch { }
            try
            {
                var totalCount = reader.GetOrdinal("result_count");
                meta.TotalItems = reader.GetInt16(totalCount);
            }
            catch { }

            return meta;
        }
    }
}
