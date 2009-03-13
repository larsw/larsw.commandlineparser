namespace LarsW.CommandlineParser
{
    using System;
    using System.Dataflow;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class CommandLineProcessor
    {
        private static bool _initialized;
        private static DynamicParser _parser;

        public event EventHandler<CommandlineArgumentArgs> ProcessingArgument = delegate { };

        public static void Process(object handlerInstance, string[] args)
        {
            if (!_initialized)
            {
                Initialize();
            }
            var reader = new StringReader(string.Join(" ", args));
            var root = _parser.Parse<object>(null, reader, ErrorReporter.Standard);
            var builder = _parser.GraphBuilder;
            foreach (var element in builder.GetSuccessors(root))
            {
                var identifier = builder.GetLabel(element) as Identifier;
                if (identifier != null && identifier == Identifier.Get("Parameter"))
                {
                    ProcessParameter(builder, element);
                }
            }
        }

        private static void Initialize()
        {
            _parser = DynamicParser.LoadFromResource(Assembly.GetExecutingAssembly().GetName().Name, "LarsW.Languages.CommandLineLang");
            _initialized = true;
        }

        private static void ProcessParameter(IGraphBuilder builder, object parameterNode)
        {
            var subNodes = builder.GetSuccessors(parameterNode);
            Debug.Assert(subNodes != null && subNodes.Count() == 2);

            var nameNode = subNodes.First();
            Debug.Assert(builder.IsNode(nameNode));
            var nameValue = builder.GetSuccessors(nameNode).First();
            Console.WriteLine("Parameter: " + Convert.ToString(nameValue));

            var payloadNode = subNodes.Last();
            var payloadValueNodes = builder.GetSuccessors(payloadNode);
            if (payloadValueNodes.Count() == 1)
            {
                var payloadValue = payloadValueNodes.First();
                Debug.Assert(builder.IsNode(payloadNode));
                Console.WriteLine("Value: " + Convert.ToString(payloadValue));
            }
            else
            {
                Console.WriteLine("[No value]");
            }
        }

    }
}
