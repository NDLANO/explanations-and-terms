/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.ComponentModel.DataAnnotations;
using ConceptsMicroservice.Models.Domain;
using ConceptsMicroservice.UnitTests.Helpers;
using Xunit;

namespace ConceptsMicroservice.UnitTests.TestModels
{
    public class MetaStatusTest
    {
        [Fact]
        public void Name_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Status, RequiredAttribute>(nameof(Status.Name)));
        }
        
        [Fact]
        public void Description_Should_Have_Attribute_Required()
        {
            Assert.True(AttributeHelper.AttributeIsPresentOnProperty<Status, RequiredAttribute>(nameof(Status.Description)));
        }
    }
}
