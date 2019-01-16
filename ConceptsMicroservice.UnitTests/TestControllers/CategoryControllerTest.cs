/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.Net;
using ConceptsMicroservice.Controllers;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestControllers
{
    
    public class CategoryControllerTest
    {
        private readonly ICategoryService _service;
        private readonly CategoryController _controller;
        private readonly Response _listResponse;
        private readonly Response _singleResponse;

        public CategoryControllerTest()
        {
            _service = A.Fake<ICategoryService>();
            _controller = new CategoryController(_service);
            _listResponse = new Response { Data = new List<MetaCategory>() };
            _singleResponse = new Response { Data = new MetaCategory() };
        }

        #region GetById

        [Fact]
        public void GetById_Returns_Category_When_Id_Is_Valid()
        {
            A.CallTo(() => _service.GetCategoryById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetCategoryById(0);
            var okresult = result.Result as OkObjectResult;

            var response = ((Response)okresult.Value);
            Assert.NotNull(response.Data);
            Assert.IsType<MetaCategory>(response.Data);
        }

        [Fact]
        public void GetById_Returns_Status_200_When_A_Category_Is_Found()
        {
            A.CallTo(() => _service.GetCategoryById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetCategoryById(0);
            var okresult = result.Result as OkObjectResult;

            Assert.Equal(200, okresult.StatusCode);
        }

        [Fact]
        public void GetById_Returns_401_When_Id_Is_Not_Valid()
        {
            A.CallTo(() => _service.GetCategoryById(A<int>._)).Returns(new Response{Data = null});

            var result = _controller.GetCategoryById(0);
            var notFoundResult = result.Result as NotFoundResult;

            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetById_Returns_500_When_Service_Is_Null()
        {
            A.CallTo(() => _service.GetCategoryById(A<int>._)).Returns(null);

            var result = _controller.GetCategoryById(0);
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, status.StatusCode);
        }
        #endregion

        #region GetAll

        [Fact]
        public void GetAll_Returns_Status_500_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.GetAllCategories()).Returns(null);

            var result = _controller.GetAllCategories();
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, status.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_Status_200_Categories_Is_Found()
        {
            A.CallTo(() => _service.GetAllCategories()).Returns(_listResponse);

            var result = _controller.GetAllCategories();
            var okResult = result.Result as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_A_List_Of_Categories_If_There_Exists_Categories()
        {
            A.CallTo(() => _service.GetAllCategories()).Returns(_listResponse);

            var result = _controller.GetAllCategories();
            var okResult = result.Result as OkObjectResult;

            Assert.IsType<List<MetaCategory>>((okResult.Value as Response).Data);
        }

        #endregion
    }
}
