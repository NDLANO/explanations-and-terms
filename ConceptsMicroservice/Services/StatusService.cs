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
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class StatusService : BaseService, IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository status, IMapper mapper, IOptions<LanguageConfig> language, IUrlHelper urlHelper) : base(mapper, urlHelper, language)
        {
            _statusRepository = status;
        }

        public Response GetAllStatus(BaseListQuery query)
        {
            if (query == null)
                query = BaseListQuery.DefaultValues(LanguageConfig.Default);

            query.SetDefaultValuesIfNotInitilized(LanguageConfig);
            try
            {
                var status = _statusRepository.GetAll(query);
                return new Response
                {
                    Data = new StatusPagingDTO(status, query, UrlHelper.Action("GetAllStatus", "Status", query))
                };
            }
            catch
            {
                return null;
            }
        }

    }
}
