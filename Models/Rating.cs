using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI6.Models
{
	public class Rating
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public int? BookId { get; set; }

        [Required]
        public string? MemberId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Score { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }
    }
}

