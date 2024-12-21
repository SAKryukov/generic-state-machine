Generic State Machine{title}

The class [`StateMachine`](#heading-class-statemachine) provides a way to create a [finite-state machine](https://en.wikipedia.org/wiki/Finite-state_machine) based on any enumeration type representing a set of states. For the class instance, a state transition graph can be created. The instance can walk between states using permitted transitions, and the optional delegate representing the side effect of a transition can be called. An attempt to perform an invalid transition can provide optional information, explaining why the transition is invalid. The class also implements several graph algorithms, including [NP-hard](https://en.wikipedia.org/wiki/NP-hardness) ones, such as [finding the longest possible paths](#heading-longestpaths).

@toc

# StateMachines Namespace

## Delegate ValidStateTransitionAction

An instance of the delegate provides a way to define a side effect of a valid transition between two states, `startState`, and `finishState`. For example, in the hardware automation applications, it can operate the hardware.

~~~{lang=C#}{id=api-valid-state-transition-action}
<span class="keyword highlighter">public</span> delegate <span class="keyword highlighter">void</span> <span class="_custom-word_ highlighter">ValidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

The delegate instance is used in the [StateMachine.AddValidStateTransition](#heading-addvalidstatetransition) and [StateMachine.AddValidStateTransitionChain](#heading-addinvalidstatetransition) call.

## Delegate InvalidStateTransitionAction

An instance of the delegate provides the optional information on an invalid transition between two states, `startState` and `finishState`. Its return `string` value can be used to provide an explanation of why an attempted transition between the states is considered invalid.

~~~{lang=C#}{id=api-invalid-state-transition-action}
<span class="keyword highlighter">public</span> delegate string <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt;(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

The delegate instance is used in the [StateMachine.AddInvalidStateTransition](#heading-addinvalidstatetransition) call.

## NotAState Attribute

This attribute can be used to mark some enumeration-type members to exclude them from the set of states of a state machine. It can be useful to create members irrelevant to the state machine behavior but used for some calculations. For example, such a member can be a bitwise `OR` combination of several states.

~~~{lang=C#}{id=api-not-a-state}
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

An attempt to perform a state transition to a `NotAState` enumeration value using [`TryTransitionTo`](#heading-trytransitionto) will throw an exception.

## Class StateMachine

~~~{lang=C#}{id=api-state-machine}
<span class="keyword highlighter">public</span> <span class="keyword highlighter">class</span> <span class="_custom-word_ highlighter">StateMachine</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; {/* &hellip; */}
~~~

The class can be instantiated with the generic type parameter `STATE`. Typically, it should be any enumeration type with its enumeration members representing states. However, it is not a strict rule. In principle, any type with public static fields can be used for the `STATE` type. In this case, the public static fields will represent the state machine states. Please see [the example](#heading-non-enumeration-example) illustrating the use of a non-enumeration type `STATE`.

### Public Constructor

Creates an instance of `StateMachine`.

Parameter: `STATE initialState = default`. Defines the initial state of the state machine. See also [`ResetState`](#heading-resetstate).

Note that the use of a non-default initial state can be critically important when a default value for the `STATE` type is not a state. It can happen if this default value is excluded using the [[`NotAState`](#heading-notastate-attribute)] attribute. Another case of a non-state value is demonstrated by the [non-enumeration example](#heading-non-enumeration-example).

~~~{lang=C#}{id=api-constructor}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">StateMachine</span>(<span class="_custom-word_ highlighter">STATE</span> initialState = <span class="keyword highlighter">default</span>);
~~~

### Public Methods

#### ResetState

Unconditionally jumps to the *initial state* defined by the [constructor](#heading-public-constructor). No [delegate instances](#heading-delegate-validstatetransitionaction) are called even if there is a valid transition between the [current state](#heading-currentstate) and the initial state.

~~~{lang=C#}{id=api-reset-state}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> ResetState();
~~~

#### AddValidStateTransition

Adds an edge to the state machine's *transition graph* between the states `startState` and `finishState`. If the parameter `undirected` is `true`, it makes both transitions valid, from `startState` to `finishState`, and from `finishState` to `startState`.

Optionally, a delegate instance of the type [`StateTransitionAction`](#heading-delegate-validstatetransitionaction) is specified. In this case (when the delegate instance is not `null`), the delegate's method will be called on each call to [`TryTransitionTo`](#heading-trytransitionto) between corresponding states.

~~~{lang=C#}{id=api-add-valid-state-transition}
<span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddValidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>);
~~~

#### AddValidStateTransitionChain

Adds a transition chain to the state machine's *transition graph*. Note that if the parameter `undirected` is `true`, it makes all the graph edges between the adjacent pairs of states *undirected*, that is, the transitions in both directions become valid.

Optionally, a delegate instance of the type [`StateTransitionAction`](#heading-delegate-validstatetransitionaction) is specified. In this case (when the delegate instance is not `null`), the same delegate's method will be called on each call to [`TryTransitionTo`](#heading-trytransitionto) between corresponding states in the chain.

~~~{lang=C#}{id=api-add-valid-state-transition-chain}
<span class="keyword highlighter">public</span> int AddValidStateTransitionChain(<span class="_custom-word_ highlighter">StateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action, bool undirected = <span class="keyword highlighter">false</span>, params <span class="_custom-word_ highlighter">STATE</span>[] chain)
~~~

#### AddInvalidStateTransition

Defines the information in an invalid state transition between the states `startState` and `finishState`. This information is used in the return of the call to [IsTransitionValid](#heading-istransitionvalid) and [TryTransitionTo](#heading-trytransitionto). See also [InvalidStateTransitionAction](#heading-delegate-invalidstatetransitionaction).

~~~{lang=C#}{id=api-add-invalid-state-transition}
<span class="keyword highlighter">public</span> <span class="keyword highlighter">void</span> AddInvalidStateTransition(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState, <span class="_custom-word_ highlighter">InvalidStateTransitionAction</span>&lt;<span class="_custom-word_ highlighter">STATE</span>&gt; action)
~~~

#### IsTransitionValid

Checks if a transition between the state `startState` and `finishState` is valid. In addition to the status `isValid`, returns `validityComment` that can explain why the transition is invalid. This information is optional, defined using [AddInvalidStateTransition](#heading-addinvalidstatetransition).

~~~{lang=C#}{id=api-is-transition-valid}
<span class="keyword highlighter">public</span> (bool isValid, string validityComment) IsTransitionValid(<span class="_custom-word_ highlighter">STATE</span> startState, <span class="_custom-word_ highlighter">STATE</span> finishState);
~~~

#### TryTransitionTo

Tries to perform the transition from the [current state](#heading-currentstate) to the state `state`. Returns the success status of the transition and the comment on the transition validity `validityComment`. If the transition is invalid, `validityComment` can provide some information on the reason. This information is optional, see [AddInvalidStateTransition](#heading-addinvalidstatetransition).

If the transition is successful, the [current state](#heading-currentstate) becomes `state`.

An attempt to perform the transition to the same state is considered valid. In this case, no delegate methods are called.

~~~{lang=C#}{id=api-try-transition-to}
<span class="keyword highlighter">public</span> (bool success, string validityComment) TryTransitionTo(<span class="_custom-word_ highlighter">STATE</span> state);
~~~

#### Labyrinth

Returns all the permitted paths between the states `start` and `finish`. If `shortest` is `true` the method still finds all paths, but then it determines the shortest path length and returns only the array of paths of the minimal length.

~~~{lang=C#}{id=api-labyrinth}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[][] Labyrinth(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish, bool shortest = <span class="keyword highlighter">false</span>);
~~~

#### FindDeadEnds

Find all "dead ends", the states not visited along any paths between the states `start` and `finish`. Returns `allPaths`, all permitted paths between `start` and `finish`, and `deadEnds`.

~~~{lang=C#}{id=api-find-dead-ends-2}
<span class="keyword highlighter">public</span> (<span class="_custom-word_ highlighter">STATE</span>[][] allPaths, <span class="_custom-word_ highlighter">STATE</span>[] deadEnds) FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish);
~~~

Another form of `FindDeadEnds` assumes that the parameter `allPaths` is the array of paths obtained through the prior call to [Labyrinth](#heading-labyrinth) between the state `start` and some other state `finish`. In this case, the `allPaths` array will contain all paths ending with the state `finish` passed as the second parameter to [Labyrinth](#heading-labyrinth).

~~~{lang=C#}{id=api-find-dead-ends}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span>[] FindDeadEnds(<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span>[][] allPaths);
~~~

### Public Properties

#### CurrentState

Returns the *current state* of the state machine. Before the very first transition of the instance, the current state is the one defined by the [constructor](#heading-public-constructor).

~~~{lang=C#}{id=api-current-state}
<span class="keyword highlighter">public</span> <span class="_custom-word_ highlighter">STATE</span> CurrentState;
~~~

#### LongestPaths

~~~{lang=C#}{id=api-longest-paths}
<span class="keyword highlighter">public</span> (int numberOfPaths, int longestPathLength, <span class="_custom-word_ highlighter">STATE</span>[][] longestPaths) LongestPaths; <span class="comment text highlighter">//NP-hard</span>
~~~

#### MaximumPaths

~~~{lang=C#}{id=api-maximum-paths}
<span class="keyword highlighter">public</span> (int maximumNumberOfPaths, (<span class="_custom-word_ highlighter">STATE</span> start, <span class="_custom-word_ highlighter">STATE</span> finish)[] pairsAtMax) MaximumPaths; <span class="comment text highlighter">//NP-hard</span>
~~~

## Notes

All paths returned by [`Labyrinth`](#heading-labyrinth), [`FindDeadEnds`](#heading-finddeadends), and [`LongestPaths`](#heading-longestpaths) are represented as arrays `STATE[]` or arrays of paths `STATE[][]`, each path represented as an array of `STATE`. In the array of states, the starting state of the path is not included, and the final state of the path is included.

In other words, given a permitted path between the [current state](#heading-currentstate) of a state machine and the other states in the array, we can perform the chain of transitions,
sequentially to all the states in the array, and all the transitions will be valid.

Example:
~~~ {lang=C#}
<span class="keyword highlighter">var</span> solution = stateMachine.Labyrinth(<span class="_custom-word_ highlighter">VisitorState</span>.Entry, <span class="_custom-word_ highlighter">VisitorState</span>.Exit);
<span class="keyword highlighter">if</span> (solution.Length &gt; <span class="literal numeric highlighter">0</span>)
    <span class="keyword highlighter">foreach</span> (<span class="keyword highlighter">var</span> state <span class="keyword highlighter">in</span> solution[<span class="literal numeric highlighter">0</span>]) <span class="comment highlighter">// or any other solution path</span>
        stateMachine.TryTransitionTo(state); <span class="comment highlighter">// always valid</span>
~~~

# Examples

## Room Door Example

Basic state machine example with 6 states forming a linear transition graph. Demonstrates valid transitions and attempts to perform an invalid transition.

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestDoor)
<br/>[Description](Example.Door.html)

## Non-Enumeration Example

This example shows the use of a non-enumerable type as a `StateMachine` generic parameter. The type `double` is used as a source of the state set.

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/Test.Non-Emumeration)
<br/>[Description](Example.Non-Emumeration.html)

## Grid Example

This example demonstrates that NP-hard problems can be pretty hard even for the 24 states. The states in this
example form a 6x4 grid with the permitted *undirected* transition between all neighboring cells.
The example demonstrates the calculation of the longest path between two states and the maximum number of possible paths.

Maximum number of paths between a pair of states is 5493.<br/>
Total number of paths: 1603536, longest path length: 23.

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestGrid)
<br/>[Description](Example.Grid.html)

## Zoo Example

The Zoo example represents the state machine representing the visitor location at the zoo. This example demonstrates all the `StateMachine` features.

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestZoo)
<br/>[Description](Example.Zoo.html)

# Compatibility and Build

The solution is compatible with any [.NET](https://en.wikipedia.org/wiki/.NET) version. For the earliest common denominator, I use v.&nbsp;5; by the moment of writing, I tested the versions from 5 to 9.

To change the target .NET version, edit the file ["Directory.Build.props"](https://github.com/SAKryukov/generic-state-machine/blob/main/code/Directory.Build.props), the property `<TargetFramework>`.
Also, the code can be used for some [.NET Framework](https://en.wikipedia.org/wiki/.NET_Framework) versions. However, it will require re-creation of project files, which is not hard to do.

The build does not require Visual Studio, Visual Studio Code, or any other IDE. It can be done using the commands

~~~
dotnet build -c Debug
dotnet build -c Release
~~~

With Visual Studio or Visual Studio Code, the code can be built in the usual ways. Besides, for Visual Studio Code, a [launch configuration "Build All"](https://github.com/SAKryukov/generic-state-machine/blob/main/code/.vscode/launch.json) and a [task "Build All"](https://github.com/SAKryukov/generic-state-machine/blob/main/code/.vscode/tasks.json) are provided.

This documentation is generated from the extended Markdown documentation using [Extensible Markdown](https://marketplace.visualstudio.com/items?itemName=sakryukov.extensible-markdown)
for Visual Studio Code.{.extensible-markdown}

<script src="https://SAKryukov.github.io/publications/code/source-code-decorator.js"></script>
