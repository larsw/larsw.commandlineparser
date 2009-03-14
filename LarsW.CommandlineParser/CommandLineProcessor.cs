using System.Text;

namespace LarsW.CommandLineParser
{
    using System;
    using System.Collections.Generic;
    using System.Dataflow;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class CommandLineProcessor
    {
        internal class Handler
        {
            public MethodInfo MethodInfo { get; set; }
            public CommandLineArgumentHandlerAttribute Attribute { get; set; }
        }
        private static List<Handler> _handlers = new List<Handler>();

        public event EventHandler<CommandLineArgumentArgs> ProcessingArgument = delegate { };

        public static void Process(object handlerInstance, string[] args)
        {
            var parser = DynamicParser.LoadFromResource(Assembly.GetExecutingAssembly().GetName().Name, "LarsW.Languages.CommandLineLang");

            string commandLine = null;
            commandLine = BuildCommandLine(args);
            var errorReporter = ErrorReporter.Standard;
            var root = parser.Parse<object>(null, new StringReader(commandLine), errorReporter);
            if (root == null)
            {
                throw new CommandLineParsingFailedException();
            }
            var builder = parser.GraphBuilder;

            var handlerType = handlerInstance.GetType();
            _handlers = (from methodInfo in handlerType.GetMethods()
                            let attr =
                                Attribute.GetCustomAttribute(methodInfo, typeof (CommandLineArgumentHandlerAttribute))
                                as CommandLineArgumentHandlerAttribute
                            where attr != null
                            select new Handler {MethodInfo = methodInfo, Attribute = attr}).ToList();
            
            foreach (var element in builder.GetSuccessors(root))
            {
                var identifier = builder.GetLabel(element) as Identifier;
                if (identifier != null && identifier == Identifier.Get("Parameter"))
                {
                    ProcessParameter(builder, element, handlerInstance);
                }
            }
        }

        private static string BuildCommandLine(string[] args)
        {
            var sb = new StringBuilder();
            for(int i = 0; i < args.Length; i++)
            {
                string val = args[i];
                if (val.Contains(" ") || val.Contains('\t'))
                    val = string.Concat("\"", val, "\"");

                sb.Append(string.Concat(val, " "));
            }
            return sb.ToString().Trim();
        }

        private static void ProcessParameter(IGraphBuilder builder, object parameterNode, object handlerInstance)
        {
            
            var subNodes = builder.GetSuccessors(parameterNode);
            Debug.Assert(subNodes != null && subNodes.Count() == 2);

            var nameNode = subNodes.First();
            Debug.Assert(builder.IsNode(nameNode));
            var nameValue = builder.GetSuccessors(nameNode).First().ToString();

            var payloadNode = subNodes.Last();
            var payloadValueNodes = builder.GetSuccessors(payloadNode);
            string payloadValue = null;
            if (payloadValueNodes.Count() == 1)
            {
                payloadValue = payloadValueNodes.First().ToString();
                Debug.Assert(builder.IsNode(payloadNode));
            }
            var handler = _handlers.Find(h => (h.Attribute.LongArgumentName == nameValue ||
                                 h.Attribute.ShortArgumentName == nameValue));
            if (handler == null)
                throw new CommandLineArgumentHandlerNotFoundException(nameValue);
            if (payloadValue != null)
                handler.MethodInfo.Invoke(handlerInstance, new object[] {payloadValue});
            else
                handler.MethodInfo.Invoke(handlerInstance, null);
        }
    }
}
