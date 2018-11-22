/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class StatusIdExistsInDatabaseAttributeTest : BaseAttributeTest
    {
        private readonly StatusIdExistsInDatabaseAttribute _attribute;

        public StatusIdExistsInDatabaseAttributeTest()
        {
            _attribute = new StatusIdExistsInDatabaseAttribute();
        }

        [Fact]
        public void Service_Is_NullShould_ThrowValidationException()
        {
            const int id = 1;
            A.CallTo(() => ServiceProvider.GetService(typeof(IConceptValidationService))).Returns(null);

            
            Assert.Throws<ValidationException>(() => _attribute.Validate(id, ValidationContext));
        }

        [Fact]
        public void Status_Id_IsValidShould_ReturnValidationSuccess()
        {
            const int id = 1;
            A.CallTo(() => ValidationService.StatusIdIsValidId(id)).Returns(true);


            var validationException = Record.Exception(() => _attribute.Validate(id, ValidationContext));
            Assert.Null(validationException);
        }

        [Fact]
        public void Status_Id_Is_NotValidShould_ThrowValidationException()
        {
            const int id = 1;
            A.CallTo(() => ValidationService.StatusIdIsValidId(id)).Returns(false);

            Assert.Throws<ValidationException>(() => _attribute.Validate(id, ValidationContext));
        }
        
    }
}
