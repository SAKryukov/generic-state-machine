/*
    Generic State Machine, test

    Transition graph: Locked <-> Closed <-> Opened <-> OpenedInside <-> ClosedInside <-> LockedInside

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Console = System.Console;


    class Test {

        enum RoomDoorState { Locked, Closed, Opened, OpenedInside, ClosedInside, LockedInside };
        static StateMachine<RoomDoorState> PopulateRoom() {
            StateMachine<RoomDoorState> stateMachine = new();
            stateMachine.AddValidStateTransition(RoomDoorState.Locked, RoomDoorState.Closed, (start, finish) => {
                Console.WriteLine("Unlocking the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Opened, (start, finish) => {
                Console.WriteLine("Opening the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.OpenedInside, (start, finish) => {
                Console.WriteLine("Getting inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.ClosedInside, (start, finish) => {
                Console.WriteLine("Closing the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside, (start, finish) => {
                Console.WriteLine("Locking the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.LockedInside, RoomDoorState.ClosedInside, (start, finish) => {
                Console.WriteLine("Unlocking the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.OpenedInside, (start, finish) => {
                Console.WriteLine("Opening the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.Opened, (start, finish) => {
                Console.WriteLine("Moving out");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.Closed, (start, finish) => {
                Console.WriteLine("Closing the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Locked, (start, finish) => {
                Console.WriteLine("Locking the door outside");
            });
            static string YouCannot(string verb, string adjective) => $"You cannot {verb} through the {adjective} door";
            const string getIn = "get in"; const string goOut = "go out"; const string locked = nameof(locked); const string closed = nameof(closed);
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.OpenedInside, (start, finish) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.ClosedInside, (start, finish) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside, (start, finish) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.OpenedInside, (start, finish) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.ClosedInside, (start, finish) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.LockedInside, (start, finish) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Locked, (start, finish) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Closed, (start, finish) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Opened, (start, finish) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Locked, (start, finish) => YouCannot(goOut, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Closed, (start, finish) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Opened, (start, finish) => YouCannot(goOut, locked));
            return stateMachine;
        } //PopulateBox

        static void Main() {
            var boxDoorStateMachine = PopulateRoom();
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.LockedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.Closed));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.Opened));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.Opened));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.OpenedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.ClosedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.LockedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(RoomDoorState.Opened));
        } //Main

    } //class Test

}
