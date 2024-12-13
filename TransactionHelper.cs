using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace Student_Document_Management_for_G12
{
    public static class TransactionHelper
    {
        private static readonly string HistoryFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

        public static void SearchTransactionHistoryByTransactionNumber(string transactionNumber)
        {
            try
            {
                if (File.Exists(HistoryFilePath))
                {
                    string[] historyLines = File.ReadAllLines(HistoryFilePath);

                    var matchingTransaction = historyLines.FirstOrDefault(line =>
                        line.StartsWith($"Transaction Number: {transactionNumber}", StringComparison.OrdinalIgnoreCase));

                    if (matchingTransaction != null)
                    {
                        Console.WriteLine("\n<----- Transaction History ----->");
                        Console.WriteLine(matchingTransaction);
                    }
                    else
                    {
                        Console.WriteLine("Transaction not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Transaction history file does not exist.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading the transaction history file: " + ex.Message);
            }
        }

        public static void ClearTransactionHistory()
        {
            string historyFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            try
            {
                string confirmation = GetConfirmation("Are you sure you want to clear the transaction history? (yes/no): ");

                if (confirmation == "yes")
                {
                    if (File.Exists(historyFilePath))
                    {
                        File.WriteAllText(historyFilePath, string.Empty);
                        Console.WriteLine("Transaction history cleared successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Transaction history file does not exist.");
                    }
                }
                else
                {
                    Console.WriteLine("Action canceled. Transaction history was not cleared.");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error clearing transaction history: " + ex.Message);
            }

            Pause(); 
        }
        static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        private static string GetConfirmation(string promptMessage)
        {
            while (true)
            {
                Console.Write(promptMessage);
                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("No input provided. Operation canceled.");
                    return "no"; 
                }

                if (input == "yes" || input == "no")
                {
                    return input; 
                }

                Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
            }
        }
        public static List<Student> LoadTransactionHistory()
        {
            var students = new List<Student>();

            if (!File.Exists(HistoryFilePath))
            {
                Console.WriteLine($"Transaction history file not found at {HistoryFilePath}");
                return students;
            }

            var lines = File.ReadAllLines(HistoryFilePath);

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var student = Student.FromDataString(line);
                    if (student != null)
                    {
                        students.Add(student);
                    }
                }
            }

            return students;
        }
    }
}