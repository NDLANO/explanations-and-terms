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
    [Table("concept_media", Schema = "public")]
    public class ConceptMedia
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("concept_id")] public int ConceptId { get; set; }
        [Column("media_id")] public int MediaId { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }

        public virtual Media Media { get; set; }
    }
}
