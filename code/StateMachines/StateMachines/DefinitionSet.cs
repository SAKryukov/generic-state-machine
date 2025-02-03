/*
    Generic State Machines

    Copyright (C) 2024-2025 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {

    static class DefinitionSet<STATE, INPUT, OUTPUT> {

        internal static string GraphPopulationExceptionMessage(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is already added to the Transition System graph";

        internal static string StateTransitionFunctionPopulationExceptionMessage(INPUT input, STATE state) =>
            $"The state handler for the input {input} and the state {state} is already added to the State Machine's transition function";

        internal static string InputRegistryPopulationExceptionMessage(INPUT input, STATE state) =>
            $"The invalid input handler for the input {input} and the state {state} is already added to the State Machine's invalid input registry";

        internal static string OutputFunctionPopulationExceptionMessage(INPUT input, STATE state) =>
            $"The state handler for the input {input} and the state {state} is already added to the State Machine's ouput function";

        internal static string InvalidStateExceptionMessage(STATE state) =>
            $"The value {state} is not a part of the State Machine state set";

        internal static string ValidStateExceptionMessage(string className) =>
            $"For the class {className}, the transition graph is ignored";

        internal static string InvalidInputExceptionMessage(INPUT input) =>
            $"The value {input} is not a part of the State Machine input alphabet";

        internal static string TransitionNotDefined(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is not permitted";

        internal static string TransitionIsValid(STATE startState, STATE finishState) =>
            $"The transition between {startState} and {finishState} is permitted";

        internal static string TransitionToTheSameState(STATE state) =>
            $"Attempted transition to the same state: {state}";

        internal static string TransitionSuccess(STATE state) =>
            $"{state}";

        internal static string UndefinedStateTransitionFunction(STATE state, INPUT input) =>
            $"State transition function is not defined for the inpit {input} and current state {state}";

        internal static string UndefinedOutputFunction(STATE state, INPUT input) =>
            $"Output function is not defined for the inpit {input} and current state {state}";

    } //DefinitionSet

}
