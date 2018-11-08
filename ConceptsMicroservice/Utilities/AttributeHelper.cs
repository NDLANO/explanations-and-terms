/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Reflection;

namespace ConceptsMicroservice.Utilities
{
    public class AttributeHelper
    {
        public static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return System.Attribute.GetCustomAttribute(member, typeof(TAttribute)) as TAttribute;
        }
    }
}
