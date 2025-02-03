/*
    Generic Transition System, test

    Transition graph: Locked <-> Closed <-> Opened <-> OpenedInside <-> ClosedInside <-> LockedInside

    Copyright (C) 2024-2025 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;


    class Test {

        enum RoomDoorState { Locked, Closed, Opened, OpenedInside, ClosedInside, LockedInside };
        static TransitionSystem<RoomDoorState> PopulateRoom() {
            TransitionSystem<RoomDoorState> transitionSystem = new();
            transitionSystem.AddValidStateTransition(RoomDoorState.Locked, RoomDoorState.Closed, (start, finish) => {
                Console.WriteLine("Unlocking the door outside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Opened, (start, finish) => {
                Console.WriteLine("Opening the door outside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.OpenedInside, (start, finish) => {
                Console.WriteLine("Getting inside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.ClosedInside, (start, finish) => {
                Console.WriteLine("Closing the door inside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside, (start, finish) => {
                Console.WriteLine("Locking the door inside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.LockedInside, RoomDoorState.ClosedInside, (start, finish) => {
                Console.WriteLine("Unlocking the door inside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.OpenedInside, (start, finish) => {
                Console.WriteLine("Opening the door inside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.Opened, (start, finish) => {
                Console.WriteLine("Moving out");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.Closed, (start, finish) => {
                Console.WriteLine("Closing the door outside");
            });
            transitionSystem.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Locked, (start, finish) => {
                Console.WriteLine("Locking the door outside");
            });
            static string YouCannot(string verb, string adjective) => $"You cannot {verb} through the {adjective} door";
            const string getIn = "get in"; const string goOut = "go out"; const string locked = nameof(locked); const string closed = nameof(closed);
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.OpenedInside, (start, finish) => YouCannot(getIn, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.ClosedInside, (start, finish) => YouCannot(getIn, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside, (start, finish) => YouCannot(getIn, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.OpenedInside, (start, finish) => YouCannot(getIn, closed));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.ClosedInside, (start, finish) => YouCannot(getIn, closed));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.LockedInside, (start, finish) => YouCannot(getIn, closed));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Locked, (start, finish) => YouCannot(goOut, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Closed, (start, finish) => YouCannot(goOut, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Opened, (start, finish) => YouCannot(goOut, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Locked, (start, finish) => YouCannot(goOut, closed));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Closed, (start, finish) => YouCannot(goOut, locked));
            transitionSystem.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Opened, (start, finish) => YouCannot(goOut, locked));
            return transitionSystem;
        } //PopulateBox

        static void Main() {
            var transitionSystem = PopulateRoom();
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.LockedInside));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.Closed));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.Opened));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.Opened));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.OpenedInside));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.ClosedInside));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.LockedInside));
            Console.WriteLine(transitionSystem.TryTransitionTo(RoomDoorState.Opened));
        } //Main

    } //class Test

}
