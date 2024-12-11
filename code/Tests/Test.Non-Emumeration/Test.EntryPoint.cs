/*
    Generic State Machine, test

    // Exotic case based in the type double, its public static fields
    // Transition graph:
    // NegativeInfinity <-> MinValue <-> Epsilon <-> MaxValue <-> PositiveInfinity
    // MinValue <-> MaxValue
    // NaN -> anything, anything -X-> NaN

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;

    class Test {

        static readonly double[] nonNaN = new double[] { double.NegativeInfinity, double.MinValue, double.Epsilon, double.MaxValue, double.PositiveInfinity, };

        static StateMachine<double> PopulateNumericStateMachine() {
            StateMachine<double> stateMachine = new(double.NaN);
            stateMachine.AddValidStateTransitionChain(
                (start, finish) => { Console.Write($"Moving from {start} to {finish}: " ); },
                undirected: true,
                double.NegativeInfinity, double.MinValue, double.Epsilon, double.MaxValue, double.PositiveInfinity);
            stateMachine.AddValidStateTransition(double.MinValue, double.MaxValue, (start, finish) => {
                Console.Write($"Maximum ({start}) is one step away from minumum ({finish}) :-): ");
            }, undirected: true);
            foreach (double value in nonNaN)
                stateMachine.AddValidStateTransition(double.NaN, value, (start, finish) => 
                    Console.Write("Who needs NaN, anyway? :-) "));
            foreach (double value in nonNaN)
                stateMachine.AddInvalidStateTransition(value, double.NaN, (start, finish) =>
                    "Who needs NaN? :-) ");
            return stateMachine;
        } //PopulateBox

        static void Main() {
            var stateMachine = PopulateNumericStateMachine();
            Console.WriteLine(stateMachine.TryTransitionTo(double.NegativeInfinity));
            Console.WriteLine(stateMachine.TryTransitionTo(double.MinValue));
            Console.WriteLine(stateMachine.TryTransitionTo(double.MaxValue));
            Console.WriteLine(stateMachine.TryTransitionTo(double.PositiveInfinity));
            Console.WriteLine(stateMachine.TryTransitionTo(double.Epsilon));
            try {
                Console.WriteLine();
                double target = 42.42;
                Console.WriteLine($"Trying to jump to {target}:");
                stateMachine.TryTransitionTo(target);
            } catch (System.Exception e) {
                Console.WriteLine($"      {e.GetType().Name}: {e.Message}");
            } //exception
        } //Main

    } //class Test

}
