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
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
        /// <summary>
        /// Is id possible to have more then one meta of this category.
        /// </summary>
        [Column("can_have_multiple")] public bool CanHaveMultiple { get; set; }
        /// <summary>
        /// Is it required to have at least one meta of this category.
        /// </summary>
        [Column("is_required")] public bool IsRequired { get; set; }
        [Column("language_id")] public int LanguageId { get; set; }

        public virtual Language Language { get; set; }
    }
}
