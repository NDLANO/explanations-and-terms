/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConceptsMicroservice.Attributes;
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
        #region AuthorName

        [Fact]
        public void Author_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.AuthorName)));
        }
        #endregion
        #region Source

        [Fact]
        public void Source_Should_Not_Have_Attribute_Required()
        {
            Assert.False(AttributeHelper.AttributeIsPresentOnProperty<Concept, RequiredAttribute>(nameof(Concept.Source)));
        }
        #endregion
        #region StatusId

        [Fact]
        public void StatusId_Should_Have_Attribute_StatusIdExistsInDatabase()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, StatusIdExistsInDatabaseAttribute>(nameof(Concept.StatusId)));
        }
        #endregion
        #region Meta
        [Fact]
        public void Meta_Should_Have_Attribute_NotMapped()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, NotMappedAttribute>(nameof(Concept.Meta)));
        }
        #endregion
        #region MetaIds
        [Fact]
        public void MetaIds_Should_Have_Attribute_NoDuplicateIntValues()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, NoDuplicateIntValuesAttribute>(nameof(Concept.MetaIds)));
        }
        [Fact]
        public void MetaIds_Should_Have_Attribute_MustContainOneOfEachCategoryAttribute()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Concept, MustContainOneOfEachCategoryAttribute>(nameof(Concept.MetaIds)));
        }
        #endregion
    }
}
