/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories.ConceptRepository
{
    
    [Collection("Database tests")]
    public class CRUDTest : BaseTest
    {
        #region GetAll
        [Fact]
        public void GetAll_Returns_Empty_List_If_No_Concept_Exists()
        {
            var concepts = ConceptRepository.GetAll();

            Assert.Empty(concepts);
        }

        [Fact]
        public void GetAll_Returns_Concepts_With_Metadata()
        {
            Mock.Database.CreateAndInsertAConcept();
            var concepts = ConceptRepository.GetAll();

            Assert.NotEmpty(concepts);
            foreach (var concept in concepts)
            {
                Assert.NotEmpty(concept.Meta);
            }
        }

        #endregion

        private readonly string languageCode = "nb";

        #region GetAllTitles
        [Fact]
        public void GetAllTitles_Returns_Empty_List_If_No_Concept_Exists()
        {
            var concepts = ConceptRepository.GetAllTitles(languageCode);

            Assert.Empty(concepts);
        }

        [Fact]
        public void GetAllTitles_Returns_A_List_With_Concept_Titles()
        {
            var status = Mock.MockStatus();
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, Mock.MockCategory()));
            var insertedConcepts = new List<Concept>
            {
                Mock.Database.InsertConcept(Mock.MockConcept(status, new List<MetaData>{meta})),
                Mock.Database.InsertConcept(Mock.MockConcept(status, new List<MetaData>{meta})),
                Mock.Database.InsertConcept(Mock.MockConcept(status, new List<MetaData>{meta}))
            };

            var titles = ConceptRepository.GetAllTitles(languageCode);

            Assert.NotEmpty(titles);
            foreach (var concept in insertedConcepts)
            {
                Assert.Contains(titles, x => x.Equals(concept.Title));
            }
        }

        #endregion

        #region GetById
        [Fact]
        public void GetById_Returns_Null_If_Does_Not_Concept_Exist()
        {
            var concept = ConceptRepository.GetById(-1);

            Assert.Null(concept);
        }

        [Fact]
        public void GetById_Returns_Concept_If_It_Exists()
        {
            var c = Mock.Database.CreateAndInsertAConcept();
            var concept = ConceptRepository.GetById(c.Id);

            Assert.NotNull(concept);
        }

        [Fact]
        public void GetById_Returns_Concept_With_Metadata()
        {
            var c = Mock.Database.CreateAndInsertAConcept();
            var concept = ConceptRepository.GetById(c.Id);

            Assert.NotEmpty(concept.Meta);
        }
        #endregion
        #region Insert

        [Fact]
        public void Insert_Inserts_Concept()
        {
            Assert.Empty(ConceptRepository.GetAll());

            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));

            var concept = Mock.MockConcept(status);
            concept.MetaIds = new List<int> { meta.Id };

            var language = Mock.Database.InsertLanguage();
            concept.LanguageId = language.Id;

            ConceptRepository.Insert(concept);

            var c = ConceptRepository.Insert(concept);

            Assert.NotNull(ConceptRepository.GetById(c.Id));
        }

        [Fact]
        public void Insert_Existing_Concept_Creates_A_New_Concept()
        {
            Assert.Empty(ConceptRepository.GetAll());

            var category = Mock.Database.InsertCategory(Mock.MockCategory());
            var status = Mock.Database.InsertStatus(Mock.MockStatus());
            var meta = Mock.Database.InsertMeta(Mock.MockMeta(status, category));

            var concept = Mock.MockConcept(status);
            concept.MetaIds = new List<int> { meta.Id };

            var language = Mock.Database.InsertLanguage();
            concept.LanguageId = language.Id;

            var insertedConceptId = ConceptRepository.Insert(concept).Id;
            var toBeCloned = ConceptRepository.GetById(insertedConceptId);
            var clone = ConceptRepository.Insert(toBeCloned);


            Assert.NotNull(ConceptRepository.GetById(concept.Id));
            Assert.NotNull(ConceptRepository.GetById(clone.Id));
            Assert.NotEqual(insertedConceptId, clone.Id);
        }
        #endregion

        [Fact]
        public void Update_Updates_Concept()
        {
            var c = Mock.Database.CreateAndInsertAConcept();
            c.Title = "Updated Title";
            ConceptRepository.Update(c);
            var concept = ConceptRepository.GetById(c.Id);

            Assert.Equal(c.Title, concept.Title);
        }
    }
}
