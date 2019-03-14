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
        public MetadataService(IMetadataRepository metadataRepository, IMapper mapper, IUrlHelper urlHelper) : base (mapper, urlHelper)
        {
            _metadataRepository = metadataRepository;
        }

        public Response SearchForMetadata(MetaSearchQuery query)
        {
            try
            {
                var results = _metadataRepository.SearchForMetadata(query);

                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = results.FirstOrDefault().TotalItems;
                    numberOfPages = results.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var n = UrlHelper.Action("Search", "Metadata", query);
                var l = Mapper.Map<List<MetaDataDTO>>(results);
                var dto = new PagingDTO<MetaDataDTO>(l, query, n, numberOfPages, totalItems);

                return new Response
                {
                    Data = dto
                };
            }
            catch(Exception e)
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
            try
            {
                var results = _metadataRepository.GetAll(query);
                var totalItems = 0;
                var numberOfPages = 0;
                try
                {
                    totalItems = results.FirstOrDefault().TotalItems;
                    numberOfPages = results.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var resultAsDto = Mapper.Map<List<MetaDataDTO>>(results);

                return new Response
                {
                    Data = new PagingDTO<MetaDataDTO>(resultAsDto, query, UrlHelper.Action("GetAll", "Metadata", query), numberOfPages, totalItems)
            };
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
