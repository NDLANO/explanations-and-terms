/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;

namespace ConceptsMicroservice.Repositories
{
    public interface IMetadataRepository
    {
        MetaData GetById(int id);
        List<MetaData> GetAll();
        List<MetaData> GetByRangeOfIds(IEnumerable<int> ids);
        List<MetaData> SearchForMetadata(MetaSearchQuery query);
    }
}
