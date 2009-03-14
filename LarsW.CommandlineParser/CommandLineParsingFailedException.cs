namespace LarsW.CommandLineParser
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CommandLineParsingFailedException : Exception
    {
        public CommandLineParsingFailedException()
        {
        }

        public CommandLineParsingFailedException(string message) : base(message)
        {
        }

        public CommandLineParsingFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CommandLineParsingFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}