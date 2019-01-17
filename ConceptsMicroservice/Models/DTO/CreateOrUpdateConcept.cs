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
using ConceptsMicroservice.Attributes;

namespace ConceptsMicroservice.Models.DTO
{
    public class CreateOrUpdateConcept
    {
            public int Id { get; set; }
            public int ExternalId { get; set; }
            [Required]
            [NoDuplicateIntValues]
            [MetaIdsMustExistInDatabase]
            [MustContainOneOfEachCategory]
            public List<int> MetaIds { get; set; }
            [Required] public string Title { get; set; }
            [Required] public string Content { get; set; }
            /// <summary>
            /// The name of the author who wrote the text
            /// </summary>
            [Required] public string SourceAuthor { get; set; }
            /// <summary>
            /// The email of who created this concept.
            /// </summary>
            public string AuthorEmail { get; set; }
            /// <summary>
            /// The name of who created this concept.
            /// </summary>
            public string AuthorName { get; set; }
            /// <summary>
            /// Where the content originated
            /// </summary>
            public string Source { get; set; }
            public DateTime Created { get; set; }
            public DateTime Updated { get; set; }
            public string DeletedBy { get; set; }
            [StatusIdExistsInDatabase]
            public int StatusId { get; set; }
            /// <summary>
            /// List of media objects. I.E Image, audio, video
            /// </summary>
            public List<MediaWithMediaType> Media { get; set; } = new List<MediaWithMediaType>();
        }
}
