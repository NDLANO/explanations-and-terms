/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

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

        public List<ConceptMedia> GetMediaForConcept(Concept concept)
        {
            return _context.ConceptMedia
                .Include(x => x.Media)
                .Where(x => x.ConceptId == concept.Id)
                .ToList();
        }
        public bool DeleteMediaForConcept(Concept concept, List<ConceptMedia> conceptMedia)
        {
            if (concept == null || conceptMedia == null)
                return false;

            _context.ConceptMedia.RemoveRange(conceptMedia);
            return _context.SaveChanges() == conceptMedia.Count;
        }
        public List<ConceptMedia> InsertMediaForConcept(Concept concept, List<MediaWithMediaType> conceptMedia)
        {
            if (concept == null || conceptMedia == null)
                return new List<ConceptMedia>();

            return conceptMedia
                .Select(x => Insert(concept, x.ExternalId, x.MediaTypeId))
                .ToList();
        }

        public bool Delete(ConceptMedia relation)
        {
            if (_context.Entry(relation).State == EntityState.Detached)
                _context.ConceptMedia.Attach(relation);

            _context.ConceptMedia.Remove(relation);

            return _context.SaveChanges() == 1;
        }

        private Media AddOrGetMediaWithExternalId(string externalId, int mediaType)
        {
            var media = _context.Media
                .Include(x => x.MediaType)
                .FirstOrDefault(x => x.ExternalId == externalId);
            if (media == null)
            {
                media = new Media
                {
                    ExternalId = externalId,
                    MediaTypeId = mediaType,
                    MediaType = _context.MediaTypes.Find(mediaType)
                };
                _context.Media.Add(media);
            }

            return media;
        }

        public ConceptMedia Insert(Concept concept, string externalId, int mediaType)
        {
            if (concept == null)
                return null;

            var media = AddOrGetMediaWithExternalId(externalId, mediaType);

            var relation = new ConceptMedia
            {
                ConceptId = concept.Id,
                MediaId = media.Id,
                Media = media
            };
            _context.ConceptMedia.Add(relation);
            _context.SaveChanges();
            return relation;
        }
    }
}
