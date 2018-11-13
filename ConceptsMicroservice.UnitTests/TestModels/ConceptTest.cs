/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.UnitTests.Helpers;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestModels
{
    public class ConceptTest
    {
        #region Title
        [Fact]
        public void Title_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.Title)));
        }
        #endregion
        #region Content
        [Fact]
        public void Content_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.Content)));
        }
        #endregion
        #region Author

        [Fact]
        public void Author_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.Author)));
        }
        #endregion
        #region Source

        [Fact]
        public void Source_Should_Not_Have_Attribute_Required()
        {
            Assert.False(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.Source)));
        }
        #endregion
    }
}
