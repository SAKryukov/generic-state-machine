/*
    Generic State Machine, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
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

        static StateMachine<VisitorState> PopulateTrails() {
            StateMachine<VisitorState> stateMachine = new();
            static void Move(VisitorState start, VisitorState finish) =>
                Console.WriteLine($"Moving from {start} to {finish}...");
            // horizontal trails:
            stateMachine.AddValidStateTransitionChain(Move, false, VisitorState.AfricanPorcupine, VisitorState.SnowLeopard,
                VisitorState.AfricanLion, VisitorState.BengalTiger, VisitorState.DromedaryCamel, VisitorState.WhiteRhino);
            stateMachine.AddValidStateTransitionChain(Move, false, VisitorState.NorthAmericanPorcupine, VisitorState.Mandrill,
                VisitorState.Sloth, VisitorState.Coati, VisitorState.Emu);
            stateMachine.AddValidStateTransitionChain(Move, false, VisitorState.Serval, VisitorState.Yak, VisitorState.BactrianCamel,
                VisitorState.CapuchinMonkey, VisitorState.Kookaburra, VisitorState.Tortoise);
            stateMachine.AddValidStateTransitionChain(Move, false, VisitorState.Cheetah, VisitorState.Watusu, VisitorState.Alligator,
                VisitorState.Flamingo, VisitorState.Llama, VisitorState.Wallaby);
            // vertical passages:
            stateMachine.AddValidStateTransitionChain(Move, false,
                VisitorState.AfricanPorcupine, VisitorState.NorthAmericanPorcupine, VisitorState.Serval, VisitorState.Cheetah);
            stateMachine.AddValidStateTransitionChain(Move, false,
                VisitorState.AfricanLion, VisitorState.BactrianCamel, VisitorState.Watusu);
            stateMachine.AddValidStateTransition(VisitorState.WhiteRhino, VisitorState.Tortoise, Move, directed: false);
            // vertical chains from entry:
            stateMachine.AddValidStateTransitionChain(Move, false,
                VisitorState.Entry, VisitorState.Llama, VisitorState.CapuchinMonkey);
            stateMachine.AddValidStateTransitionChain(Move, false,
                VisitorState.Entry, VisitorState.Wallaby, VisitorState.Kookaburra);
            // exit
            stateMachine.AddValidStateTransition(VisitorState.Flamingo, VisitorState.Exit, Move, directed: true);
            stateMachine.AddInvalidStateTransition(VisitorState.Entry, VisitorState.Exit, (start, finish) =>
                $"You cannot go directly from {start} to {finish}, you have to pass through the zoo");
            VisitorState[] openSites = new VisitorState[] { VisitorState.AfricanPorcupine, VisitorState.SnowLeopard,
                VisitorState.AfricanLion, VisitorState.BengalTiger, VisitorState.DromedaryCamel, VisitorState.WhiteRhino,
                VisitorState.NorthAmericanPorcupine, VisitorState.Mandrill, VisitorState.Sloth, VisitorState.Coati,
                VisitorState.Emu, VisitorState.Serval, VisitorState.Yak, VisitorState.BactrianCamel, VisitorState.CapuchinMonkey,
                VisitorState.Kookaburra, VisitorState.Tortoise, VisitorState.Cheetah, VisitorState.Watusu,
                VisitorState.Alligator, VisitorState.Flamingo, VisitorState.Llama, VisitorState.Wallaby,
                VisitorState.Entry, VisitorState.Exit };
            foreach (VisitorState state in openSites)
                stateMachine.AddInvalidStateTransition(state, VisitorState.Chimpanzee, (start, _) =>
                    $"{VisitorState.Chimpanzee} area is temporarily closed");
            return stateMachine;
        } //PopulateTrails

        static void PresentRoutesOfRoutes(VisitorState[][] routes) {
            int index = 1;
            foreach (var route in routes) {
                Console.Write($"Route #{index++:D4}: [");
                foreach (var state in route)
                    Console.Write($" {state}");
                Console.WriteLine(" ]");
            } //loop
        } //PresentRoutesOfRoutes

        enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }
        static void Main() {
            var stateMachine = PopulateTrails();
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Exit)); // cannot go directly to exit
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.CapuchinMonkey));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Entry));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Wallaby));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Chimpanzee)); // area closed
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Kookaburra));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Tortoise));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.WhiteRhino));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.BactrianCamel)); // no way
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.DromedaryCamel));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.BengalTiger));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.AfricanLion));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Watusu));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.BactrianCamel));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Watusu));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Alligator));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Exit));
            Console.WriteLine($"Resetting state: {stateMachine.ResetState()}. Let's make a shorter loop:");
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Wallaby));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Exit));
            Console.WriteLine($"Resetting state: {stateMachine.ResetState()}, Now a shortest loop:");
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Llama));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Flamingo));
            Console.WriteLine(stateMachine.TryTransitionTo(VisitorState.Exit));
            var (numberOfPaths, longestPathLength, longestPaths) = stateMachine.LongestPaths;
            Console.WriteLine();
            Console.WriteLine($"Total number of routes: {numberOfPaths}, longest route: {longestPathLength}, longest routes:");
            if (longestPaths.Length > 0)
                PresentRoutesOfRoutes(longestPaths);
            Console.WriteLine();
            var (longestNumberOfPaths, pairsAtMax) = stateMachine.LongestNumberOfPaths;
            Console.WriteLine($"Maximum number of routes: {longestNumberOfPaths}");
            foreach (var (start, finish) in pairsAtMax)
                Console.WriteLine($"        {longestNumberOfPaths} routes from {start} to {finish}");
            Console.WriteLine();
            Console.WriteLine("Labyrinth solution demo:");
            var fullLabyrinthSolution = stateMachine.Labyrinth(VisitorState.Entry, VisitorState.Exit);
            Console.WriteLine($"{fullLabyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Exit} found:");
            PresentRoutesOfRoutes(fullLabyrinthSolution);
            var labyrinthSolution = stateMachine.Labyrinth(VisitorState.Entry, VisitorState.Emu);
            Console.WriteLine($"      Even more, {labyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Emu} found");
            labyrinthSolution = stateMachine.Labyrinth(VisitorState.Entry, VisitorState.Yak);
            Console.WriteLine($"      Even more, {labyrinthSolution.Length} possible routes from {VisitorState.Entry} to {VisitorState.Yak} found");
            Console.WriteLine();
            labyrinthSolution = stateMachine.Labyrinth(VisitorState.Entry, VisitorState.Exit, shortest: true);
            string plural = labyrinthSolution.Length == 1 ? "" : "s";
            Console.WriteLine($"{labyrinthSolution.Length} shortest route{plural} from {VisitorState.Entry} to {VisitorState.Exit} found:");
            PresentRoutesOfRoutes(labyrinthSolution);
#if TryAllRoutes
            Console.WriteLine();
            Console.WriteLine("Trying all routes from {VisitorState.Entry} to {VisitorState.Exit}:");
            int index = 1;
            foreach (var route in fullLabyrinthSolution) {
                stateMachine.ResetState();
                Console.WriteLine($"================================= Route #{index++:D4}:");
                foreach (var state in route)
                    Console.WriteLine(stateMachine.TryTransitionTo(state));
            } //loop
#endif //TryAllRoutes
        } //Main

    } //class Test

}
