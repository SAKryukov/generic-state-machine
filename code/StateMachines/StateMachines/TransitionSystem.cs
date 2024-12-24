/*
    Generic State Machines

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using System.Collections.Generic;

    public delegate void StateTransitionAction<STATE>(STATE startState, STATE finishState);
    public delegate string InvalidStateTransitionAction<STATE>(STATE startState, STATE finishState);

    public class TransitionSystem<STATE> : TransitionSystemBase<STATE> {

        #region API

        public TransitionSystem(STATE initialState = default) {
            Traverse<STATE>((name, state) => {
                stateDictionary.Add(state, new State(name, state));
                if (state.Equals(initialState))
                    CurrentState = state;
            });
            this.initialState = CurrentState;
            digest = new(this);
        } //TransitionSystem

        public void AddValidStateTransition(STATE startState, STATE finishState, StateTransitionAction<STATE> action, bool undirected = false) {
            if (startState.Equals(finishState)) return;
            StateGraphKey key = new(FindState(startState), FindState(finishState), undirected);
            if (stateGraph.ContainsKey(key))
                throw new GraphPopulationException(startState, finishState);
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
            if (startState.Equals(finishState)) return;
            StateGraphKey key = new(FindState(startState), FindState(finishState));
            if (stateGraph.TryGetValue(key, out StateGraphValue value))
                throw new GraphPopulationException(startState, finishState);
            stateGraph.Add(key, new StateGraphValue(false, null, action));
        } //AddInvalidStateTransition

        public STATE CurrentState { get; private protected set; }

        public STATE ResetState() => // unconditional jump to initial state, ignoring the transition graph
            CurrentState = initialState;

        public (bool isValid, string validityComment) IsTransitionValid(STATE startState, STATE finishState) {
            State start = FindState(startState);
            State finish = FindState(finishState);
            StateGraphKey key = new(start, finish);
            var transition = stateGraph.TryGetValue(key, out StateGraphValue value) ? value : null;
            if (transition == null)
                return (false, DefinitionSet<STATE, bool, bool>.TransitionNotDefined(startState, finishState));
            return IsTransitionValid(transition, startState, finishState);
        } //IsTransitionValid
  
        public (bool success, string validityComment) TryTransitionTo(STATE state) {
            if (CurrentState.Equals(state))
                return (true, DefinitionSet<STATE, bool, bool>.TransitionToTheSameState(CurrentState));
            State start = FindState(CurrentState);
            State finish = FindState(state);
            StateGraphKey key = new(start, finish);
            var transition = stateGraph.TryGetValue(key, out StateGraphValue stateGraphValue) ? stateGraphValue : null;
            bool found = transition != null;
            string validityComment = DefinitionSet<STATE, bool, bool>.TransitionSuccess(state);
            if (found) {
                var validity = IsTransitionValid(stateGraphValue, CurrentState, state);
                if (!validity.IsValid)
                    return (false, validity.ValidityComment);
                transition.ValidAction?.Invoke(CurrentState, state);
                CurrentState = state;
            } else
                return (false, DefinitionSet<STATE, bool, bool>.TransitionNotDefined(CurrentState, state));
            return (found, validityComment);
        } //TryTransitionTo

        public STATE[][] Labyrinth(STATE start, STATE finish, bool shortest = false) {
            digest.BuildFollowingStates();
            static void RecursiveWalk(State start, State finish, List<State> localPath, List<List<State>> solution) {
                if (start == finish) {
                    List<State> solutionElement = new(localPath);
                    solution.Add(solutionElement);
                    return;
                } //if
                start.digest.isVisited = true;
                foreach (var followingState in start.digest.followingStates) {
                    if (followingState.digest.isVisited) continue;
                    localPath.Add(followingState);
                    RecursiveWalk(followingState, finish, localPath, solution);
                    localPath.Remove(followingState);
                } //loop
                start.digest.isVisited = false;
            } //RecursiveWalk
            List<List<State>> solution = new();
            RecursiveWalk(FindState(start), FindState(finish), new List<State>(), solution);
            int shortestPathLength = int.MaxValue;
            STATE[][] stateSolution = System.Array.ConvertAll(solution.ToArray(), path => {
                if (path.Count < shortestPathLength)
                    shortestPathLength = path.Count;
                return System.Array.ConvertAll(path.ToArray(), state => state.UnderlyingMember);
            });
            if (shortest) {
                List<STATE[]> shortestList = new();
                foreach (var path in stateSolution) {
                    if (path.Length == shortestPathLength)
                        shortestList.Add(path);
                } //loop
                stateSolution = shortestList.ToArray();
            } //if shortest
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

        private protected string TransitionTo(STATE state) {
            if (CurrentState.Equals(state))
                return DefinitionSet<STATE, bool, bool>.TransitionToTheSameState(CurrentState);
            State start = FindState(CurrentState);
            State finish = FindState(state);
            CurrentState = state;
            StateGraphKey key = new(start, finish);
            bool found = stateGraph.TryGetValue(key, out StateGraphValue stateGraphValue);
            if (found && stateGraphValue.ValidAction != null)
                stateGraphValue.ValidAction(start.UnderlyingMember, finish.UnderlyingMember);
            return DefinitionSet<STATE, bool, bool>.TransitionSuccess(state);
        } //TransitionTo

        class Digest {
            internal void BuildFollowingStates() {
                if (populated) return;
                populated = true;
                foreach (var (key, value) in owner.stateGraph)
                    if (value.IsValid)
                        Update(key);
            } //BuildFollowingStates
            internal void Update(StateGraphKey key) {
                if (!populated) return;
                key.StartState.digest.followingStates.Add(key.FinishState);
                if (key.IsUndirected)
                    key.FinishState.digest.followingStates.Add(key.StartState);
            } //Update
            internal Digest(TransitionSystem<STATE> owner) { this.owner = owner; }
            readonly TransitionSystem<STATE> owner;
            bool populated;
        } //class Digest

        static bool IsValid(StateGraphValue value) => value.ValidAction != null;

        static (bool IsValid, string ValidityComment) IsTransitionValid(StateGraphValue value, STATE startState, STATE finishState) {
            if (!IsValid(value) && value.InvalidAction != null) {
                return (false, value.InvalidAction(startState, finishState));
            } //if
            return (true, DefinitionSet<STATE, bool, bool>.TransitionIsValid(startState, finishState));
        } //IsTransitionValid

        readonly Dictionary<StateGraphKey, StateGraphValue> stateGraph = new();
        readonly Digest digest;

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
                bool nameMatch = (objectStateGraphKey.StartState.Name == StartState.Name
                    && objectStateGraphKey.FinishState.Name == FinishState.Name);
                return IsUndirected
                    ? nameMatch || 
                        (objectStateGraphKey.StartState.Name == FinishState.Name
                        && objectStateGraphKey.FinishState.Name == StartState.Name)
                    : nameMatch;
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

        class GraphPopulationException : System.ApplicationException {
            internal GraphPopulationException(STATE startState, STATE finishState)
                : base(DefinitionSet<STATE, bool, bool>.GraphPopulationExceptionMessage(startState, finishState)) { }
        } //class GraphPopulationException

        internal State FindState(STATE key) {
            if (stateDictionary.TryGetValue(key, out State state))
                return state;
            else
                throw new InvalidStateException(key);
        } //FindState

        internal class State : Element<STATE> {
            internal State(string name, STATE underlyingMember) : base(name, underlyingMember) {}
            internal (bool isVisited, List<State> followingStates) digest = (false, new());
        } //class State

        class InvalidStateException : System.ApplicationException {
            internal InvalidStateException(STATE state)
                : base(DefinitionSet<STATE, bool, bool>.InvalidStateExceptionMessage(state)) { }
        } //class InvalidStateException

        readonly STATE initialState;
        readonly Dictionary<STATE, State> stateDictionary = new();

        #endregion implementation

    } //class TransitionSystem

}
