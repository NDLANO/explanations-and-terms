using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Repositories;
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
