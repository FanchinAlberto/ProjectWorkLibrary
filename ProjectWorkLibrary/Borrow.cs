using System;

namespace ProjectWorkLibrary
{
    class Borrow
    {
        public string BorrowedBookISBN { get; set; }
        public string debtorUser { get; set; }
        public DateTime startBorrow { get; set; }
        public DateTime endBorrow { get; set; }
        public string LoanID { get; set; }
    }
}
