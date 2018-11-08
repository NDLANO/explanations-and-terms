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

namespace ConceptsMicroservice.Models
{
    [Table("meta_categories", Schema = "public")]
    public class MetaCategory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("updated")]
        public DateTime Updated { get; set; }
    }
}
