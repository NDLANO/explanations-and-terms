/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.UnitTests.Helpers;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestHelpers
{
    public class AttributeHelperTest
    {
        internal class Dummy
        {
            [Required]
            public string HaveRequiredAttribute { get; set; }
            public string DoesNotHaveRequiredAttribute { get; set; }
        }

        [Fact]
        public void Returns_True_If_Attribute_Exist_On_Property()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Dummy, RequiredAttribute>(nameof(Dummy.HaveRequiredAttribute)));
        }
        [Fact]
        public void Returns_False_If_Attribute_Does_Not_Exist_On_Property()
        {
            Assert.False(AttributeHelper.AttributeIsPresentOnProperty<Dummy, RequiredAttribute>(nameof(Dummy.DoesNotHaveRequiredAttribute)));
        }
    }
}
