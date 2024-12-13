using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Student_Document_Management_for_G12
{
    public class BirthCertificate : Document
    {
        public BirthCertificate(bool isRequested = false) : base("Birth Certificate", isRequested) { }
    }
}