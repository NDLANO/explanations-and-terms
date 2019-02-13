using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConceptsMicroservice.Models.DTO
{
    public class ConceptResultDTO
    {
        public int NumberOfPages { get; set; }
        public int page { get; set; }
        public int TotalItems { get; set; }
        public string PathToNextPage { get; set; }
        public List<ConceptDto> Concepts { get; set; }
    }
}
