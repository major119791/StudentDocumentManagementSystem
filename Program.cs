using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using Student_Document_Management_for_G12;

namespace Student_Document_Management_for_G12
{
    public class Program
    {
        private static string filePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Main Data\Main Data File.txt";
        private static string title = @"
____ ___ _  _ ___  ____ _  _ ___    ___  ____ ____ _  _ _  _ ____ _  _ ___
[__   |  |  | |  \ |___ |\ |  |     |  \ |  | |    |  | |\/| |___ |\ |  | 
___]  |  |__| |__/ |___ | \|  |     |__/ |__| |___ |__| |  | |___ | \|  | 
                                                                          
_  _ ____ _  _ ____ ____ ____ _  _ ____ _  _ ___                          
|\/| |__| |\ | |__| | __ |___ |\/| |___ |\ |  |                           
|  | |  | | \| |  | |__] |___ |  | |___ | \|  |                           
                                                                          
____ _   _ ____ ___ ____ _  _                                             
[__   \_/  [__   |  |___ |\/|                                             
___]   |   ___]  |  |___ |  |                                             ";

        public static void Main(string[] args)
        {
            Program program = new Program();
            List<Student> students = LoadStudentsFromFile();
            program.DisplayMainMenu(students);
        }
        private void ClearConsoleWithTitle()
        {
            Console.Clear();
            CenterText(title);
            Console.WriteLine();
        }

        private void CenterText(string text)
        {
            string[] lines = text.Split('\n');
            int consoleWidth = Console.WindowWidth;

            foreach (string line in lines)
            {
                int padding = (consoleWidth - line.Length) / 2;
                Console.WriteLine(new string(' ', Math.Max(0, padding)) + line);
            }
        }
        public static void PrintSeparator(char separatorChar = '=', int length = 38)
        {
            Console.WriteLine(new string(separatorChar, length));
        }
        static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        public void DisplayMainMenu(List<Student> students)
        {
            while (true)
            {
                ClearConsoleWithTitle();
                PrintSeparator();
                Console.WriteLine("\n<----- Main Menu ----->\n");
                PrintSeparator();
                Console.WriteLine("1. Student Manager");
                Console.WriteLine("2. Request Handler");
                Console.WriteLine("3. Transaction Handler");
                Console.WriteLine("4. Status Handler");
                Console.WriteLine("5. Exit");

                string choice = GetInput(true, "\nEnter your choice: ");
                Console.WriteLine(); 

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Redirecting to Student Manager...\n");
                        StudentManager(students);
                        break;
                    case "2":
                        Console.WriteLine("Redirecting to Request Handler...\n");
                        RequestHandler(students);
                        break;
                    case "3":
                        Console.WriteLine("Redirecting to Transaction Handler...\n");
                        TransactionHandler();
                        break;
                    case "4":
                        Console.WriteLine("Redirecting to Status Handler...\n");
                        StatusHandler(students);
                        break;
                    case "5":
                        Console.WriteLine("Exiting program.");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        private static void SaveStudentsToFile(List<Student> students)
        {
            string filePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Main Data\Main Data File.txt";

            List<string> studentData = students.Select(s => s.ToDataString()).ToList();
            File.WriteAllLines(filePath, studentData);
        }
        public void StudentManager(List<Student> students)
        {
            while (true)
            {
                ClearConsoleWithTitle();
                PrintSeparator();
                Console.WriteLine("\n<----- Student Manager ----->\n");
                PrintSeparator();
                Console.WriteLine("1. View Students");
                Console.WriteLine("2. Search Student");
                Console.WriteLine("3. Add New Student");
                Console.WriteLine("4. Remove Student");
                Console.WriteLine("5. Back to Main Menu");

                string choice = GetInput(true, "\nEnter your choice: ");
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ViewStudents(students);
                        break;
                    case "2":
                        SearchStudent(students);
                        break;
                    case "3":
                        AddNewStudent(students);
                        break;
                    case "4":
                        Console.Write("Enter the Student ID to remove: ");
                        string studentID = Console.ReadLine()?.Trim();

                        if (!string.IsNullOrEmpty(studentID))
                        {
                            RemoveStudentById(students, studentID);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Student ID. Please try again.");
                        }
                        break;
                    case "5":
                        Console.WriteLine("Returning to Main Menu...\n");
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        public static void ViewStudents(List<Student> students)
        {
            PrintSeparator();
            Console.WriteLine("<----- Student List ----->");
            PrintSeparator();

            if (students.Count > 0)
            {
                for (int i = 0; i < students.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {students[i].Name} ({students[i].StudentID})");
                }
            }
            else
            {
                Console.WriteLine("No students are currently in the system.");
            }

            PrintSeparator();
            Console.WriteLine("\nPress any key to return to the Student Manager menu...");
            Console.ReadKey(); 
        }
        static void SearchStudent(List<Student> students)
        {
            try
            {
                Console.Write("Enter student name or ID to search (or type 'back'): ");
                string searchTerm = GetInput(true).ToLower();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    Console.WriteLine("Search term cannot be empty.");
                    Pause(); 
                    return;
                }

                var foundStudents = students.Where(s =>
                    s.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.StudentID.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                if (foundStudents.Count > 0)
                {
                    Console.WriteLine("\n<----- Search Results ----->");
                    foreach (var student in foundStudents)
                    {
                        student.DisplayStatus();
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("No students found with that name or ID.");
                }

                Pause(); 
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Returning to the main menu...");
                Pause(); 
            }
        }
        static void AddNewStudent(List<Student> students)
        {
            try
            {
                Console.Write("Enter student's name (or type 'back'): ");
                string newStudentName = GetInput(true);

                if (string.IsNullOrWhiteSpace(newStudentName))
                {
                    Console.WriteLine("Name cannot be empty.");
                    Pause(); 
                    return;
                }

                if (students.Exists(s => s.Name.Equals(newStudentName, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Student already in the list.");
                    Pause(); 
                }
                else
                {
                    string generatedID = new Student().GenerateStudentID();

                    Student newStudent = new Student(name: newStudentName, studentID: generatedID, isManual: true);

                    students.Add(newStudent);

                    Console.WriteLine($"Student added successfully. ID: {newStudent.StudentID}");

                    SaveStudentsToFile(students);
                    Pause(); 
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Returning to the main menu...");
                Pause(); 
            }
        }

        static string GetInput(bool allowBack = false, string promptMessage = "")
        {
            while (true)
            {
                Console.Write(promptMessage);
                string input = Console.ReadLine()?.Trim();

                if (allowBack && !string.IsNullOrEmpty(input) && input.Equals("back", StringComparison.OrdinalIgnoreCase))
                {
                    throw new OperationCanceledException("User chose to return to the main menu.");
                }

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Please try again.");
                }
                else
                {
                    return input;
                }
            }
        }

        public void RequestHandler(List<Student> students)
        {
            while (true)
            {
                ClearConsoleWithTitle();
                PrintSeparator();
                Console.WriteLine("\n<----- Request Handler ----->\n");
                PrintSeparator();
                Console.WriteLine("1. Request Documents for a Student");
                Console.WriteLine("2. Clear Receipt File");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("");

                string requestOption = GetInput(false, "Select an option: ");

                switch (requestOption)
                {
                    case "1":
                        RequestDocumentsForStudent(students);
                        SaveStudentsToFile(students);  
                        break;
                    case "2":
                        ClearReceiptFile();
                        break;
                    case "3":
                        return;  
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        static void RequestDocumentsForStudent(List<Student> students)
        {
            try
            {
                Console.Write("Enter student name or ID to request documents for (or type 'back'): ");
                string searchTerm = GetInput(true).ToLower();

                var student = students.FirstOrDefault(s => s.Name.ToLower().Contains(searchTerm) || s.GetStudentID().ToLower().Contains(searchTerm));

                if (student != null)
                {
                    Console.WriteLine("\n<----- Student Information ----->");
                    student.DisplayStatus();

                    Console.WriteLine("\nEnter document names to request (or type 'back'): ");
                    string documentNames = GetInput();

                    List<string> documents = documentNames.Split(',').Select(d => d.Trim()).ToList();

                    decimal totalAmount;
                    List<string> newlyClaimedDocuments = student.RequestDocuments(documents, out totalAmount);

                    if (newlyClaimedDocuments.Count > 0)
                    {
                        Console.WriteLine($"Total Amount: {totalAmount} PHP");

                        string transactionNumber = GenerateTransactionNumber();

                        Console.Write("Enter the handler's name: ");
                        string transactionHandler = GetInput();

                        student.GenerateReceipt(newlyClaimedDocuments, totalAmount, transactionNumber, transactionHandler);
                        student.LogTransactionHistory(transactionNumber, newlyClaimedDocuments, totalAmount, transactionHandler);
                    }
                    else
                    {
                        Console.WriteLine("No new documents were claimed.");
                    }
                }
                else
                {
                    Console.WriteLine("No student found with that name or ID.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Returning to the main menu...");
            }

            Pause(); 
        }
        static void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        private static void ClearReceiptFile()
        {
            string receiptFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Receipt Data\Receipt Data.txt";

            try
            {
                File.WriteAllText(receiptFilePath, string.Empty);
                Console.WriteLine("Receipt file has been successfully cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing the receipt file: {ex.Message}");
            }

            Pause();
        }
        public static string GenerateTransactionNumber()
        {
            return $"TX-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
        }


        public void TransactionHandler()
        {
            ShowTransactionHandlerMenu();
        }

        void ShowTransactionHandlerMenu()
        {
            while (true)
            {
                ClearConsoleWithTitle();
                PrintSeparator();
                Console.WriteLine("\n<----- Transaction Handler ----->\n");
                PrintSeparator();
                Console.WriteLine("1. Search Transaction History");
                Console.WriteLine("2. Clear Transaction History");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("");

                string transactionOption = GetInput(false, "Select an option: ");

                switch (transactionOption)
                {
                    case "1":
                        SearchTransactionMenu();
                        break;
                    case "2":
                        TransactionHelper.ClearTransactionHistory();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        public static void SearchTransactionMenu()
        {
            Console.WriteLine("\n<----- Search Transaction History ----->");
            Console.Write("Enter transaction number: ");
            string transactionNumber = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(transactionNumber))
            {
                Console.WriteLine("Transaction number cannot be empty.");
                return;
            }

            SearchTransactionHistoryByTransactionNumber(transactionNumber);
            Pause(); 
        }

        public static void SearchTransactionHistoryByTransactionNumber(string transactionNumber)
        {
            string historyFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            try
            {
                if (File.Exists(historyFilePath))
                {
                    string[] historyLines = File.ReadAllLines(historyFilePath);

                    string normalizedTransactionNumber = transactionNumber.Trim();

                    var matchingTransaction = historyLines.FirstOrDefault(line =>
                        line.Contains(normalizedTransactionNumber, StringComparison.OrdinalIgnoreCase));

                    if (matchingTransaction != null)
                    {
                        Console.WriteLine("\n<----- Transaction History ----->");
                        Console.WriteLine(matchingTransaction);
                    }
                    else
                    {
                        Console.WriteLine("Transaction number not found.");
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
        public void StatusHandler(List<Student> students)
        {
            while (true)
            {
                ClearConsoleWithTitle();
                PrintSeparator();
                Console.WriteLine("\n<----- Status Handler ----->\n");
                PrintSeparator();
                Console.WriteLine("1. Update Document Status");
                Console.WriteLine("2. Cancel Document Request");
                Console.WriteLine("3. Back to Main Menu");
                Console.WriteLine("");

                string choice = GetInput(true, "Select an option: ");

                switch (choice)
                {
                    case "1":
                        UpdateDocumentStatusMenu(); 
                        break;
                    case "2":
                        CancelDocumentRequestMenu(students);  
                        break;
                    case "3":
                        return;  
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        public void UpdateDocumentStatusMenu()
        {
            Console.WriteLine("\nUpdate Document Status");
            Console.Write("Enter the transaction number: ");
            string transactionNumber = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(transactionNumber))
            {
                Console.WriteLine("Transaction number cannot be empty.");
                return;
            }

            Console.Write("Enter the new status ('Requested' or 'Claimed'): ");
            string newStatus = Console.ReadLine()?.Trim();

            if (newStatus != "Requested" && newStatus != "Claimed")
            {
                Console.WriteLine("Invalid status. Please enter 'Requested' or 'Claimed'.");
                return;
            }

            UpdateDocumentStatus(transactionNumber, newStatus);

            Pause();
        }


        public static void UpdateDocumentStatus(string transactionNumber, string newStatus)
        {
            string transactionFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            if (!File.Exists(transactionFilePath))
            {
                Console.WriteLine("Transaction file not found.");
                return;
            }

            var lines = File.ReadAllLines(transactionFilePath).ToList();
            bool transactionFound = false;

            for (int i = 0; i < lines.Count; i++)
            {
                var parts = lines[i].Split('|');
                if (parts[0] == transactionNumber)
                {
                    if (parts.Length >= 9)
                    {
                        parts[8] = newStatus; 
                        lines[i] = string.Join('|', parts); 
                        transactionFound = true;
                    }
                    else
                    {
                        Console.WriteLine("Transaction data format is incorrect.");
                        return;
                    }
                }
            }

            if (!transactionFound)
            {
                Console.WriteLine("Transaction number not found.");
                return;
            }

            File.WriteAllLines(transactionFilePath, lines);

            Console.WriteLine("Document status updated successfully");
        }
        public static Transaction FindTransaction(string transactionNumber)
        {
            string transactionFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            if (!File.Exists(transactionFilePath))
            {
                return null;
            }

            string[] lines = File.ReadAllLines(transactionFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts[0] == transactionNumber)
                {
                    if (parts.Length >= 9)
                    {
                        return new Transaction(
                            transactionNumber: parts[0],
                            status: parts[7],
                            documents: new List<string> { parts[3] }, 
                            totalAmount: decimal.Parse(parts[4])
                        );
                    }
                }
            }
            return null;
        }
        public static void CancelDocumentRequestMenu(List<Student> students)
        {
            Console.WriteLine("\n--- Cancel Document Request ---");

            Console.Write("Enter Transaction Number: ");
            string transactionNumber = Console.ReadLine().Trim();

            string transactionFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            var lines = File.ReadAllLines(transactionFilePath).ToList();
            var transactionFound = false;

            foreach (var line in lines)
            {
                var parts = line.Split('|');

                if (parts[0] == transactionNumber)
                {
                    transactionFound = true;

                    var studentID = parts[1];
                    var documentName = parts[3];

                    Console.WriteLine($"Transaction Found: {transactionNumber} - Document: {documentName}");
                    Console.WriteLine("\nWarning: No further process will be done if canceled, and you will need to make a new request.");

                    Console.Write("Are you sure you want to cancel this request? (yes/no): ");
                    string confirmation = Console.ReadLine().Trim().ToLower();

                    if (confirmation == "yes")
                    {
                        parts[8] = "Canceled";

                        lines[lines.IndexOf(line)] = string.Join("|", parts);

                        File.WriteAllLines(transactionFilePath, lines);

                        Console.WriteLine($"Document request '{documentName}' for transaction '{transactionNumber}' has been successfully canceled.");
                    }
                    else if (confirmation == "no")
                    {
                        Console.WriteLine("Cancellation process aborted.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. No changes have been made.");
                    }

                    break;
                }
            }

            if (!transactionFound)
            {
                Console.WriteLine("No transaction found with the provided transaction number.");
            }

            Pause(); 
        }

        private static List<Student> LoadStudentsFromFile()
        {
            List<Student> students = new List<Student>();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Student student = Student.FromDataString(line);
                        if (student != null)
                        {
                            students.Add(student);
                        }
                    }
                }
            }

            return students;
        }
        private static void RemoveStudentById(List<Student> students, string studentID)
        {
            try
            {
                Student studentToRemove = students.FirstOrDefault(s => s.StudentID.Equals(studentID, StringComparison.OrdinalIgnoreCase));

                if (studentToRemove != null)
                {
                    students.Remove(studentToRemove);
                    SaveStudentsToFile(students);
                    Console.WriteLine($"{studentToRemove.Name} has been successfully removed.");
                }
                else
                {
                    Console.WriteLine($"No student found with ID {studentID}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Pause(); 
        }
    }
}