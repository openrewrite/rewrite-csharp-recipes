using Rewrite.Core;
using Rewrite.Core.Marker;
using Rewrite.RewriteJava;
using Rewrite.RewriteJava.Tree;
using ExecutionContext = Rewrite.Core.ExecutionContext;

namespace Rewrite.Recipes;

public class FindClass(string? description): Recipe
{

    public override ITreeVisitor<Tree, ExecutionContext> GetVisitor()
    {
        return new FindClassVisitor(description);
    }

    private class FindClassVisitor(string? description) : JavaVisitor<ExecutionContext>
    {
        public override J VisitClassDeclaration(J.ClassDeclaration classDeclaration, ExecutionContext ctx)
        {
            var tree = (MutableTree<J.ClassDeclaration>)base.VisitClassDeclaration(classDeclaration, ctx)!;
            return tree.WithMarkers(tree.Markers.AddIfAbsent<SearchResult>(new SearchResult(Tree.RandomId(), description)));
        }
    }
}
