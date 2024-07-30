using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI6.Models
{
    public class BookCopy
    {
        public int Id { get; set; }
        public int CopyNumber { get; set; }
        public int BookId { get; set; }
        public bool IsAvailable { get; set; }
        public bool BookStatus { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        public List<Loan>? Loans { get; set; }
    }
}
