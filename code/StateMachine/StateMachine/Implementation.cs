/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using Type = System.Type;
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;
    using System.Collections.Generic;

    public delegate void StateTransitionAction<STATE>(STATE startState, STATE finishState);
    public delegate string InvalidStateTransitionAction<STATE>(STATE startState, STATE finishState);

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NotAStateAttribute : System.Attribute {}

    public class StateMachine<STATE> {

        #region API

        public StateMachine(STATE initialState = default) {
            Type type = typeof(STATE);
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                if (field.GetCustomAttributes(typeof(NotAStateAttribute), inherit: false).Length > 0) continue;
                STATE value = (STATE)field.GetValue(null);
                State state = new(field.Name, value);
                stateDictionary.Add(value, state);
                if (value.Equals(initialState))
                    CurrentState = value;
            } //loop
            this.initialState = CurrentState;
            digest = new(this);
        } //StateMachine

        public STATE CurrentState { get; private set; }

        public STATE ResetState() => // unconditional jump to initial state, ignoring the transition graph
            CurrentState = initialState;

        public void AddValidStateTransition(STATE startState, STATE finishState, StateTransitionAction<STATE> action, bool undirected = false) {
            StateGraphKey key = new(FindState(startState), FindState(finishState), undirected);
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new StateMachineGraphPopulationException(startState, finishState);
            stateGraph.Add(key, new StateGraphValue(true, action, null));
            digest.Update(key);
        } //AddValidStateTransition

        public int AddValidStateTransitionChain(StateTransitionAction<STATE> action, bool undirected = false, params STATE[] chain) {
            if (chain == null) return 0;
            if (chain.Length < 2) return 0;
            STATE current = chain[0];
            int count = 0;
            foreach (var state in chain) {
                if (state.Equals(current)) continue; // drop first
                AddValidStateTransition(current, state, action, undirected);
                ++count;
                current = state;
            } //loop
            return count;
        } //AddValidStateTransitionChain

        public void AddInvalidStateTransition(STATE startState, STATE finishState, InvalidStateTransitionAction<STATE> action) {
            StateGraphKey key = new(FindState(startState), FindState(finishState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new StateMachineGraphPopulationException(startState, finishState);
            stateGraph.Add(key, new StateGraphValue(false, null, action));
        } //AddInvalidStateTransition       

        public (bool isValid, string validityComment) IsTransitionValid(STATE startState, STATE finishState) {
            State start = FindState(startState);
            State finish = FindState(finishState);
            StateGraphKey key = new(start, finish);
            var transition = stateGraph.TryGetValue(key, out StateGraphValue value) ? value : null;
            if (transition == null)
                return (false, DefinitionSet<STATE>.TransitionNotDefined(startState, finishState));
            return IsTransitionValid(transition, startState, finishState);
        } //IsTransitionValid
  
        public (bool success, string validityComment) TryTransitionTo(STATE state) {
            if (CurrentState.Equals(state))
                return (true, DefinitionSet<STATE>.TransitionToTheSameState(CurrentState));
            State start = FindState(CurrentState);
            State finish = FindState(state);
            StateGraphKey key = new(start, finish);
            var transition = stateGraph.TryGetValue(key, out StateGraphValue value) ? value : null;
            bool found = transition != null;
            string validityComment = DefinitionSet<STATE>.TransitionSuccess(state);
            if (found) {
                var validity = IsTransitionValid(value, CurrentState, state);
                if (!validity.IsValid)
                    return (false, validity.ValidityComment);
                transition.ValidAction?.Invoke(CurrentState, state);
                CurrentState = state;
            } else
                return (false, DefinitionSet<STATE>.TransitionNotDefined(CurrentState, state));
            return (found, validityComment);
        } //TryTransitionTo

        public STATE[][] Labyrinth(STATE start, STATE finish, bool shortest = false) {
            digest.BuildFollowingStates();
            Dictionary<State, List<State>> followingNodes = new(); // populated on call
            void RecursiveWalk(State start, State finish, Dictionary<State, bool> visited, List<State> localPath, List<List<State>> solution) {
                if (start == finish) {
                    List<State> solutionElement = new(localPath);
                    solution.Add(solutionElement);
                    return;
                } //if
                visited[start] = true;
                List<State> followingStates = digest.GetFollowingStates(start);
                foreach (var followingState in followingStates) {
                    if (visited[followingState]) continue;
                    localPath.Add(followingState);
                    RecursiveWalk(followingState, finish, visited, localPath, solution);
                    localPath.Remove(followingState);
                } //loop
                visited[start] = false;
            } //RecursiveWalk
            Dictionary<State, bool> visited = new();
            foreach (var state in stateDictionary.Values)
                visited.Add(state, false);
            List<List<State>> solution = new();
            RecursiveWalk(FindState(start), FindState(finish), visited, new List<State>(), solution);
            STATE[][] stateSolution = new STATE[solution.Count][];
            int shortestPathLength = int.MaxValue;
            int index = 0;
            foreach (var element in solution) {
                if (element.Count < shortestPathLength)
                    shortestPathLength = element.Count;
                STATE[] row = new STATE[element.Count];
                for (var indexInRow = 0; indexInRow < row.Length; ++indexInRow)
                    row[indexInRow] = element[indexInRow].UnderlyingMember;
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

        // Find all states not visited along any of the paths between start and finish states
        // It is assumed that the object paths is returned by Labyrinth, and the finish state is
        // the last state of each path
        public STATE[] FindDeadEnds(STATE start, STATE[][] allPaths) {
            HashSet<STATE> found = new(new STATE[1] { start });
            foreach (var row in allPaths)
                foreach (STATE state in row)
                    found.Add(state);
            List<STATE> deadEnds = new();
            foreach (STATE state in stateDictionary.Keys)
                if (!found.Contains(state))
                    deadEnds.Add(state);
            return deadEnds.ToArray();
        } //FindDeadEnds

        // Find all states not visited along any of the paths between start and finish states
        public (STATE[][] allPaths, STATE[] deadEnds) FindDeadEnds(STATE start, STATE finish) {
            STATE[][] allPaths = Labyrinth(start, finish, shortest: false);
            return (allPaths, FindDeadEnds(start, allPaths));
        } //FindDeadEnds

        public (int numberOfPaths, int longestPathLength, STATE[][] longestPaths) LongestPaths { //NP-hard
            get {
                List<STATE[]> longestPaths = new();
                int max = -1;
                int pathCount = 0;
                foreach (var startValue in stateDictionary.Values)
                    foreach (var finishValue in stateDictionary.Values) {
                        STATE[][] solution = Labyrinth(startValue.UnderlyingMember, finishValue.UnderlyingMember);
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
                foreach (var startValue in stateDictionary.Values)
                    foreach (var finishValue in stateDictionary.Values) {
                        STATE[][] solution = Labyrinth(startValue.UnderlyingMember, finishValue.UnderlyingMember);
                        if (solution.Length >= max) {
                            if (solution.Length > max)
                                pairList.Clear();
                            max = solution.Length;
                            pairList.Add((startValue.UnderlyingMember, finishValue.UnderlyingMember));
                        } //if
                    } //outer loop
                return (max, pairList.ToArray());
            } //get LongestNumberOfPaths 
        } //LongestNumberOfPaths

        #endregion API

        #region implementation

        class Digest {
            internal void BuildFollowingStates() {
                if (followingNodes.Count > 0) return;
                used = true;
                foreach (var stateValue in owner.stateDictionary.Values)
                    followingNodes.Add(stateValue, new List<State>());
                foreach (var (key, value) in owner.stateGraph) {
                    if (!value.IsValid) continue;
                    Update(key);
                } //loop
            } //BuildFollowingStates
            internal List<State> GetFollowingStates(State state) =>
                followingNodes[state];
            internal void Update(StateGraphKey key) {
                if (!used) return;
                Update(key.StartState, key.FinishState);
                if (key.IsUndirected)
                    Update(key.FinishState, key.StartState);
            } //Update
            void Update(State start, State finish) {
                followingNodes[start].Add(finish);
            } //Update
            internal Digest(StateMachine<STATE> owner) { this.owner = owner; }
            readonly StateMachine<STATE> owner;
            readonly Dictionary<State, List<State>> followingNodes = new();
            bool used;
        } //class Digest

        State FindState(STATE value) {
            if (stateDictionary.TryGetValue(value, out State state))
                return state;
            else
                throw new InvalidStateException(value);
        } //FindState

        static bool IsValid(StateGraphValue value) => value.ValidAction != null;

        static (bool IsValid, string ValidityComment) IsTransitionValid(StateGraphValue value, STATE startState, STATE finishState) {
            if (!IsValid(value) && value.InvalidAction != null) {
                return (false, value.InvalidAction(startState, finishState));
            } //if
            return (true, DefinitionSet<STATE>.TransitionIsValid(startState, finishState));
        } //IsTransitionValid

        readonly Dictionary<STATE, State> stateDictionary = new();
        readonly Dictionary<StateGraphKey, StateGraphValue> stateGraph = new();
        readonly Digest digest;
        readonly STATE initialState;

        class StateMachineGraphPopulationException : System.ApplicationException {
            internal StateMachineGraphPopulationException(STATE startState, STATE finishState)
                : base(DefinitionSet<STATE>.StateMachineGraphPopulationExceptionMessage(startState, finishState)) { }
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
            internal StateGraphKey(State start, State finish, bool undirected = false) {
                StartState = start; FinishState = finish;
                IsUndirected = undirected;
            }
            public override int GetHashCode() { // important!
                return StartState.Name.GetHashCode()
                    ^ FinishState.Name.GetHashCode();
            }
            public override bool Equals(object @object) { // important!
                if (@object == null) return false;
                if (@object is not StateGraphKey objectStateGraphKey) return false;
                return IsUndirected
                    ? (objectStateGraphKey.StartState.Name == StartState.Name
                    && objectStateGraphKey.FinishState.Name == FinishState.Name)
                    || (objectStateGraphKey.StartState.Name == FinishState.Name
                    && objectStateGraphKey.FinishState.Name == StartState.Name)
                    : objectStateGraphKey.StartState.Name == StartState.Name
                    && objectStateGraphKey.FinishState.Name == FinishState.Name;
            } //Equals
            internal State StartState { get; init; }
            internal State FinishState { get; init; }
            internal bool IsUndirected { get; init; }
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

        #endregion implementation

    } //class StateMachine

}
