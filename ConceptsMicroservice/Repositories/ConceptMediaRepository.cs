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
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Repositories
{
    public class ConceptMediaRepository : IConceptMediaRepository
    {
        private readonly ConceptsContext _context;

        public ConceptMediaRepository(ConceptsContext context)
        {
            _context = context;
        }

        public List<ConceptMedia> GetMediaForConcept(int conceptId)
        {
            return _context.ConceptMedia
                .Include(x => x.Media)
                .Where(x => x.ConceptId == conceptId)
                .ToList();
        }
        public bool DeleteConnectionBetweenConceptAndMedia(int conceptId, IEnumerable<int> mediaToBeDeleted)
        {
            if (mediaToBeDeleted == null)
                return false;
            var conceptMediaToBeDeleted = _context.ConceptMedia
                .Where(x => x.ConceptId == conceptId && mediaToBeDeleted.Contains(x.MediaId));
            _context.ConceptMedia.RemoveRange(conceptMediaToBeDeleted);
            return _context.SaveChanges() == mediaToBeDeleted.Count();
        }
        public List<ConceptMedia> InsertMediaForConcept(int conceptId, List<MediaWithMediaType> conceptMedia)
        {
            if (conceptMedia == null)
                return new List<ConceptMedia>();

            return conceptMedia
                .Select(x => Insert(conceptId, x.ExternalId, x.MediaTypeId))
                .ToList();
        }

        public bool Delete(ConceptMedia relation)
        {
            if (_context.Entry(relation).State == EntityState.Detached)
                _context.ConceptMedia.Attach(relation);

            _context.ConceptMedia.Remove(relation);

            return _context.SaveChanges() == 1;
        }

        private Media MediaWithExternalIdOfTypeExists(string externalId, int mediaType)
        {
            return _context.Media
                .Include(x => x.MediaType)
                .FirstOrDefault(x => x.ExternalId == externalId && x.MediaTypeId == mediaType);
        }

        private Media CreateMedia(string externalId, int mediaType)
        {

            var media = new Media
            {
                ExternalId = externalId,
                MediaTypeId = mediaType,
                MediaType = _context.MediaTypes.Find(mediaType),
                Created = DateTime.Now,
                Updated= DateTime.Now
            };
            _context.Media.Add(media);
            return media;
        }

        public ConceptMedia Insert(int conceptId, string externalId, int mediaType)
        {
            var media = MediaWithExternalIdOfTypeExists(externalId, mediaType);
            if (media == null)
                media = CreateMedia(externalId, mediaType);

            var relation = new ConceptMedia
            {
                ConceptId = conceptId,
                MediaId = media.Id,
                Media = media,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };
            _context.ConceptMedia.Add(relation);
            _context.SaveChanges();
            return relation;
        }
    }
}
