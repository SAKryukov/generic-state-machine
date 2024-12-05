/*
    Generic State Machine, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
*/

namespace StateMachines {
    using Console = System.Console;


    class Test {

        static StateMachine<System.Double> PopulateNumericStateMachine() {
            StateMachine<System.Double> stateMachine = new(System.Double.NaN);
            stateMachine.AddValidStateTransition(System.Double.MaxValue, System.Double.MinValue, (start, finish) => {
                Console.WriteLine("Maximum is one step away from minumum");
            });
            return stateMachine;
        } //PopulateBox

        static void Main() {
            var stateMachine = PopulateNumericStateMachine();
        } //Main

    } //class Test

}
