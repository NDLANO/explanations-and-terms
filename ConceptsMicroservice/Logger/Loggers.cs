/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ConceptsMicroservice.Logger
{
    public class Loggers
    {
        /// <summary>
        /// Logs all sql queries with log-level = debug.
        /// </summary>
        public static readonly LoggerFactory SqlLoggerFactory
            = new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name
                       && level == LogLevel.Debug, true)
            });
    }
}
