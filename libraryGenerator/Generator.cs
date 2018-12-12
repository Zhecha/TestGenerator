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
        private MethodDeclarationSyntax MethodDeclar(string name)
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),Identifier("Test" + name)).WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("TestMethod"))))))
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


    }
}
