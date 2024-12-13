using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Student_Document_Management_for_G12
{
    public class Person
    {
        public string Name;

        public Person(string name)
        {
            Name = name;
        }

        public void ChangeName(string value)
        {
            Name = value;
        }

        public void DisplayName()
        {
            Console.WriteLine($"Name: {Name}");
        }
    }
}