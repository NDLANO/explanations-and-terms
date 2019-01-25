/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */

using ConceptsMicroservice.Logger;
using ConceptsMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace ConceptsMicroservice.Context
{
    public class ConceptsContext : DbContext
    {
        public ConceptsContext(DbContextOptions<ConceptsContext> options): base(options){ }

        public DbSet<MetaData> MetaData { get; set; }
        public DbSet<Concept> Concepts { get; set; }
        public DbSet<MetaCategory> Categories { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Media> Media { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseLoggerFactory(Loggers.SqlLoggerFactory);
    }
}
