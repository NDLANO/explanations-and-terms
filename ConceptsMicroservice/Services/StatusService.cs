/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models;
using ConceptsMicroservice.Repositories;

namespace ConceptsMicroservice.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository status)
        {
            _statusRepository = status;
        }

        public Response GetAllStatus()
        {
            try
            {
                return new Response
                {
                    Data = _statusRepository.GetAll()
                };
            }
            catch
            {
                return null;
            }
        }

    }
}
