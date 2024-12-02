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

        public StateMachine(STATE initialState = default(STATE)) {
            Type type = typeof(STATE);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                STATE value = (STATE)field.GetValue(null);
                State<STATE> state = new(field.Name, value);
                stateSet.Add(state);
                stateSearchDictionary.Add(value, state);
                if (value.Equals(initialState))
                    CurrentState = value;
            } //loop
        } //StateMachine
        public STATE CurrentState { get; private set; }
        State<STATE> FindState(STATE value) {
            if (stateSearchDictionary.TryGetValue(value, out State<STATE> state))
                return state;
            else
                throw new InvalidStateException<STATE>(value); // fallback for the value out of enum
        } //FindState
        bool IsValid(StateGraphValue<STATE> value) => value.ValidAction != null;
        public void AddValidStateTransition(STATE startingState, STATE endingState, StateTransitionAction<STATE> action) {
            StateGraphKey<STATE> key = new(FindState(startingState), FindState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue<STATE> value))
                throw new StateMachineGraphPopulationException<STATE>(startingState, endingState);
            stateGraph.Add(key, new StateGraphValue<STATE>(action, null));
        } //AddValidStateTransition
        public void AddInvalidStateTransition(STATE startingState, STATE endingState, InvalidStateTransitionAction<STATE> action) {
            StateGraphKey<STATE> key = new(FindState(startingState), FindState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue<STATE> value))
                throw new StateMachineGraphPopulationException<STATE>(startingState, endingState);
            stateGraph.Add(key, new StateGraphValue<STATE>(null, action));
        } //AddInvalidStateTransition
        public (bool IsValid, string ValidityComment) IsTransitionValid(STATE startingState, STATE endingState) {
            State<STATE> starting = FindState(startingState);
            State<STATE> ending = FindState(endingState);
            StateGraphKey<STATE> key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue<STATE> value);
            if (!found)
                return (false, DefinitionSet<STATE>.TransitionNotDefined(startingState, endingState));
            if (!IsValid(value) && value.InvalidAction != null) {
                return (false, value.InvalidAction(startingState, endingState));
            } //if
            return (true, DefinitionSet<STATE>.TransitionIsValid(startingState, endingState));
        } //IsTransitionValid
        public bool TryTransitionTo(STATE state, out string invalidTransitionReason) {
            invalidTransitionReason = null;
            State<STATE> starting = FindState(CurrentState);
            State<STATE> ending = FindState(state);
            StateGraphKey<STATE> key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue<STATE> value);
            if (IsValid(value))
                value.ValidAction(CurrentState, state);
            else
                value.InvalidAction(CurrentState, state);
            CurrentState = state;
            return found;
        } //TryTransitionTo
        public bool TryTransitionTo(STATE state) =>
            TryTransitionTo(state, out string _);
        public void PerformTransitionIndirect(STATE startingState, STATE endingState) {
            //SA??? complicated algorithm of graph search to be implemented
        } //PerformTransition
        System.Collections.Generic.HashSet<State<STATE>> stateSet = new();
        System.Collections.Generic.Dictionary<STATE, State<STATE>> stateSearchDictionary = new();
        System.Collections.Generic.Dictionary<StateGraphKey<STATE>, StateGraphValue<STATE>> stateGraph = new();
    } //class StateMachine

    class StateMachineGraphPopulationException<STATE> : System.ApplicationException {
        internal StateMachineGraphPopulationException(STATE stargingState, STATE endingState)
            : base(DefinitionSet<STATE>.StateMachineGraphPopulationExceptionMessage(stargingState, endingState)) { }
    } //class StateMachineGraphPopulationException

    class InvalidStateException<STATE> : System.ApplicationException {
        internal InvalidStateException(STATE state)
            : base(DefinitionSet<STATE>.InvalidStateExceptionMessage(state)) { }
    } //class InvalidStateException

    sealed class State<STATE> {
        internal State(string name, STATE underlyingMember) {
            Name = name;
            UnderlyingMember = underlyingMember;
        } //State
        internal string Name { get; init; }
        internal STATE UnderlyingMember { get; init; }
    } //class State

    class StateGraphKey<STATE> {
        internal StateGraphKey(State<STATE> starting, State<STATE> ending) {
            StartingState = starting; EndingState = ending;
        }
        public override int GetHashCode() { // important!
            return StartingState.Name.GetHashCode()
                ^ EndingState.Name.GetHashCode();
        }
        public override bool Equals(object @object) { // important!
            if (@object == null) return false;
            if (@object is not StateGraphKey<STATE> objectStateGraphKey) return false;
            return objectStateGraphKey.StartingState.Name == StartingState.Name
                && objectStateGraphKey.EndingState.Name == EndingState.Name;
        }
        internal State<STATE> StartingState { get; init; }
        internal State<STATE> EndingState { get; init; }
    } //class StateGraphKey

    class StateGraphValue<STATE> {
        internal StateGraphValue(StateTransitionAction<STATE> valid, InvalidStateTransitionAction<STATE> invalid) {
            ValidAction = valid; InvalidAction = invalid;
        }
        internal StateTransitionAction<STATE> ValidAction { get; init; }
        internal InvalidStateTransitionAction<STATE> InvalidAction { get; init; }
    } //class StateGraphValue

    abstract class StateTransition<STATE> : StateGraphKey<STATE> {
        internal StateTransition(State<STATE> starting, State<STATE> ending) :
            base(starting, ending) { }
        internal abstract bool IsValid { get; }
    } //StateTransition

    class ValidStateTransition<STATE> : StateTransition<STATE> {
        internal ValidStateTransition(State<STATE> starting, State<STATE> ending) :
            base(starting, ending) { }
        internal override bool IsValid { get { return true; } }
    } //class ValidStateTransition

    class InvalidStateTransition<STATE> : StateTransition<STATE> {
        internal InvalidStateTransition(State<STATE> starting, State<STATE> ending) :
            base(starting, ending) { }
        internal override bool IsValid { get { return false; } }
    } //class ValidStateTransition

}
