using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Student_Document_Management_for_G12
{
    public class GoodMoral : Document
    {
        public GoodMoral(bool isRequested = false) : base("Good Moral", isRequested) { }
    }
}