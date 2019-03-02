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
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class MediaTypeService : BaseService, IMediaTypeService
    {
        private readonly IMediaTypeRepository _mediaTypeRepository;

        public MediaTypeService(IMediaTypeRepository media, IMapper mapper, IUrlHelper urlHelper) : base(mapper, urlHelper)
        {
            _mediaTypeRepository = media;
        }

        public Response GetAllMediaTypes(BaseListQuery query)
        {
            try
            {
                var categories = _mediaTypeRepository.GetAll(query);
                var results = Mapper.Map<List<MediaTypeDTO>>(categories);

                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = categories.FirstOrDefault().TotalItems;
                    numberOfPages = categories.FirstOrDefault().NumberOfPages;
                }
                catch { }

                return new Response
                {
                    Data = new PagingDTO<MediaTypeDTO>(results, query, UrlHelper.Action("GetAllMediaTypes", "MediaType", query), numberOfPages, totalItems)
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
