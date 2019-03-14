/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Threading.Tasks;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Binders
{
    public class BaseListQueryBinder : IModelBinder
    {
        private readonly LanguageConfig _languageConfig;

        public BaseListQueryBinder(IOptions<LanguageConfig> languageConfig)
        {
            _languageConfig = languageConfig.Value;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var queryCollection = bindingContext.HttpContext.Request.Query;

            var languageParam = _languageConfig.Default;
            var pageSizeParam = BaseListQuery.DefaultPageSize;
            var pageParam = BaseListQuery.DefaultPage;

            if (queryCollection.ContainsKey("language") && _languageConfig.Supported.Contains(queryCollection["language"]))
                languageParam = queryCollection["language"];

            if (queryCollection.ContainsKey("page") && int.TryParse(queryCollection["page"], out pageParam)) { }
            if (queryCollection.ContainsKey("pageSize") && int.TryParse(queryCollection["pageSize"], out pageSizeParam)) { }
            
            

            bindingContext.Result = ModelBindingResult.Success(new BaseListQuery
            {
                Language = languageParam,
                Page = pageParam,
                PageSize = pageSizeParam
            });
            return Task.CompletedTask;
        }
    }
}