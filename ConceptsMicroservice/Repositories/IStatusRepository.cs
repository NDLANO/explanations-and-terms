﻿/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using System.Collections.Generic;
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Models.Domain;

namespace ConceptsMicroservice.Repositories
{
    public interface IStatusRepository
    {
        List<StatusDto> GetAll();
        StatusDto GetById(int id);
        StatusDto GetByName(string name);
    }
}