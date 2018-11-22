/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes
{
    public class StatusIdExistsInDatabaseAttributeTest
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ValidationContext _validationContext;
        private readonly StatusIdExistsInDatabaseAttribute _attribute;
        private readonly IStatusRepository _statusRepository;

        public StatusIdExistsInDatabaseAttributeTest()
        {
            _statusRepository = A.Fake<IStatusRepository>();
            _serviceProvider = A.Fake<IServiceProvider>();
            _validationContext = new ValidationContext(new object(), _serviceProvider, null);
            _attribute = new StatusIdExistsInDatabaseAttribute();
        }

        [Fact]
        public void Service_Is_Null_Should_Throw_ValidationException()
        {
            const int id = 1;
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(null);

            
            Assert.Throws<ValidationException>(() => _attribute.Validate(id, _validationContext));
        }

        [Fact]
        public void Status_Id_Is_Valid_Should_Return_ValidationSuccess()
        {
            const int id = 1;
            A.CallTo(() => _statusRepository.GetById(id)).Returns(new Status());
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(new ConceptValidationService(_statusRepository));


            var validationException = Record.Exception(() => _attribute.Validate(id, _validationContext));
            Assert.Null(validationException);
        }

        [Fact]
        public void Status_Id_Is_Not_Valid_Should_Throw_ValidationException()
        {
            const int id = 1;
            A.CallTo(() => _statusRepository.GetById(id)).Returns(null);
            A.CallTo(() => _serviceProvider.GetService(typeof(IConceptValidationService))).Returns(new ConceptValidationService(_statusRepository));

            Assert.Throws<ValidationException>(() => _attribute.Validate(id, _validationContext));
        }
        
    }
}
