/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace ConceptsMicroservice.Models.Search
{
    public class ConceptSearchQuery
    {
        [FromQuery] public string Title { get; set; }
        [FromQuery(Name = "meta")] public List<int> MetaIds { get; set; }

        /// <summary>
        /// Checks whether the title or queryList have any values
        /// </summary>
        /// <returns></returns>
        public bool HasQuery()
        {
            return !string.IsNullOrWhiteSpace(Title) || (MetaIds != null && MetaIds.Count > 0);
        }

        public List<Npgsql.NpgsqlParameter> GetSqlParameters()
        {
            var sqlParameters = new List<NpgsqlParameter>();
            if (!string.IsNullOrWhiteSpace(Title))
            {
                sqlParameters.Add(new NpgsqlParameter("concept_title", NpgsqlDbType.Varchar)
                {
                    Value = Title
                });
            }

            if (MetaIds != null && MetaIds.Count > 0)
            {
                sqlParameters.Add(new NpgsqlParameter("list_of_meta_id", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                {
                    Value = MetaIds.ToArray()
                });
            }

            return sqlParameters;
        }
    }
}
