using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace VaPe;

public static class AttributeExtensions
{
    /// <summary>
    /// Gets dictionary of attribute properties specified by argument in attribute constructor.
    /// </summary>
    /// <param name="argumentList">Argument list.</param>
    /// <returns>Dictionary with attribute property name as key.</returns>
    public static Dictionary<string, object> GetAttributeProperties(this AttributeArgumentListSyntax argumentList)
    {
        var dic = new Dictionary<string, object>();

        foreach (var attributeParam in argumentList.Arguments)
        {
            if (attributeParam is AttributeArgumentSyntax ags)
            {
                if (ags.Expression is LiteralExpressionSyntax les)
                {
                    var kind = les.Kind();
                    object value = kind switch
                    {
                        Microsoft.CodeAnalysis.CSharp.SyntaxKind.FalseLiteralExpression => false,
                        Microsoft.CodeAnalysis.CSharp.SyntaxKind.TrueLiteralExpression => true,
                        Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralExpression => ParseNumericLiteral(ags.Expression.GetText()),
                        _ => ags.Expression.GetText().ToString().Trim('"'),
                    };

                    dic.Add(ags.NameEquals?.ToFullString().Trim().TrimEnd('=').Trim() ?? string.Empty, value);
                }
            }
        }

        return dic;

        static object ParseNumericLiteral(Microsoft.CodeAnalysis.Text.SourceText text)
        {
            if (int.TryParse(text.ToString(), out var intResult))
            {
                return intResult;
            }

            if (decimal.TryParse(text.ToString(), out var decimalResult))
            {
                return decimalResult;
            }

            return 0;
        }
    }

    /// <summary>
    /// Get single attribute property specified by argument in attribute constructor.
    /// </summary>
    /// <param name="argumentList">Argument list.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Property value, or null.</returns>
    public static object? GetAttributeProperty(this AttributeArgumentListSyntax argumentList, string propertyName)
    {
        var list = argumentList.GetAttributeProperties();

        if (list.ContainsKey(propertyName))
        {
            return list[propertyName];
        }

        return null;
    }

    /// <summary>
    /// Get single attribute property specified by argument in attribute constructor.
    /// </summary>
    /// <typeparam name="T">Type of expected property.</typeparam>
    /// <param name="argumentList">Argument list.</param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Property value, null or default <typeparamref name="T"/>.</returns>
    public static T? GetAttributeProperty<T>(this AttributeArgumentListSyntax argumentList, string propertyName)
    {
        var obj = argumentList.GetAttributeProperty(propertyName);

        if (obj == null)
        {
            return default;
        }

        try
        {
            return (T)obj;
        }
        catch
        {
            return default;
        }
    }
}
