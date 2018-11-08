/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;

namespace ConceptsMicroservice.Services
{
    public interface IMetadataService
    {
        Response SearchForMetadata(MetaSearchQuery query);
        Response GetById(int id);
        Response GetAll();

    }
}
