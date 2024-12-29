# Generic state machine

The classes [TransitionSystem](https://SAKryukov.GitHub.io/generic-state-machine#heading-class-transitionsystem),
[Acceptor](https://SAKryukov.GitHub.io/generic-state-machine#heading-class-acceptor),
and [Transducer](https://SAKryukov.GitHub.io/generic-state-machine#heading-class-transducer)
can be instantiated with any enumeration types representing the sets of states and the input and output alphabets. For example:

~~~
enum RoomDoorState { Locked, Closed, Opened,
                     OpenedInside, ClosedInside, LockedInside };
TransitionSystem<RoomDoorState> transitionSystem = new();
~~~

Then the transition graph can be populated:

~~~
transitionSystem.AddValidStateTransition(RoomDoorState.ClosedInside, RoomDoorState.LockedInside,
    (start, finish) => {
        // command hardware actuator to lock the door
});
//...
transitionSystem.AddInvalidStateTransition(RoomDoorState.Locked, RoomDoorState.LockedInside,
    (start, finish) =>
        $"You cannot get in through the locked door"));
~~~

Note that both valid and some invalid transitions can be defined. The purpose of the invalid transition is to provide some information on why the transition is not permitted.
Both valid and invalid transitions can come with optional handlers accepting two arguments, the enumeration values representing starting and finishing states. In the case of a valid transition, the handler provides a side effect of transition.
For example, in the hardware automation applications, it can operate the hardware. In the case of an invalid transition, the handler returns a comment string explaining why the transition is not permitted.

Please see the comprehensive [API documentation](https://SAKryukov.GitHub.io/generic-state-machine).

For the rationale, general ideas, and mathematical aspects, please see the
[original publication](https://sakryukov.github.io/publications/2024-12-29.Enumeration-Based-Generic-State-Machine.html).

