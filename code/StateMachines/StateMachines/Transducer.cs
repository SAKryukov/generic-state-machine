/*
    Generic State Machines

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public delegate OUTPUT MooreMachineOutputAction<STATE, INPUT, OUTPUT> (STATE state);
    public delegate OUTPUT MealyMachineOutputAction<STATE, INPUT, OUTPUT> (STATE state, INPUT input);

    public class Transducer<STATE, INPUT, OUTPUT> : Acceptor<STATE, INPUT> {

        #region API

        public Transducer(STATE initialState = default) : base(initialState) {
            Traverse<OUTPUT>((name, output) => {
                outputDictionary.Add(output, new Output(name, output));
            });
        } //Transducer

        public void AddOutputFunctionPart(
            INPUT input, STATE state,
            MooreMachineOutputAction<STATE, INPUT, OUTPUT> handler)
        {
            outputFunction.Add(
                GetStateMachineFunctionKey(input, state),
                new OutputFunctionValue() {
                    MooreMachineOutputAction = handler,
                    MachineType = MachineType.Moore });
        } //AddOutputFunctionPart
        public void AddOutputFunctionPart(

            INPUT input, STATE state,
            MealyMachineOutputAction<STATE, INPUT, OUTPUT> handler)
        {
            outputFunction.Add(
                GetStateMachineFunctionKey(input, state),
                new OutputFunctionValue() {
                    MealyMachineOutputAction = handler,
                    MachineType = MachineType.Mealy });
        } //AddOutputFunctionPart

        public record SignalResult(
            TransitionSignalResult TransitionResult,
            OUTPUT Output, bool OutputSuccess = true, string OutputComment = null);

        public SignalResult Signal(INPUT input)
        {
            TransitionSignalResult transitionResult = TransitionSignal(input);
            StateMachineFunctionKey key = new(FindInput(input), FindState(CurrentState));
            if (outputFunction.TryGetValue(key, out OutputFunctionValue outputFunctionValue)) {
                OUTPUT output = default;
                bool handlerFound = false;
                switch (outputFunctionValue.MachineType) {
                    case MachineType.Moore:
                        if (outputFunctionValue.MooreMachineOutputAction != null)
                            output = outputFunctionValue.MooreMachineOutputAction(CurrentState);
                        handlerFound = true;
                        break;
                    case MachineType.Mealy:
                        if (outputFunctionValue.MealyMachineOutputAction != null)
                            output = outputFunctionValue.MealyMachineOutputAction(CurrentState, input);
                        handlerFound = true;
                        break;
                } //switch
                if (handlerFound)
                    return new SignalResult(transitionResult, output);
                else
                    return new SignalResult(transitionResult, output, false,
                        DefinitionSet<STATE, INPUT, bool>.UndefinedOutputFunction(CurrentState, input));
            } //if
            return new SignalResult(transitionResult, default, false,
                DefinitionSet<STATE, INPUT, bool>.UndefinedOutputFunction(CurrentState, input));
        } //Signal

        #endregion API

        #region implementation

        StateMachineFunctionKey GetStateMachineFunctionKey(INPUT input, STATE state) {
            StateMachineFunctionKey key = new (FindInput(input), FindState(state));
            if (outputFunction.ContainsKey(key))
                throw new StateTransitionFunctionPopulationException(input, state);
            return key;
        } //GetStateMachineFunctionKey

        enum MachineType { Moore, Mealy, }
        class OutputFunctionValue {
            internal MooreMachineOutputAction<STATE, INPUT, OUTPUT> MooreMachineOutputAction { get; init;}
            internal MealyMachineOutputAction<STATE, INPUT, OUTPUT> MealyMachineOutputAction { get; init; }
            internal MachineType MachineType { get; init; }
        } //OutputFunctionValue

        class StateTransitionFunctionPopulationException : System.ApplicationException {
            internal StateTransitionFunctionPopulationException(INPUT input, STATE state)
                : base(DefinitionSet<STATE, INPUT, bool>.OutputFunctionPopulationExceptionMessage(input, state)) { }
        } //class StateTransitionFunctionPopulationException

        readonly Dictionary<OUTPUT, Output> outputDictionary = new();
        readonly Dictionary<StateMachineFunctionKey, OutputFunctionValue> outputFunction = new();

        internal class Output : Element<OUTPUT> {
            internal Output(string name, OUTPUT output) : base(name, output) {}
        } //class Input

        #endregion implementation

    } //class Transducer

}
