using Student_Document_Management_for_G12;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;


namespace Student_Document_Management_for_G12
{
    public class Student : Person
    {
        public List<Document> Documents { get; private set; } = new List<Document>();
        public Dictionary<string, List<string>> Transactions { get; set; } = new Dictionary<string, List<string>>();
        public string StudentID { get; set; }
        public double TotalAmount { get; private set; }
        public List<string> TransactionHistory { get; private set; } = new List<string>();

        public bool GoodMoral => Documents.OfType<GoodMoral>().Any(d => d.IsRequested);
        public bool BirthCertificate => Documents.OfType<BirthCertificate>().Any(d => d.IsRequested);
        public bool Form138 => Documents.OfType<Form138>().Any(d => d.IsRequested);

        private const decimal GoodMoralFee = 100.00m;
        private const decimal BirthCertificateFee = 0.00m;
        private const decimal Form138Fee = 150.00m;

        public Student() : base("Default Name")
        {
            InitializeStudent("Default Name", GenerateStudentID());
        }
        public Student(string name) : base(name)
        {
            InitializeStudent(name, GenerateStudentID());
        }

        public Student(string name, string studentID, bool isManual = false) : base(name)
        {
            InitializeStudent(name, isManual ? studentID : GenerateStudentID());
        }

        public Student(string studentID, string name) : base(name)
        {
            InitializeStudent(name, studentID);
        }

        private void InitializeStudent(string name, string studentID)
        {
            this.Name = name;
            this.StudentID = studentID; 
            this.Documents = new List<Document>
        {
        new GoodMoral(),
        new BirthCertificate(),
        new Form138()
        };
        }


        public string GetStudentID()
        {
            return StudentID;
        }

        public string GenerateStudentID()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randomID = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
            return "CIT-" + randomID;
        }

        public void DisplayStatus()
        {
            Console.WriteLine($"Student ID: {StudentID}");
            Console.WriteLine($"Student: {Name}");
            Console.WriteLine($"Good Moral: {(GoodMoral ? "Requested" : "Still in the school's possession")}");
            Console.WriteLine($"Birth Certificate: {(BirthCertificate ? "Requested" : "Still in the school's possession")}");
            Console.WriteLine($"Form 138: {(Form138 ? "Requested" : "Still in the school's possession")}");
        }
        public List<string> RequestDocuments(List<string> documents, out decimal totalAmount)
        {
            List<string> newlyClaimedDocuments = new List<string>();
            totalAmount = 0;

            foreach (string document in documents)
            {
                switch (document.ToLower().Trim())
                {
                    case "good moral":
                        var goodMoralDoc = Documents.OfType<GoodMoral>().FirstOrDefault();
                        if (goodMoralDoc != null && !goodMoralDoc.IsRequested)
                        {
                            goodMoralDoc.MarkAsRequested(); 
                            totalAmount += GoodMoralFee;
                            newlyClaimedDocuments.Add("Good Moral");
                        }
                        break;

                    case "birth certificate":
                        var birthCertDoc = Documents.OfType<BirthCertificate>().FirstOrDefault();
                        if (birthCertDoc != null && !birthCertDoc.IsRequested)
                        {
                            birthCertDoc.MarkAsRequested(); 
                            totalAmount += BirthCertificateFee;
                            newlyClaimedDocuments.Add("Birth Certificate");
                        }
                        break;

                    case "form 138":
                        var form138Doc = Documents.OfType<Form138>().FirstOrDefault();
                        if (form138Doc != null && !form138Doc.IsRequested)
                        {
                            form138Doc.MarkAsRequested(); 
                            totalAmount += Form138Fee;
                            newlyClaimedDocuments.Add("Form 138");
                        }
                        break;
                }
            }
            return newlyClaimedDocuments;
        }
        public void GenerateReceipt(List<string> newlyClaimedDocuments, decimal totalAmount, string transactionNumber, string transactionHandler)
        {
            string filePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Receipt Data\Receipt Data.txt";

            try
            {
                DateTime expectedReleaseDate;
                while (true)
                {
                    Console.Write("Enter the expected date to be released (MM/DD/YYYY): ");
                    string input = Console.ReadLine();

                    if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out expectedReleaseDate))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please enter the date in MM/DD/YYYY format.");
                    }
                }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine("<----- Document Request Receipt ----->");
                    writer.WriteLine($"Transaction Number: {transactionNumber}");
                    writer.WriteLine($"Student ID: {StudentID}");
                    writer.WriteLine($"Student Name: {Name}");
                    writer.WriteLine($"Date Issued: {DateTime.Now:MM/dd/yyyy}");
                    writer.WriteLine($"Expected Date to be Released: {expectedReleaseDate:MM/dd/yyyy}");
                    writer.WriteLine();
                    writer.WriteLine($"Handler's Name: {transactionHandler}");
                    writer.WriteLine("------------------------------------------------");
                    writer.WriteLine("Requested Documents:");

                    foreach (string document in newlyClaimedDocuments)
                    {
                        writer.WriteLine($" - {document}");
                    }
                    writer.WriteLine($"Total Amount: {totalAmount:F2} PHP");
                    writer.WriteLine("------------------------------------------------");
                }

                Console.WriteLine("Receipt generated successfully.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error generating receipt: " + ex.Message);
            }
        }
        public void LogTransactionHistory(string transactionNumber, List<string> documents, decimal totalAmount, string handler)
        {
            string historyFilePath = @"C:\Users\HOME\Desktop\StudentDocumentManagementG12\Transaction History\Transaction Data.txt";

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(historyFilePath));

                string logEntry = $"{transactionNumber}|{this.GetStudentID()}|{this.Name}|{string.Join(",", documents)}|{totalAmount:F2}|{handler}|{DateTime.Now:MM/dd/yyyy HH:mm:ss}|Paid|Requested";

                File.AppendAllText(historyFilePath, logEntry + Environment.NewLine);

                Console.WriteLine("Transaction logged successfully.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error logging transaction: " + ex.Message);
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
        private Document GetDocumentByName(string documentName)
        {
            return Documents.FirstOrDefault(d => d.GetType().Name.Equals(documentName, StringComparison.OrdinalIgnoreCase));
        }
        public void UpdateGoodMoralStatus(bool requested)
        {
            var goodMoralDoc = Documents.OfType<GoodMoral>().FirstOrDefault();
            if (goodMoralDoc != null) goodMoralDoc.SetRequestedStatus(requested);
        }

        public void UpdateBirthCertificateStatus(bool requested)
        {
            var birthCertDoc = Documents.OfType<BirthCertificate>().FirstOrDefault();
            if (birthCertDoc != null) birthCertDoc.SetRequestedStatus(requested);
        }

        public void UpdateForm138Status(bool requested)
        {
            var form138Doc = Documents.OfType<Form138>().FirstOrDefault();
            if (form138Doc != null) form138Doc.SetRequestedStatus(requested);
        }
        public string ToDataString()
        {
            return $"{StudentID}|{Name}|{(GoodMoral ? "Requested" : "Not Requested")}|{(BirthCertificate ? "Requested" : "Not Requested")}|{(Form138 ? "Requested" : "Not Requested")}";
        }
        public static Student FromDataString(string data)
        {
            string[] fields = data.Split('|');
            if (fields.Length == 5)
            {
                string studentID = fields[0];
                string name = fields[1];

                var student = new Student(name, studentID, isManual: true); 
                if (fields[2].Equals("Requested", StringComparison.OrdinalIgnoreCase))
                    student.UpdateGoodMoralStatus(true);
                if (fields[3].Equals("Requested", StringComparison.OrdinalIgnoreCase))
                    student.UpdateBirthCertificateStatus(true);
                if (fields[4].Equals("Requested", StringComparison.OrdinalIgnoreCase))
                    student.UpdateForm138Status(true);

                return student;
            }
            return null;
        }
    }
}