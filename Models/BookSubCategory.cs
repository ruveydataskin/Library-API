using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI6.Models
{
    public class BookSubCategory
    {
        public int BooksId { get; set; }

        public short Id { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(Id))]
        public SubCategory? SubCategory { get; set; }
    }
}
