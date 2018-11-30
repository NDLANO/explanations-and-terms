/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Extensions;
using ConceptsMicroservice.Utilities;
using Newtonsoft.Json;

namespace ConceptsMicroservice.Models
{
    [Table("concepts", Schema = "public")]
    public class Concept
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Column("external_id")] public int ExternalId { get; set; }

        [Column("meta")]
        [Required]
        [NoDuplicateIntValues]
        [MetaIdsMustExistInDatabase]
        [MustContainOneOfEachCategory]
        public List<int> MetaIds { get; set; }
        [Required] [Column("title")] public string Title { get; set; }
        [Required] [Column("content")] public string Content { get; set; }
        [Required] [Column("author")] public string Author { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
        [Column("source_author")] public string Source_Author { get; set; }
        [Column("deleted_by")] public string Deleted_By { get; set; }

        [Column("status_id")]
        [StatusIdExistsInDatabase]
        public int StatusId { get; set; }

        public virtual Status Status {get;set; }
        
        [NotMapped]

        public List<MetaData> Meta { get; set; }

        public static Concept DataReaderToConcept(Npgsql.NpgsqlDataReader reader)
        {
            //Get column names
            var idColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.Id, attr => attr.Name));
            var metaIdsColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, List<int>, ColumnAttribute, string>(prop => prop.MetaIds, attr => attr.Name));
            var externalIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.ExternalId, attr => attr.Name));
            var titleColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Title, attr => attr.Name));
            var contentColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Content, attr => attr.Name));
            var authorColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Author, attr => attr.Name));
            var sourceColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Source, attr => attr.Name));
            var createdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Created, attr => attr.Name));
            var updatedColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Updated, attr => attr.Name));
            var metaObjectsColumn = reader.GetOrdinal("meta_object");

            var meta = new List<MetaData>();
            try
            {
                meta = JsonConvert.DeserializeObject<List<MetaData>>(reader.GetString(metaObjectsColumn));
            }
            catch { }


            var concept = new Concept
            {
                Id = reader.GetInt32(idColumn),
                MetaIds = reader.GetFieldValue<int[]>(metaIdsColumn).ToList(),
                ExternalId = reader.GetInt32(externalIdColumn),
                Title = reader.SafeGetString(titleColumn),
                Content = reader.SafeGetString(contentColumn),
                Author = reader.SafeGetString(authorColumn),
                Source = reader.SafeGetString(sourceColumn),
                Created = reader.GetDateTime(createdColumn),
                Updated = reader.GetDateTime(updatedColumn),
                Meta = meta,
                //Status = reader.GetFieldValue<Status>(9)
            };

            return concept;
        }
    }
}

