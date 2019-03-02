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

            var languageParam = _languageConfig.Default;
            var pageSizeParam = BaseListQuery.DefaultPageSize;
            var pageParam = BaseListQuery.DefaultPage;

            try
            {
                var langParam = bindingContext.ValueProvider.GetValue("language").FirstValue;
                if (_languageConfig.Supported.Contains(langParam))
                    languageParam = langParam;
            }
            catch { }

            try
            {
                pageSizeParam = Convert.ToInt32(bindingContext.ValueProvider.GetValue("pageSize").FirstValue);
            }
            catch { }
            try
            {
                pageParam = Convert.ToInt32(bindingContext.ValueProvider.GetValue("page"));
            }
            catch { }
            

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