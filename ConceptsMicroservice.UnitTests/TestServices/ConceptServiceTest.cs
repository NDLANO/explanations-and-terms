/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using Auth0.AuthenticationApi.Models;
using AutoMapper;
using ConceptsMicroservice.Models;
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
    public class ConceptServiceTest : IClassFixture<InitMapper>
    {

        protected readonly IMock Mock;
        protected readonly IConceptService Service;
        protected readonly IConceptRepository ConceptRepository;
        protected readonly IConceptMediaRepository ConceptMediaRepository;
        protected readonly IStatusRepository StatusRepository;
        protected readonly IMapper Mapper;
        protected BaseListQuery BaseListQuery;
        private readonly IUrlHelper UrlHelper;
        private readonly string allowedUserEmail = "somebody@somedomain";
        
        private Status _status;
        

        public ConceptServiceTest()
        {
            ConceptMediaRepository = A.Fake<IConceptMediaRepository>();
            ConceptRepository = A.Fake<IConceptRepository>();
            StatusRepository = A.Fake<IStatusRepository>();
            UrlHelper = A.Fake<IUrlHelper>();

            Mapper = AutoMapper.Mapper.Instance;
            var languageConfig = Options.Create(ConfigHelper.GetLanguageConfiguration());

            Service = new ConceptService(ConceptRepository, StatusRepository, ConceptMediaRepository, Mapper, UrlHelper, languageConfig);
            Mock = new Mock.Mock();
            _status = new Status();
            BaseListQuery = BaseListQuery.DefaultValues("nb");
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(null);
        }
        #region GetAll
        [Fact]
        public void GetAllConcepts_Returns_A_Response_With_A_List_Of_Concepts()
        {
            //Nasser 14.02.2019
            //A.CallTo(() => ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage)).Returns(new List<Concept>());

            //var response = Service.GetAllConcepts(itemsPrPage, pageNumber, language, defaultLanguage);

            //Assert.IsType<List<ConceptDto>>(response.Data);
            var listOfConcepts = A.Fake<Response>();
            var service = A.Fake<IConceptService>();
            A.CallTo(() => service.GetAllConcepts(A<BaseListQuery>._)).Returns(listOfConcepts);
        }
        #endregion

        #region GetById

        [Fact]
        public void GetById_Returns_With_Empty_Data_If_The_Concept_With_ID_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var response = Service.GetConceptById(1);

            Assert.Null(response.Data);
        }

        [Fact]
        public void GetById_Returns_Null_If_An_Error_Occured()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Throws<Exception>();

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
            Assert.IsType<ConceptDto>(response.Data);
        }
        #endregion

        #region Archive
        [Fact]
        public void ArchiveConcept_Returns_Deleted_Concept_On_Archive_Successful()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(Mock.MockConcept(_status));

            var response = Service.ArchiveConcept(0, allowedUserEmail);

            Assert.NotNull(response.Data);
            Assert.IsType<ConceptDto>(response.Data);
        }
        [Fact]
        public void ArchiveConcept_Returns_With_No_Errors_When_Archiving_Successful()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(Mock.MockConcept(_status));

            var response = Service.ArchiveConcept(0, allowedUserEmail);

            Assert.False(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_With_Errors_When_Repo_Archiving_Throws_Exception()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Throws<Exception>();

            var response = Service.ArchiveConcept(0, allowedUserEmail);

            Assert.True(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_With_Errors_When_Archiving_Status_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => StatusRepository.GetByName(A<string>._)).Returns(null);

            var response = Service.ArchiveConcept(0, allowedUserEmail);

            Assert.True(response.HasErrors());
        }
        [Fact]
        public void ArchiveConcept_Returns_Null_When_Concept_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var response = Service.ArchiveConcept(0, allowedUserEmail);

            Assert.Null(response);
        }
        
        #endregion

        #region Search

        [Fact]
        public void SearchForConcepts_Fetches_No_Concepts_If_No_Query_Is_Specified()
        {
            A.CallTo(() => ConceptRepository.GetAll(A<BaseListQuery>._)).Returns(new List<Concept>());
            var results = Service.SearchForConcepts(null);

            A.CallTo(() => ConceptRepository.GetAll(A<BaseListQuery>._)).MustHaveHappenedOnceExactly();
            Assert.IsType<ConceptPagingDTO>(results.Data);
        }

        [Fact]
        public void SearchForConcepts_Fetches_Concepts_When_Query_Is_Specified()
        {
            A.CallTo(() => ConceptRepository.SearchForConcepts(A<ConceptSearchQuery>._)).Returns(new List<Concept>());
            var results = Service.SearchForConcepts(new ConceptSearchQuery());

            A.CallTo(() => ConceptRepository.GetAll(BaseListQuery)).MustNotHaveHappened();
            A.CallTo(() => ConceptRepository.SearchForConcepts(A<ConceptSearchQuery>._)).MustHaveHappenedOnceExactly();

            Assert.IsType<ConceptPagingDTO>(results.Data);
        }


        [Fact]
        public void SearchForConcepts_Returns_Null_If_An_Error_Occured()
        {
            A.CallTo(() => ConceptRepository.SearchForConcepts(A<ConceptSearchQuery>._)).Throws<Exception>();

            var results = Service.SearchForConcepts(new ConceptSearchQuery());

            Assert.Null(results);
        }
        #endregion

        #region Create


        [Fact]
        public void CreateConcept_Returns_Null_Data_When_RepoInsert_Throws_Exception()
        {
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._)).Throws<Exception>();

            var mockConcept = Mock.MockCreateOrUpdateConcept();

            var viewModel = Service.CreateConcept(mockConcept, new UserInfo());
            
            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Calls_InsertMediaForConcept_On_Success()
        {
            var mockConcept = Mock.MockConcept(_status);
            var mockMediaConcept = Mock.MockCreateOrUpdateConcept();

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._)).Returns(mockConcept);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());
            var viewModel = Service.CreateConcept(mockMediaConcept, new UserInfo());

            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void CreateConcept_Returns_Inserted_Media_On_Success()
        {
            var oneMedia = new MediaWithMediaType
            {
                ExternalId = "1",
                MediaTypeId = 1
            };
            var twoMedia = new MediaWithMediaType
            {
                ExternalId = "2",
                MediaTypeId = 2
            };
            var mockConcept = Mock.MockConcept(_status);
            var mockMediaConcept = Mock.MockCreateOrUpdateConcept();
            mockMediaConcept.Media = new List<MediaWithMediaType>
            {
                oneMedia,
                twoMedia
            };

            var conceptMediaList = new List<ConceptMedia>
            {
                new ConceptMedia
                {
                    Media = new Media
                    {
                        ExternalId = oneMedia.ExternalId,
                        MediaType = new MediaType
                        {
                            Id = oneMedia.MediaTypeId
                        }
                    }
                },
                new ConceptMedia
                {
                    Media = new Media
                    {
                        ExternalId = twoMedia.ExternalId,
                        MediaType = new MediaType
                        {
                            Id = twoMedia.MediaTypeId
                        }
                    }
                }
            };

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._)).Returns(mockConcept);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(conceptMediaList);
            var viewModel = Service.CreateConcept(mockMediaConcept, new UserInfo());

            var concept = viewModel.Data as ConceptDto;

            Assert.Equal(concept.Media.Count, mockMediaConcept.Media.Count);

            for (var i = 0; i < concept.Media.Count; i++)
            {
                Assert.Equal(concept.Media[i].ExternalId, mockMediaConcept.Media[i].ExternalId);
                Assert.Equal(concept.Media[i].MediaType.Id, mockMediaConcept.Media[i].MediaTypeId);
            }
        }

        [Fact]
        public void CreateConcept_Returns_Response_With_ConceptDTO_On_Success()
        {
            var mockConcept = Mock.MockConcept(_status);
            var mockMediaConcept = Mock.MockCreateOrUpdateConcept();

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._)).Returns(mockConcept);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());
            var viewModel = Service.CreateConcept(mockMediaConcept, new UserInfo());

            Assert.NotNull(viewModel.Data);
            Assert.IsType<ConceptDto>(viewModel.Data);
        }

        [Fact]
        public void CreateConcept_Returns_With_No_Errors_On_Success()
        {
            var mockConcept = Mock.MockConcept(_status);

            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.Insert(A<Concept>._)).Returns(mockConcept);

            var viewModel = Service.CreateConcept(Mock.MockCreateOrUpdateConcept(), new UserInfo());

            Assert.False(viewModel.HasErrors());
        }
        #endregion

        #region Update
        [Fact]
        public void UpdateConcept_Returns_Null_Data_When_RepoUpdate_Throws_Exception()
        {
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Throws<Exception>();

            var mockConcept = Mock.MockUpdateConceptDto();

            var viewModel = Service.UpdateConcept(mockConcept);

            Assert.Null(viewModel.Data);
        }

        [Fact]
        public void UpdateConcept_Returns_Null_When_ConceptId_Does_Not_Exist()
        {
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(null);

            var mockConcept = Mock.MockUpdateConceptDto();

            var viewModel = Service.UpdateConcept(mockConcept);

            Assert.Null(viewModel);
        }
        
        [Fact]
        public void UpdateConcept_Returns_ConceptDto_On_Success()
        {
            var concept = Mock.MockConcept(_status);
            concept.Media = new List<Media>();
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(concept);
            A.CallTo(() => ConceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(A<int>._, A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());

            var result = Service.UpdateConcept(Mock.MockUpdateConceptDto());

            Assert.NotNull(result.Data);
            Assert.IsType<ConceptDto>(result.Data);
        }

        [Fact]
        public void UpdateConcept_Calls_InsertMediaForConcept_On_Success()
        {
            var concept = Mock.MockConcept(_status);
            concept.Media = new List<Media>();
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(concept);
            A.CallTo(() => ConceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(A<int>._, A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());

            var result = Service.UpdateConcept(Mock.MockUpdateConceptDto());

            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._))
                .MustHaveHappened(1, Times.Exactly);
        }
        
        [Fact]
        public void UpdateConcept_Calls_DeleteConnectionBetweenConceptAndMedia_On_Success_If_There_Is_Media_To_Delete()
        {
            var conceptFromDb = Mock.MockConcept(_status);
            conceptFromDb.Media = new List<Media>
            {
                new Media()
            };

            var concept = Mock.MockConcept(_status);
            concept.Media = new List<Media>();
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(conceptFromDb);
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(concept);
            A.CallTo(() => ConceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(A<int>._, A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());

            var result = Service.UpdateConcept(Mock.MockUpdateConceptDto());

            A.CallTo(() => ConceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(A<int>._, A<List<int>>._))
                .MustHaveHappened(1, Times.Exactly);
        }


        [Fact]
        public void UpdateConcept_Returns_With_No_Errors_On_Success()
        {
            var concept = Mock.MockConcept(_status);
            concept.Media = new List<Media>();
            A.CallTo(() => StatusRepository.GetById(A<int>._)).Returns(_status);
            A.CallTo(() => ConceptRepository.GetById(A<int>._)).Returns(Mock.MockConcept(_status));
            A.CallTo(() => ConceptRepository.Update(A<Concept>._)).Returns(concept);
            A.CallTo(() => ConceptMediaRepository.DeleteConnectionBetweenConceptAndMedia(A<int>._, A<List<int>>._)).Returns(true);
            A.CallTo(() => ConceptMediaRepository.InsertMediaForConcept(A<int>._, A<List<MediaWithMediaType>>._)).Returns(new List<ConceptMedia>());

            var result = Service.UpdateConcept(Mock.MockUpdateConceptDto());

            Assert.False(result.HasErrors());
        }
        #endregion
    }
}
