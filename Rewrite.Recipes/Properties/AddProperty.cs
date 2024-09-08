using System.Linq;
using Rewrite.Core;
using Rewrite.Core.Marker;
using Rewrite.RewriteProperties;
using ExecutionContext = Rewrite.Core.ExecutionContext;

namespace Rewrite.Recipes.Properties;

using ExecutionContext = ExecutionContext;

public class AddProperty(string property, string value) : Recipe
{
    public override ITreeVisitor<Tree, ExecutionContext> GetVisitor()
    {
        return new AddPropertyVisitor(property, value);
    }

    private class AddPropertyVisitor(string property, string value) : PropertiesVisitor<ExecutionContext>
    {
        public override RewriteProperties.Tree.Properties VisitFile(RewriteProperties.Tree.Properties.File file, ExecutionContext p)
        {
            var first = file.Content.FirstOrDefault(e => e is RewriteProperties.Tree.Properties.Entry entry && entry.Key == property);
            if (first == null)
            {
                var entry = new RewriteProperties.Tree.Properties.Entry(Tree.RandomId(), "\n", Markers.EMPTY, property,
                    " ", RewriteProperties.Tree.Properties.Entry.Delimiter.EQUALS,
                    new RewriteProperties.Tree.Properties.Value(Tree.RandomId(), " ", Markers.EMPTY, value));
                return file.WithContent(ListUtils.Concat(file.Content, entry).ToList());
            }
            else
            {
                return file.WithContent(ListUtils.Map(file.Content, e =>
                {
                    if (e is RewriteProperties.Tree.Properties.Entry entry && entry.Key == property)
                    {
                        return entry.Value.Text.Equals(value) ? entry : entry.WithValue(entry.Value.WithText(value));
                    }

                    return e;
                }));
            }
        }
    }
}
