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
using ConceptsMicroservice.Models.Configuration;
using ConceptsMicroservice.Services.Validation;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes.Concept
{
    public class MustContainOneOfEachCategoryAttributeTest : BaseAttributeTest
    {
        private readonly MustContainOneOfEachCategoryAttribute _attribute;

        public MustContainOneOfEachCategoryAttributeTest()
        {
            _attribute = new MustContainOneOfEachCategoryAttribute();
        }

        [Fact]
        public void ValidatingService_is_Null_Should_Throw_ValidationException()
        {
            A.CallTo(() => ServiceProvider.GetService(typeof(IConceptValidationService))).Returns(null);
            
            Assert.Throws<ValidationException>(() => _attribute.Validate(new List<int>{1}, ValidationContext));
        }

        [Fact]
        public void LanguageService_is_Null_Should_Throw_ValidationException()
        {
            A.CallTo(() => ServiceProvider.GetService(typeof(IOptions<LanguageConfig>))).Returns(null);

            Assert.Throws<ValidationException>(() => _attribute.Validate(new List<int> { 1 }, ValidationContext));
        }

        [Fact]
        public void HttpContext_is_Null_Should_Throw_ValidationException()
        {
            A.CallTo(() => ServiceProvider.GetService(typeof(IHttpContextFactory))).Returns(null);

            Assert.Throws<ValidationException>(() => _attribute.Validate(new List<int> { 1 }, ValidationContext));
        }

        [Fact]
        public void Returns_ValidationSuccess_When_There_Is_No_Missing_Categories()
        {
            SucceedConditions();
            var exception = Record.Exception(() => _attribute.Validate(new List<int> { 1 }, ValidationContext));
            Assert.Null(exception);
        }

        [Fact]
        public void Throws_ValidationException_When_There_Is_Missing_Categories()
        {
            SucceedConditions();
            A.CallTo(() => ValidationService.GetMissingRequiredCategories(A<List<int>>._, A<string>._)).Returns(new List<string>{"Missing!"});

            Assert.Throws<ValidationException>(() => _attribute.Validate(new List<int> { 1 }, ValidationContext));
        }

        private void SucceedConditions()
        {

            A.CallTo(() => ValidationService.GetMissingRequiredCategories(A<List<int>>._, A<string>._)).Returns(new List<string>());
            A.CallTo(() => ServiceProvider.GetService(typeof(IOptions<LanguageConfig>))).Returns(new OptionsWrapper<LanguageConfig>(new LanguageConfig()));
            var httpContextFake = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => ServiceProvider.GetService(typeof(IHttpContextAccessor))).Returns(httpContextFake);
            A.CallTo(() => httpContextFake.HttpContext).Returns(new DefaultHttpContext());
        }
    }
}
