using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI6.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookCopyId { get; set; }
        public string? MemberId { get; set; } 
        public string? EmployeeId { get; set; }
        public DateTime BorrowedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public decimal FineAmount { get; set; }
        public bool BookStatus { get; set; }

        [ForeignKey(nameof(BookCopyId))]
        public BookCopy? BookCopy { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        //*************

        public decimal CalculateFine()
        {
            if (ReturnedDate == null || ReturnedDate <= DueDate)
            {
                return 0;
            }

            var daysLate = (ReturnedDate.Value.Date - DueDate.Date).Days;
            const decimal initialFine = 20.0m;
            const decimal dailyFine = 10.0m;

            if (BookStatus == true)
            {
                return initialFine + (daysLate * dailyFine) + 100;
            }

            return initialFine + (daysLate * dailyFine);
        }
    }
}
