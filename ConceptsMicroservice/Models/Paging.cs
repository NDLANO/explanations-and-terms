/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations.Schema;

namespace ConceptsMicroservice.Models
{
    public class Paging
    {
        [Column("number_of_total_pages")]
        [NotMapped]
        public int NumberOfPages { get; set; }

        [Column("total_number_of_items")]
        [NotMapped]
        public int TotalItems { get; set; }
    }
}
