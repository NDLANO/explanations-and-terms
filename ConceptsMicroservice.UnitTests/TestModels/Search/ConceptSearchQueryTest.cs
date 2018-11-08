/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestModels.Search
{
    
    public class ConceptSearchQueryTest
    {
        [Fact]
        public void Title_Should_Have_Attribute_FromQuery()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<ConceptSearchQuery, FromQueryAttribute>(nameof(ConceptSearchQuery.Title)));
        }
        [Fact]
        public void QueryDataList_Should_Have_Attribute_FromQuery()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<ConceptSearchQuery, FromQueryAttribute>(nameof(ConceptSearchQuery.MetaIds)));
        }

        [Fact]
        public void MetaIds_Is_Empty_List_When_MetaIds_Is_Empty()
        {
            var query = new ConceptSearchQuery { MetaIds = new List<int>() };
            Assert.Empty(query.MetaIds);
        }

        [Fact]
        public void HasQuery_Is_False_When_No_Title_And_MetaIds_Is_Null()
        {
            var query = new ConceptSearchQuery { Title = null, MetaIds = null };
            Assert.False(query.HasQuery());
        }

        [Fact]
        public void HasQuery_Is_False_When_No_Title_And_MetaIds_Is_Empty()
        {
            var query = new ConceptSearchQuery {Title = null, MetaIds = new List<int>()};
            Assert.False(query.HasQuery());
        }

        [Fact]
        public void HasQuery_Is_True_When_Title_Has_Content_And_MetaIds_Is_Empty()
        {
            var query = new ConceptSearchQuery { Title = "Title", MetaIds = new List<int>() };
            Assert.True(query.HasQuery());
        }

        [Fact]
        public void HasQuery_Is_True_When_Title_Has_Content_And_MetaIds_Is_Null()
        {
            var query = new ConceptSearchQuery { Title = "Title", MetaIds = null };
            Assert.True(query.HasQuery());
        }

        [Fact]
        public void HasQuery_Is_True_When_No_Title_And_MetaIds_Has_Elements()
        {
            var query = new ConceptSearchQuery { Title = null, MetaIds = new List<int>{1} };
            Assert.True(query.HasQuery());
        }
    }
}
