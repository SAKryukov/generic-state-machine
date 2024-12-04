# Generic state machine

Generic State Machine can be instantiated with any enumeration type representing the set of states.

~~~
enum RoomDoorState { Locked, Closed, Opened, OpenedInside, ClosedInside, LockedInside };
StateMachine<RoomDoorState> stateMachine = new();
~~~

Then the transition graph can be populated:

~~~
stateMachine.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside, (starting, ending) => {
    Console.WriteLine("Locking the door inside");
});
//...
stateMachine.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside, (starting, ending) =>
    $"You cannot get in through the locked door"));
~~~

Note that both valid and some invalid transitions can be defined. The purpose of the invalid transition is to provide some information on why the transition is not permitted.
Both valid and invalid transitions can come with optional handlers accepting two arguments, the enumeration values representing starting and ending states. In the case of a valid transition, the handler provides a side effect of transition.
For example, in the hardware automation applications, it can operate the hardware. In the case of an invalid transition, the handler returns a comment string explaining why the transition is not permitted.

#### Constructor:

~~~
public StateMachine(STATE initialState = default);
~~~

#### Public methods:

~~~
void ResetState(); // jump to the initial state, ignoring the transition graph
void AddValidStateTransition(
    STATE startingState, STATE endingState,
    StateTransitionAction<STATE> action, bool directed = true);
void AddValidStateTransitionChain(
   StateTransitionAction<STATE> action, bool directed, params STATE[] chain);
void AddInvalidStateTransition(
    STATE startingState, STATE endingState,
    InvalidStateTransitionAction<STATE> action);
(bool IsValid, string validityComment) IsTransitionValid(STATE startingState, STATE endingState);
(bool success, string validityComment) TryTransitionTo(STATE state);
(bool success, string invalidTransitionReason) TryTransitionTo(STATE state);
// this method finds all permitted paths between two states:
public STATE[][] Labyrinth(STATE start, STATE finish, bool shortest = false); 
~~~

#### Public properties:

~~~
STATE CurrentState { get; private set; }

// NP-hard:
(int numberOfPaths, int longestPathLength, STATE[][] longestPaths) LongestPaths;
(int longestNumberOfPaths, STATE start, STATE finish) LongestNumberOfPaths;
~~~

Here, the type `STATE` is a generic parameter of the class `StateMachine<STATE>`. It can be any enumeration type, or, in principle, any type having some static public fields.
Every such field is determined through reflection and interpreted as a state, and its reflected name is interpreted as a state name.
