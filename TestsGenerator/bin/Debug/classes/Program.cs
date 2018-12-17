using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using libraryGenerator;

namespace TestsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader();
            Writer writer = new Writer("Tests");
            Generator generator = new Generator(1, 1, 1, reader, writer);
            generator.Generate(new List<string>
            {
                Path.Combine("in","Writer.cs"),
                Path.Combine("in","Reader.cs"),
                Path.Combine("in","TestClass.cs"),
                Path.Combine("in","Generator.cs"),
                Path.Combine("in","Program.cs")
            }
                ).Wait();
            Console.WriteLine("Finish...");
            Console.ReadKey();
        }
    }
}
