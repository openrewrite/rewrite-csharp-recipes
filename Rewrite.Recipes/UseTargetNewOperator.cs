using Rewrite.Core;
using Rewrite.Core.Marker;
using Rewrite.RewriteJava;
using Rewrite.RewriteJava.Tree;
using ExecutionContext = Rewrite.Core.ExecutionContext;

namespace Rewrite.Recipes;

public class UseTargetNewOperator : Recipe
{
    public override string DisplayName => "Use the target-typed new operator";
    public override string Description => "Replaces full typed objects with the target-typed new operator (new()).";
    
    public override ITreeVisitor<Tree, ExecutionContext> GetVisitor()
    {
        return new UseTargetNewOperatorVisitor();
    }

    private class UseTargetNewOperatorVisitor : JavaVisitor<ExecutionContext>
    {
        public override J? VisitVariableDeclarations(J.VariableDeclarations variableDeclarations, ExecutionContext ctx)
        {
            var varDecls = (J.VariableDeclarations)base.VisitVariableDeclarations(variableDeclarations, ctx);
            if (varDecls.Variables.Count == 1 && varDecls.Variables.First().Initializer != null && varDecls.TypeExpression is J.ParameterizedType) {
                varDecls = varDecls.WithVariables(varDecls.Variables.Map(nv => {
                    if (nv.Initializer is J.NewClass) {
                        nv = nv.WithInitializer(MaybeRemoveParams((J.NewClass) nv.Initializer));
                    }
                    return nv;
                }));
            }
            return varDecls;
        }

        public override J? VisitAssignment(J.Assignment assignment, ExecutionContext ctx)
        {
            var asgn = (J.Assignment) base.VisitAssignment(assignment, ctx);
            if (asgn.Expression is J.NewClass { Clazz: J.ParameterizedType } a) {
                asgn = asgn.WithExpression(MaybeRemoveParams(a));
            }
            return asgn;
        }

        private J.NewClass MaybeRemoveParams(J.NewClass newClass) {
            if (newClass.Body != null || newClass.Clazz is not J.ParameterizedType newClassType) return newClass;
            if (newClassType.TypeParameters == null) return newClass;
            return newClass.WithClazz(new J.Empty(Guid.NewGuid(), Space.EMPTY, Markers.EMPTY));
        }
    }
}
