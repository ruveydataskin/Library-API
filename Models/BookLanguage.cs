using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI6.Models
{
    public class BookLanguage
    {
        [Required]
        public int BooksId { get; set; }

        public string Code { get; set; } = "";

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(Code))]
        public Language? Language { get; set; }
    }
}
