/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class MetadataService : BaseService, IMetadataService
    {
        private readonly IMetadataRepository _metadataRepository;
        public MetadataService(IMetadataRepository metadataRepository, IOptions<LanguageConfig> languageConfig, IMapper mapper, IUrlHelper urlHelper) : base (mapper, urlHelper, languageConfig)
        {
            _metadataRepository = metadataRepository;
        }

        public Response SearchForMetadata(MetaSearchQuery query)
        {
            if (query == null)
                query = BaseListQuery.DefaultValues(LanguageConfig.Default) as MetaSearchQuery;

            query.SetDefaultValuesIfNotInitilized(LanguageConfig);
            try
            {
                var results = _metadataRepository.SearchForMetadata(query);
                var dto = new MetaDataPagingDTO(results, query, UrlHelper.Action("Search", "Metadata", query));

                return new Response
                {
                    Data = dto
                };
            }
            catch
            {
                return null;
            }
        }

        public Response GetById(int id)
        {
            try
            {
                return new Response
                {
                    Data = _metadataRepository.GetById(id)
                };
            }
            catch
            {
                return null;
            }
            
        }

        public Response GetAll(BaseListQuery query)
        {
            if (query == null)
                query = BaseListQuery.DefaultValues(LanguageConfig.Default);

            query.SetDefaultValuesIfNotInitilized(LanguageConfig);
            try
            {
                var results = _metadataRepository.GetAll(query);

                return new Response
                {
                    Data = new MetaDataPagingDTO(results, query, UrlHelper.Action("GetAll", "Metadata", query))
            };
            }
            catch
            {
                return null;
            }
        }
    }
}
