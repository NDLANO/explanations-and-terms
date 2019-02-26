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
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace ConceptsMicroservice.Models.Domain
{
    [Table("concepts", Schema = "public")]
    public class Concept
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Column("external_id")] public string ExternalId { get; set; }

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
        [Column("language_id")] public int LanguageId { get; set; }


        public virtual Status Status {get;set; }
        public virtual Language Language { get; set; }
        [NotMapped] public List<MetaData> Meta { get; set; }
        [NotMapped] public List<Media> Media { get; set; }
        [Column("language_variation")] public string  LanguageVariation { get; set; }
        [Column("group_id")] public string GroupId { get; set; }
        public static T GetJson<T>(Npgsql.NpgsqlDataReader reader, int column)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(reader.GetString(column));
            }
            catch
            {
                return default(T);
            }
        }

       
        [Column("number_of_total_pages")]
        [NotMapped]
        public int NumberOfPages { get; set; }

        [Column("total_number_of_items")]
        [NotMapped]
        public int TotalItems { get; set; }

        public static Concept DataReaderToConcept(Npgsql.NpgsqlDataReader reader)
        {
            //Get column names
            var idColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.Id, attr => attr.Name));
            var metaIdsColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, List<int>, ColumnAttribute, string>(prop => prop.MetaIds, attr => attr.Name));
            var externalIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.ExternalId, attr => attr.Name));
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
            var languageIdColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, int, ColumnAttribute, string>(prop => prop.LanguageId, attr => attr.Name));
            var mediaIdsColumn = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, List<int>, ColumnAttribute, string>(prop => prop.MediaIds, attr => attr.Name));
            var metaObjectsColumn = reader.GetOrdinal("meta_object");
            var mediaObjectsColumn = reader.GetOrdinal("media_object");
            var statusObjectColumn = reader.GetOrdinal("status_object");
            var languageObjectColumn = reader.GetOrdinal("language_object");
            var languageVariation = reader.GetOrdinal(AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(prop => prop.LanguageVariation,attr => attr.Name));
            var groupId =
                reader.GetOrdinal(
                    AttributeHelper.GetPropertyAttributeValue<Concept, string, ColumnAttribute, string>(
                        prop => prop.GroupId, attr => attr.Name));
            var meta = GetJson<List<MetaData>>(reader, metaObjectsColumn);
            var media = GetJson<List<Media>>(reader, mediaObjectsColumn);
            var status = GetJson<Status>(reader, statusObjectColumn);
            var language = GetJson<Language>(reader, languageObjectColumn);


            var concept = new Concept
            {
                Id = reader.GetInt32(idColumn),
                MetaIds = reader.GetFieldValue<int[]>(metaIdsColumn).ToList(),
                MediaIds = reader.GetFieldValue<int[]>(mediaIdsColumn).ToList(),
                ExternalId = reader.SafeGetString(externalIdColumn),
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
                Meta = meta ?? new List<MetaData>(),
                Media = media ?? new List<Media>(),
                LanguageId = reader.GetInt16(languageIdColumn),
                Status = status ?? new Status(),
                Language = language ?? new Language(),
                LanguageVariation = reader.SafeGetString(languageVariation),
                GroupId = reader.SafeGetString(groupId)
            };


            try
            {
                var numberOfPages = reader.GetOrdinal("page_count");
                concept.NumberOfPages = reader.GetInt16(numberOfPages);
            }
            catch { }
            try
            {
                var totalCount = reader.GetOrdinal("result_count");
                concept.TotalItems = reader.GetInt16(totalCount);
            }
            catch { }

            return concept;
        }
    }
}

