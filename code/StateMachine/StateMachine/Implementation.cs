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
    using System.Collections.Generic;

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
                stateDictionary.Add(value, state);
                if (value.Equals(initialState))
                    CurrentState = value;
            } //loop
            this.initialState = CurrentState;
        } //StateMachine

        public STATE CurrentState { get; private set; }

        public STATE ResetState() => // unconditional jump to initial state, ignoring the transition graph
            CurrentState = initialState;

        public void AddValidStateTransition(STATE startingState, STATE endingState, StateTransitionAction<STATE> action, bool undirected = false) {
            if (undirected) {
                AddValidStateTransition(startingState, endingState, action);
                AddValidStateTransition(endingState, startingState, action);
            } else {
                StateGraphKey key = new(FindState(startingState), FindState(endingState));
                if (stateGraph.TryGetValue(key, out StateGraphValue value))
                    throw new StateMachineGraphPopulationException(startingState, endingState);
                stateGraph.Add(key, new StateGraphValue(true, action, null));
            } //if
        } //AddValidStateTransition

        public void AddValidStateTransitionChain(StateTransitionAction<STATE> action, bool undirected = false, params STATE[] chain) {
            if (chain == null) return;
            if (chain.Length < 2) return;
            STATE current = chain[0];
            foreach (var state in chain) {
                if (state.Equals(current)) continue; // drop first
                AddValidStateTransition(current, state, action, undirected);
                current = state;
            } //loop
        } //AddValidStateTransitionChain

        public void AddInvalidStateTransition(STATE startingState, STATE endingState, InvalidStateTransitionAction<STATE> action) {
            StateGraphKey key = new(FindState(startingState), FindState(endingState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new StateMachineGraphPopulationException(startingState, endingState);
            stateGraph.Add(key, new StateGraphValue(false, null, action));
        } //AddInvalidStateTransition       

        public (bool isValid, string validityComment) IsTransitionValid(STATE startingState, STATE endingState) {
            State starting = FindState(startingState);
            State ending = FindState(endingState);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            if (!found)
                return (false, DefinitionSet<STATE>.TransitionNotDefined(startingState, endingState));
            return IsTransitionValid(value, startingState, endingState);
        } //IsTransitionValid
  
        public (bool success, string validityComment) TryTransitionTo(STATE state) {
            if (CurrentState.Equals(state))
                return (true, DefinitionSet<STATE>.TransitionToTheSameState(CurrentState));
            State starting = FindState(CurrentState);
            State ending = FindState(state);
            StateGraphKey key = new(starting, ending);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue value);
            string validityComment = DefinitionSet<STATE>.TransitionSuccess(state);
            if (found) {
                var validity = IsTransitionValid(value, CurrentState, state);
                if (!validity.IsValid)
                    return (false, validity.ValidityComment);
                value.ValidAction?.Invoke(CurrentState, state);
                CurrentState = state;
            } else
                return (false, DefinitionSet<STATE>.TransitionNotDefined(CurrentState, state));
            return (found, validityComment);
        } //TryTransitionTo

        public STATE[][] Labyrinth(STATE start, STATE finish, bool shortest = false) {
            Dictionary<State, List<State>> followingNodes = new(); // populated on call
            List<State> BuildFollowingStates(int stateIndex, State[] indexed) {
                if (followingNodes.TryGetValue(indexed[stateIndex], out List<State> nodes))
                    return nodes;
                List<State> newList = new();
                foreach (var pair in stateGraph) {
                    if (!pair.Value.IsValid) continue;
                    if (!pair.Key.StartingState.UnderlyingMember.Equals(indexed[stateIndex].UnderlyingMember)) continue;
                    newList.Add(pair.Key.EndingState);
                } //loop
                followingNodes.Add(indexed[stateIndex], newList);
                return newList;
            } //BuildFollowingStates
            void RecursiveWalk(int start, int finish, bool[] visited, List<int> localPath, State[] indexed, Dictionary<State, int> stateIndex, List<List<int>> solution) {
                if (start == finish) {
                    List<int> solutionElement = new(localPath);
                    solution.Add(solutionElement);
                    return;
                } //if
                visited[start] = true;
                List<State> followingStates = BuildFollowingStates(start, indexed);
                foreach (var followingState in followingStates) {
                    int followingStateIndex = stateIndex[followingState];
                    if (visited[followingStateIndex]) continue;
                    localPath.Add(followingStateIndex);
                    RecursiveWalk(followingStateIndex, finish, visited, localPath, indexed, stateIndex, solution);
                    localPath.Remove(followingStateIndex);
                } //loop
                visited[start] = false;
            } //RecursiveWalk
            bool[] visited = new bool[stateDictionary.Count];
            State[] indexed = new State[stateDictionary.Count];
            Dictionary<State, int> stateIndex = new();
            int index = 0;
            int startIndex = 0, finishIndex = 0;
            foreach (var pair in stateDictionary) {
                indexed[index] = pair.Value;
                if (start.Equals(pair.Value.UnderlyingMember))
                    startIndex = index;
                if (finish.Equals(pair.Value.UnderlyingMember))
                    finishIndex = index;
                stateIndex.Add(pair.Value, index);
                ++index;
            } //loop
            List<List<int>> solution = new();
            RecursiveWalk(startIndex, finishIndex, visited, new List<int>(), indexed, stateIndex, solution);
            STATE[][] stateSolution = new STATE[solution.Count][];
            index = 0;
            int shortestPathLength = int.MaxValue;
            foreach (var element in solution) {
                if (element.Count < shortestPathLength)
                    shortestPathLength = element.Count;
                STATE[] row = new STATE[element.Count];
                int indexInRow = 0;
                foreach (int stateIndex0 in element)
                    row[indexInRow++] = indexed[stateIndex0].UnderlyingMember;
                stateSolution[index++] = row;
            } //loop
            if (shortest) {
                List<STATE[]> shortestList = new();
                foreach (var element in stateSolution)
                    if (element.Length == shortestPathLength)
                        shortestList.Add(element);
                return shortestList.ToArray();
            } //if
            return stateSolution;
        } //Labyrinth

        public (int numberOfPaths, int longestPathLength, STATE[][] longestPaths) LongestPaths { //NP-hard
            get {
                List<STATE[]> longestPaths = new();
                int max = -1;
                int pathCount = 0;
                foreach(var startPair in stateDictionary)
                    foreach (var finishPair in stateDictionary) {
                        STATE[][] solution = Labyrinth(startPair.Value.UnderlyingMember, finishPair.Value.UnderlyingMember);
                        pathCount += solution.Length;
                        foreach (STATE[] item in solution) {
                            if (item.Length >= max) {
                                if (item.Length > max)
                                    longestPaths.Clear();
                                longestPaths.Add(item);
                                max = item.Length;
                            } //if
                        } // inner loop
                    } //outer loop
                return (pathCount, max, longestPaths.ToArray());
            } //get LongestPaths
        } //LongestPaths

        public (int maximumNumberOfPaths, (STATE start, STATE finish)[] pairsAtMax) MaximumPaths { //NP-hard
            get {
                int max = 0;
                List<(STATE start, STATE finish)> pairList = new();
                foreach (var startPair in stateDictionary)
                    foreach (var finishPair in stateDictionary) {
                        STATE[][] solution = Labyrinth(startPair.Value.UnderlyingMember, finishPair.Value.UnderlyingMember);
                        if (solution.Length >= max) {
                            if (solution.Length > max)
                                pairList.Clear();
                            max = solution.Length;
                            pairList.Add((startPair.Value.UnderlyingMember, finishPair.Value.UnderlyingMember));
                        } //if
                    } //outer loop
                return (max, pairList.ToArray());
            } //get LongestNumberOfPaths 
        } //LongestNumberOfPaths 

        #endregion API

        #region implementation

        State FindState(STATE value) {
            if (stateDictionary.TryGetValue(value, out State state))
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

        readonly Dictionary<STATE, State> stateDictionary = new();
        readonly Dictionary<StateGraphKey, StateGraphValue> stateGraph = new();
        readonly STATE initialState;

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
            internal StateGraphValue(bool isValid, StateTransitionAction<STATE> valid, InvalidStateTransitionAction<STATE> invalid) {
                IsValid = isValid;
                ValidAction = valid; InvalidAction = invalid;
            }
            internal bool IsValid { get; init; }
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

        #endregion implementation

    } //class StateMachine

}
