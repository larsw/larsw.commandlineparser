namespace LarsW.CommandLineParser.TestApp
{
    using System;
    using System.Text;

    class Processor
    {
        private StringBuilder _output = new StringBuilder();
        public bool BogusSet { get; set; }

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
            BogusSet = true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new Processor();
            Console.WriteLine("Processing the command line:");
            CommandLineProcessor.Process(processor);
            Console.WriteLine("Output: " + processor.Output);
            Console.WriteLine("Bogus set: " + processor.BogusSet);
            Console.ReadLine();
        }
    }
}