/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using System.Collections.Generic;
using UtilityHelpers = ConceptsMicroservice.Utilities.Helpers;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestHelpers
{
    public class HelpersTest
    {

        [Fact]
        public void GetDuplicates_Returns_Empty_When_There_Exists_No_Duplicates()
        {
            Assert.Empty(UtilityHelpers.GetDuplicates<int>(new List<int>{1,2,3}));
        }
        [Fact]
        public void GetDuplicates_Returns_Empty_When_Input_Is_Empty()
        {
            Assert.Empty(UtilityHelpers.GetDuplicates(new List<int>()));
        }
        [Fact]
        public void GetDuplicates_Returns_Empty_When_Input_Is_Null()
        {
            Assert.Empty(UtilityHelpers.GetDuplicates<int>(null));
        }
        [Fact]
        public void GetDuplicates_Returns_Duplicates_When_There_Exists_Duplicate()
        {
            const int duplicate = 1;
            foreach (var d in UtilityHelpers.GetDuplicates<int>(new List<int> { duplicate, 2, 3, duplicate }))
            {
                Assert.Equal(duplicate, d);
            }
        }
    }
}
