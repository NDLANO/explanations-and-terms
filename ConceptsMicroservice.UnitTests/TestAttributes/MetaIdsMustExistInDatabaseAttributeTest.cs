/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes
{
    public class MetaIdsMustExistInDatabaseAttributeTest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ValidationContext _validationContext;
        private readonly MetaIdsMustExistInDatabaseAttribute _attribute;
        private readonly IStatusRepository _statusRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IConceptValidationService _validationService;

        public MetaIdsMustExistInDatabaseAttributeTest()
        {
            _metadataRepository = A.Fake<IMetadataRepository>();
            _statusRepository = A.Fake<IStatusRepository>();
            _serviceProvider = A.Fake<IServiceProvider>();
            _validationContext = new ValidationContext(new object(), _serviceProvider, null);
            _attribute = new MetaIdsMustExistInDatabaseAttribute();
            _validationService = new ConceptValidationService(_statusRepository, _metadataRepository);
        }

        [Fact]
        public void Service_Is_Null_Should_Throw_ValidationException()
        {
            const int id = 1;
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(null);

            
            Assert.Throws<ValidationException>(() => _attribute.Validate(id, _validationContext));
        }

        [Fact]
        public void MetaIds_Contains_Only_Ids_In_Database_Should_Return_ValidationSuccess()
        {
            A.CallTo(() => _metadataRepository.GetById(A<int>._)).Returns(new MetaData());
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(_validationService);

            var listOfIdsFromDb = new List<int> {1, 2, 3};

            var validationException = Record.Exception(() => _attribute.Validate(listOfIdsFromDb, _validationContext));
            Assert.Null(validationException);
        }

        [Fact]
        public void MetaIds_Contains_Some_Ids_Not_In_Database_Should_Throw_ValidationException()
        {

            A.CallTo(() => _metadataRepository.GetById(A<int>._)).Returns(new MetaData());
            A.CallTo(() => _metadataRepository.GetById(4)).Returns(null);
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(_validationService);

            var listOfIdsFromDb = new List<int> { 1, 2, 3, 4 };

            var validationException = Record.Exception(() => _attribute.Validate(listOfIdsFromDb, _validationContext));
            Assert.IsType<ValidationException>(validationException);
        }
        
    }
}
