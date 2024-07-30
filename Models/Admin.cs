using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI6.Models
{
	public class Admin
	{
        [Key]
        public string Id { get; set; } = "";

        [ForeignKey(nameof(Id))]
        public ApplicationUser? ApplicationUser { get; set; }

        public string Title { get; set; } = "";
        public float Salary { get; set; }
        public string Department { get; set; } = "";
        public string? Shift { get; set; }
    }
}
