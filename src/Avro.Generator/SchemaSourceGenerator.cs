using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Avro.Generator
{
    [Generator]
    public class SchemaSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // nothing to do here
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var schemas = context.AdditionalFiles
                .Where(x => Path.GetExtension(x.Path).Equals(".avsc", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var text = x.GetText();
                    return text != null ? Schema.Parse(text.ToString()) : null;
                }).Where(x => x != null);

            var codeGen = new CodeGen();
            foreach (var schema in schemas)
            {
                codeGen.AddSchema(schema);
            }

            codeGen.GenerateCode();
            var sources = codeGen.GetTypes();
            foreach (var source in sources)
            {
                context.AddSource(source.Key, SourceText.From(source.Value, Encoding.UTF8));
            }
        }
    }
}
