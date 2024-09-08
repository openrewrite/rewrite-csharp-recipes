using Rewrite.RewriteCSharp.Test.Api;
using Rewrite.Test;
using Xunit;

namespace Rewrite.Recipes;

using static Assertions;

[Collection("C# remoting")]
public class Tests : RewriteTest
{
    public override void Defaults(RecipeSpec spec)
    {
        spec.Recipe = new InvertAssertion();
    }

    [Fact]
    public void VerifyItWorksTest()
    {
        RewriteRun(
            CSharp(
                """
                    class MyClass
                    {
                        void test()
                        {
                            bool a = false;
                            Assert.True(!a);
                        }
                    }             
                """,
                """
                    class MyClass
                    {
                        void test()
                        {
                            bool a = false;
                            Assert.False(a);
                        }
                    }             
                """
            )
        );
    }
}
