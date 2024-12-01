/*
    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    https://www.codeproject.com/Members/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
*/

namespace StateMachines {
    using Console = System.Console;
    using Type = System.Type;
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;
    using StateSet = System.Collections.Generic.HashSet<State>;
    using StateGraph = System.Collections.Generic.Dictionary<StateGraphKey, StateGraphValue>;

    sealed class State {
        internal State(string name, object underlyingMember) {
            Name = name;
            UnderlyingMember = underlyingMember;
        } //State
        internal string Name { get; init; }
        internal object UnderlyingMember { get; init; }
    } //class State

    delegate void StateTransitionAction(State startingState, State endingState);
    delegate string InvalidStateTransitionAction(State startingState, State endingState);

    class StateGraphKey {
        internal StateGraphKey(State starting, State ending) {
            StartingState = starting; EndingState = ending;
        }
        public override int GetHashCode() { // important!
            string representation = StartingState.Name + EndingState.Name;
            return representation.GetHashCode();
        }
        public override bool Equals(object @object) { // important!
            if (@object == null) return false;
            return GetHashCode() == @object.GetHashCode(); //sic!
        }
        internal State StartingState { get; init; }
        internal State EndingState { get; init; }
    } //class StateGraphKey

    class StateGraphValue {
        internal StateGraphValue(StateTransitionAction valid, InvalidStateTransitionAction invalid) {
            ValidAction = valid; InvalidAction = invalid;
        }
        internal StateTransitionAction ValidAction { get; init; }
        internal InvalidStateTransitionAction InvalidAction { get; init; }
        } //class StateGraphValue

    abstract class StateTransition : StateGraphKey {
        internal StateTransition(State starting, State ending) :
            base(starting, ending) { }
        internal abstract bool IsValid { get; }
    } //StateTransition

    class ValidStateTransition : StateTransition {
        internal ValidStateTransition(State starting, State ending) :
            base(starting, ending) { }
        internal override bool IsValid { get { return true; } }
    } //class ValidStateTransition

    class InvalidStateTransition : StateTransition {
        internal InvalidStateTransition(State starting, State ending) :
            base(starting, ending) { }
        internal override bool IsValid { get { return false; } }
    } //class ValidStateTransition

class StateMachine<STATE> {

        internal StateMachine(STATE initialState = default(STATE)) {
            Type type = typeof(STATE);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                State state = new State(field.Name, field.GetValue(null));
                stateSet.Add(state);
                if (initialState.ToString() == field.Name)
                    CurrentState = state;
            } //loop
        } //StateMachine
        internal State CurrentState { get; init; }
        static State CreateState(STATE value) => new State(value.ToString(), value);
        static bool IsValid(StateGraphValue value) => value.ValidAction != null;
        internal void AddValidStateTransition(STATE startingState, STATE endingState, StateTransitionAction action) {
            StateGraphKey key = new(CreateState(startingState), CreateState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                return; //SA???
            stateGraph.Add(key, new StateGraphValue(action, null)); 
        } //AddValidStateTransition
        internal void AddInvalidStateTransition(STATE startingState, STATE endingState, InvalidStateTransitionAction action) {
            StateGraphKey key = new(CreateState(startingState), CreateState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                return; //SA???
            stateGraph.Add(key, new StateGraphValue(null, action));
        } //AddInvalidStateTransition
        internal string IsTransitionValid(STATE startingState, STATE endingState) {
            State starting = CreateState(startingState);
            State ending = CreateState(endingState);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            if (found && !IsValid(value) && value.InvalidAction != null) {
                return value.InvalidAction(starting, ending);
            }
            return null;
        } //IsTransitionValid
        internal bool PerformTransition(STATE startingState, STATE endingState) {
            State starting = CreateState(startingState);
            State ending = CreateState(endingState);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            if (IsValid(value))
                value.ValidAction(starting, ending);
            return found;
        } //PerformTransition
        internal void PerformTransitionIndirect(STATE startingState, STATE endingState) {
            //SA??? complicated algorithm of graph search
        } //PerformTransition
        StateSet stateSet = new();
        StateGraph stateGraph = new();
    } //class StateMachine

    enum TestState { Draft, Denied, Approved, WaitForApprovalManager, WaitForApprovalTechnical, WaitForApprovalFinance, }
    class Test {
        static void Main() {
            var stateMachine = new StateMachine<TestState>();
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalManager, (starting, ending) => { });
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalTechnical, (starting, ending) => { });
            stateMachine.AddValidStateTransition(TestState.Draft, TestState.WaitForApprovalFinance, (starting, ending) => { });
            stateMachine.AddInvalidStateTransition(TestState.Denied, TestState.WaitForApprovalManager, (starting, ending) =>
                $"{TestState.Denied} to {TestState.WaitForApprovalManager}? Come on! It is already denied, don't wait!");
            Console.WriteLine(stateMachine.IsTransitionValid(TestState.Draft, TestState.WaitForApprovalManager));
            Console.WriteLine(stateMachine.IsTransitionValid(TestState.Denied, TestState.WaitForApprovalManager));
        } //Main
    } //class Test

}
