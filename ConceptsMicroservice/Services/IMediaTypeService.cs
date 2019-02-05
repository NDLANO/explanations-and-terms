/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;

namespace ConceptsMicroservice.Services
{
    public interface IMediaTypeService
    {
        Response GetAllMediaTypes();
    }
}
