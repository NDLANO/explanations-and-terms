﻿/**
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
    [Table("status", Schema = "public")]
    public class Status : Paging
    {
        public static readonly string STATUS_ARCHVIED = "Archived";

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Column("language_id")] public int LanguageId { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("language_variation")]
        public Guid LanguageVariation { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("updated")]
        public DateTime Updated { get; set; }
        [Column("type_group_id")] public int TypeGroupId { get; set; }

        public virtual Language Language { get; set; }
        public virtual TypeGroup TypeGroup { get; set; }
    }
}
