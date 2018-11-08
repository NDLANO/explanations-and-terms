/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly IMetadataRepository _metadataRepository;
        public MetadataService(IMetadataRepository metadataRepository)
        {
            _metadataRepository = metadataRepository;
        }

        public Response SearchForMetadata(MetaSearchQuery query)
        {
            var response = new Response();
            if (query == null || query.HasNoQuery())
                response.Data = _metadataRepository.GetAll();
            else
                response.Data = _metadataRepository.SearchForMetadata(query);

            return response;
        }

        public Response GetById(int id)
        {
            var meta = _metadataRepository.GetById(id);
            if (meta == null)
                return null;
            return new Response
            {
                Data = meta
            };
        }

        public Response GetAll()
        {
            return new Response
            {
                Data = _metadataRepository.GetAll()
            };
        }
    }
}
