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

namespace ConceptsMicroservice.Models.Domain
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
        /// <summary>
        /// The name of the author who wrote the text
        /// </summary>
        [Required] [Column("source_author")] public string SourceAuthor { get; set; }
        /// <summary>
        /// The email of who created this concept.
        /// </summary>
        [Column("author_email")] public string AuthorEmail { get; set; }
        /// <summary>
        /// The name of who created this concept.
        /// </summary>
        [Column("author_name")] public string AuthorName { get; set; }
        /// <summary>
        /// Where the content originated
        /// </summary>
        [Column("source")] public string Source { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
        [Column("deleted_by")] public string DeletedBy { get; set; }
        [Column("media")] public List<int> MediaIds { get; set; } = new List<int>();
        [Column("status_id")]
        [StatusIdExistsInDatabase]
        public int StatusId { get; set; }
        
        public virtual Status Status {get;set; }
        [NotMapped] public List<MetaData> Meta { get; set; }
        [NotMapped] public List<ConceptMedia> Media { get; set; }

        public static List<T> GetJsonList<T>(Npgsql.NpgsqlDataReader reader, int column)
        {
            var member = new List<T>();
            try
            {
                member = JsonConvert.DeserializeObject<List<T>>(reader.GetString(column));
            }
            catch
            {
                // ignored
            }

            return member;
        }

        public static Concept DataReaderToConcept(Npgsql.NpgsqlDataReader reader)
        {
            //Get column names
            var idColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.Id, attr => attr.Name));
            var metaIdsColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, List<int>, ColumnAttribute, string>(prop => prop.MetaIds, attr => attr.Name));
            var externalIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.ExternalId, attr => attr.Name));
            var titleColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Title, attr => attr.Name));
            var contentColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Content, attr => attr.Name));
            var authorNameColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.AuthorName, attr => attr.Name));
            var authorEmailColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.AuthorEmail, attr => attr.Name));
            var sourceAuthorColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.SourceAuthor, attr => attr.Name));
            var sourceColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.Source, attr => attr.Name));
            var createdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Created, attr => attr.Name));
            var updatedColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, DateTime, ColumnAttribute, string>(prop => prop.Updated, attr => attr.Name));
            var statusIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.StatusId, attr => attr.Name));
            var deletedByColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.DeletedBy, attr => attr.Name));
            var metaObjectsColumn = reader.GetOrdinal("meta_object");
            var mediaObjectsColumn = reader.GetOrdinal("media_object");

            var concept = new Concept
            {
                Id = reader.GetInt32(idColumn),
                MetaIds = reader.GetFieldValue<int[]>(metaIdsColumn).ToList(),
                MediaIds = reader.GetFieldValue<int[]>(metaIdsColumn).ToList(),
                ExternalId = reader.GetInt32(externalIdColumn),
                Title = reader.SafeGetString(titleColumn),
                Content = reader.SafeGetString(contentColumn),
                AuthorName = reader.SafeGetString(authorNameColumn),
                AuthorEmail = reader.SafeGetString(authorEmailColumn),
                Source = reader.SafeGetString(sourceColumn),
                SourceAuthor = reader.SafeGetString(sourceAuthorColumn),
                Created = reader.GetDateTime(createdColumn),
                Updated = reader.GetDateTime(updatedColumn),
                StatusId = reader.GetInt32(statusIdColumn),
                DeletedBy = reader.SafeGetString(deletedByColumn),
                Meta = GetJsonList<MetaData>(reader, metaObjectsColumn),
                Media = GetJsonList<ConceptMedia>(reader, mediaObjectsColumn),
            };

            return concept;
        }
    }
}

