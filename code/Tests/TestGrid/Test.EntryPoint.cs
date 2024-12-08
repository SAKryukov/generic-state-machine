/*
    Generic State Machine, test

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
        static StateMachine<Node> PopulateGrid() {
            StateMachine<Node> stateMachine = new();
            static void handler(Node start, Node finish) {
                Console.WriteLine($"Moving from {start} to {finish}");
            };
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n00, Node.n01, Node.n02, Node.n03, Node.n04, Node.n05);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n10, Node.n11, Node.n12, Node.n13, Node.n14, Node.n15);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n20, Node.n21, Node.n22, Node.n23, Node.n24, Node.n25);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n30, Node.n31, Node.n32, Node.n33, Node.n34, Node.n35);
            //
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n00, Node.n10, Node.n20, Node.n30);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n01, Node.n11, Node.n21, Node.n31);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n02, Node.n12, Node.n22, Node.n32);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n03, Node.n13, Node.n23, Node.n33);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n04, Node.n14, Node.n24, Node.n34);
            stateMachine.AddValidStateTransitionChain(handler, undirected: true, Node.n05, Node.n15, Node.n25, Node.n35);
            return stateMachine;
        } //PopulateBox

        static void Main() {
            var stateMachine = PopulateGrid();
            var (maximumNumberOfPaths, pairsAtMax) = stateMachine.MaximumPaths;
            Console.WriteLine($"Maximum number of paths: {maximumNumberOfPaths}");
            foreach (var pair in pairsAtMax)
                Console.WriteLine($"{pair.start} to {pair.finish}");
        } //Main

    } //class Test

}
