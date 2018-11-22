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
using Newtonsoft.Json;

namespace ConceptsMicroservice.Models
{
    [Table("concepts", Schema = "public")]
    public class Concept
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Column("external_id")] public int ExternalId { get; set; }
        [Column("meta")] public List<int> MetaIds { get; set; }

        [Required]
        [NotMapped]
        [ConceptMustContainMeta("Language", "Licence")]

        public List<MetaData> Meta { get; set; }
        [Required] [Column("title")] public string Title { get; set; }
        [Required] [Column("content")] public string Content { get; set; }
        [Required] [Column("author")] public string Author { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
        [StatusIdExistsInDatabase][Column("status_id")] public int StatusId { get; set; }
        public virtual Status Status {get;set;}

        public static Concept DataReaderToConcept(Npgsql.NpgsqlDataReader reader)
        {
            var meta = new List<MetaData>();
            try
            {
                meta = JsonConvert.DeserializeObject<List<MetaData>>(reader.GetString(9));
            }
            catch { }

            var concept = new Concept
            {
                Id = reader.GetInt32(0),
                MetaIds = reader.GetFieldValue<int[]>(1).ToList(),
                ExternalId = reader.GetInt32(2),
                Title = reader.GetString(3),
                Content = reader.GetString(4),
                Author = reader.GetString(5),
                Source = reader.GetString(6),
                Created = reader.GetDateTime(7),
                Updated = reader.GetDateTime(8),
                Meta = meta,
                //Status = reader.GetFieldValue<Status>(9)
            };

            return concept;
        }
    }
}

