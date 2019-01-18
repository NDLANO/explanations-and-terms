/** Copyright(c) 2018-present, NDLA.

*
* This source code is licensed under the GPLv3 license found in the
* LICENSE file in the root directory of this source tree.
*
*/

using System;
using AutoMapper.Configuration;
using ConceptsMicroservice.Profiles;

namespace ConceptsMicroservice.UnitTests.Helpers
{
    public class InitMapper : IDisposable
    {
        public InitMapper()
        {
            try
            {
                var mappings = new MapperConfigurationExpression();
                mappings.AddProfile<MappingProfile>();
                AutoMapper.Mapper.Initialize(mappings);
            }
            catch
            {
                // ignore
            }
        }

        public void Dispose()
        {
            AutoMapper.Mapper.Reset();
        }
    }
}
