/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Services
{
    public class BaseService
    {
        protected readonly IUrlHelper UrlHelper;
        protected readonly IMapper Mapper;

        public BaseService(IMapper mapper, IUrlHelper urlHelper)
        {
            UrlHelper = urlHelper;
            Mapper = mapper;
        }
    }
}
