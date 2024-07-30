using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI6.Models
{
    public class Language
    {
        [Key]
        [Required]
        [StringLength(3, MinimumLength = 3)]
        [Column(TypeName = "char(2)")]
        public string Code { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = "";
    }
}
