namespace LarsW.CommandLineParser
{
    using System;
    using System.Collections.Generic;
    using System.Dataflow;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Processes the command line and invoke handlers for the command line arguments.
    /// </summary>
    public class CommandLineProcessor
    {
        private static List<Handler> _handlers = new List<Handler>();
        private const string LanguageName = "LarsW.Languages.CommandLineLang";

        public static string CurrentCommandline
        {
            get
            {
                return Environment.CommandLine.Substring(Environment.CommandLine.IndexOf(' ')).Trim();
            }
        }

        /// <summary>
        /// Processes the command line.
        /// </summary>
        /// <param name="handlerInstance">Instance that contains the parameter handlers.</param>
        public static void Process(object handlerInstance)
        {
            Process(handlerInstance, CurrentCommandline);
        }

        /// <summary>
        /// Processes a command line.
        /// </summary>
        /// <param name="handlerInstance">Instance that contains the parameter handlers.</param>
        /// <param name="commandLine">The command line.</param>
        public static void Process(object handlerInstance, string commandLine)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var parser = DynamicParser.LoadFromResource(assemblyName, LanguageName);
            var errorReporter = ErrorReporter.Standard;
            var root = parser.Parse<object>(null, new StringReader(commandLine), errorReporter);
            if (root == null)
            {
                throw new CommandLineParsingFailedException();
            }
            var builder = parser.GraphBuilder;

            var handlerType = handlerInstance.GetType();
            DiscoverHandlerMethods(handlerType);
            
            foreach (var element in builder.GetSuccessors(root))
            {
                var identifier = builder.GetLabel(element) as Identifier;
                if (identifier != null && identifier == Identifier.Get("Parameter"))
                {
                    ProcessParameter(builder, element, handlerInstance);
                }
            }
        }

        private static void DiscoverHandlerMethods(Type handlerType)
        {
            _handlers = (from methodInfo in handlerType.GetMethods()
                         let attr =
                             Attribute.GetCustomAttribute(methodInfo, typeof (CommandLineArgumentHandlerAttribute))
                             as CommandLineArgumentHandlerAttribute
                         where attr != null
                         select new Handler {MethodInfo = methodInfo, Attribute = attr}).ToList();
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
            InvokeHandler(nameValue, payloadValue, handlerInstance);
        }

        private static void InvokeHandler(string nameValue, string payloadValue, object handlerInstance)
        {
            var handler = _handlers.Find(h => (h.Attribute.LongArgumentName == nameValue ||
                                               h.Attribute.ShortArgumentName == nameValue));
            if (handler == null)
                throw new CommandLineArgumentHandlerNotFoundException(nameValue);
            if (payloadValue != null)
                handler.MethodInfo.Invoke(handlerInstance, new object[] {payloadValue});
            else
                handler.MethodInfo.Invoke(handlerInstance, null);
        }

        class Handler
        {
            public MethodInfo MethodInfo { get; set; }
            public CommandLineArgumentHandlerAttribute Attribute { get; set; }
        }
    }
}