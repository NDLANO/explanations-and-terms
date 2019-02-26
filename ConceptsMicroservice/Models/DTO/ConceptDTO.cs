/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using ConceptsMicroservice.Models.Domain;

namespace ConceptsMicroservice.Models.DTO
{
    public class ConceptDto
    {
        public int Id { get; set; }

        public string ExternalId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// The name of the author who wrote the text
        /// </summary>
        public string SourceAuthor { get; set; }
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
        public Status Status { get; set; }
        public List<MetaData> Meta { get; set; }
        public List<Media> Media { get; set; }
        public Language Language { get; set; }
        public string LanguageVariation { get; set; }
        public string GroupId { get; set; }
    }
}
