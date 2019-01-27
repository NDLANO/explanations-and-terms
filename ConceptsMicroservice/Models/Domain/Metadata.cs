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

namespace ConceptsMicroservice.Models.Domain
{
    [Table("meta", Schema = "public")]
    public class MetaData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("category_id")]
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
    }
}
