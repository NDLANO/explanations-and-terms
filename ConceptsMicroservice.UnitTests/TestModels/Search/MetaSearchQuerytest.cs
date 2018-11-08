/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestModels.Search
{
    public class MetaSearchQueryTest
    {
        [Fact]
        public void Name_Should_Have_Attribute_FromQuery()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<MetaSearchQuery, FromQueryAttribute>(nameof(MetaSearchQuery.Name)));
        }
        [Fact]
        public void Category_Should_Have_Attribute_FromQuery()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<MetaSearchQuery, FromQueryAttribute>(nameof(MetaSearchQuery.Category)));
        }
        
        [Fact]
        public void HasNoQuery_Is_False_When_Category_Is_Specified_And_Name_Is_Null()
        {
            var query = new MetaSearchQuery
            {
                Name = null,
                Category = "Category"
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Category_Is_Specified_And_Name_Is_Empty()
        {
            var query = new MetaSearchQuery
            {
                Name = "",
                Category = "Category"
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Category_Is_Specified_And_Name_Is_Whitespace()
        {
            var query = new MetaSearchQuery
            {
                Name = " ",
                Category = "Category"
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Name_Is_Specified_And_Category_Is_Null()
        {
            var query = new MetaSearchQuery
            {
                Name = "name",
                Category = null
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Name_Is_Specified_And_Category_Is_Empty()
        {
            var query = new MetaSearchQuery
            {
                Name = "name",
                Category = ""
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Name_Is_Specified_And_Category_Is_Whitespace()
        {
            var query = new MetaSearchQuery
            {
                Name = "name",
                Category = " "
            };
            Assert.False(query.HasNoQuery());
        }
        [Fact]
        public void HasNoQuery_Is_False_When_Name_And_Category_Is_Specified()
        {
            var query = new MetaSearchQuery
            {
                Name = "name",
                Category = "category"
            };
            Assert.False(query.HasNoQuery());
        }

    }
}
