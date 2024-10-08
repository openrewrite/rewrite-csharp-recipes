using System.Text;
using Rewrite.Core;
using Rewrite.RewriteJava;
using Rewrite.RewriteJava.Tree;
using ExecutionContext = Rewrite.Core.ExecutionContext;

namespace Rewrite.Recipes;

public class InvertAssertion : Recipe
{
    private const string ASSERT_TRUE = "Xunit.Assert.True";
    
    public override string DisplayName => "Invert Assertion";
    public override string Description => "Find for all the `Assert.True(!someBool)` and transform it into `Assert.False(someBool)`.";

    public override ITreeVisitor<Tree, ExecutionContext> GetVisitor()
    {
        return new InvertAssertionVisitor();
    }

    private class InvertAssertionVisitor : JavaVisitor<ExecutionContext>
    {
        public override J.MethodInvocation VisitMethodInvocation(J.MethodInvocation method, ExecutionContext ctx)
        {
            // Assert.True(!a);
            var mi = (J.MethodInvocation)base.VisitMethodInvocation(method, ctx);

            if (!ASSERT_TRUE.EndsWith(ExtractName(mi)) || !IsUnaryOperatorNot(mi)) return mi;

            var unary = (J.Unary)mi.Arguments[0];

            return mi.WithArguments([unary.Expression]).WithName(mi.Name.WithSimpleName("False"));
        }

        private static string ExtractName(J.MethodInvocation mi)
        {
            return (mi.Select is J.Identifier i ? (i.SimpleName + ".") : "") + mi.Name.SimpleName;
        }

        private static bool IsUnaryOperatorNot(J.MethodInvocation method)
        {
            return method.Arguments is [J.Unary { Operator: J.Unary.Type.Not }];
        }
    };
}
