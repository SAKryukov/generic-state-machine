/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
*/

namespace StateMachines {
    using Type = System.Type;
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;

    public delegate void StateTransitionAction<STATE>(STATE startingState, STATE endingState);
    public delegate string InvalidStateTransitionAction<STATE>(STATE startingState, STATE endingState);

    public class StateMachine<STATE> {

        #region API

        public StateMachine(STATE initialState = default) {
            Type type = typeof(STATE);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                STATE value = (STATE)field.GetValue(null);
                State state = new(field.Name, value);
                stateSet.Add(state);
                stateSearchDictionary.Add(value, state);
                if (value.Equals(initialState))
                    CurrentState = value;
            } //loop
        } //StateMachine

        public STATE CurrentState { get; private set; }

        public void AddValidStateTransition(STATE startingState, STATE endingState, StateTransitionAction<STATE> action) {
            StateGraphKey key = new(FindState(startingState), FindState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new StateMachineGraphPopulationException(startingState, endingState);
            stateGraph.Add(key, new StateGraphValue(action, null));
        } //AddValidStateTransition

        public void AddInvalidStateTransition(STATE startingState, STATE endingState, InvalidStateTransitionAction<STATE> action) {
            StateGraphKey key = new(FindState(startingState), FindState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new StateMachineGraphPopulationException(startingState, endingState);
            stateGraph.Add(key, new StateGraphValue(null, action));
        } //AddInvalidStateTransition       

        public (bool IsValid, string ValidityComment) IsTransitionValid(STATE startingState, STATE endingState) {
            State starting = FindState(startingState);
            State ending = FindState(endingState);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            if (!found)
                return (false, DefinitionSet<STATE>.TransitionNotDefined(startingState, endingState));
            return IsTransitionValid(value, startingState, endingState);
        } //IsTransitionValid
        public (bool success, string invalidTransitionReason) TryTransitionTo(STATE state) {
            if (CurrentState.Equals(state))
                return (true, DefinitionSet<STATE>.TransitionToTheSameState(CurrentState));
            State starting = FindState(CurrentState);
            State ending = FindState(state);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            string invalidTransitionReason = DefinitionSet<STATE>.TransitionSuccess(state);
            if (found) {
                var validity = IsTransitionValid(value, CurrentState, state);
                if (!validity.IsValid)
                    return (false, validity.ValidityComment);
                value.ValidAction(CurrentState, state);
                CurrentState = state;
            } else
                return (false, null);
            return (found, invalidTransitionReason);
        } //TryTransitionTo

        #endregion API

        State FindState(STATE value) {
            if (stateSearchDictionary.TryGetValue(value, out State state))
                return state;
            else
                throw new InvalidStateException(value);
        } //FindState

        static bool IsValid(StateGraphValue value) => value.ValidAction != null;

        static (bool IsValid, string ValidityComment) IsTransitionValid(StateGraphValue value, STATE startingState, STATE endingState) {
            if (!IsValid(value) && value.InvalidAction != null) {
                return (false, value.InvalidAction(startingState, endingState));
            } //if
            return (true, DefinitionSet<STATE>.TransitionIsValid(startingState, endingState));
        } //IsTransitionValid

        readonly System.Collections.Generic.HashSet<State> stateSet = new();
        readonly System.Collections.Generic.Dictionary<STATE, State> stateSearchDictionary = new();
        readonly System.Collections.Generic.Dictionary<StateGraphKey, StateGraphValue> stateGraph = new();

        class StateMachineGraphPopulationException : System.ApplicationException {
            internal StateMachineGraphPopulationException(STATE stargingState, STATE endingState)
                : base(DefinitionSet<STATE>.StateMachineGraphPopulationExceptionMessage(stargingState, endingState)) { }
        } //class StateMachineGraphPopulationException

        class InvalidStateException : System.ApplicationException {
            internal InvalidStateException(STATE state)
                : base(DefinitionSet<STATE>.InvalidStateExceptionMessage(state)) { }
        } //class InvalidStateException

        sealed class State {
            internal State(string name, STATE underlyingMember) {
                Name = name;
                UnderlyingMember = underlyingMember;
            } //State
            internal string Name { get; init; }
            internal STATE UnderlyingMember { get; init; }
        } //class State

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

        class StateGraphValue {
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

    } //class StateMachine

}
