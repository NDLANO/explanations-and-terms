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
using Microsoft.AspNetCore.Http;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class BaseAttributeTest
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly HttpContext HttpContext;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ValidationContext ValidationContext;
        protected readonly IConceptValidationService ValidationService;

        public BaseAttributeTest()
        {
            ServiceProvider = A.Fake<IServiceProvider>();
            ValidationService = A.Fake<IConceptValidationService>();
            HttpContextAccessor = A.Fake<IHttpContextAccessor>();
            HttpContext = new DefaultHttpContext();
            ValidationContext = new ValidationContext(new object(), ServiceProvider, null);

            A.CallTo(() => HttpContextAccessor.HttpContext).Returns(HttpContext);
            A.CallTo(() => ServiceProvider.GetService(typeof(IConceptValidationService))).Returns(ValidationService);
            A.CallTo(() => ServiceProvider.GetService(typeof(IHttpContextAccessor))).Returns(HttpContextAccessor);
        }
    }
}
