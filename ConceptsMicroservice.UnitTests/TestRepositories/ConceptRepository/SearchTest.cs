/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models.Search;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories.ConceptRepository
{
    
    [Collection("Database tests")]
    public class SearchTest : BaseTest
    {
        private readonly int itemsPrPage = 10;
        private readonly int pageNumber = 2;
        private readonly string language = "en";
        private readonly string defaultLanguage = "nb";

        [Fact]
        public void Search_Returns_All_Concepts_If_Query_Is_Null()
        {
            Mock.Database.CreateAndInsertAConcept();
            var concepts = ConceptRepository.SearchForConcepts(null);

            Assert.NotEmpty(concepts);
        }

        [Fact]
        public void Search_Returns_Empty_List_If_No_Concepts_Exists()
        {
            var concepts = ConceptRepository.SearchForConcepts(null);

            Assert.Empty(concepts);
        }

        [Fact]
        public void Search_Returns_All_Concepts_If_No_Query_Is_Specified()
        {
            Mock.Database.CreateAndInsertAConcept();
            var concepts = ConceptRepository.SearchForConcepts(new ConceptSearchQuery());

            Assert.NotEmpty(concepts);
        }

        [Fact]
        public void Search_Returns_Concepts_When_Only_Specified_Title()
        {
            var query = new ConceptSearchQuery{Title = "ConceptSearchQueryTitle" };

            var status = Mock.Database.InsertStatus(Mock.MockStatus());

            var category1 = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, category1));

            var category2 = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta2 = Mock.Database.InsertMeta(Mock.MockMeta(status, category2));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> {meta1.Id};
            Mock.Database.InsertConcept(c1);

            var c2 = Mock.MockConcept(status);
            c2.MetaIds = new List<int> { meta2.Id };
            c2.Title = $"Should match {query.Title}";
            Mock.Database.InsertConcept(c2);

            Assert.Equal(2, ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage).Count);

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.Single(searchResult);
        }

        [Fact]
        public void Search_Returns_Concepts_By_MetaId_Only()
        {
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            
            var category1 = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, category1));

            var category2 = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta2 = Mock.Database.InsertMeta(Mock.MockMeta(status, category2));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> { meta1.Id };
            Mock.Database.InsertConcept(c1);

            var c2 = Mock.MockConcept(status);
            c2.MetaIds = new List<int> { meta2.Id };
            Mock.Database.InsertConcept(c2);

            var query = new ConceptSearchQuery
            {
                MetaIds = new List<int> {meta1.Id }
            };

            Assert.Equal(2, ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage).Count);

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.Single(searchResult);
        }

        [Fact]
        public void Search_Returns_Everything_When_MetaId_And_Title_Is_Null()
        {
            var status = Mock.MockStatus();
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, Mock.MockCategory()));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> { meta1.Id };
            Mock.Database.InsertConcept(c1);
            
            var query = new ConceptSearchQuery();


            Assert.Single(ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage));

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.NotEmpty(searchResult);
        }

        [Fact]
        public void Search_Returns_Everything_When_MetaId_Is_Empty_And_Title_Is_Null()
        {
            var status = Mock.MockStatus();
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, Mock.MockCategory()));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> { meta1.Id };
            Mock.Database.InsertConcept(c1);
            
            var query = new ConceptSearchQuery
            {
                MetaIds = new List<int>()
            };


            Assert.Single(ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage));

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.NotEmpty(searchResult);
        }
        
        [Fact]
        public void Search_Returns_Concept_When_Title_And_MetaId_Is_Specified()
        {
            const string title = "title";

            var status = Mock.Database.InsertStatus(Mock.MockStatus());

            var category1 = Mock.Database.InsertCategory(Mock.MockCategory());
            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, category1));
            var meta2 = Mock.Database.InsertMeta(Mock.MockMeta(Mock.MockStatus(), Mock.MockCategory()));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> { meta1.Id };
            c1.Title = title;
            Mock.Database.InsertConcept(c1);

            var c2 = Mock.MockConcept(status);
            c2.MetaIds = new List<int> { meta2.Id };
            c2.Title = title;
            Mock.Database.InsertConcept(c2);


            var query = new ConceptSearchQuery
            {
                Title = title,
                MetaIds = new List<int> { meta1.Id}
            };

            Assert.Equal(2, ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage).Count);

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.Single(searchResult);
        }

        [Fact]
        public void Search_Returns_Concept_When_Query_Consist_Of_Multiple_Valid_Meta_Queries()
        {
            const string title = "title";

            var status = Mock.Database.InsertStatus(Mock.MockStatus());

            var category1 = Mock.Database.InsertCategory(Mock.MockCategory());
            var category2 = Mock.Database.InsertCategory(Mock.MockCategory());

            var meta1 = Mock.Database.InsertMeta(Mock.MockMeta(status, category1));
            var meta2 = Mock.Database.InsertMeta(Mock.MockMeta(status, category2));
            var meta3 = Mock.Database.InsertMeta(Mock.MockMeta(status, category2));

            var c1 = Mock.MockConcept(status);
            c1.MetaIds = new List<int> { meta1.Id, meta2.Id, meta3.Id };
            c1.Title = title;
            Mock.Database.InsertConcept(c1);

            var c2 = Mock.MockConcept(status);
            c2.MetaIds = new List<int> { meta2.Id };
            c2.Title = title;
            Mock.Database.InsertConcept(c2);



            var query = new ConceptSearchQuery
            {
               MetaIds = new List<int>
               {
                   meta1.Id, 
                   meta3.Id
               }
            };

            Assert.Equal(2, ConceptRepository.GetAll(itemsPrPage, pageNumber, language, defaultLanguage).Count);

            var searchResult = ConceptRepository.SearchForConcepts(query);

            Assert.Single(searchResult);
        }
    }
}
