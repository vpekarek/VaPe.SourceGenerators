using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace VaPe;

public static class SourceGeneratorExtension
{
    public static IEnumerable<AttributeSyntax> GetAttributes<TDeclarationSyntax>(this TDeclarationSyntax declarationSyntax)
        where TDeclarationSyntax : TypeDeclarationSyntax
    {
        return declarationSyntax.AttributeLists.SelectMany(x => x.Attributes);
    }

    public static IEnumerable<AttributeSyntax> GetAttributes<TDeclarationSyntax>(this TDeclarationSyntax declarationSyntax, string attributeName)
        where TDeclarationSyntax : TypeDeclarationSyntax
    {
        const string attributeSuffix = "Attribute";
        if (attributeName.EndsWith(attributeSuffix))
        {
            attributeName = attributeName.Substring(0, attributeName.Length - attributeSuffix.Length);
        }

        return declarationSyntax.GetAttributes().Where(x => x.Name is IdentifierNameSyntax ins && ins.Identifier.ValueText == attributeName);
    }

    public static AttributeSyntax? GetAttribute<TDeclarationSyntax>(this TDeclarationSyntax declarationSyntax, string attributeName)
        where TDeclarationSyntax : TypeDeclarationSyntax
    {
        return declarationSyntax.GetAttributes(attributeName).FirstOrDefault();
    }
}