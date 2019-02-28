/**
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
    public class CategoryRepositoryTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected readonly ICategoryRepository CategoryRepository;
        protected IMock Mock;
        private readonly string _language;
        private readonly MetaCategory _category;
        private readonly BaseListQuery _queryWithDefaultValues;

        public CategoryRepositoryTest()
        {
            _queryWithDefaultValues = BaseListQuery.DefaultValues("nb");
            Mock = new Mock.Mock();
            CategoryRepository = new Repositories.CategoryRepository(Mock.Database.Context);
            _category = Mock.MockCategory();
            _language = "nb";
        }

        public void Dispose()
        {
            Mock.Dispose();
        }

        #region GetById

        [Fact]
        public void GetById_Fetches_Category()
        {
            Assert.Empty(CategoryRepository.GetAll(_queryWithDefaultValues));

            var id = Mock.Database.InsertCategory(_category).Id;

            Assert.NotNull(CategoryRepository.GetById(id));
        }
        [Fact]
        public void GetById_Returns_Null_If_Category_Does_Not_Exist()
        {
            Assert.Empty(CategoryRepository.GetAll(_queryWithDefaultValues));
            Assert.Null(CategoryRepository.GetById(1));
        }

        #endregion

        #region GetAll


        [Fact]
        public void GetAll_Fetches_A_List_Of_Categories()
        {
            Assert.Empty(CategoryRepository.GetAll(_queryWithDefaultValues));

            Mock.Database.InsertCategory(_category);

            Assert.NotEmpty(CategoryRepository.GetAll(_queryWithDefaultValues));
        }

        [Fact]
        public void GetAll_Returns_EmptyList_If_Categories_Does_Not_Exist()
        {
            Assert.Empty(CategoryRepository.GetAll(_queryWithDefaultValues));
        }
        #endregion

        #region GetRequiredCategories

        [Fact]
        public void GetRequiredCategories_Returns_EmptyList_If_No_Required_Categories_Exists()
        {
            Assert.Empty(CategoryRepository.GetAll(BaseListQuery.DefaultValues(_language)));
            var notRequiredCategory = Mock.MockCategory();
            notRequiredCategory.IsRequired = false;
            Mock.Database.InsertCategory(notRequiredCategory);

            Assert.Empty(CategoryRepository.GetRequiredCategories(_language));
        }
        [Fact]
        public void GetRequiredCategories_Returns_List_Of_Categories_When_Some_Exists()
        {

            Assert.Empty(CategoryRepository.GetAll(BaseListQuery.DefaultValues(_language)));
            var notRequiredCategory = Mock.MockCategory();
            notRequiredCategory.IsRequired = false;

            var requiredCategory = Mock.MockCategory();
            requiredCategory.IsRequired = true;
            Mock.Database.InsertCategory(requiredCategory);

            Assert.Single(CategoryRepository.GetRequiredCategories(_language));
        }
        #endregion
    }
}
