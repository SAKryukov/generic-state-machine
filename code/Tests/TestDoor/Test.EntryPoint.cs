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

        enum RoomDoorState { Locked, Closed, Opened, OpenedInside, ClosedInside, LockedInside };
        static StateMachine<RoomDoorState> PopulateRoom() {
            StateMachine<RoomDoorState> stateMachine = new();
            stateMachine.AddValidStateTransition(RoomDoorState.Locked, RoomDoorState.Closed, (starting, ending) => {
                Console.WriteLine("Unlocking the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Opened, (starting, ending) => {
                Console.WriteLine("Opening the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.OpenedInside, (starting, ending) => {
                Console.WriteLine("Getting inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.ClosedInside, (starting, ending) => {
                Console.WriteLine("Closing the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside, (starting, ending) => {
                Console.WriteLine("Locking the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.LockedInside, RoomDoorState.ClosedInside, (starting, ending) => {
                Console.WriteLine("Unlocking the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.OpenedInside, (starting, ending) => {
                Console.WriteLine("Opening the door inside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.OpenedInside, RoomDoorState.Opened, (starting, ending) => {
                Console.WriteLine("Moving out");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Opened, RoomDoorState.Closed, (starting, ending) => {
                Console.WriteLine("Closing the door outside");
            });
            stateMachine.AddValidStateTransition(RoomDoorState.Closed, RoomDoorState.Locked, (starting, ending) => {
                Console.WriteLine("Locking the door outside");
            });
            static string YouCannot(string verb, string adjective) => $"You cannot {verb} through the {adjective} door";
            const string getIn = "get in"; const string goOut = "go out"; const string locked = nameof(locked); const string closed = nameof(closed);
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.OpenedInside, (starting, ending) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.ClosedInside, (starting, ending) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside, (starting, ending) => YouCannot(getIn, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.OpenedInside, (starting, ending) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.ClosedInside, (starting, ending) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.Closed, RoomDoorState.LockedInside, (starting, ending) => YouCannot(getIn, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Locked, (starting, ending) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Closed, (starting, ending) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.LockedInside, RoomDoorState.Opened, (starting, ending) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Locked, (starting, ending) => YouCannot(goOut, closed));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Closed, (starting, ending) => YouCannot(goOut, locked));
            stateMachine.AddInvalidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.Opened, (starting, ending) => YouCannot(goOut, locked));
            return stateMachine;
        } //PopulateBox

        enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }
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
