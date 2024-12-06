using Rewrite.RewriteCSharp.Test.Api;
using Rewrite.Test;
using Xunit;

namespace Rewrite.Recipes;

using static Assertions;

[Collection("C# remoting")]
public class UseTargetNewOperatorTest : RewriteTest
{
    protected override void Defaults(RecipeSpec spec)
    {
        spec.Recipe = new UseTargetNewOperator();
    }

    [Fact]
    public void VerifyItWorksTest()
    {
        RewriteRun(
            CSharp(
                //language=csharp
                """
                    class MyClass
                    {
                        List<int> tests = new List<int>();
                        
                        void test()
                        {
                        }
                    }             
                """,
                //language=csharp
                """
                    class MyClass
                    {
                        List<int> tests = new();
                        
                        void test()
                        {
                        }
                    }             
                """
            )
        );
    }
}
