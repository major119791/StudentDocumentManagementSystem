using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Document_Management_for_G12
{
    public class Transaction
    {
        public string TransactionNumber { get; set; }
        public string Status { get; set; } 
        public string StudentID { get; set; }
        public List<string> Documents { get; set; } 
        public decimal TotalAmount { get; set; }
        public string HandlerName { get; set; }

        public Transaction(string transactionNumber, string status, List<string> documents, decimal totalAmount)
        {
            TransactionNumber = transactionNumber;
            Status = status;
            Documents = documents;
            TotalAmount = totalAmount;
        }
    }
}