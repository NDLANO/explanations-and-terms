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
using Newtonsoft.Json;

namespace ConceptsMicroservice.Models.Domain
{
    /// <summary>
    /// This class needs JsonProperty to map from json to object in concept.
    /// </summary>
    [Table("media", Schema = "public")]
    public class Media
    {
        [JsonProperty("id")][Key] [Column("id")] public int Id { get; set; }
        [JsonProperty("external_id")] [Column("external_id")] public string ExternalId { get; set; }
        [JsonProperty("media_type_id")] [Column("media_type_id")] public int MediaTypeId { get; set; }
        [JsonProperty("created")] [Column("created")] public DateTime Created { get; set; }
        [JsonProperty("updated")] [Column("updated")] public DateTime Updated { get; set; }

        public virtual MediaType MediaType { get; set; }
    }
}
