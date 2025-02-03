/*
    Generic Transition System, test

    Zoo transition system
    See chart.StateMachineGraph.txt for the transition graph
    See also:
    https://SAKryukov.GitHub.io/generic-state-machine/zoo.svg
    https://GitHub.com/SAKryukov/generic-state-machine/blob/main/docs/zoo.svg
    https://raw.GitHubUserContent.com/SAKryukov/generic-state-machine/refs/heads/main/docs/zoo.svg

    Copyright (C) 2024-2025 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

//#define TryAllRoutes // uncomment to try
namespace StateMachines {
    using Console = System.Console;

    class Test {

        enum VisitorState {
            Entry,
            AfricanPorcupine, SnowLeopard, AfricanLion, BengalTiger, DromedaryCamel, WhiteRhino,
            NorthAmericanPorcupine, Chimpanzee, Mandrill, Sloth, Coati, Emu,
            Serval, Yak, BactrianCamel, CapuchinMonkey, Kookaburra, Tortoise,
            Cheetah, Watusu, Alligator, Flamingo, Llama, Wallaby,
            Exit
        };

        static TransitionSystem<VisitorState> PopulateTrails() {
            TransitionSystem<VisitorState> transitionSystem = new();
            static void Move(VisitorState start, VisitorState finish) =>
                Console.WriteLine($"Moving from {start} to {finish}...");
            // horizontal trails:
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true, VisitorState.AfricanPorcupine, VisitorState.SnowLeopard,
                VisitorState.AfricanLion, VisitorState.BengalTiger, VisitorState.DromedaryCamel, VisitorState.WhiteRhino);
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true, VisitorState.NorthAmericanPorcupine, VisitorState.Mandrill,
                VisitorState.Sloth, VisitorState.Coati, VisitorState.Emu);
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true, VisitorState.Serval, VisitorState.Yak, VisitorState.BactrianCamel,
                VisitorState.CapuchinMonkey, VisitorState.Kookaburra, VisitorState.Tortoise);
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true, VisitorState.Cheetah, VisitorState.Watusu, VisitorState.Alligator,
                VisitorState.Flamingo, VisitorState.Llama, VisitorState.Wallaby);
            // vertical passages:
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true,
                VisitorState.AfricanPorcupine, VisitorState.NorthAmericanPorcupine, VisitorState.Serval, VisitorState.Cheetah);
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true,
                VisitorState.AfricanLion, VisitorState.BactrianCamel, VisitorState.Watusu);
            transitionSystem.AddValidStateTransition(VisitorState.WhiteRhino, VisitorState.Tortoise, Move, undirected: true);
            // vertical chains from entry:
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true,
                VisitorState.Entry, VisitorState.Llama, VisitorState.CapuchinMonkey);
            transitionSystem.AddValidStateTransitionChain(Move, undirected: true,
                VisitorState.Entry, VisitorState.Wallaby, VisitorState.Kookaburra);
            // exit
            transitionSystem.AddValidStateTransition(VisitorState.Flamingo, VisitorState.Exit, Move, undirected: false);
            transitionSystem.AddInvalidStateTransition(VisitorState.Entry, VisitorState.Exit, (start, finish) =>
                $"You cannot go directly from {start} to {finish}, you have to pass through the zoo");
            VisitorState[] openSites = new VisitorState[] { VisitorState.AfricanPorcupine, VisitorState.SnowLeopard,
                VisitorState.AfricanLion, VisitorState.BengalTiger, VisitorState.DromedaryCamel, VisitorState.WhiteRhino,
                VisitorState.NorthAmericanPorcupine, VisitorState.Mandrill, VisitorState.Sloth, VisitorState.Coati,
                VisitorState.Emu, VisitorState.Serval, VisitorState.Yak, VisitorState.BactrianCamel, VisitorState.CapuchinMonkey,
                VisitorState.Kookaburra, VisitorState.Tortoise, VisitorState.Cheetah, VisitorState.Watusu,
                VisitorState.Alligator, VisitorState.Flamingo, VisitorState.Llama, VisitorState.Wallaby,
                VisitorState.Entry, VisitorState.Exit };
            foreach (VisitorState state in openSites)
                transitionSystem.AddInvalidStateTransition(state, VisitorState.Chimpanzee, (start, finish) =>
                    $"Moving from {start}: {finish} area is temporarily closed");
            return transitionSystem;
        } //PopulateTrails

        static void PresentRoutesOfRoutes(VisitorState[][] routes) {
            int index = 1;
            foreach (var route in routes) {
                string routePresentation = string.Join(" ", route);
                Console.WriteLine($"Route #{index++:D4}: [ {routePresentation} ]");
            } //loop
        } //PresentRoutesOfRoutes

        enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }
        static void Main() {
            var transitionSystem = PopulateTrails();
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Exit)); // cannot go directly to exit
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.CapuchinMonkey));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Entry));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Wallaby));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Chimpanzee)); // area closed
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Kookaburra));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Tortoise));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.WhiteRhino));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.BactrianCamel)); // no way
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.DromedaryCamel));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.BengalTiger));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.AfricanLion));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Watusu));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.BactrianCamel));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Watusu));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Alligator));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Exit));
            Console.WriteLine($"Resetting state: {transitionSystem.ResetState()}. Let's make a shorter loop:");
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Wallaby));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Exit));
            Console.WriteLine($"Resetting state: {transitionSystem.ResetState()}, Now a shortest loop:");
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(transitionSystem.TryTransitionTo(VisitorState.Exit));
            var (numberOfPaths, longestPathLength, longestPaths) = transitionSystem.LongestPaths;
            Console.WriteLine();
            Console.WriteLine($"Total number of routes: {numberOfPaths}, longest route: {longestPathLength}, longest routes:");
            if (longestPaths.Length > 0)
                PresentRoutesOfRoutes(longestPaths);
            Console.WriteLine();
            var (maximumNumberOfPaths, pairsAtMax) = transitionSystem.MaximumPaths;
            Console.WriteLine($"Maximum number of routes: {maximumNumberOfPaths}");
            foreach (var (start, finish) in pairsAtMax)
                Console.WriteLine($"        {maximumNumberOfPaths} routes from {start} to {finish}");
            Console.WriteLine();
            Console.WriteLine("Labyrinth solution demo:");
            var fullLabyrinthSolution = transitionSystem.Labyrinth(VisitorState.Entry, VisitorState.Exit);
            Console.WriteLine($"{fullLabyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Exit} found:");
            PresentRoutesOfRoutes(fullLabyrinthSolution);
            var labyrinthSolution = transitionSystem.Labyrinth(VisitorState.Entry, VisitorState.Emu);
            Console.WriteLine($"      Even more, {labyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Emu} found");
            labyrinthSolution = transitionSystem.Labyrinth(VisitorState.Entry, VisitorState.Yak);
            Console.WriteLine($"      Even more, {labyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Yak} found");
            Console.WriteLine();
            labyrinthSolution = transitionSystem.Labyrinth(VisitorState.Entry, VisitorState.Exit, shortest: true);
            string plural = labyrinthSolution.Length == 1 ? "" : "s";
            Console.WriteLine($"{labyrinthSolution.Length} shortest route{plural} from {VisitorState.Entry} to {VisitorState.Exit} found:");
            PresentRoutesOfRoutes(labyrinthSolution);
            Console.WriteLine();
            var deadEnds = transitionSystem.FindDeadEnds(VisitorState.Entry, VisitorState.Exit).deadEnds;
            string deadEndsPresentation = string.Join(", ", deadEnds);
            Console.WriteLine($"Dead ends found on the routes from {VisitorState.Entry} to {VisitorState.Exit}: {deadEndsPresentation}");
#if TryAllRoutes
            Console.WriteLine();
            Console.WriteLine("Trying all routes from {VisitorState.Entry} to {VisitorState.Exit}:");
            int index = 1;
            foreach (var route in fullLabyrinthSolution) {
                transitionSystem.ResetState();
                Console.WriteLine($"================================= Route #{index++:D4}:");
                foreach (var state in route)
                    Console.WriteLine(transitionSystem.TryTransitionTo(state));
            } //loop
#endif //TryAllRoutes
        } //Main

    } //class Test

}
