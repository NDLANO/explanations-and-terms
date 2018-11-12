/**
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

        public Status MockStatus(string n = null)
        {
            var name = $"Status_{Guid.NewGuid()}";
            if (!string.IsNullOrWhiteSpace(n))
                name = n;
            return new Status
            {
                Name = name,
                Description = "Description"
            };
        }

        public MetaData MockMeta(Status s, MetaCategory c)
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

        public Concept MockConcept(Status status, List<MetaData> m = null)
        {
            var meta = new List<MetaData>();
            if (m != null)
                meta = m;
            return new Concept
            {
                Source = $"Source_{Guid.NewGuid()}",
                Title = $"Title_{Guid.NewGuid()}",
                Author = $"Author_{Guid.NewGuid()}",
                Content = $"Content_{Guid.NewGuid()}",
                VersionNumber = "0.1",
                Meta = meta,
                MetaIds = meta.Select(x => x.Id).ToList(),
                Status = status
            };
        }

        public void Dispose()
        {
            Database?.Dispose();
        }
    }
}
