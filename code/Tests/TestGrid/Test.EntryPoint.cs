/*
    Generic Transition System, test

    Test NP-hard problems on interconnected rectangular grid

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;


    class Test {

        enum Node {
            n00, n01, n02, n03, n04, n05,
            n10, n11, n12, n13, n14, n15,
            n20, n21, n22, n23, n24, n25,
            n30, n31, n32, n33, n34, n35,
        };
        static TransitionSystem<Node> PopulateGrid() {
            TransitionSystem<Node> transitionSystem = new();
            static void handler(Node start, Node finish) {
                Console.WriteLine($"Moving from {start} to {finish}");
            };
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n00, Node.n01, Node.n02, Node.n03, Node.n04, Node.n05);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n10, Node.n11, Node.n12, Node.n13, Node.n14, Node.n15);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n20, Node.n21, Node.n22, Node.n23, Node.n24, Node.n25);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n30, Node.n31, Node.n32, Node.n33, Node.n34, Node.n35);
            //
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n00, Node.n10, Node.n20, Node.n30);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n01, Node.n11, Node.n21, Node.n31);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n02, Node.n12, Node.n22, Node.n32);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n03, Node.n13, Node.n23, Node.n33);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n04, Node.n14, Node.n24, Node.n34);
            transitionSystem.AddValidStateTransitionChain(handler, undirected: true, Node.n05, Node.n15, Node.n25, Node.n35);
            return transitionSystem;
        } //PopulateBox

        static void Present(TransitionSystem<Node> transitionSystem) {
            var (maximumNumberOfPaths, pairsAtMax) = transitionSystem.MaximumPaths;
            Console.WriteLine($"Maximum number of paths: {maximumNumberOfPaths}, those paths are between states:");
            foreach (var (start, finish) in pairsAtMax)
                Console.WriteLine($"      {start} to {finish}");
            var (numberOfPaths, longestPathLength, longestPaths) = transitionSystem.LongestPaths;
            Console.Write($"Total number of paths: {numberOfPaths}, longest path length: {longestPathLength}");
            Console.WriteLine(", for example:");
            if (longestPaths.Length < 1) return;
            if (longestPaths[0].Length < 1) return;
            Console.Write($"[");
            foreach (var state in longestPaths[0])
                Console.Write($" {state}");
            Console.WriteLine(" ]");
        } //Present

        static void Main() {
            var transitionSystem = PopulateGrid();
            Present(transitionSystem);
        } //Main

    } //class Test

}
