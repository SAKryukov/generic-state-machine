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

        static StateMachine<double> PopulateNumericStateMachine() {
            StateMachine<double> stateMachine = new(double.NaN);
            stateMachine.AddValidStateTransition(double.MinValue, double.MaxValue, (start, finish) => {
                Console.WriteLine("Maximum is one step away from minumum");
            }, undirected: true);
            stateMachine.AddValidStateTransition(double.NaN, double.MinValue, (start, finish) => {
                Console.WriteLine("Who needs NaN?");
            });
            stateMachine.AddValidStateTransition(double.NaN, double.MaxValue, (start, finish) => {
                Console.WriteLine("Who needs NaN? :-)");
            });
            return stateMachine;
        } //PopulateBox

        static void Main() {
            var stateMachine = PopulateNumericStateMachine();
            stateMachine.TryTransitionTo(double.MaxValue);
            stateMachine.TryTransitionTo(double.MinValue);
        } //Main

    } //class Test

}
