using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libraryGenerator
{
    public class TestClass
    {
        public string name { get; set; }
        public string include { get; set; }

        public TestClass(string Name, string Include)
        {
            name = Name;
            include = Include;
        }
    }
}
