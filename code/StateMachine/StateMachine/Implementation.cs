/*
    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    https://www.codeproject.com/Members/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
*/

namespace StateMachines {
    using Type = System.Type;
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;
    using StateSet = System.Collections.Generic.HashSet<State>;

    sealed class State {
        internal State(string name, object underlyingMember) {
            Name = name;
            UnderlyingMember = underlyingMember;
        } //State
        internal string Name { get; init; }
        internal object UnderlyingMember { get; init; }
    } //class State

    public delegate void StateTransitionAction<STATE>(STATE startingState, STATE endingState);
    public delegate string InvalidStateTransitionAction<STATE>(STATE startingState, STATE endingState);

    class StateGraphKey {
        internal StateGraphKey(State starting, State ending) {
            StartingState = starting; EndingState = ending;
        }
        public override int GetHashCode() { // important!
            return StartingState.Name.GetHashCode()
                ^ EndingState.Name.GetHashCode();
        }
        public override bool Equals(object @object) { // important!
            if (@object == null) return false;
            if (@object is not StateGraphKey objectStateGraphKey) return false;
            return objectStateGraphKey.StartingState.Name == StartingState.Name
                && objectStateGraphKey.EndingState.Name == EndingState.Name;
        }
        internal State StartingState { get; init; }
        internal State EndingState { get; init; }
    } //class StateGraphKey

    class StateGraphValue<STATE> {
        internal StateGraphValue(StateTransitionAction<STATE> valid, InvalidStateTransitionAction<STATE> invalid) {
            ValidAction = valid; InvalidAction = invalid;
        }
        internal StateTransitionAction<STATE> ValidAction { get; init; }
        internal InvalidStateTransitionAction<STATE> InvalidAction { get; init; }
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

    public class StateMachine<STATE> {

        public StateMachine(STATE initialState = default(STATE)) {
            Type type = typeof(STATE);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                State state = new State(field.Name, field.GetValue(null));
                stateSet.Add(state);
                if (initialState.ToString() == field.Name)
                    CurrentState = (STATE)state.UnderlyingMember;
            } //loop
        } //StateMachine
        public STATE CurrentState { get; private set; }
        static State CreateState(STATE value) => new State(value.ToString(), value);
        bool IsValid(StateGraphValue<STATE> value) => value.ValidAction != null;
        public void AddValidStateTransition(STATE startingState, STATE endingState, StateTransitionAction<STATE> action) {
            StateGraphKey key = new(CreateState(startingState), CreateState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue<STATE> value))
                return; //SA???
            stateGraph.Add(key, new StateGraphValue<STATE>(action, null));
        } //AddValidStateTransition
        public void AddInvalidStateTransition(STATE startingState, STATE endingState, InvalidStateTransitionAction<STATE> action) {
            StateGraphKey key = new(CreateState(startingState), CreateState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue<STATE> value))
                return; //SA???
            stateGraph.Add(key, new StateGraphValue<STATE>(null, action));
        } //AddInvalidStateTransition
        public string IsTransitionValid(STATE startingState, STATE endingState) {
            State starting = CreateState(startingState);
            State ending = CreateState(endingState);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue<STATE> value);
            if (found && !IsValid(value) && value.InvalidAction != null) {
                return value.InvalidAction(startingState, endingState);
            }
            return null;
        } //IsTransitionValid
        public bool TryTransitionTo(STATE state, out string invalidTransitionReason) {
            invalidTransitionReason = null;
            State starting = CreateState(CurrentState);
            State ending = CreateState(state);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue<STATE> value);
            if (IsValid(value))
                value.ValidAction(CurrentState, state);
            else
                value.InvalidAction(CurrentState, state);
            return found;
        } //TryTransitionTo
        public void PerformTransitionIndirect(STATE startingState, STATE endingState) {
            //SA??? complicated algorithm of graph search
        } //PerformTransition
        StateSet stateSet = new();
        System.Collections.Generic.Dictionary<StateGraphKey, StateGraphValue<STATE>> stateGraph = new();
    } //class StateMachine

}

