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
                Path.Combine("classes","Generator.cs"),
                Path.Combine("classes","Program.cs"),
                Path.Combine("classes","Reader.cs"),
                Path.Combine("classes","TestClass.cs"),
                Path.Combine("classes","Writer.cs")
            }
                ).Wait();
            Console.WriteLine("Finish...");
            Console.ReadKey();
        }
    }
}
