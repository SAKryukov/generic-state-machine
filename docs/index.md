Generic State Machine{title}

@toc

# StateMachines Namespace

## Delegate ValidStateTransitionAction

~~~{lang=C#}
<span class="keyword highlighter">public</span> delegate <span class="keyword highlighter">void</span> <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

## Delegate InvalidStateTransitionAction

~~~{lang=C#}
<span class="keyword highlighter">public</span> delegate string <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

## NotAState Attribute

This attribute can be used to mark some enumeration type members to exclude them from the set of states of a state machine. It can be useful to create members irrelevant for the state machine behavior but use for some calculations. For example, such a member can be a bitwize `OR` combination of several states.

~~~{lang=C#}
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = <span class="keyword highlighter">false</span>, Inherited = <span class="keyword highlighter">false</span>)]
<span class="keyword highlighter">public</span> <span class="keyword highlighter">class</span> <span class="_custom-word_ highlighter">NotAStateAttribute</span> : System.Attribute {}
~~~

Example:

~~~{lang=C#}
<span class="keyword highlighter">enum</span> <span class="_custom-word_ highlighter">NotAStateAttribute</span> {
    Locked = 1, Closed = 2, Opened = 4,
    OpenedInside = 8, ClosedInside = 16, LockedInside = 32,
    [<span class="_custom-word_ highlighter">NotAState</span>] Inside = OpenedInside | ClosedInside | LockedInside };
~~~

An attempt to perform a state transition to a `NotAState` enumeration value using [`TryTransitionTo`](#try-transition-to) will throw an exception.

## Class StateMachine

### Generic Parameter STATE

The SA???

### Public Constructor

Creates and instance of `StateMachine`.

Parameter: `STATE initialState = default`. Defines initial state of the state machine. See also [`ResetState`](#reset-state).

~~~{lang=C#}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">StateMachine</span>(<span class="_custom-word_ highlighter">STATE</span> initialState = <span class="keyword highlighter">default</span>);
~~~

### Public Methods

#### ResetState

~~~{lang=C#}{id=reset-state}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> ResetState();
~~~

#### AddValidStateTransition

~~~{lang=C#}
<span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddValidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>);
~~~

#### AddValidStateTransitionChain

~~~{lang=C#}
<span class="keyword highlighter">public</span> int AddValidStateTransitionChain(<span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>, params <span class="_custom-word_ highlighter">STATE</span>[] chain)
~~~

#### AddInvalidStateTransition

~~~{lang=C#}
<span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddInvalidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action)
~~~

#### IsTransitionValid

~~~{lang=C#}
<span class="keyword highlighter">public</span> (bool isValid, string validityComment) IsTransitionValid(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

#### TryTransitionTo

~~~{lang=C#}{id=try-transition-to}
<span class="keyword highlighter">public</span> (bool success, string validityComment) TryTransitionTo(<span class="_custom-word_ highlighter">STATE</span> state);
~~~

#### Labyrinth

~~~{lang=C#}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[][] Labyrinth(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish, bool shortest = <span class="keyword highlighter">false</span>);
~~~

#### FindDeadEnds

~~~{lang=C#}
<span class="comment text highlighter">// Find all states not visited along any of the paths between start and finish states</span>
<span class="comment text highlighter">// It is assumed that the object paths is returned by Labyrinth, and the finish state is</span>
<span class="comment text highlighter">// the last state of each path</span>
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[] FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span>[][] allPaths);
~~~

~~~{lang=C#}
<span class="comment text highlighter">// Find all states not visited along any of the paths between start and finish states</span>
<span class="keyword highlighter">public</span> (<span class="_custom-word_ highlighter">STATE</span>[][] allPaths, <span class="_custom-word_ highlighter">STATE</span>[] deadEnds) FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish);
~~~

### Public Properties

#### CurrentState

~~~{lang=C#}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> CurrentState;
~~~

#### LongestPaths

~~~{lang=C#}
<span class="keyword highlighter">public</span> (int numberOfPaths, int longestPathLength, <span class="_custom-word_ highlighter">STATE</span>[][] longestPaths) LongestPaths; <span class="comment text highlighter">//NP-hard</span>
~~~

#### MaximumPaths

~~~{lang=C#}
<span class="keyword highlighter">public</span> (int maximumNumberOfPaths, (<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish)[] pairsAtMax) MaximumPaths; <span class="comment text highlighter">//NP-hard</span>
~~~

<!--
~~~ {lang=C#}
<span class="comment block highlighter">/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/</span>

namespace StateMachines {
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;
    using System.Collections.Generic;

    <span class="keyword highlighter">public</span> delegate <span class="keyword highlighter">void</span> <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
    <span class="keyword highlighter">public</span> delegate string <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = <span class="keyword highlighter">false</span>, Inherited = <span class="keyword highlighter">false</span>)]
    <span class="keyword highlighter">public</span> <span class="keyword highlighter">class</span> <span class="_custom-word_ highlighter">NotAStateAttribute</span> : System.Attribute {}

    <span class="keyword highlighter">public</span> <span class="keyword highlighter">class</span> <span class="_custom-word_ highlighter">StateMachine</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; {

        #region API

        <span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">StateMachine</span>(<span class="_custom-word_ highlighter">STATE</span> initialState = <span class="keyword highlighter">default</span>) {
            FieldInfo[] fields = <span class="keyword highlighter">typeof</span>(<span class="_custom-word_ highlighter">STATE</span>).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (<span class="keyword highlighter">var</span> field <span class="keyword highlighter">in</span> fields) {
                <span class="keyword highlighter">if</span> (field.GetCustomAttributes(<span class="keyword highlighter">typeof</span>(<span class="_custom-word_ highlighter">NotAStateAttribute</span>), inherit: <span class="keyword highlighter">false</span>).Length &gt; <span class="literal numeric highlighter">0</span>) <span class="keyword highlighter">continue</span>;
                <span class="_custom-word_ highlighter">STATE</span> value = (<span class="_custom-word_ highlighter">STATE</span>)field.GetValue(<span class="keyword highlighter">null</span>);
                State state = <span class="keyword highlighter">new</span>(field.Name, value);
                stateDictionary.Add(value, state);
                <span class="keyword highlighter">if</span> (value.Equals(initialState))
                    CurrentState = value;
            } <span class="comment text highlighter">//loop</span>
            <span class="keyword highlighter">this</span>.initialState = CurrentState;
            digest = <span class="keyword highlighter">new</span>(<span class="keyword highlighter">this</span>);
        } <span class="comment text highlighter">//StateMachine</span>

        <span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> CurrentState { get; <span class="keyword highlighter">private</span> set; }

        <span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> ResetState() =&gt; <span class="comment text highlighter">// unconditional jump to initial state, ignoring the transition graph</span>
            CurrentState = initialState;

        <span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddValidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>) {
            <span class="keyword highlighter">if</span> (startState.Equals(finishState)) <span class="keyword highlighter">return</span>;
            StateGraphKey key = <span class="keyword highlighter">new</span>(FindState(startState), FindState(finishState), undirected);
            <span class="keyword highlighter">if</span> (stateGraph.TryGetValue(key, out StateGraphValue value))
                <span class="keyword highlighter">throw</span> <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">StateMachineGraphPopulationException</span>(startState, finishState);
            stateGraph.Add(key, <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">StateGraphValue</span>(<span class="keyword highlighter">true</span>, action, <span class="keyword highlighter">null</span>));
            digest.Update(key);
        } <span class="comment text highlighter">//AddValidStateTransition</span>

        <span class="keyword highlighter">public</span> int AddValidStateTransitionChain(<span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>, params <span class="_custom-word_ highlighter">STATE</span>[] chain) {
            <span class="keyword highlighter">if</span> (chain == <span class="keyword highlighter">null</span>) <span class="keyword highlighter">return</span> <span class="literal numeric highlighter">0</span>;
            <span class="keyword highlighter">if</span> (chain.Length &lt; <span class="literal numeric highlighter">2</span>) <span class="keyword highlighter">return</span> <span class="literal numeric highlighter">0</span>;
            <span class="_custom-word_ highlighter">STATE</span> current = chain[<span class="literal numeric highlighter">0</span>];
            int count = <span class="literal numeric highlighter">0</span>;
            foreach (<span class="keyword highlighter">var</span> state <span class="keyword highlighter">in</span> chain) {
                <span class="keyword highlighter">if</span> (state.Equals(current)) <span class="keyword highlighter">continue</span>; <span class="comment text highlighter">// drop first</span>
                AddValidStateTransition(current, state, action, undirected);
                ++count;
                current = state;
            } <span class="comment text highlighter">//loop</span>
            <span class="keyword highlighter">return</span> count;
        } <span class="comment text highlighter">//AddValidStateTransitionChain</span>

        <span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddInvalidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action) {
            <span class="keyword highlighter">if</span> (startState.Equals(finishState)) <span class="keyword highlighter">return</span>;
            StateGraphKey key = <span class="keyword highlighter">new</span>(FindState(startState), FindState(finishState));
            <span class="keyword highlighter">if</span> (stateGraph.TryGetValue(key, out StateGraphValue value))
                <span class="keyword highlighter">throw</span> <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">StateMachineGraphPopulationException</span>(startState, finishState);
            stateGraph.Add(key, <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">StateGraphValue</span>(<span class="keyword highlighter">false</span>, <span class="keyword highlighter">null</span>, action));
        } <span class="comment text highlighter">//AddInvalidStateTransition       </span>

        <span class="keyword highlighter">public</span> (bool isValid, string validityComment) IsTransitionValid(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState) {
            State start = FindState(startState);
            State finish = FindState(finishState);
            StateGraphKey key = <span class="keyword highlighter">new</span>(start, finish);
            <span class="keyword highlighter">var</span> transition = stateGraph.TryGetValue(key, out StateGraphValue value) ? value : <span class="keyword highlighter">null</span>;
            <span class="keyword highlighter">if</span> (transition == <span class="keyword highlighter">null</span>)
                <span class="keyword highlighter">return</span> (<span class="keyword highlighter">false</span>, DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.TransitionNotDefined(startState, finishState));
            <span class="keyword highlighter">return</span> IsTransitionValid(transition, startState, finishState);
        } <span class="comment text highlighter">//IsTransitionValid</span>
  
        <span class="keyword highlighter">public</span> (bool success, string validityComment) TryTransitionTo(<span class="_custom-word_ highlighter">STATE</span> state) {
            <span class="keyword highlighter">if</span> (CurrentState.Equals(state))
                <span class="keyword highlighter">return</span> (<span class="keyword highlighter">true</span>, DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.TransitionToTheSameState(CurrentState));
            State start = FindState(CurrentState);
            State finish = FindState(state);
            StateGraphKey key = <span class="keyword highlighter">new</span>(start, finish);
            <span class="keyword highlighter">var</span> transition = stateGraph.TryGetValue(key, out StateGraphValue value) ? value : <span class="keyword highlighter">null</span>;
            bool found = transition != <span class="keyword highlighter">null</span>;
            string validityComment = DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.TransitionSuccess(state);
            <span class="keyword highlighter">if</span> (found) {
                <span class="keyword highlighter">var</span> validity = IsTransitionValid(value, CurrentState, state);
                <span class="keyword highlighter">if</span> (!validity.IsValid)
                    <span class="keyword highlighter">return</span> (<span class="keyword highlighter">false</span>, validity.ValidityComment);
                transition.ValidAction?.Invoke(CurrentState, state);
                CurrentState = state;
            } <span class="keyword highlighter">else</span>
                <span class="keyword highlighter">return</span> (<span class="keyword highlighter">false</span>, DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.TransitionNotDefined(CurrentState, state));
            <span class="keyword highlighter">return</span> (found, validityComment);
        } <span class="comment text highlighter">//TryTransitionTo</span>

        <span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[][] Labyrinth(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish, bool shortest = <span class="keyword highlighter">false</span>) {
            digest.BuildFollowingStates();
            <span class="keyword highlighter">static</span> <span class="keyword highlighter">void</span> RecursiveWalk(State start, State finish, List&lt;State&gt; localPath, List&lt;List&lt;State&gt;&gt; solution) {
                <span class="keyword highlighter">if</span> (start == finish) {
                    List&lt;State&gt; solutionElement = <span class="keyword highlighter">new</span>(localPath);
                    solution.Add(solutionElement);
                    <span class="keyword highlighter">return</span>;
                } <span class="comment text highlighter">//if</span>
                start.digest.isVisited = <span class="keyword highlighter">true</span>;
                foreach (<span class="keyword highlighter">var</span> followingState <span class="keyword highlighter">in</span> start.digest.followingStates) {
                    <span class="keyword highlighter">if</span> (followingState.digest.isVisited) <span class="keyword highlighter">continue</span>;
                    localPath.Add(followingState);
                    RecursiveWalk(followingState, finish, localPath, solution);
                    localPath.Remove(followingState);
                } <span class="comment text highlighter">//loop</span>
                start.digest.isVisited = <span class="keyword highlighter">false</span>;
            } <span class="comment text highlighter">//RecursiveWalk</span>
            List&lt;List&lt;State&gt;&gt; solution = <span class="keyword highlighter">new</span>();
            RecursiveWalk(FindState(start), FindState(finish), <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">List&lt;State&gt;</span>(), solution);
            int shortestPathLength = int.MaxValue;
            <span class="_custom-word_ highlighter">STATE</span>[][] stateSolution = System.Array.ConvertAll(solution.ToArray(), path =&gt; {
                <span class="keyword highlighter">if</span> (path.Count &lt; shortestPathLength)
                    shortestPathLength = path.Count;
                <span class="keyword highlighter">return</span> System.Array.ConvertAll(path.ToArray(), state =&gt; state.UnderlyingMember);
            });
            <span class="keyword highlighter">if</span> (shortest) {
                List&lt;<span class="_custom-word_ highlighter">STATE</span>[]&gt; shortestList = <span class="keyword highlighter">new</span>();
                foreach (<span class="keyword highlighter">var</span> path <span class="keyword highlighter">in</span> stateSolution) {
                    <span class="keyword highlighter">if</span> (path.Length == shortestPathLength)
                        shortestList.Add(path);
                } <span class="comment text highlighter">//loop</span>
                stateSolution = shortestList.ToArray();
            } <span class="comment text highlighter">//if shortest</span>
            <span class="keyword highlighter">return</span> stateSolution;
        } <span class="comment text highlighter">//Labyrinth</span>

        <span class="comment text highlighter">// Find all states not visited along any of the paths between start and finish states</span>
        <span class="comment text highlighter">// It is assumed that the object paths is returned by Labyrinth, and the finish state is</span>
        <span class="comment text highlighter">// the last state of each path</span>
        <span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[] FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span>[][] allPaths) {
            HashSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; found = <span class="keyword highlighter">new</span>(<span class="keyword highlighter">new</span> <span class="literal keyword highlighter">STATE[1] { start });
            foreach </span>(<span class="keyword highlighter">var</span> row <span class="keyword highlighter">in</span> allPaths)
                foreach (<span class="_custom-word_ highlighter">STATE</span> state <span class="keyword highlighter">in</span> row)
                    found.Add(state);
            List&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; deadEnds = <span class="keyword highlighter">new</span>();
            foreach (<span class="_custom-word_ highlighter">STATE</span> state <span class="keyword highlighter">in</span> stateDictionary.Keys)
                <span class="keyword highlighter">if</span> (!found.Contains(state))
                    deadEnds.Add(state);
            <span class="keyword highlighter">return</span> deadEnds.ToArray();
        } <span class="comment text highlighter">//FindDeadEnds</span>

        <span class="comment text highlighter">// Find all states not visited along any of the paths between start and finish states</span>
        <span class="keyword highlighter">public</span> (<span class="_custom-word_ highlighter">STATE</span>[][] allPaths, <span class="_custom-word_ highlighter">STATE</span>[] deadEnds) FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish) {
            <span class="_custom-word_ highlighter">STATE</span>[][] allPaths = Labyrinth(start, finish, shortest: <span class="keyword highlighter">false</span>);
            <span class="keyword highlighter">return</span> (allPaths, FindDeadEnds(start, allPaths));
        } <span class="comment text highlighter">//FindDeadEnds</span>

        <span class="keyword highlighter">public</span> (int numberOfPaths, int longestPathLength, <span class="_custom-word_ highlighter">STATE</span>[][] longestPaths) LongestPaths { <span class="comment text highlighter">//NP-hard</span>
            get {
                List&lt;<span class="_custom-word_ highlighter">STATE</span>[]&gt; longestPaths = <span class="keyword highlighter">new</span>();
                int max = -<span class="literal numeric highlighter">1</span>;
                int pathCount = <span class="literal numeric highlighter">0</span>;
                foreach (<span class="keyword highlighter">var</span> startValue <span class="keyword highlighter">in</span> stateDictionary.Values)
                    foreach (<span class="keyword highlighter">var</span> finishValue <span class="keyword highlighter">in</span> stateDictionary.Values) {
                        <span class="_custom-word_ highlighter">STATE</span>[][] solution = Labyrinth(startValue.UnderlyingMember, finishValue.UnderlyingMember);
                        pathCount += solution.Length;
                        foreach (<span class="_custom-word_ highlighter">STATE</span>[] item <span class="keyword highlighter">in</span> solution) {
                            <span class="keyword highlighter">if</span> (item.Length &gt;= max) {
                                <span class="keyword highlighter">if</span> (item.Length &gt; max)
                                    longestPaths.Clear();
                                longestPaths.Add(item);
                                max = item.Length;
                            } <span class="comment text highlighter">//if</span>
                        } <span class="comment text highlighter">// inner loop</span>
                    } <span class="comment text highlighter">//outer loop</span>
                <span class="keyword highlighter">return</span> (pathCount, max, longestPaths.ToArray());
            } <span class="comment text highlighter">//get LongestPaths</span>
        } <span class="comment text highlighter">//LongestPaths</span>

        <span class="keyword highlighter">public</span> (int maximumNumberOfPaths, (<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish)[] pairsAtMax) MaximumPaths { <span class="comment text highlighter">//NP-hard</span>
            get {
                int max = <span class="literal numeric highlighter">0</span>;
                List&lt;(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish)&gt; pairList = <span class="keyword highlighter">new</span>();
                foreach (<span class="keyword highlighter">var</span> startValue <span class="keyword highlighter">in</span> stateDictionary.Values)
                    foreach (<span class="keyword highlighter">var</span> finishValue <span class="keyword highlighter">in</span> stateDictionary.Values) {
                        <span class="_custom-word_ highlighter">STATE</span>[][] solution = Labyrinth(startValue.UnderlyingMember, finishValue.UnderlyingMember);
                        <span class="keyword highlighter">if</span> (solution.Length &gt;= max) {
                            <span class="keyword highlighter">if</span> (solution.Length &gt; max)
                                pairList.Clear();
                            max = solution.Length;
                            pairList.Add((startValue.UnderlyingMember, finishValue.UnderlyingMember));
                        } <span class="comment text highlighter">//if</span>
                    } <span class="comment text highlighter">//outer loop</span>
                <span class="keyword highlighter">return</span> (max, pairList.ToArray());
            } <span class="comment text highlighter">//get LongestNumberOfPaths </span>
        } <span class="comment text highlighter">//LongestNumberOfPaths</span>

        #endregion API

        #region implementation

        <span class="keyword highlighter">class</span> Digest {
            internal <span class="keyword highlighter">void</span> BuildFollowingStates() {
                <span class="keyword highlighter">if</span> (populated) <span class="keyword highlighter">return</span>;
                populated = <span class="keyword highlighter">true</span>;
                foreach (<span class="keyword highlighter">var</span> (key, value) <span class="keyword highlighter">in</span> owner.stateGraph)
                    <span class="keyword highlighter">if</span> (value.IsValid)
                        Update(key);
            } <span class="comment text highlighter">//BuildFollowingStates</span>
            internal <span class="keyword highlighter">void</span> Update(StateGraphKey key) {
                <span class="keyword highlighter">if</span> (!populated) <span class="keyword highlighter">return</span>;
                key.StartState.digest.followingStates.Add(key.FinishState);
                <span class="keyword highlighter">if</span> (key.IsUndirected)
                    key.FinishState.digest.followingStates.Add(key.StartState);
            } <span class="comment text highlighter">//Update</span>
            internal Digest(<span class="_custom-word_ highlighter">StateMachine</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; owner) { <span class="keyword highlighter">this</span>.owner = owner; }
            readonly <span class="_custom-word_ highlighter">StateMachine</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; owner;
            bool populated;
        } <span class="comment text highlighter">//class Digest</span>

        State FindState(<span class="_custom-word_ highlighter">STATE</span> value) {
            <span class="keyword highlighter">if</span> (stateDictionary.TryGetValue(value, out State state))
                <span class="keyword highlighter">return</span> state;
            <span class="keyword highlighter">else</span>
                <span class="keyword highlighter">throw</span> <span class="keyword highlighter">new</span> <span class="literal keyword highlighter">InvalidStateException</span>(value);
        } <span class="comment text highlighter">//FindState</span>

        <span class="keyword highlighter">static</span> bool IsValid(StateGraphValue value) =&gt; value.ValidAction != <span class="keyword highlighter">null</span>;

        <span class="keyword highlighter">static</span> (bool IsValid, string ValidityComment) IsTransitionValid(StateGraphValue value, <span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState) {
            <span class="keyword highlighter">if</span> (!IsValid(value) &amp;&amp; value.InvalidAction != <span class="keyword highlighter">null</span>) {
                <span class="keyword highlighter">return</span> (<span class="keyword highlighter">false</span>, value.InvalidAction(startState, finishState));
            } <span class="comment text highlighter">//if</span>
            <span class="keyword highlighter">return</span> (<span class="keyword highlighter">true</span>, DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.TransitionIsValid(startState, finishState));
        } <span class="comment text highlighter">//IsTransitionValid</span>

        readonly Dictionary&lt;<span class="_custom-word_ highlighter">STATE</span>, State&gt; stateDictionary = <span class="keyword highlighter">new</span>();
        readonly Dictionary&lt;StateGraphKey, StateGraphValue&gt; stateGraph = <span class="keyword highlighter">new</span>();
        readonly Digest digest;
        readonly <span class="_custom-word_ highlighter">STATE</span> initialState;

        <span class="keyword highlighter">class</span> StateMachineGraphPopulationException : System.ApplicationException {
            internal StateMachineGraphPopulationException(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState)
                : base(DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.StateMachineGraphPopulationExceptionMessage(startState, finishState)) { }
        } <span class="comment text highlighter">//class StateMachineGraphPopulationException</span>

        <span class="keyword highlighter">class</span> InvalidStateException : System.ApplicationException {
            internal InvalidStateException(<span class="_custom-word_ highlighter">STATE</span> state)
                : base(DefinitionSet&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;.InvalidStateExceptionMessage(state)) { }
        } <span class="comment text highlighter">//class InvalidStateException</span>

        <span class="keyword highlighter">class</span> State {
            internal State(string name, <span class="_custom-word_ highlighter">STATE</span> underlyingMember) {
                Name = name;
                UnderlyingMember = underlyingMember;
            } <span class="comment text highlighter">//State</span>
            internal string Name { get; init; }
            internal <span class="_custom-word_ highlighter">STATE</span> UnderlyingMember { get; init; }
            internal (bool isVisited, List&lt;State&gt; followingStates) digest = (<span class="keyword highlighter">false</span>, <span class="keyword highlighter">new</span>());
        } <span class="comment text highlighter">//class State</span>

        <span class="keyword highlighter">class</span> StateGraphKey {
            internal StateGraphKey(State start, State finish, bool undirected = <span class="keyword highlighter">false</span>) {
                StartState = start; FinishState = finish;
                IsUndirected = undirected;
            }
            <span class="keyword highlighter">public</span> override int GetHashCode() { <span class="comment text highlighter">// important!</span>
                <span class="keyword highlighter">return</span> StartState.Name.GetHashCode()
                    ^ FinishState.Name.GetHashCode();
            }
            <span class="keyword highlighter">public</span> override bool Equals(object @object) { <span class="comment text highlighter">// important!</span>
                <span class="keyword highlighter">if</span> (@object == <span class="keyword highlighter">null</span>) <span class="keyword highlighter">return</span> <span class="keyword highlighter">false</span>;
                <span class="keyword highlighter">if</span> (@object is not StateGraphKey objectStateGraphKey) <span class="keyword highlighter">return</span> <span class="keyword highlighter">false</span>;
                bool nameMatch = (objectStateGraphKey.StartState.Name == StartState.Name
                    &amp;&amp; objectStateGraphKey.FinishState.Name == FinishState.Name);
                <span class="keyword highlighter">return</span> IsUndirected
                    ? nameMatch || 
                        (objectStateGraphKey.StartState.Name == FinishState.Name
                        &amp;&amp; objectStateGraphKey.FinishState.Name == StartState.Name)
                    : nameMatch;
            } <span class="comment text highlighter">//Equals</span>
            internal State StartState { get; init; }
            internal State FinishState { get; init; }
            internal bool IsUndirected { get; init; }
        } <span class="comment text highlighter">//class StateGraphKey</span>

        <span class="keyword highlighter">class</span> StateGraphValue {
            internal StateGraphValue(bool isValid, <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; valid, <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; invalid) {
                IsValid = isValid;
                ValidAction = valid; InvalidAction = invalid;
            }
            internal bool IsValid { get; init; }
            internal <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; ValidAction { get; init; }
            internal <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; InvalidAction { get; init; }
        } <span class="comment text highlighter">//class StateGraphValue</span>

        #endregion implementation

    } <span class="comment text highlighter">//class StateMachine</span>

}
~~~
-->


# Examples

## Room Door Example

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestDoor)

## Non-Enumeration Example

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/Test.Non-Emumeration)

## Grid Example

This example demostrates that NP-hard problems can be pretty hard even for the 24 states. The states in this
example form a 6x4 grid with permitted undirected transition between every neighboring cells.
The example demonstrates the calculation of the longest path between two states and maximum number of possible paths.

Maximum number of paths between a pair of states is 5493
Total number of paths: 1603536, longest path length: 23.

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestGrid)

## Zoo Example

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestZoo)

<script src="https://SAKryukov.github.io/publications/code/source-code-decorator.js"></script>