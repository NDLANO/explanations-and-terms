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

            CreateMap<CreateConceptDto, Concept>()
                .ForMember(x => x.Media, opt => opt.Ignore())
                .ForMember(x => x.MediaIds, opt => opt.Ignore())
                .ForMember(x => x.Status, opt => opt.Ignore())
                .ForMember(x => x.Meta, opt => opt.Ignore());

            CreateMap<UpdateConceptDto, Concept>()
                .ForMember(x => x.Media, opt => opt.Ignore())
                .ForMember(x => x.MediaIds, opt => opt.Ignore())
                .ForMember(x => x.Status, opt => opt.Ignore())
                .ForMember(x => x.Meta, opt => opt.Ignore());
        }
    }
}
