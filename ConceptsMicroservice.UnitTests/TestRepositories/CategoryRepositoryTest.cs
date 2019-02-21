/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using System.Data;
using ConceptsMicroservice.Context;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.UnitTests.Mock;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories
{
    [Collection("Database tests")]
    public class CategoryRepositoryTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected readonly ICategoryRepository CategoryRepository;
        protected IMock Mock;
        
        private readonly MetaCategory _category;

        public CategoryRepositoryTest()
        {
            //Nasser 14.02.2019
            Mock = new Mock.Mock();
            CategoryRepository = new Repositories.CategoryRepository(Mock.Database.Context);
            //_category = Mock.MockCategory();
        }

        public void Dispose()
        {
            Mock.Dispose();
        }

        #region GetById

        [Fact]
        public void GetById_Fetches_Category()
        {
            Assert.Empty(CategoryRepository.GetAll());

            var id = Mock.Database.InsertCategory(_category).Id;

            Assert.NotNull(CategoryRepository.GetById(id));
        }
        [Fact]
        public void GetById_Returns_Null_If_Category_Does_Not_Exist()
        {
            Assert.Empty(CategoryRepository.GetAll());
            Assert.Null(CategoryRepository.GetById(1));
        }

        #endregion

        #region GetAll


        [Fact]
        public void GetAll_Fetches_A_List_Of_Categories()
        {
            //Nasser 14.02.2019
            Assert.Empty(CategoryRepository.GetAll());

            Mock.Database.InsertCategory(_category);

            Assert.NotEmpty(CategoryRepository.GetAll());

            //var fakeMock = A.Fake<IMock>();
            //var fakeDB = A.Fake<IMockDatabase>();

            //fakeDB.Context.Results = A.Fake<DbContextOptionsBuilder<Context.DbContext>>();

            //var fakeAllCategories = A.Fake<List<MetaCategory>>();
            //var fakeCategoryRepositories = A.Fake<CategoryRepository>();
            //ICategoryRepository cat = new CategoryRepository(fakeMock.Database.Context);
            //var fakeCategoryRepository = new fakeCategoryRepositories(fakeMock.Database.Context);// A.Fake<ICategoryRepository>();
            //fakeCategoryRepository.
            //A.CallTo(() => fakeCategoryRepositories.GetAll()).Returns(fakeAllCategories);
        }

        [Fact]
        public void GetAll_Returns_EmptyList_If_Categories_Does_Not_Exist()
        {
            Assert.Empty(CategoryRepository.GetAll());
        }
        #endregion

        #region GetRequiredCategories

        [Fact]
        public void GetRequiredCategories_Returns_EmptyList_If_No_Required_Categories_Exists()
        {

            Assert.Empty(CategoryRepository.GetAll());
            var notRequiredCategory = Mock.MockCategory();
            notRequiredCategory.IsRequired = false;
            Mock.Database.InsertCategory(notRequiredCategory);

            Assert.Empty(CategoryRepository.GetRequiredCategories());
        }
        [Fact]
        public void GetRequiredCategories_Returns_List_Of_Categories_When_Some_Exists()
        {

            Assert.Empty(CategoryRepository.GetAll());
            var notRequiredCategory = Mock.MockCategory();
            notRequiredCategory.IsRequired = false;

            var requiredCategory = Mock.MockCategory();
            requiredCategory.IsRequired = true;
            Mock.Database.InsertCategory(requiredCategory);

            Assert.Single(CategoryRepository.GetRequiredCategories());
        }
        #endregion
    }
}
