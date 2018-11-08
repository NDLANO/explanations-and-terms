/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services;
using ConceptsMicroservice.UnitTests.Mock;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestServices
{
    public class MetaDataServiceTest
    {

        protected readonly IMock Mock;
        protected readonly IMetadataService Service;
        protected readonly IMetadataRepository MetaRepository;

        public MetaDataServiceTest()
        {
            MetaRepository = A.Fake<IMetadataRepository>();
            Service = new MetadataService(MetaRepository);
            Mock = new Mock.Mock();
        }

        #region Search

        [Fact]
        public void SearchForMetadata_Calls_Repo_GetAll_When_Query_Is_Null()
        {
            Service.SearchForMetadata(null);

            A.CallTo(() => MetaRepository.GetAll()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SearchForMetadata_Returns_List_Of_MetaData_When_Query_Is_Null()
        {
            A.CallTo(() => MetaRepository.GetAll()).Returns(new List<MetaData>());
            var result = Service.SearchForMetadata(null);

            Assert.IsType<List<MetaData>>(result.Data);
        }

        [Fact]
        public void SearchForMetadata_Calls_Repo_GetAll_When_Query_Is_Empty()
        {
            Service.SearchForMetadata(new MetaSearchQuery());

            A.CallTo(() => MetaRepository.GetAll()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void SearchForMetadata_Returns_List_Of_MetaData_When_Query_Is_Empty()
        {
            A.CallTo(() => MetaRepository.GetAll()).Returns(new List<MetaData>());
            var result = Service.SearchForMetadata(new MetaSearchQuery());

            Assert.IsType<List<MetaData>>(result.Data);
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

            Assert.IsType<List<MetaData>>(result.Data);
        }

        #endregion
    }
}
