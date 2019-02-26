/**
 * Copyright (c) 2018-present, NDLA.
 *
 * This source code is licensed under the GPLv3 license found in the
 * LICENSE file in the root directory of this source tree.
 *
 */
using ConceptsMicroservice.Models.Domain;
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
        public DbSet<Models.Domain.Media> Media { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<ConceptMedia> ConceptMedia { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<MetaCategoryType> MetaCategoryTypes { get; set; }
    }
}
