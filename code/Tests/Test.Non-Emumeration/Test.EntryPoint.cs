/*
    Generic Transition System, test

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

        static TransitionSystem<double> Populate() {
            TransitionSystem<double> transitionSystem = new(double.NaN);
            transitionSystem.AddValidStateTransitionChain(
                (start, finish) => { Console.Write($"Moving from {start} to {finish}: " ); },
                undirected: true,
                double.NegativeInfinity, double.MinValue, double.Epsilon, double.MaxValue, double.PositiveInfinity);
            transitionSystem.AddValidStateTransition(double.MinValue, double.MaxValue, (start, finish) => {
                Console.Write($"Maximum ({start}) is one step away from minumum ({finish}) :-): ");
            }, undirected: true);
            foreach (double value in nonNaN)
                transitionSystem.AddValidStateTransition(double.NaN, value, (start, finish) => 
                    Console.Write("Who needs NaN, anyway? :-) "));
            foreach (double value in nonNaN)
                transitionSystem.AddInvalidStateTransition(value, double.NaN, (start, finish) =>
                    "Who needs NaN? :-) ");
            return transitionSystem;
        } //Populate

        static void Main() {
            var transitionSystem = Populate();
            Console.WriteLine(transitionSystem.TryTransitionTo(double.NegativeInfinity));
            Console.WriteLine(transitionSystem.TryTransitionTo(double.MinValue));
            Console.WriteLine(transitionSystem.TryTransitionTo(double.MaxValue));
            Console.WriteLine(transitionSystem.TryTransitionTo(double.PositiveInfinity));
            Console.WriteLine(transitionSystem.TryTransitionTo(double.Epsilon));
            try {
                Console.WriteLine();
                double target = 42.42;
                Console.WriteLine($"Trying to jump to {target}:");
                transitionSystem.TryTransitionTo(target);
            } catch (System.Exception e) {
                Console.WriteLine($"      {e.GetType().Name}: {e.Message}");
            } //exception
        } //Main

    } //class Test

}
