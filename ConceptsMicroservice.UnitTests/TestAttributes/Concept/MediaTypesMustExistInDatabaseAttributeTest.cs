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
using ConceptsMicroservice.Models.DTO;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class MediaTypesMustExistInDatabaseAttributeTest : BaseAttributeTest
    {
        private readonly MediaTypesMustExistInDatabaseAttribute _attribute;

        private List<MediaWithMediaType> _listOfIdsFromDb;
        public MediaTypesMustExistInDatabaseAttributeTest()
        {
            _attribute = new MediaTypesMustExistInDatabaseAttribute();
            _listOfIdsFromDb = new List<MediaWithMediaType>
            {
                new MediaWithMediaType
                {
                    MediaTypeId = 1
                },
                new MediaWithMediaType
                {
                    MediaTypeId = 2
                },
                new MediaWithMediaType
                {
                    MediaTypeId = 3
                }
            };
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
            A.CallTo(() => ValidationService.MediaTypesNotExistInDatabase(A<List<MediaWithMediaType>>._, A<string>._)).Returns(new List<int>());

            var validationException = Record.Exception(() => _attribute.Validate(_listOfIdsFromDb, ValidationContext));
            Assert.Null(validationException);
        }

        [Fact]
        public void MetaIds_Contains_Some_Ids_Not_In_Database_Should_Throw_ValidationException()
        {
            A.CallTo(() => ValidationService.MediaTypesNotExistInDatabase(A<List<MediaWithMediaType>>._, A<string>._)).Returns(new List<int> { 1 });

            var validationException = Record.Exception(() => _attribute.Validate(_listOfIdsFromDb, ValidationContext));
            Assert.IsType<ValidationException>(validationException);
        }

    }
}
