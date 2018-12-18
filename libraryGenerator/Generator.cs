using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace libraryGenerator
{
    public class Generator
    {
        private int maxWrite;
        private int maxRead;
        private int maxTask;
        private Reader reader;
        private Writer writer;

        public Generator(int maxWrite, int maxRead, int maxTask, Reader reader, Writer writer)
        {
            this.maxWrite = maxWrite;
            this.maxRead = maxRead;
            this.maxTask = maxTask;
            this.reader = reader;
            this.writer = writer;
        }

        private MethodDeclarationSyntax MethodDeclar(string name)
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),Identifier(name + "_" + "Test")).WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestMethod"))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(ExpressionStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,IdentifierName("Assert"),IdentifierName("Fail")))
                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal("test")))))))));
        }

        private MemberDeclarationSyntax ClassDeclar(string name, List<MemberDeclarationSyntax> methods)
        {
            return ClassDeclaration(name).WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestClass"))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithMembers(List(methods));
        }

        private CompilationUnitSyntax ResultDeclar(List<UsingDirectiveSyntax> directiveSyntaxes, MemberDeclarationSyntax member, string param)
        {
            return CompilationUnit().WithUsings(List(directiveSyntaxes)).WithMembers(SingletonList<MemberDeclarationSyntax>(NamespaceDeclaration(QualifiedName(IdentifierName(param),IdentifierName("Test")))))
                .WithMembers(SingletonList(member));
        }

        private async Task<List<TestClass>> TestsFiles(string code)
        {
            CompilationUnitSyntax unitSyntax = ParseCompilationUnit(code);
            var classes = unitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>();
            List<TestClass> testClasses = new List<TestClass>();
            List<UsingDirectiveSyntax> usings = new List<UsingDirectiveSyntax>
            {
                UsingDirective(QualifiedName(IdentifierName("Microsoft.VisualStudio"), IdentifierName("TestTools.UnitTesting")))
            };
            foreach (var clas in classes)
            {
                var methods = clas.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(x => x.Modifiers.Any(y => y.ValueText == "public"));
                string nameSpace = (clas.Parent as NamespaceDeclarationSyntax)?.Name.ToString();
                List<MemberDeclarationSyntax> members = new List<MemberDeclarationSyntax>();
                foreach(var method in methods)
                {
                    string name = method.Identifier.ToString();
                    int count = 1;
                    if (members.Count != 0 && members.Any(x => (x as MethodDeclarationSyntax)?.Identifier.ToString() == method.Identifier.ToString() + "_Test"))
                    {
                        while (members.Any(x => (x as MethodDeclarationSyntax)?.Identifier.ToString() == method.Identifier.ToString() + count + "_Test"))
                            count++;
                        name += count;
                    }
                    members.Add(MethodDeclar(name));
                }
                var classdeclar = ClassDeclar(clas.Identifier.ValueText + "Test", members);
                if (nameSpace == null)
                    testClasses.Add(new TestClass(clas.Identifier.ValueText + "Test", ResultDeclar(usings, classdeclar, "Global").NormalizeWhitespace().ToFullString()));
                else
                    testClasses.Add(new TestClass(clas.Identifier.ValueText + "Test", ResultDeclar(usings, classdeclar, nameSpace).NormalizeWhitespace().ToFullString()));
            }
            return testClasses;
        }

        public Task Generate(List<string> files)
        {
            var readblock = new TransformBlock<string, string>(
                new Func<string, Task<string>>(reader.Read),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxRead }
                );
            var taskblock = new TransformBlock<string, List<TestClass>>(
                new Func<string, Task<List<TestClass>>>(TestsFiles),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxTask }
                );
            var writeblock = new ActionBlock<List<TestClass>>(
                new Action<List<TestClass>>((x) => writer.Write(x).Wait()),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWrite }
                );

            var option = new DataflowLinkOptions {PropagateCompletion = true };
            readblock.LinkTo(taskblock,option);
            taskblock.LinkTo(writeblock,option);
            foreach (var file in files)
                readblock.Post(file);
            readblock.Complete();
            return writeblock.Completion;
        }
    }
}
