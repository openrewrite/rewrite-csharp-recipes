using FluentAssertions;
using Rewrite.Core;
using Rewrite.Core.Marker;
using Rewrite.Recipes;
using Rewrite.RewriteJava;
using Rewrite.RewriteJava.Tree;
using Rewrite.Test;
using Xunit;
using FileAttributes = Rewrite.Core.FileAttributes;

namespace Rewrite.Java.Tests;

public class FindClassTests : RewriteTest
{

    [Fact]
    public void Test1()
    {
        var source = new J.CompilationUnit(
            Tree.RandomId(),
            Space.EMPTY,
            Markers.EMPTY, "Foo.java", new FileAttributes(), null, false, null, null, [],
            [
                new J.ClassDeclaration(
                    Tree.RandomId(),
                    Space.EMPTY,
                    Markers.EMPTY,
                    [],
                    [],
                    new J.ClassDeclaration.Kind(Tree.RandomId(), Space.EMPTY, Markers.EMPTY, [],
                        J.ClassDeclaration.Kind.Type.Class),
                    new J.Identifier(Tree.RandomId(), Space.EMPTY, Markers.EMPTY, [], "Foo", null, null),
                    null,
                    null,
                    null,
                    null,
                    null,
                    new J.Block(Tree.RandomId(), Space.EMPTY, Markers.EMPTY,
                        new JRightPadded<bool>(false, Space.EMPTY, Markers.EMPTY), [], Space.EMPTY),
                    null
                )
            ],
            Space.EMPTY
        );
        var after = new FindClass().GetVisitor().Visit(source, new InMemoryExecutionContext()) as J.CompilationUnit;
        after.Should().NotBeSameAs(source);
        after.Classes[0].Markers.MarkerList.Should().Contain(e => e is SearchResult);
    }
}
