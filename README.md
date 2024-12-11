# Generic state machine

Generic State Machine can be instantiated with any enumeration type representing the set of states.

~~~
enum RoomDoorState { Locked, Closed, Opened,
                     OpenedInside, ClosedInside, LockedInside };
StateMachine<RoomDoorState> stateMachine = new();
~~~

Then the transition graph can be populated:

~~~
stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside,
    (start, finish) => {
        // command hardware actuator to lock the door
});
//...
stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside,
    (start, finish) =>
        $"You cannot get in through the locked door"));
~~~

Note that both valid and some invalid transitions can be defined. The purpose of the invalid transition is to provide some information on why the transition is not permitted.
Both valid and invalid transitions can come with optional handlers accepting two arguments, the enumeration values representing starting and finishing states. In the case of a valid transition, the handler provides a side effect of transition.
For example, in the hardware automation applications, it can operate the hardware. In the case of an invalid transition, the handler returns a comment string explaining why the transition is not permitted.

#### Constructor:

~~~
public StateMachine(STATE initialState = default);
~~~

#### Public methods:

~~~
void ResetState(); // jump to the initial state, ignoring the transition graph
void AddValidStateTransition(STATE startState, STATE finishState,
    StateTransitionAction<STATE> action, bool undirected = false);
void AddValidStateTransitionChain(
    StateTransitionAction<STATE> action, bool undirected = false, params STATE[] chain);
void AddInvalidStateTransition(STATE startState, STATE finishState,
    InvalidStateTransitionAction<STATE> action);
(bool isValid, string validityComment) IsTransitionValid(STATE startState, STATE finishState);
(bool success, string validityComment) TryTransitionTo(STATE state);

// Find all permitted paths between two states:
STATE[][] Labyrinth(STATE start, STATE finish, bool shortest = false); 

// Find all states not visited along any of the paths between start and finish states:
(STATE[][] allPaths, STATE[] deadEnds) FindDeadEnds(STATE start, STATE finish);
STATE[] FindDeadEnds(STATE start, STATE[][] allPaths);
~~~

#### Public properties:

~~~
STATE CurrentState { get; private set; }

// NP-hard:
(int numberOfPaths, int longestPathLength, STATE[][] longestPaths) LongestPaths;
(int maximumNumberOfPaths, (STATE start, STATE finish)[] pairsAtMax) MaximumPaths;
~~~

Here, the type `STATE` is a generic parameter of the class `StateMachine<STATE>`. It can be any enumeration type, or, in principle, any type having some static public fields.
Every such field is determined through reflection and interpreted as a state, and its reflected name is interpreted as a state name.