/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConceptsMicroservice.Utilities
{
    public class AttributeHelper
    {
        public static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return System.Attribute.GetCustomAttribute(member, typeof(TAttribute)) as TAttribute;
        }


        public static TValue GetPropertyAttributeValue<T, TOut, TAttribute, TValue>(
            Expression<Func<T, TOut>> propertyExpression,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var expression = propertyExpression.Body as MemberExpression;
            if (expression == null)
                return default(TValue);

            var propertyInfo = expression.Member as PropertyInfo;
            if (propertyInfo == null)
                return default(TValue);

            var attr = propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            return attr != null ? valueSelector(attr) : default(TValue);
        }
    }
}
