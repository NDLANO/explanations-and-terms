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
    [Table("media_type", Schema = "public")]
    public class MediaType
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("title")] public string Title { get; set; }
        [Column("description")] public string Description { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
        [Column("language_id")] public int LanguageId { get; set; }
    }
}
