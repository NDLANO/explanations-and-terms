/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models;

namespace ConceptsMicroservice.Repositories
{
    public interface IStatusRepository
    {
        List<Status> GetAll();
        Status GetById(int id);
        Status GetByName(string name);
    }
}