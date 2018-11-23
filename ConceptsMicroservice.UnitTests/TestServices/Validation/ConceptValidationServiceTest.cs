﻿using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestServices.Validation
{
    public class ConceptValidationServiceTest
    {
        private readonly IConceptValidationService _validationService;
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ConceptValidationServiceTest()
        {
            _statusRepository = A.Fake<IStatusRepository>();
            _metadataRepository = A.Fake<IMetadataRepository>();
            _categoryRepository = A.Fake<ICategoryRepository>();
            _validationService = new ConceptValidationService(_statusRepository, _metadataRepository, _categoryRepository);
        }
        #region StatusIdIsValid
        [Fact]
        public void StatusIdIsValidId_Returns_False_When_Id_Does_Not_Exist_In_The_DB()
        {
            A.CallTo(() => _statusRepository.GetById(A<int>._)).Returns(null);

            Assert.False(_validationService.StatusIdIsValidId(0));
        }
        [Fact]
        public void StatusIdIsValidId_Returns_True_When_Id_Does_Exist_In_The_DB()
        {
            A.CallTo(() => _statusRepository.GetById(A<int>._)).Returns(new Status());

            Assert.True(_validationService.StatusIdIsValidId(0));
        }
        #endregion
        #region MetaIdsDoesNotExistInDatabase
        [Fact]
        public void MetaIdsDoesNotExistInDatabase_Returns_Empty_List_When_Input_Is_Empty()
        {
            Assert.Empty(_validationService.MetaIdsDoesNotExistInDatabase(new List<int>()));
        }
        [Fact]
        public void MetaIdsDoesNotExistInDatabase_Returns_Empty_List_When_Input_Is_Null()
        {
            Assert.Empty(_validationService.MetaIdsDoesNotExistInDatabase(null));
        }
        [Fact]
        public void MetaIdsDoesNotExistInDatabase_Returns_Empty_List_When_All_Ids_Exist_DB()
        {
            A.CallTo(() => _metadataRepository.GetById(1)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(2)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(3)).Returns(new MetaData());

            Assert.Empty(_validationService.MetaIdsDoesNotExistInDatabase(new List<int>{1,2,3}));
        }

        [Fact]
        public void MetaIdsDoesNotExistInDatabase_Returns_List_Of_Ids_When_Not_All_Exists_In_DB()
        {
            A.CallTo(() => _metadataRepository.GetById(1)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(2)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(3)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(4)).Returns(null);

            var notExistingIds = _validationService.MetaIdsDoesNotExistInDatabase(new List<int> {1, 2, 3, 4});
            Assert.Single(notExistingIds);
        }

        #endregion
        #region GetMissingRequiredCategories

        [Fact]
        public void GetMissingRequiredCategories_Returns_All_Required_Categories_When_Input_Is_Null()
        {
            var presentCategory = new MetaCategory { Name = "Present", Id = 1 };
            var missingCategory = new MetaCategory { Name = "Missing", Id = 2 };
            var requiredCategories = new List<MetaCategory> { missingCategory, presentCategory };
            A.CallTo(() => _categoryRepository.GetRequiredCategories()).Returns(requiredCategories);
            Assert.NotEmpty(_validationService.GetMissingRequiredCategories(null));
        }

        [Fact]
        public void GetMissingRequiredCategories_Returns_All_Required_Categories_When_Input_Is_Empty()
        {
            var presentCategory = new MetaCategory { Name = "Present", Id = 1 };
            var missingCategory = new MetaCategory { Name = "Missing", Id = 2 };
            var requiredCategories = new List<MetaCategory> { missingCategory, presentCategory };
            A.CallTo(() => _categoryRepository.GetRequiredCategories()).Returns(requiredCategories);

            Assert.NotEmpty(_validationService.GetMissingRequiredCategories(new List<int>()));
        }

        [Fact]
        public void GetMissingRequiredCategories_Returns_Missing_Required_Categories_When_They_Are_Missing()
        {
            var presentCategory = new MetaCategory {Name = "Present", Id = 1};
            var missingCategory = new MetaCategory {Name = "Missing", Id = 2};
            var requiredCategories = new List<MetaCategory>{missingCategory, presentCategory};
            var presentMeta = new MetaData {Category = presentCategory, Id = 1};

            A.CallTo(() => _metadataRepository.GetByRangeOfIds(A<List<int>>._)).Returns(new List<MetaData>{ presentMeta });
            A.CallTo(() => _categoryRepository.GetRequiredCategories()).Returns(requiredCategories);

            Assert.Single(_validationService.GetMissingRequiredCategories(new List<int>{presentMeta.Id}));
        }

        [Fact]
        public void GetMissingRequiredCategories_Returns_Empty_List_When_All_Required_Categories_Is_Present()
        {
            var presentCategory = new MetaCategory { Name = "Present", Id = 1 };
            var requiredCategories = new List<MetaCategory> { presentCategory };
            var presentMeta = new MetaData { Category = presentCategory, Id = 1 };

            A.CallTo(() => _metadataRepository.GetByRangeOfIds(A<List<int>>._)).Returns(new List<MetaData> { presentMeta });
            A.CallTo(() => _categoryRepository.GetRequiredCategories()).Returns(requiredCategories);

            Assert.Empty(_validationService.GetMissingRequiredCategories(new List<int> { presentMeta.Id }));
        }
        #endregion
        
    }
}