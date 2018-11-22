/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using ConceptsMicroservice.Controllers;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;
using NotFoundResult = Microsoft.AspNetCore.Mvc.NotFoundResult;

namespace ConceptsMicroservice.UnitTests.TestControllers
{
    public class ConceptControllerTest
    {
        private readonly IConceptService _service;
        private readonly ConceptController _controller;
        private readonly Concept _concept;
        private Response _errorResponse;
        private readonly ConceptSearchQuery _searchQuery;
        private readonly Response _listResponse;
        private readonly Response _singleResponse;

        public ConceptControllerTest()
        {
            _service = A.Fake<IConceptService>();
            _controller = new ConceptController(_service);
            _concept = new Concept
            {
                Title = "Title",
                Author = "Author",
                Content = "Content",
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Meta = new List<MetaData>
                {
                    new MetaData
                    {
                        Name= "name",
                        Description = "Description",
                        Category =  new MetaCategory
                        {
                            Name = "Language",
                            Description = "Description"
                        }
                    },
                    new MetaData
                    {
                        Name= "name",
                        Description = "Description",
                        Category =  new MetaCategory
                        {
                            Name = "Licence",
                            Description = "Description"
                        }
                    }
                }
            };

            _searchQuery = new ConceptSearchQuery {Title = "title", MetaIds = new List<int> {1, 2}};

            _errorResponse = new Response
            {
                Errors = new ModelStateDictionary()
            };
            _errorResponse.Errors.TryAddModelError("err", "err");

            _listResponse = new Response {Data = new List<Concept>()};
            _singleResponse = new Response { Data = new Concept() };
        }

        #region GetById

        [Fact]
        public void GetById_Returns_Concept_When_Id_Is_Valid()
        {
            A.CallTo(() => _service.GetConceptById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetById(0);
            var okresult = result.Result as OkObjectResult;

            var response = ((Response)okresult.Value);
            Assert.NotNull(response.Data);
            Assert.IsType<Concept>(response.Data);
        }

        [Fact]
        public void GetById_Returns_Status_200_When_A_Concept_Is_Found()
        {
            A.CallTo(() => _service.GetConceptById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetById(0);
            var okresult = result.Result as OkObjectResult;
            
            Assert.Equal(200, okresult.StatusCode);
        }

        [Fact]
        public void GetById_Returns_400_When_Id_Is_Not_Valid()
        {
            A.CallTo(() => _service.GetConceptById(A<int>._)).Returns(null);

            var result = _controller.GetById(0);
            var notFoundResult = result.Result as NotFoundResult;

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        #endregion

        #region GetAll

        [Fact]
        public void GetAll_Returns_Status_400_No_Concepts_Is_Found()
        {
            A.CallTo(() => _service.GetAllConcepts()).Returns(null);

            var result = _controller.GetAll();
            var bad = result.Result as BadRequestResult;

            Assert.Equal(400, bad.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_Status_200_Concepts_Is_Found()
        {
            A.CallTo(() => _service.GetAllConcepts()).Returns(_listResponse);

            var result = _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_A_List_Of_Concepts_If_There_Exists_Concepts()
        {
            A.CallTo(() => _service.GetAllConcepts()).Returns(_listResponse);

            var result = _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsType<List<Concept>>((okResult.Value as Response).Data);
        }

        #endregion

        #region Update
        [Fact]
        public void UpdateConcept_Returns_400_When_Concept_Is_Null()
        {
            var result = _controller.UpdateConcept(null);
            var badRequest = result.Result as BadRequestObjectResult;
            
            Assert.Equal(400, badRequest.StatusCode);
        }
        [Fact]
        public void UpdateConcept_Returns_With_Errors_When_Concept_Is_Null()
        {
            var result = _controller.UpdateConcept(null);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.True(((ModelStateErrorResponse)badRequest.Value).Errors.Count > 0);
        }
        [Fact]
        public void UpdateConcept_Returns_400_On_ModelState_Error()
        {
            _controller.ModelState.TryAddModelError("error", "error");
            var result = _controller.UpdateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;
            
            Assert.Equal(400, badRequest.StatusCode);
        }
        [Fact]
        public void UpdateConcept_Returns_400_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.UpdateConcept(A<Concept>._)).Returns(null);
            var result = _controller.UpdateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public void UpdateConcept_Returns_400_If_Service_Returns_Errors()
        {
            A.CallTo(() => _service.UpdateConcept(A<Concept>._)).Returns(_errorResponse);
            var result = _controller.UpdateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }


        [Fact]
        public void UpdateConcept_Returns_400_If_ViewModel_Contains_Errors()
        {
            A.CallTo(() => _service.UpdateConcept(A<Concept>._))
                .Returns(_errorResponse);

            var result = _controller.UpdateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.NotNull(badRequest.Value);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public void UpdateConcept_Returns_200_On_Successful_Update()
        {
            A.CallTo(() => _service.UpdateConcept(A<Concept>._)).Returns(_singleResponse);

            var result = _controller.UpdateConcept(_concept);
            var ok = result.Result as OkObjectResult;
            
            Assert.Equal(200, ok.StatusCode);
        }
        [Fact]
        public void UpdateConcept_Returns_A_Response_With_A_Concept_On_Successful_Update()
        {
            A.CallTo(() => _service.UpdateConcept(A<Concept>._)).Returns(_singleResponse);

            var result = _controller.UpdateConcept(_concept);
            var ok = result.Result as OkObjectResult;

            Assert.IsType<Concept>(((Response)ok.Value).Data);
        }
        #endregion

        #region Insert
        [Fact]
        public void CreateConcept_Returns_400_When_Concept_Is_Null()
        {
            var result = _controller.CreateConcept(null);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }
        [Fact]
        public void CreateConcept_Returns_With_Errors_When_Concept_Is_Null()
        {
            var result = _controller.CreateConcept(null);
            var badRequest = result.Result as BadRequestObjectResult;
            Assert.True(((ModelStateErrorResponse)badRequest.Value).Errors.Count > 0);
        }
        [Fact]
        public void CreateConcept_Returns_400_On_ModelState_Error()
        {
            _controller.ModelState.TryAddModelError("error", "error");
            var result = _controller.CreateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }
        [Fact]
        public void CreateConcept_Returns_400_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.CreateConcept(A<Concept>._)).Returns(null);
            var result = _controller.CreateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public void CreateConcept_Returns_400_If_Service_Returns_Errors()
        {
            A.CallTo(() => _service.CreateConcept(A<Concept>._)).Returns(_errorResponse);
            var result = _controller.CreateConcept(_concept);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public void CreateConcept_Returns_200_On_Successful_Update()
        {
            A.CallTo(() => _service.CreateConcept(A<Concept>._)).Returns(_singleResponse);

            var result = _controller.CreateConcept(_concept);
            var ok = result.Result as OkObjectResult;

            Assert.Equal(200, ok.StatusCode);
        }
        [Fact]
        public void CreateConcept_Returns_A_Response_With_A_Concept_On_Successful_Update()
        {
            A.CallTo(() => _service.CreateConcept(A<Concept>._)).Returns(_singleResponse);

            var result = _controller.CreateConcept(_concept);
            var ok = result.Result as OkObjectResult;

            Assert.IsType<Concept>(((Response)ok.Value).Data);
        }
        #endregion

        #region Delete
        
        [Fact]
        public void DeleteConcept_Returns_204_When_Deletion_Was_Successful()
        {
            A.CallTo(() => _service.ArchiveConcept(A<int>._)).Returns(new Response());

            var result = _controller.DeleteConcept(0);
            var noContentResult = result.Result as NoContentResult;

            Assert.Equal(204, noContentResult.StatusCode);
        }
        [Fact]
        public void DeleteConcept_Returns_400_When_Viewmodel_Has_Errors()
        {
            A.CallTo(() => _service.ArchiveConcept(A<int>._)).Returns(_errorResponse);

            var result = _controller.DeleteConcept(0);
            var badResult = result.Result as BadRequestObjectResult;

            Assert.Equal(400, badResult.StatusCode);
        }
        [Fact]
        public void DeleteConcept_Returns_404_When_Concept_Does_Not_Exist()
        {
            A.CallTo(() => _service.ArchiveConcept(A<int>._)).Returns(null);

            var result = _controller.DeleteConcept(0);
            var notFoundResult = result.Result as NotFoundResult;

            Assert.Equal(404, notFoundResult.StatusCode);
        }

            #endregion

        #region Search

        [Fact]
        public void Search_Returns_400_If_Service_Returned_Null()
        {
            A.CallTo(() => _service.SearchForConcepts(A<ConceptSearchQuery>._)).Returns(null);
            var result = _controller.Search(null);
            var bad = result.Result as BadRequestResult;

            Assert.Equal(400, bad.StatusCode);
        }

        [Fact]
        public void Search_Returns_200_When_Query_Contains_Parameters()
        {
            A.CallTo(() => _service.SearchForConcepts(_searchQuery)).Returns(_singleResponse);

            var result = _controller.Search(_searchQuery);
            var ok = result.Result as OkObjectResult;

            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public void Search_Returns_List_Of_Concepts_When_Query_Contains_Parameters()
        {
            A.CallTo(() => _service.SearchForConcepts(_searchQuery)).Returns(_listResponse);

            var result = _controller.Search(_searchQuery);
            var ok = result.Result as OkObjectResult;

            Assert.IsType<List<Concept>>(((Response)ok.Value).Data);
        }


        [Fact]
        public void Search_Returns_200_If_Query_Is_Null()
        {
            A.CallTo(() => _service.SearchForConcepts(null)).Returns(_singleResponse);

            var result = _controller.Search();
            var ok = result.Result as OkObjectResult;

            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public void Search_Returns_List_Of_Concepts_When_Query_Is_Null()
        {
            A.CallTo(() => _service.SearchForConcepts(null)).Returns(_listResponse);

            var result = _controller.Search();
            var ok = result.Result as OkObjectResult;

            Assert.IsType<List<Concept>>(((Response)ok.Value).Data);
        }

        #endregion
        

        #region GetAllTitles
        [Fact]
        public void AllTitles_Returns_400_If_Service_Returns_Null()
        {
            A.CallTo(() => _service.GetAllConceptTitles()).Returns(null);

            var result = _controller.AllTitles();
            var bad = result.Result as BadRequestResult;

            Assert.Equal(400, bad.StatusCode);
        }

        [Fact]
        public void AllTitles_Returns_200_If_Service_Is_Not_Null()
        {
            A.CallTo(() => _service.GetAllConceptTitles()).Returns(new Response());

            var result = _controller.AllTitles();
            var ok = result.Result as OkObjectResult;

            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public void AllTitles_Returns_A_List_Of_Strings()
        {
            A.CallTo(() => _service.GetAllConceptTitles()).Returns(new Response{Data = new List<string>()});

            var result = _controller.AllTitles();
            var ok = result.Result as OkObjectResult;

            Assert.IsType<List<string>>(((Response)ok.Value).Data);
        }

        #endregion
    }
}
