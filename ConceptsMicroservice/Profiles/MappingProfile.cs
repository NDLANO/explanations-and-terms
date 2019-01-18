/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using AutoMapper;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;

namespace ConceptsMicroservice.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<CreateOrUpdateConcept, Concept>()
                    .ForMember(x => x.Media, opt => opt.Ignore());
            });
        }
    }
}
