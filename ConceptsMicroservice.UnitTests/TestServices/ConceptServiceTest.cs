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
        protected readonly ICategoryRepository CategoryRepository;

        private Status _status;

        public ConceptServiceTest()
        {
            MetaRepository = A.Fake<IMetadataRepository>();
            ConceptRepository = A.Fake<IConceptRepository>();
            StatusRepository = A.Fake<IStatusRepository>();
            CategoryRepository = A.Fake<ICategoryRepository>();
            Service = new ConceptsMicroservice.Services.ConceptService(ConceptRepository, MetaRepository, StatusRepository, CategoryRepository);
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

        #region ValidateConcept

        [Fact]
        public void ValidateConcept_Concept_Status_Is_Null_Result_Should_Have_Errors()
        {

            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var concept = Mock.MockConcept(status, new List<MetaData> {meta});
            concept.Status = null;

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(null);
            A.CallTo(() => MetaRepository.GetByListOfIds(A<List<int>>._)).Returns(new List<MetaData> {meta});

            var validateModel = Service.ValidateConcept(concept);

            Assert.True(validateModel.HasErrors());

        }

        [Fact]
        public void ValidateConcept_Concept_Status_Does_Not_Exist_In_Database_Result_Should_Have_Errors()
        {
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var concept = Mock.MockConcept(status, new List<MetaData> { meta });
            concept.Status = null;

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(null);
            A.CallTo(() => MetaRepository.GetByListOfIds(A<List<int>>._)).Returns(new List<MetaData> { meta });

            var validateModel = Service.ValidateConcept(concept);

            Assert.True(validateModel.HasErrors());
        }

        [Fact]
        public void ValidateConcept_Concept_Contains_Not_All_Required_Metas_Result_Should_Have_Errors()
        {
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var requiredCategory = Mock.MockCategory();
            requiredCategory.IsRequired = true;
            requiredCategory = Mock.Database.InsertCategory(requiredCategory);

            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var concept = Mock.MockConcept(status, new List<MetaData> { meta });

            A.CallTo(() => CategoryRepository.GetRequiredCategories()).Returns(new List<MetaCategory>{ requiredCategory });
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(status);
            A.CallTo(() => MetaRepository.GetByListOfIds(A<List<int>>._)).Returns(new List<MetaData> { meta });

            var validateModel = Service.ValidateConcept(concept);

            Assert.True(validateModel.HasErrors());
        }

        [Fact]
        public void ValidateConcept_Concept_Contains_Metas_Which_Does_Not_Exist_In_DB_Result_Should_Have_Errors()
        {
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var concept = Mock.MockConcept(status, new List<MetaData> { meta, Mock.MockMeta(status, category) });

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(status);
            A.CallTo(() => MetaRepository.GetByListOfIds(A<List<int>>._)).Returns(new List<MetaData> { meta });

            var validateModel = Service.ValidateConcept(concept);

            Assert.True(validateModel.HasErrors());
        }

        [Fact]
        public void ValidateConcept_Concept_Contains_Multiple_Metas_Of_Same_Category_And_ConceptCanHaveMultiple_Is_False_Result_Should_Have_Errors()
        {
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var c = Mock.MockCategory();
            c.ConceptCanHaveMultipleMeta = false;
            var category = Mock.Database.InsertCategory(c);
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var meta2 = Mock.Database.InsertMeta(Mock.MockMeta(status, category));
            var concept = Mock.MockConcept(status, new List<MetaData> { meta1, meta2 });

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(status);
            A.CallTo(() => MetaRepository.GetByListOfIds(A<List<int>>._)).Returns(new List<MetaData> { meta1, meta2 });

            var validateModel = Service.ValidateConcept(concept);

            Assert.True(validateModel.HasErrors());
        }

        [Fact]
        public void
            ValidateConcept_Concept_Contains_Multiple_Metas_Of_Same_Category_And_ConceptCanHaveMultiple_Is_True_Should_Not_Result_In_Errors()
        {
            Assert.True(false);
        }

        [Fact]
        public void ValidateConcept_Validates_Success_Should_Not_Result_In_Errors()
        {
            Assert.True(false);
        }
        #endregion
    }
}
