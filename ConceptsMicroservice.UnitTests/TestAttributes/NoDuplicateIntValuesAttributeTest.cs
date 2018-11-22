/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using ConceptsMicroservice.Attributes;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestAttributes
{
    public class NoDuplicateIntValuesAttributeTest
    {
        private readonly NoDuplicateIntValuesAttribute _attribute;

        public NoDuplicateIntValuesAttributeTest()
        {
            _attribute = new NoDuplicateIntValuesAttribute();
        }

        [Fact]
        public void Input_Is_Null_Should_Be_Valid()
        {
            Assert.True(_attribute.IsValid(null));
        }

        [Fact]
        public void Input_Is_Empty_Should_Be_Valid()
        {
            Assert.True(_attribute.IsValid(new List<int>()));
        }

        [Fact]
        public void Contains_Duplicates_Should_Not_Be_Valid()
        {
            Assert.False(_attribute.IsValid(new List<int> { 1, 2, 3, 1 }));
        }

        [Fact]
        public void Contains_No_Duplicates_Should_Be_Valid()
        {
            Assert.True(_attribute.IsValid(new List<int> { 1, 2, 3 }));
        }

    }
}
