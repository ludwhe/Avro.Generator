using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Avro.Generator
{
    [Generator]
    public class ProtocolSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // nothing to do here
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var protocols = context.AdditionalFiles
                .Where(x => Path.GetExtension(x.Path).Equals(".avproto", StringComparison.OrdinalIgnoreCase))
                .Select(x =>
                {
                    var text = x.GetText();
                    return text != null ? Protocol.Parse(text.ToString()) : null;
                }).Where(x => x != null);

            var codeGen = new CodeGen();
            foreach (var protocol in protocols)
            {
                codeGen.AddProtocol(protocol);
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
