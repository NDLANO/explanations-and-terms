/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
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
    public class ConceptServiceTest
    {

        protected readonly IMock Mock;
        protected readonly IConceptService Service;
        protected readonly IConceptRepository ConceptRepository;
        protected readonly IMetadataRepository MetaRepository;
        protected readonly IStatusRepository StatusRepository;

        private Status _status;

        public ConceptServiceTest()
        {
            MetaRepository = A.Fake<IMetadataRepository>();
            ConceptRepository = A.Fake<IConceptRepository>();
            StatusRepository = A.Fake<IStatusRepository>();
            Service = new ConceptsMicroservice.Services.ConceptService(ConceptRepository, MetaRepository, StatusRepository);
            Mock = new Mock.Mock();
            _status = new Status();

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(null);
        }
        #region GetAll
        [Fact]
        public void GetAllConcepts_Returns_A_Response_With_A_List_Of_Concepts()
        {
            A.CallTo(() => ConceptRepository.GetAll()).Returns(new List<Concept>());

            var response = Service.GetAllConcepts();

            Assert.IsType<List<Concept>>(response.Data);
        }
        #endregion

        #region GetById

        [Fact]
        public void GetById_Returns_Null_If_The_Concept_With_ID_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var response = Service.GetConceptById(1);

            Assert.Null(response);
        }

        [Fact]
        public void GetById_Returns_A_Concept_When_It_Exists()
        {
            const int conceptId = 3;
            var concept = Mock.MockConcept(_status);
            concept.Id = conceptId;

            A.CallTo(() => ConceptRepository.GetById(conceptId)).Returns(concept);

            var response = Service.GetConceptById(conceptId);

            Assert.NotNull(response.Data);
            Assert.IsType<Concept>(response.Data);
        }
        #endregion

        #region Archive
        [Fact]
        public void ArchiveConcept_Returns_Deleted_Concept_On_Archive_Successful()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Returns(Mock.MockConcept(_status));

            var response = Service.SetStatusForConcept(0, "");

            Assert.NotNull(response.Data);
            Assert.IsType<Concept>(response.Data);
        }
        [Fact]
        public void ArchiveConcept_Returns_With_No_Errors_When_Archiving_Successful()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Returns(Mock.MockConcept(_status));

            var response = Service.SetStatusForConcept(0, "");

            Assert.False(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_With_Errors_When_Repo_Archiving_Throws_Exception()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Throws<Exception>();

            var response = Service.SetStatusForConcept(0, "");

            Assert.True(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_With_Errors_When_Archiving_Status_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(null);

            var response = Service.SetStatusForConcept(0, "");

            Assert.True(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_Null_When_Concept_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var response = Service.SetStatusForConcept(0, "");

            Assert.Null(response);
        }
        
        #endregion

        #region Search

        [Fact]
        public void SearchForConcepts_Fetches_All_Concepts_If_No_Query_Is_Specified()
        {
            A.CallTo(() => ConceptRepository.GetAll()).Returns(new List<Concept>());
            var results = Service.SearchForConcepts(null);

            A.CallTo(() => ConceptRepository.GetAll()).MustHaveHappenedOnceExactly();
            Assert.IsType<List<Concept>>(results.Data);
        }

        [Fact]
        public void SearchForConcepts_Fetches_Concepts_When_Query_Is_Specified()
        {
            A.CallTo(() => ConceptRepository.SearchForConcepts(A<ConceptSearchQuery>._)).Returns(new List<Concept>());
            var results = Service.SearchForConcepts(new ConceptSearchQuery());

            A.CallTo(() => ConceptRepository.GetAll()).MustNotHaveHappened();
            A.CallTo(() => ConceptRepository.SearchForConcepts(A<ConceptSearchQuery>._)).MustHaveHappenedOnceExactly();

            Assert.IsType<List<Concept>>(results.Data);
        }
        #endregion

        #region Create


        [Fact]
        public void CreateConcept_Returns_With_Error_When_Concept_Does_Not_Have_Any_Existing_Metas()
        {
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(false);

            var mockConcept = Mock.MockConcept(_status);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.True(viewModel.HasErrors());
            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_With_Error_When_Concept_Status_Is_Null()
        {
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);

            var mockConcept = Mock.MockConcept(null);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.True(viewModel.HasErrors());
            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_With_Error_When_Concept_Status_Does_Not_Exist()
        {
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(null);

            var mockConcept = Mock.MockConcept(_status);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.True(viewModel.HasErrors());
            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_With_Error_When_RepoInsert_Throws_Exception()
        {
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._, false)).Throws<Exception>();

            var mockConcept = Mock.MockConcept(_status);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.True(viewModel.HasErrors());
            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_Response_With_Concept_On_Success()
        {
            var mockConcept = Mock.MockConcept(_status);

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._, false)).Returns(mockConcept);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.NotNull(viewModel.Data);
            Assert.IsType<Concept>(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_With_No_Errors_On_Success()
        {
            var mockConcept = Mock.MockConcept(_status);

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._, false)).Returns(mockConcept);

            var viewModel = Service.CreateConcept(mockConcept);

            Assert.False(viewModel.HasErrors());
        }
        #endregion

        #region Update

        [Fact]
        public void UpdateConcept_Returns_Errors_When_Concept_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var result = Service.UpdateConcept(Mock.MockConcept(_status));

            Assert.True(result.HasErrors());
            Assert.Null(result.Data);
        }

        [Fact]
        public void UpdateConcept_Returns_Errors_When_Concept_Has_No_Existing_Metas()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(false);

            var result = Service.UpdateConcept(Mock.MockConcept(_status));

            Assert.True(result.HasErrors());
            Assert.Null(result.Data);
        }

        [Fact]
        public void UpdateConcept_Returns_Errors_Repo_Throws_Exception()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Throws<Exception>();
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);

            var result = Service.UpdateConcept(Mock.MockConcept(_status));

            Assert.True(result.HasErrors());
            Assert.Null(result.Data);
        }


        [Fact]
        public void UpdateConcept_Returns_No_Errors_On_Success()
        {
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);

            var result = Service.UpdateConcept(Mock.MockConcept(_status));

            Assert.False(result.HasErrors());
        }

        [Fact]
        public void UpdateConcept_Returns_Concept_On_Success()
        {
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._, false)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => MetaRepository.MetaObjectsExists(A<List<int>>._)).Returns(true);

            var result = Service.UpdateConcept(Mock.MockConcept(_status));

            Assert.NotNull(result.Data);
            Assert.IsType<Concept>(result.Data);
        }
        #endregion

        #region GetAllTitles
        [Fact]
        public void GetAllConceptsTitles_Returns_A_Response_With_A_List_Of_Strings()
        {
            A.CallTo(() => ConceptRepository.GetAllTitles()).Returns(new List<string>());

            var response = Service.GetAllConceptTitles();

            Assert.IsType<List<string>>(response.Data);
        }
        #endregion
    }
}
