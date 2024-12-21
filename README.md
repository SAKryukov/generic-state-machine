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

Please see the comprehensive [API documentation](https://SAKryukov.GitHub.io/generic-state-machine).
