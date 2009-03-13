using System;

namespace LarsW.CommandlineParser
{
    public sealed class CommandlineArgumentArgs : EventArgs
    {
        public string ShortArgumentName { get; private set; }
        public string LongArgumentName { get; private set; }
        public string Argument { get; private set; }

        public CommandlineArgumentArgs(string shortArgumentName, string longArgumentName, string argument)
        {
            ShortArgumentName = shortArgumentName;
            LongArgumentName = longArgumentName;
            Argument = argument;
        }
    }
}