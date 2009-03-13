namespace LarsW.CommandlineParser
{
    using System;

    public sealed class CommandLineArgumentHandlerAttribute : Attribute
    {
        public string ShortArgumentName { get; private set; }
        public string LongArgumentName { get; private set; }
        public string Description { get; private set; }

        public CommandLineArgumentHandlerAttribute(string shortArgumentName, string longArgumentName, string description)
        {
            ShortArgumentName = shortArgumentName;
            LongArgumentName = longArgumentName;
            Description = description;
        }
    }
}