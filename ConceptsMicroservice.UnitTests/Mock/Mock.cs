﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;

namespace ConceptsMicroservice.UnitTests.Mock
{

    public class Mock : IMock
    {
        public IMockDatabase Database { get; set; }

        public Mock()
        {
            Database = new MockDatabase();
        }

        public MetaCategory MockCategory(string n = null)
        {
            var name = $"Category_{Guid.NewGuid()}";
            if (!string.IsNullOrWhiteSpace(n))
                name = n;

            return new MetaCategory
            {
                Name = name,
                Description = "Description"
            };
        }

        public StatusDto MockStatus(string n = null)
        {
            var name = $"Status_{Guid.NewGuid()}";
            if (!string.IsNullOrWhiteSpace(n))
                name = n;
            return new StatusDto
            {
                Name = name,
                Description = "Description"
            };
        }

        public MetaData MockMeta(StatusDto s, MetaCategory c)
        {
            var category = c;
            if (c == null)
                category = MockCategory();

            var status = s;
            if (s == null)
                status = MockStatus();

            return new MetaData
            {
                Name = $"Meta_{Guid.NewGuid()}",
                Description = "Description",
                Status = status,
                Category = category
            };
        }

        public Concept MockConcept(StatusDto status, List<MetaData> m = null, List<Media> me = null)
        {
            var meta = new List<MetaData>();
            if (m != null)
                meta = m;

            var media = new List<Media>();
            if (me != null)
                media = me;
            return new Concept
            {
                Source = $"Source_{Guid.NewGuid()}",
                Title = $"Title_{Guid.NewGuid()}",
                AuthorName = $"AuthorName_{Guid.NewGuid()}",
                AuthorEmail = $"AuthorEmail_{Guid.NewGuid()}",
                SourceAuthor = $"SourceAuthor_{Guid.NewGuid()}",
                Content = $"Content_{Guid.NewGuid()}",
                Meta = meta,
                MediaIds = new List<int>(),
                MetaIds = meta.Select(x => x.Id).ToList(),
                Status = status,
                Media = media
            };
        }

        public UpdateConceptDto MockUpdateConceptDto(List<int> meta = null, List<MediaWithMediaType> media = null)
        {
            var metaIds = meta ?? new List<int>();
            var mediaIds = media ?? new List<MediaWithMediaType>();
            return new UpdateConceptDto
            {
                Source = $"Source_{Guid.NewGuid()}",
                Title = $"Title_{Guid.NewGuid()}",
                SourceAuthor = $"SourceAuthor_{Guid.NewGuid()}",
                Content = $"Content_{Guid.NewGuid()}",
                MetaIds = metaIds,
                Media = mediaIds
            };
        }

        public CreateConceptDto MockCreateOrUpdateConcept()
        {
            return new CreateConceptDto()
            {
                Source = $"Source_{Guid.NewGuid()}",
                Title = $"Title_{Guid.NewGuid()}",
                SourceAuthor = $"SourceAuthor_{Guid.NewGuid()}",
                Content = $"Content_{Guid.NewGuid()}",
                Media = new List<MediaWithMediaType>(),
                DeletedBy = $"DeletedBy_{Guid.NewGuid()}",
            };
        }

        public void Dispose()
        {
            Database?.Dispose();
        }
    }
}
