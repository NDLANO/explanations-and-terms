/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using System.Linq;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Repositories;
using Microsoft.Extensions.Options;

namespace ConceptsMicroservice.Services.Validation
{
    public class ConceptValidationService : IConceptValidationService
    {
        private readonly IMediaTypeRepository _mediaTypeRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ConceptValidationService(IStatusRepository status, IMetadataRepository meta, ICategoryRepository category, IMediaTypeRepository mediaType)
        {
            _statusRepository = status;
            _metadataRepository = meta;
            _categoryRepository = category;
            _mediaTypeRepository = mediaType;
        }
        public bool StatusIdIsValidId(int id)
        {
            return _statusRepository.GetById(id) != null;
        }

        public List<int> MetaIdsDoesNotExistInDatabase(List<int> ids)
        {
            var noExistingIds = new List<int>();
            if (ids == null)
                return noExistingIds;

            foreach (var id in ids)
            {
                if (_metadataRepository.GetById(id) == null)
                    noExistingIds.Add(id);
            }

            return noExistingIds;
        }

        public List<int> MediaTypesNotExistInDatabase(List<MediaWithMediaType> mediaTypes, string language)
        {
            var noExistingIds = new List<int>();
            if (mediaTypes == null)
                return noExistingIds;

            var allMediaTypes = _mediaTypeRepository.GetAll(BaseListQuery.DefaultValues(language));
            var pages = 1;

            try
            {
                pages = allMediaTypes.FirstOrDefault().NumberOfPages;
            }
            catch { }

            for (var page = 0; page < pages; page++)
            {
                var query = BaseListQuery.DefaultValues(language);
                query.Page = page + 1;
                var medias = _mediaTypeRepository
                    .GetAll(query)
                    .Select(x => x.Id)
                    .ToList();
                foreach (var mediaType in mediaTypes)
                {
                    if (!medias.Contains(mediaType.MediaTypeId))
                        noExistingIds.Add(mediaType.MediaTypeId);
                }
            }


            return noExistingIds;
        }

        public List<string> GetMissingRequiredCategories(List<int> metaIds, string language)
        {
            var missingCategories = new List<string>();

            if (metaIds == null)
                metaIds = new List<int>();

            var requiredCategories = _categoryRepository.GetRequiredCategories(language);

            var metas = _metadataRepository.GetByRangeOfIds(metaIds);

            foreach (var requiredCategory in requiredCategories)
            {
                var category = metas.FirstOrDefault(x => x.Category.Id == requiredCategory.Id);
                if (category == null)
                    missingCategories.Add(requiredCategory.Name);
            }

            return missingCategories;
        }
    }
}
