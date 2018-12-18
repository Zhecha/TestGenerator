using Microsoft.VisualStudio.TestTools.UnitTesting;
using libraryGenerator;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace UnitTestsGenerator
{
    [TestClass]
    public class UnitTest1
    {
        static Reader reader;
        static Writer writer;
        static string outDir;
        static string inDir;
        static Generator generator;
        static CompilationUnitSyntax generTest;

        [TestInitialize]
        public void Init()
        {
            outDir = Path.Combine("..","outdir");
            inDir = Path.Combine("..","inputdir");
            reader = new Reader();
            writer = new Writer(outDir);
            generator = new Generator(1,1,1,reader,writer);
            generator.Generate(Directory.GetFiles(inDir).ToList()).Wait();
            generTest = ParseCompilationUnit(File.ReadAllText(Path.Combine(outDir,"Test1Test.cs")));
        }

        [TestMethod]
        public void TestFilesCount()
        {
            Assert.AreEqual(2,Directory.GetFiles(outDir).Count());
        }

        [TestMethod]
        public void TestClassCount()
        {
            Assert.AreEqual(1,generTest.DescendantNodes().OfType<ClassDeclarationSyntax>().Count());
        }

        [TestMethod]
        public void TestUsing()
        {
            Assert.IsTrue(generTest.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(x => x.Name.ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting"));
        }

        [TestMethod]
        public void TestAttributes()
        {
            Assert.IsTrue(generTest.DescendantNodes().OfType<ClassDeclarationSyntax>().Any(x => x.AttributeLists.Any(z => z.Attributes.Any(m => m.ToString() == "TestClass"))));
            Assert.IsTrue(generTest.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(x => x.AttributeLists.Any(z => z.Attributes.Any(m => m.ToString() == "TestMethod"))));
        }
    }

}
