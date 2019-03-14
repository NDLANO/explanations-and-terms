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
    [Table("type_group", Schema = "public")]
    public class TypeGroup
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("description")] public string Description { get; set; }
        [Column("created")] public DateTime Created { get; set; }
        [Column("updated")] public DateTime Updated { get; set; }
    }
}
