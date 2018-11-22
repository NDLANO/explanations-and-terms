/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Search;
using ConceptsMicroservice.Repositories;
using ConceptsMicroservice.UnitTests.Mock;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestRepositories
{
    [Collection("Database tests")]
    public class MetaDataRepositoryTest : IDisposable, IClassFixture<DatabaseTestFixture>
    {
        protected readonly IMetadataRepository MetaRepository;
        protected IMock Mock;

        private readonly Status _status;
        private readonly MetaCategory _category;

        public MetaDataRepositoryTest()
        {
            Mock = new Mock.Mock();
            MetaRepository = new Repositories.MetadataRepository(Mock.Database.Context);


            _status = Mock.Database.InsertStatus(Mock.MockStatus());
            _category = Mock.Database.InsertCategory(Mock.MockCategory());
        }

        public void Dispose()
        {
            Mock.Dispose();
        }

        #region GetById

        [Fact]
        public void GetById_Fetches_Meta()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            var id = Mock.Database.InsertMeta(meta).Id;

            Assert.NotNull(MetaRepository.GetById(id));
        }
        [Fact]
        public void GetById_Returns_Null_If_Meta_Does_Not_Exist()
        {
            Assert.Empty(MetaRepository.GetAll());
            Assert.Null(MetaRepository.GetById(1));
        }
        [Fact]
        public void GetById_Includes_Category()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            var id = Mock.Database.InsertMeta(meta).Id;

            Assert.NotNull(MetaRepository.GetById(id).Category);
        }
        [Fact]
        public void GetById_Includes_Status()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            var id = Mock.Database.InsertMeta(meta).Id;

            Assert.NotNull(MetaRepository.GetById(id).Status);
        }

        #endregion

        #region GetAll


        [Fact]
        public void GetAll_Fetches_A_List_Of_meta()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            Mock.Database.InsertMeta(meta);

            Assert.NotEmpty(MetaRepository.GetAll());
        }
        [Fact]
        public void GetAll_Returns_EmptyList_If_Meta_Does_Not_Exist()
        {
            Assert.Empty(MetaRepository.GetAll());
        }
        [Fact]
        public void GetAll_Includes_Category()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            Mock.Database.InsertMeta(meta);
            
            foreach (var metaData in MetaRepository.GetAll())
            {
                Assert.NotNull(metaData.Category);
            }
        }
        [Fact]
        public void GetAll_Fetches_Includes_Status()
        {
            var meta = Mock.MockMeta(_status, _category);

            Assert.Empty(MetaRepository.GetAll());

            Mock.Database.InsertMeta(meta);

            foreach (var metaData in MetaRepository.GetAll())
            {
                Assert.NotNull(metaData.Category);
            }
        }
        #endregion

       

        #region SearchForMetadata
        [Fact]
        public void SearchForMetadata_Returns_Results_Based_On_Category_Name_And_Name()
        {
            var category = Mock.MockCategory("newName");
            Mock.Database.InsertCategory(category);

            var meta1 = Mock.MockMeta(_status, category);
            meta1.Name = "searchFor";
            var meta2 = Mock.MockMeta(_status, category);
            meta2.Name = "m2";
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(meta1);
            Mock.Database.InsertMeta(meta2);
            var searchResult = MetaRepository.SearchForMetadata(new MetaSearchQuery { Category = category.Name, Name = meta1.Name });

            Assert.Equal(3, MetaRepository.GetAll().Count);
            Assert.Single(searchResult);
        }
        [Fact]
        public void SearchForMetadata_Returns_Results_Based_On_Category_Name()
        {
            var category = Mock.MockCategory("newName");
            Mock.Database.InsertCategory(category);

            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, category));
            var searchResult = MetaRepository.SearchForMetadata(new MetaSearchQuery{Category = category.Name});

            Assert.Equal(3, MetaRepository.GetAll().Count);
            Assert.Single(searchResult);
        }
        [Fact]
        public void SearchForMetadata_Returns_Results_Based_On_Name()
        {
            var meta = Mock.MockMeta(_status, _category);
            meta.Name = "newName";
            Mock.Database.InsertMeta(meta);
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            var searchResult = MetaRepository.SearchForMetadata(new MetaSearchQuery { Name = meta.Name });

            Assert.Equal(3, MetaRepository.GetAll().Count);
            Assert.Single(searchResult);
        }

        [Fact]
        public void SearchForMetadata_Returns_All_When_Query_Has_No_Query()
        {
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));

            var all = MetaRepository.GetAll();
            var searchResult = MetaRepository.SearchForMetadata(new MetaSearchQuery());

            Assert.Equal(all.Count, searchResult.Count);
        }


        [Fact]
        public void SearchForMetadata_Returns_All_When_Query_Is_Null()
        {
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));
            Mock.Database.InsertMeta(Mock.MockMeta(_status, _category));

            var all = MetaRepository.GetAll();
            var searchResult = MetaRepository.SearchForMetadata(null);

            Assert.Equal(all.Count, searchResult.Count);
        }
            #endregion
        }
}
