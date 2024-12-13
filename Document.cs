using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Student_Document_Management_for_G12
{
    public class Document
    {
        public string Name { get; private set; } 
        public bool IsRequested { get; private set; } 

        public Document(string name, bool isRequested = false)
        {
            Name = name;
            IsRequested = isRequested;
        }

        public void MarkAsRequested()
        {
            IsRequested = true;
        }
        public void ResetRequest()
        {
            IsRequested = false;
        }
        public void SetRequestedStatus(bool status)
        {
            IsRequested = status;
        }
    }
}