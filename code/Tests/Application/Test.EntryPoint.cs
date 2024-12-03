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

        enum BoxDoorState { Locked, Closed, Opened, OpenedInside, ClosedInside, LockedInside };
        static StateMachine<BoxDoorState> PopulateBox() {
            StateMachine<BoxDoorState> stateMachine = new();
            stateMachine.AddValidStateTransition(BoxDoorState.Locked, BoxDoorState.Closed, (starting, ending) => {
                Console.WriteLine("Unlocking the door outside, clang-clang");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.Closed, BoxDoorState.Opened, (starting, ending) => {
                Console.WriteLine("Opening the door outside, squick-squick");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.Opened, BoxDoorState.OpenedInside, (starting, ending) => {
                Console.WriteLine("Getting inside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.OpenedInside, BoxDoorState.ClosedInside, (starting, ending) => {
                Console.WriteLine("Closing the door inside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.ClosedInside, BoxDoorState.LockedInside, (starting, ending) => {
                Console.WriteLine("Locking the door inside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.LockedInside, BoxDoorState.ClosedInside, (starting, ending) => {
                Console.WriteLine("Unlocking the door inside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.ClosedInside, BoxDoorState.OpenedInside, (starting, ending) => {
                Console.WriteLine("Opening the door inside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.OpenedInside, BoxDoorState.Opened, (starting, ending) => {
                Console.WriteLine("Moving out");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.Opened, BoxDoorState.Closed, (starting, ending) => {
                Console.WriteLine("Closing the door outside");
            });
            stateMachine.AddValidStateTransition(BoxDoorState.Closed, BoxDoorState.Locked, (starting, ending) => {
                Console.WriteLine("Locking the door outside");
            });
            stateMachine.AddInvalidStateTransition(BoxDoorState.Locked, BoxDoorState.OpenedInside, (starting, ending) => "You cannot get in through the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.Locked, BoxDoorState.ClosedInside, (starting, ending) => "You cannot get in through the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.Locked, BoxDoorState.LockedInside, (starting, ending) => "You cannot get in through the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.Closed, BoxDoorState.OpenedInside, (starting, ending) => "You cannot get in through the closed door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.Closed, BoxDoorState.ClosedInside, (starting, ending) => "You cannot get in through the closed door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.Closed, BoxDoorState.LockedInside, (starting, ending) => "You cannot get in through the closed door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.LockedInside, BoxDoorState.Locked, (starting, ending) => "You cannot go out of the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.LockedInside, BoxDoorState.Closed, (starting, ending) => "You cannot go out of the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.LockedInside, BoxDoorState.Opened, (starting, ending) => "You cannot go out of the locked door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.ClosedInside, BoxDoorState.Locked, (starting, ending) => "You cannot go out of the closed door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.ClosedInside, BoxDoorState.Closed, (starting, ending) => "You cannot go out of the closed door");
            stateMachine.AddInvalidStateTransition(BoxDoorState.ClosedInside, BoxDoorState.Opened, (starting, ending) => "You cannot go out of the closed door");
            return stateMachine;
        } //PopulateBox

        enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }
        static void Main() {
            var boxDoorStateMachine = PopulateBox();
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.LockedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.Closed));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.Opened));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.OpenedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.ClosedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.LockedInside));
            Console.WriteLine(boxDoorStateMachine.TryTransitionTo(BoxDoorState.Opened));
        } //Main

    } //class Test

}
