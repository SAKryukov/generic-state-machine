/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {

    static class DefinitionSet<STATE> {

        internal static string StateMachineGraphPopulationExceptionMessage(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is already added to the State Machine transition graph";

        internal static string InvalidStateExceptionMessage(STATE state) =>
            $"The value {state} is not a part of the State Machine state set";

        internal static string TransitionNotDefined(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is not permitted";

        internal static string TransitionIsValid(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is permitted";

        internal static string TransitionToTheSameState(STATE state) =>
            $"Attempted transition to the same state: {state}";

        internal static string TransitionSuccess(STATE state) =>
            $"{state}";

    } //DefinitionSet

}
