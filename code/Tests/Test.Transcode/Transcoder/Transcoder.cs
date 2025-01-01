/*
    Generic Transducer, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/
namespace StateMachines {
    using Console = System.Console;
    using StreamReader = System.IO.StreamReader;
    using Assembly = System.Reflection.Assembly;
    using Path = System.IO.Path;

    class Transcoder {

        enum State { Outside, ObjectStart, ArrayStart, ValueStart, StringValueStart,
            Object, Array, Key, Value, StringValue }
        enum Output { OpenTag, Value, CloseTag }

        readonly Transducer<State, Signal, Output> transducer = new();

        (Signal signal, string value) Readlexeme(string line, ref int position) {
            return transducer.CurrentState switch {
                State.Outside => (Signal.Key, lexer.GetKey(line, ref position)),
                //State.Outside => lexer.FindObjectStart(line, ref position),
                State.StringValueStart => (Signal.StringValue, lexer.GetSimpleObject(line, ref position, isString: true)),
                State.ValueStart => (Signal.StringValue, lexer.GetSimpleObject(line, ref position, isString: false)),
                _ => (Signal.LineEnd, null),
            };
        } //Readlexeme

        void ProduceOutput(Output output, string data) {
            if (output == Output.OpenTag) //SA???
                Console.Write(data);
        } //ProduceOutput

        void ProcessLine(string line) {
            int position = 0;
            while (true) {
                (Signal signal, string text) = Readlexeme(line, ref position);
                var signalOutput = transducer.Signal(signal);
                ProduceOutput(signalOutput.Output, text);
                if (signal == Signal.LineEnd) break;
            } //loop
        } //ProcessLine
        
        internal void Run() {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            using StreamReader reader =
                new(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    DefinitionSet.inputSample));
            string line;
            while ((line = reader.ReadLine()) != null)
                ProcessLine(line);
        } //Run

        internal readonly Lexer lexer = new();

    } //class Transcoder

}