﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.UnitTests.Mock;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories
{
    [Collection("Database tests")]
    public class StatusRepositoryTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected readonly IStatusRepository StatusRepository;
        protected IMock Mock;
        protected BaseListQuery QueryWithDefaultValues;
        
        private readonly Status _status;

        public StatusRepositoryTest()
        {
            QueryWithDefaultValues = BaseListQuery.DefaultValues("nb");
            Mock = new Mock.Mock();
            StatusRepository = new Repositories.StatusRepository(Mock.Database.Context);
            _status = Mock.MockStatus();
        }

        public void Dispose()
        {
            Mock.Dispose();
        }


        #region SearchForName


        [Fact]
        public void GetByName_Returns_A_Status()
        {
            Assert.Empty(StatusRepository.GetAll(QueryWithDefaultValues));

            Mock.Database.InsertStatus(_status);
            
            Assert.NotNull(StatusRepository.GetByName(_status.TypeGroup.Name));
        }
        [Fact]
        public void GetByName_Returns_Null_If_Not_Found()
        {
            Assert.Empty(StatusRepository.GetAll(QueryWithDefaultValues));
            Assert.Null(StatusRepository.GetByName("not found"));
        }
        #endregion

        #region GetAll
        [Fact]
        public void GetAll_Fetches_A_List_Of_Status()
        {
            Assert.Empty(StatusRepository.GetAll(QueryWithDefaultValues));

            Mock.Database.InsertStatus(_status);

            Assert.NotEmpty(StatusRepository.GetAll(QueryWithDefaultValues));
        }
        [Fact]
        public void GetAll_Returns_EmptyList_If_No_Status_Exists()
        {
            Assert.Empty(StatusRepository.GetAll(QueryWithDefaultValues));
        }
        #endregion

        }
}
