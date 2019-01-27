using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConceptsMicroservice.Models
{
    [Table("media", Schema = "public")]
    public class Media
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("media_type_id")]
        public int MediaTypeId { get; set; }
        [Column("language_id")] public int LanguageId { get; set; }
        [Column("external_id")]
        public int ExternalId { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
        [Column("updated")]
        public DateTime Updated { get; set; }
        public virtual Language Language { get; set; }

        //public virtual MediaType Category { get; set; }
    }
}
