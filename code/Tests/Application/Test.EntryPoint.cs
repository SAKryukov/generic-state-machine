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

    enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }

    class Test {
        static void Main() {
            var stateMachine = new StateMachine<TestState>();
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalManager, (starting, ending) => {
                Console.WriteLine("Trying to get approval from manager");
            });
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalTechnical, (starting, ending) => { });
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalFinance, (starting, ending) => { });
            stateMachine.AddInvalidStateTransition(TestState.Denied, TestState.WaitForApprovalManager, (starting, ending) =>
                $"{TestState.Denied} to {TestState.WaitForApprovalManager}? Come on! It is already denied, don't wait!");
            Console.WriteLine(stateMachine.IsTransitionValid(TestState.Draft, TestState.WaitForApprovalManager));
            Console.WriteLine(stateMachine.IsTransitionValid(TestState.Denied, TestState.WaitForApprovalManager));
            Console.WriteLine(stateMachine.CurrentState);
            stateMachine.TryTransitionTo(TestState.WaitForApprovalManager);
            Console.WriteLine(stateMachine.CurrentState);
        } //Main
    } //class Test

}
