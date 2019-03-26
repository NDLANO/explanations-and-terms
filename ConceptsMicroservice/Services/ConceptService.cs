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
using Auth0.AuthenticationApi.Models;
using AutoMapper;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConceptsMicroservice.Services
{
    public class ConceptService : BaseService, IConceptService
    {
        private readonly IConceptRepository _conceptRepository;
        private readonly IConceptMediaRepository _conceptMediaRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metaRepository;
        private readonly ILanguageRepository _languageRepository;

        public ConceptService(IConceptRepository concept,  IStatusRepository status, IConceptMediaRepository media, IMetadataRepository meta, ILanguageRepository language, IMapper mapper, IUrlHelper urlHelper) : base(mapper, urlHelper)
        {
            _conceptRepository = concept;
            _statusRepository = status;
            _conceptMediaRepository = media;
            _metaRepository = meta;
            _languageRepository = language;
        }
        
        public Response SearchForConcepts(ConceptSearchQuery query)
        {
            try
            {
                var concepts = _conceptRepository.SearchForConcepts(query); 
                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = concepts.FirstOrDefault().TotalItems;
                    numberOfPages = concepts.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var res = new PagingDTO<ConceptDto>(
                    Mapper.Map<List<ConceptDto>>(concepts),
                    query,
                    UrlHelper.Action("Search", "Concept", query), 
                    numberOfPages,
                    totalItems);
                

                return new Response
                {
                    Data = res
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Response GetConceptById(int id)
        {
            try
            {
                return new Response
                {
                    Data = Mapper.Map<ConceptDto>(_conceptRepository.GetById(id))
                };
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Response GetConceptByExternalId(string externalId)
        {
            try
            {
                return new Response
                {
                    Data = Mapper.Map<ConceptDto>(_conceptRepository.GetByExternalId(externalId))
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public Response GetAllConcepts(BaseListQuery query)
        {
            try
            {
                var concepts = _conceptRepository.GetAll(query);
                var totalItems = 0;
                var numberOfPages = 0;

                try
                {
                    totalItems = concepts.FirstOrDefault().TotalItems;
                    numberOfPages = concepts.FirstOrDefault().NumberOfPages;
                }
                catch { }

                var res = new PagingDTO<ConceptDto>(
                    Mapper.Map<List<ConceptDto>>(concepts),
                    query,
                    UrlHelper.Action("GetAll", "Concept", query),
                    numberOfPages,
                    totalItems);

                return new Response
                {
                    Data = res
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private Language GetLanguage(Response viewModel, List<int> metaIds)
        {
            // Fetch language id
            var metas = _metaRepository.GetByRangeOfIds(metaIds);
            var languageMeta = metas.FirstOrDefault(x => x.Category.TypeGroup.Name.ToLower().Equals("language"));
            if (languageMeta == null)
            {
                viewModel.Errors.TryAddModelError("metaIds", "Did not contain an id for language");
                return null;
            }

            var language = _languageRepository.GetByAbbreviation(languageMeta.Language.Abbreviation);
            if (language == null)
            {
                viewModel.Errors.TryAddModelError("metaIds", $"Language meta with id {languageMeta.Id} is not supported");
            }

            return language;
        }

        public Response UpdateConcept(UpdateConceptDto dto)
        {
            var newConceptVersion = Mapper.Map<Concept>(dto);
            var viewModel = new Response();
            var oldConceptVersion = _conceptRepository.GetById(newConceptVersion.Id);

            // Concept does not exist in the database, so cannot update it.
            if (oldConceptVersion == null)
            {
                return null;
            }

            var language = GetLanguage(viewModel, dto.MetaIds);
            if (language == null)
                return viewModel;

            // Readonly fields
            newConceptVersion.LanguageId = language.Id;
            newConceptVersion.Created = oldConceptVersion.Created;
            newConceptVersion.ExternalId = oldConceptVersion.ExternalId;
            newConceptVersion.MediaIds = oldConceptVersion.MediaIds;
            newConceptVersion.AuthorName = oldConceptVersion.AuthorName;
            newConceptVersion.AuthorEmail = oldConceptVersion.AuthorEmail;
            newConceptVersion.GroupId = oldConceptVersion.GroupId;

            
            try
            {
                _conceptRepository.Update(newConceptVersion);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not update concept.");
                return viewModel;
            }

            //updating meta list for concept
            var oldMetaList = oldConceptVersion.Meta;
            List<MetaData> newMetaIdList = _metaRepository.GetlanguageVariationForThisList(oldMetaList, language.Id);
            List<int> metaIds = new List<int>();
            foreach (var metaData in newMetaIdList)
            {
                metaIds.Add(metaData.Id);
            }

            // Updating media for concept
            var toBeDeleted = oldConceptVersion.Media
                .Where(x => !dto.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                .Select(x => x.Id)
                .ToList();
            var toBeInserted = dto.Media
                .Where(x => !oldConceptVersion.Media.Exists(y => y.ExternalId == x.ExternalId && y.MediaTypeId == x.MediaTypeId))
                .ToList();

            try
            {
                if (toBeDeleted.Count > 0)
                    _conceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(oldConceptVersion.Id, toBeDeleted);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not delete medias for concept.");
                return viewModel;
            }

            try
            {
                _conceptMediaRepository.InsertMediaForConcept(oldConceptVersion.Id, toBeInserted);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not insert media on concept.");
                return viewModel;
            }

            viewModel.Data = Mapper.Map<ConceptDto>(_conceptRepository.GetById(dto.Id));
            return viewModel;
        }

        private bool CanCreateLanguageVariation(Response viewModel, Concept newConcept)
        {
            var group = _conceptRepository.GetByGroupId(newConcept.GroupId);

            if (group == null || group.Count == 0)
            {
                viewModel.Errors.TryAddModelError("groupId", "GroupId did not have a valid value");
                return false;
            }

            var metas = _metaRepository.GetByRangeOfIds(newConcept.MetaIds);
            var metasWithoutLanguage = metas
                .Where(x => !x.Category.TypeGroup.Name.ToLower().Equals("language"))
                .Select(x => x.Id)
                .ToList();
            var newMetaLanguage = metas.FirstOrDefault(x => x.Category.TypeGroup.Name.ToLower().Equals("language"));

            // Validate metas
            foreach (var concept in group)
            {
                var groupMeta = _metaRepository.GetByRangeOfIds(concept.MetaIds);
                var metasForConceptWithoutLanguage = groupMeta
                    .Where(x => !x.Category.TypeGroup.Name.ToLower().Equals("language"))
                    .Select(x => x.Id)
                    .ToList();
                var groupLanguage = groupMeta.FirstOrDefault(x => x.Category.TypeGroup.Name.ToLower().Equals("language"));

                if (groupLanguage != null && newMetaLanguage != null &&
                    groupLanguage.LanguageVariation.Equals(newMetaLanguage.LanguageVariation))
                {
                    viewModel.Errors.TryAddModelError("metaIds", "Cannot create a concept with the same language");
                    return false;

                }

                var commonMetas = metasForConceptWithoutLanguage.Intersect(metasWithoutLanguage).ToList();
                if (metasForConceptWithoutLanguage.Count != metasWithoutLanguage.Count || commonMetas.Count != metasWithoutLanguage.Count)
                {
                    viewModel.Errors.TryAddModelError("metaIds", "Did not contain similar meta as rest of the concept group");
                    return false;
                }
            }
            return true;
        }
        

        public Response CreateConcept(CreateConceptDto newConcept, UserInfo userInfo)
        {
            var viewModel = new Response();
            var concept = Mapper.Map<Concept>(newConcept);
            var media = new List<ConceptMedia>();

            if (userInfo == null || string.IsNullOrEmpty(userInfo.Email) || string.IsNullOrEmpty(userInfo.FullName))
            {
                viewModel.Errors.TryAddModelError("errorMessage", "Could not get user information");
                return viewModel;
            }

            var language = GetLanguage(viewModel, newConcept.MetaIds);
            if (language == null)
                return viewModel;

            // Readonly fields
            concept.LanguageId = language.Id;
            concept.AuthorEmail = userInfo.Email;
            concept.AuthorName = userInfo.FullName;

            // Create a language variation
            if (concept.GroupId != Guid.Empty)
            {
                if (!CanCreateLanguageVariation(viewModel, concept))
                    return viewModel;
            }
            else
            {
                concept.GroupId = Guid.NewGuid();
            }

            try
            {
                concept = _conceptRepository.Insert(concept);
            }
            catch (Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "Could not create concept");
                return viewModel;
            }

            try
            {
                media = _conceptMediaRepository.InsertMediaForConcept(concept.Id, newConcept.Media);
            }
            catch(Exception e)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "Could not insert media for concept");
                return viewModel;
            }

            concept.Media = media.Select(x => x.Media).ToList();
            viewModel.Data = Mapper.Map<ConceptDto>(concept);

            return viewModel;
        }

        public Response ArchiveConcept(int id, string usersEmail)
        {
            var updatedConcept = _conceptRepository.GetById(id);
            if (updatedConcept == null)
                return null;

            var viewModel = new Response();
            var inactiveStatus = _statusRepository.GetByName(Status.STATUS_ARCHVIED);
            if (inactiveStatus == null)
            {
                viewModel.Errors.TryAddModelError("errorMessage", $"Did not found \"{Status.STATUS_ARCHVIED}\" status");
                return viewModel;
            }

            updatedConcept.Status = inactiveStatus;
            updatedConcept.StatusId = inactiveStatus.Id;
            updatedConcept.DeletedBy = usersEmail;

            try
            {
                viewModel.Data = Mapper.Map<ConceptDto>(_conceptRepository.Update(updatedConcept));
            }
            catch (Exception)
            {
                viewModel.Errors.TryAddModelError("errorMessage", "An database error has occured. Could not delete concept.");
                return viewModel;
            }

            return viewModel;
        }
    }
}
