/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class MediaTypeService : IMediaTypeService
    {
        private readonly IMediaTypeRepository _mediaTypeRepository;

        public MediaTypeService(IMediaTypeRepository media)
        {
            _mediaTypeRepository = media;
        }

        public Response GetAllMediaTypes()
        {
            try
            {
                return new Response
                {
                    Data = _mediaTypeRepository.GetAll()
                };
            }
            catch
            {
                return null;
            }
        }

    }
}
