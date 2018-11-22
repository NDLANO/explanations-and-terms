/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class MetaIdsMustExistInDatabaseAttributeTest : BaseAttributeTest
    {
        private readonly MetaIdsMustExistInDatabaseAttribute _attribute;

        public MetaIdsMustExistInDatabaseAttributeTest()
        {
            _attribute = new MetaIdsMustExistInDatabaseAttribute();
        }

        [Fact]
        public void Service_Is_Null_Should_Throw_ValidationException()
        {
            const int id = 1;
            A.CallTo(() => ServiceProvider.GetService(typeof(IConceptValidationService))).Returns(null);
            
            Assert.Throws<ValidationException>(() => _attribute.Validate(id, ValidationContext));
        }

        [Fact]
        public void MetaIds_Contains_Only_Ids_In_Database_Should_Return_ValidationSuccess()
        {
            A.CallTo(() => ValidationService.MetaIdsDoesNotExistInDatabase(A<List<int>>._)).Returns(new List<int>());

            var listOfIdsFromDb = new List<int> {1, 2, 3};

            var validationException = Record.Exception(() => _attribute.Validate(listOfIdsFromDb, ValidationContext));
            Assert.Null(validationException);
        }

        [Fact]
        public void MetaIds_Contains_Some_Ids_Not_In_Database_Should_Throw_ValidationException()
        {
            A.CallTo(() => ValidationService.MetaIdsDoesNotExistInDatabase(A<List<int>>._)).Returns(new List<int>{1});

            var listOfIdsFromDb = new List<int> { 1, 2, 3, 4 };

            var validationException = Record.Exception(() => _attribute.Validate(listOfIdsFromDb, ValidationContext));
            Assert.IsType<ValidationException>(validationException);
        }
        
    }
}
