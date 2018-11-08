/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestModels
{
    public class BaseResponseTest
    {
        [Fact]
        public void Errors_Should_Have_Attribute_JsonIgnored()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<BaseResponse, JsonIgnoreAttribute>(nameof(BaseResponse.Errors)));
        }
        [Fact]
        public void RenderedErrors_Should_Have_JsonProperty()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<BaseResponse, JsonPropertyAttribute>(nameof(BaseResponse.RenderedErrors)));
        }

        [Fact]
        public void HasErrors_Is_False_When_Errors_Is_Null()
        {
            var model = new BaseResponse {Errors = null};
            Assert.False(model.HasErrors());
        }
        [Fact]
        public void HasErrors_Is_False_When_Errors_Is_Empty()
        {
            var model = new BaseResponse { Errors = new ModelStateDictionary() };
            Assert.False(model.HasErrors());
        }
        [Fact]
        public void HasErrors_Is_True_When_Errors_Exists()
        {
            var model = new BaseResponse { Errors = new ModelStateDictionary() };
            model.Errors.TryAddModelError("ex", "ex");
            Assert.True(model.HasErrors());
        }

        [Fact]
        public void RenderedErrors_Is_Null_If_Errors_Is_Null()
        {
            var model = new BaseResponse { Errors = null };
            Assert.Null(model.RenderedErrors);
        }

        [Fact]
        public void RenderedErrors_Is_Null_If_Errors_Is_Empty()
        {
            var model = new BaseResponse { Errors = new ModelStateDictionary() };
            Assert.Null(model.RenderedErrors);
        }
        [Fact]
        public void RenderedErrors_Contains_Errors_If_Errors_Contains_Erroes()
        {
            var model = new BaseResponse { Errors = new ModelStateDictionary() };
            model.Errors.TryAddModelError("ex", "ex");
            Assert.Equal(model.RenderedErrors.Count, model.Errors.Count);
        }
    }
}
