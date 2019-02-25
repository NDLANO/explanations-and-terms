/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using AutoMapper;
using ConceptsMicroservice.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services
{
    public class BaseService
    {
        protected readonly IUrlHelper UrlHelper;
        protected readonly LanguageConfig LanguageConfig;
        protected readonly IMapper Mapper;

        public BaseService(IMapper mapper, IUrlHelper urlHelper, IOptions<LanguageConfig> languageConfig)
        {
            UrlHelper = urlHelper;
            Mapper = mapper;
            LanguageConfig = languageConfig.Value;
        }
    }
}
