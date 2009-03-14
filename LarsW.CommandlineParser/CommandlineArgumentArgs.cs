namespace LarsW.CommandLineParser
{
    using System;

    public sealed class CommandLineArgumentArgs : EventArgs
    {
        public string ShortArgumentName { get; private set; }
        public string LongArgumentName { get; private set; }
        public string Argument { get; private set; }

        public CommandLineArgumentArgs(string shortArgumentName, string longArgumentName, string argument)
        {
            ShortArgumentName = shortArgumentName;
            LongArgumentName = longArgumentName;
            Argument = argument;
        }
    }
}