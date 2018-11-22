/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using System.Linq;

namespace ConceptsMicroservice.Utilities
{
    public static class Helpers
    {
        public static IEnumerable<T> GetDuplicates<T>(IEnumerable<T> source = null)
        {
            if (source== null)
                return new List<T>();

            var hash = new HashSet<T>();
            return source.Where(item => !hash.Add(item)).ToList();
        }
    }
}
