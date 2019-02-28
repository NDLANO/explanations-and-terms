/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using AutoMapper;
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.UnitTests.Helpers;
using ConceptsMicroservice.UnitTests.Mock;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestServices
{
    public class MetaDataServiceTest
    {

        protected readonly IMock Mock;
        protected readonly IMetadataService Service;
        protected readonly IMetadataRepository MetaRepository;
        protected readonly IUrlHelper UrlHelper;
        protected readonly IMapper Mapper;
        protected readonly LanguageConfig LanguageConfig;

        public MetaDataServiceTest()
        {
            LanguageConfig = ConfigHelper.GetLanguageConfiguration();
            MetaRepository = A.Fake<IMetadataRepository>();
            UrlHelper = A.Fake<IUrlHelper>();
            Mapper = A.Fake<IMapper>();
            Service = new MetadataService(MetaRepository, new OptionsWrapper<LanguageConfig>(LanguageConfig), Mapper, UrlHelper);
            Mock = new Mock.Mock();
            
        }

        #region Search

        [Fact]
        public void SearchForMetadata_Calls_Repo_GetAll_When_Query_Is_Null()
        {
            Service.SearchForMetadata(null);

            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SearchForMetadata_Returns_List_Of_MetaData_When_Query_Is_Null()
        {
            A.CallTo(() => Mapper.Map<List<MetaDataDTO>>(A<List<MetaData>>._)).Returns(new List<MetaDataDTO>());
            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).Returns(new List<MetaData>());
            var result = Service.SearchForMetadata(null);

            Assert.IsType<PagingDTO<MetaDataDTO>>(result.Data);
        }

        [Fact]
        public void SearchForMetadata_Calls_Repo_GetAll_When_Query_Is_Empty()
        {
            Service.SearchForMetadata(new MetaSearchQuery());

            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SearchForMetadata_Returns_List_Of_MetaData_When_Query_Is_Empty()
        {
            A.CallTo(() => Mapper.Map<List<MetaDataDTO>>(A<List<MetaData>>._)).Returns(new List<MetaDataDTO>());
            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).Returns(new List<MetaData>());
            var result = Service.SearchForMetadata(new MetaSearchQuery());

            Assert.IsType<PagingDTO<MetaDataDTO>>(result.Data);
        }

        [Fact]
        public void SearchForMetadata_Run_Search_in_Repo_When_Query_Is_Present()
        {
            var result = Service.SearchForMetadata(new MetaSearchQuery{Name = "name"});

            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SearchForMetadata_Returns_List_Of_MetaData_When_Query_Is_Present()
        {
            A.CallTo(() => MetaRepository.SearchForMetadata(A<MetaSearchQuery>._)).Returns(new List<MetaData>());
            var result = Service.SearchForMetadata(new MetaSearchQuery { Name = "name" });

            Assert.IsType<PagingDTO<MetaDataDTO>>(result.Data);
        }

        #endregion
    }
}
