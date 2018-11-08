/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Attributes;
using ConceptsMicroservice.Models;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes
{
    public class ConceptMustContainMetaAttributeTest
    {
        private readonly string[] _requiredMetaCategories;
        private readonly MetaData _licence;
        private readonly MetaData _language;

        public ConceptMustContainMetaAttributeTest()
        {
            _licence = new MetaData
            {
                Category = new MetaCategory
                {
                    Name = "Licence"
                }
            };
            _language = new MetaData
            {
                Category = new MetaCategory
                {
                    Name = "Licence"
                }
            };
            _requiredMetaCategories = new[] { _language.Category.Name, _licence .Category.Name};
        }

        [Fact]
        public void Is_Not_Valid_When_No_Metas_Is_Present()
        {
            var validationAttribute = new ConceptMustContainMetaAttribute(_requiredMetaCategories);

            Assert.False(validationAttribute.IsValid(new List<MetaData>()));
        }

        [Fact]
        public void Is_Not_Valid_When_Only_One_Of_Two_Required_Is_Present()
        {
            var validationAttribute = new ConceptMustContainMetaAttribute(_requiredMetaCategories);

            Assert.False(validationAttribute.IsValid(new List<MetaData>{_licence}));
        }

        [Fact]
        public void Is_Valid_When_Only_All_Required_Is_Present()
        {
            var validationAttribute = new ConceptMustContainMetaAttribute(_requiredMetaCategories);

            Assert.True(validationAttribute.IsValid(new List<MetaData> {_licence, _language}));
        }

        [Fact]
        public void Is_Valid_When_Contains_More_Metas_Then_Required()
        {
            var validationAttribute = new ConceptMustContainMetaAttribute(_requiredMetaCategories);
            var extra = new MetaData {Category = new MetaCategory {Name = "Extra"}};

            Assert.True(validationAttribute.IsValid(new List<MetaData> { _licence, _language, extra }));
        }
    }
}
