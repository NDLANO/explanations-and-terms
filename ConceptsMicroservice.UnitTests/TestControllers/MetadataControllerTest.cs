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
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MetasMicroservice.UnitTests.TestControllers
{
    
    public class MetadataControllerTest
    {
        private readonly IMetadataService _service;
        private readonly MetadataController _controller;
        private readonly Response _listResponse;
        private readonly Response _singleResponse;

        public MetadataControllerTest()
        {
            _service = A.Fake<IMetadataService>();
            _controller = new MetadataController(_service);
            _listResponse = new Response { Data = new List<MetaData>() };
            _singleResponse = new Response { Data = new MetaData() };
        }

        [Fact]
        public void Search_Returns_500_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.SearchForMetadata(A<MetaSearchQuery>._)).Returns(null);

            var result = _controller.Search();
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, status.StatusCode);
        }

        [Fact]
        public void Search_Returns_200_When_Service_Is_Not_Null()
        {
            A.CallTo(() => _service.SearchForMetadata(A<MetaSearchQuery>._)).Returns(new Response());

            var result = _controller.Search();
            var okResult = result.Result as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);
        }
        [Fact]
        public void Search_Returns_A_List_Of_Metas_When_Service_Is_Not_Null()
        {
            A.CallTo(() => _service.SearchForMetadata(A<MetaSearchQuery>._)).Returns(new Response { Data = new List<MetaData>() });

            var result = _controller.Search();
            var okResult = result.Result as OkObjectResult;

            Assert.NotNull(okResult.Value);
            Assert.IsType<List<MetaData>>(((Response)okResult.Value).Data);
        }

        #region GetAll

        [Fact]
        public void GetAll_Returns_Status_500_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.GetAll()).Returns(null);

            var result = _controller.GetAll();
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, status.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_Status_200_Metas_Is_Found()
        {
            A.CallTo(() => _service.GetAll()).Returns(_listResponse);

            var result = _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetAll_Returns_A_List_Of_Metas_If_There_Exists_Metas()
        {
            A.CallTo(() => _service.GetAll()).Returns(_listResponse);

            var result = _controller.GetAll();
            var okResult = result.Result as OkObjectResult;

            Assert.IsType<List<MetaData>>((okResult.Value as Response).Data);
        }

        #region GetById

        [Fact]
        public void GetById_Returns_MetaData_When_Id_Is_Valid()
        {
            A.CallTo(() => _service.GetById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetById(0);
            var okresult = result.Result as OkObjectResult;

            var response = ((Response)okresult.Value);
            Assert.NotNull(response.Data);
            Assert.IsType<MetaData>(response.Data);
        }

        [Fact]
        public void GetById_Returns_Status_200_When_A_MetaData_Is_Found()
        {
            A.CallTo(() => _service.GetById(A<int>._)).Returns(_singleResponse);

            var result = _controller.GetById(0);
            var okresult = result.Result as OkObjectResult;

            Assert.Equal(200, okresult.StatusCode);
        }

        [Fact]
        public void GetById_Returns_404_When_Id_Is_Not_Valid()
        {
            A.CallTo(() => _service.GetById(A<int>._)).Returns(new Response{Data = null});

            var result = _controller.GetById(0);
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.NotFound, status.StatusCode);
        }

        [Fact]
        public void GetById_Returns_500_When_Service_Returns_Null()
        {
            A.CallTo(() => _service.GetById(A<int>._)).Returns(null);

            var result = _controller.GetById(0);
            var status = result.Result as StatusCodeResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, status.StatusCode);
        }

        #endregion
        #endregion
    }
}
