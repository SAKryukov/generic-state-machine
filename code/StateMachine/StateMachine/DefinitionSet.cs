/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
    Answering to:
    https://stackoverflow.com/questions/79240035/how-to-correctly-implement-of-state-machine-pattern
*/

namespace StateMachines {

    static class DefinitionSet<STATE> {
        internal static string ExceptionMessage(STATE stargingState, STATE endingState) =>
            $"The transition between {stargingState} and {endingState} is already added to the State Machine transition graph";
    } //DefinitionSet

}