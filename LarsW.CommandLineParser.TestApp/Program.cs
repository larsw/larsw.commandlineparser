namespace Mockup.CommandLineParser
{
    using LarsW.CommandlineParser;
using System.Text;

    class Processor
    {
        private StringBuilder _output = new StringBuilder();
        public string Output
        {
            get { return _output.ToString(); }
        }

        [CommandLineArgumentHandler("a", "add", "Adds the payload to the output.")]
        public void AHandler(string payload)
        {
            _output.Append(payload);
        }

        [CommandLineArgumentHandler("b", "bogus", "Just a bogus test command.")]
        public void BHandler()
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new Processor();
            CommandLineProcessor.Process(processor, args);
        }
    }
}
