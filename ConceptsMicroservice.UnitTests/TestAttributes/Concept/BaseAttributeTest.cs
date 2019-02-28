/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class BaseAttributeTest
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ValidationContext ValidationContext;
        protected readonly IConceptValidationService ValidationService;

        public BaseAttributeTest()
        {
            ServiceProvider = A.Fake<IServiceProvider>();
            ValidationService = A.Fake<IConceptValidationService>();
            ValidationContext = new ValidationContext(new object(), ServiceProvider, null);
            A.CallTo(() => ServiceProvider.GetService(typeof(IConceptValidationService))).Returns(ValidationService);
        }
    }
}
