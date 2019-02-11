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
using ConceptsMicroservice.Models.DTO;

namespace ConceptsMicroservice.UnitTests.Mock
{
    public interface IMock : IDisposable
    {
        IMockDatabase Database { get; set; }
        MetaCategory MockCategory(string name = null);
        Status MockStatus(string name = null);
        MetaData MockMeta(Status s, MetaCategory c);
        Concept MockConcept(Status status, List<MetaData> m = null, List<Media> media = null);
        UpdateConceptDto MockUpdateConceptDto(List<int> meta = null, List<MediaWithMediaType> media = null);
        CreateConceptDto MockCreateOrUpdateConcept();
    }
}
