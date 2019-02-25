/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Newtonsoft.Json;

namespace ConceptsMicroservice.Utilities
{
    public class JsonHelper
    {
        public static T GetJson<T>(Npgsql.NpgsqlDataReader reader, int column)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(reader.GetString(column));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
