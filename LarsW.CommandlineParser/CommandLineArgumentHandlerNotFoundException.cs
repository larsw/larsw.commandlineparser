using System;
using System.Runtime.Serialization;

namespace LarsW.CommandLineParser
{
    [Serializable]
    public class CommandLineArgumentHandlerNotFoundException : Exception
    {
        public CommandLineArgumentHandlerNotFoundException()
        {
        }

        public CommandLineArgumentHandlerNotFoundException(string message) : base(message)
        {
        }

        public CommandLineArgumentHandlerNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CommandLineArgumentHandlerNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}