/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
namespace ConceptsMicroservice.UnitTests.Helpers
{
    public class AttributeHelper
    {
        public static bool AttributeIsPresentOnProperty<T, TT>(string propName)
        {
            return System.Attribute.IsDefined(typeof(T).GetProperty(propName), typeof(TT));
        }
    }
}
