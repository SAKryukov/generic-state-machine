/*
    Generic State Machines

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using System.Collections.Generic;

    public delegate OUTPUT MooreMachineOutputAction<STATE, INPUT, OUTPUT> (STATE state);
    public delegate OUTPUT MealyMachineOutputAction<STATE, INPUT, OUTPUT> (STATE state, INPUT input);
    public delegate OUTPUT OutputAction<STATE, INPUT, OUTPUT> (STATE state, INPUT input, OUTPUT output);

    public class Transducer<STATE, INPUT, OUTPUT>  : Acceptor<STATE, INPUT> {

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
        public void AddOutputFunctionPart(
            INPUT input, STATE state,
            OutputAction<STATE, INPUT, OUTPUT> handler)
        {
            outputFunction.Add(
                GetStateMachineFunctionKey(input, state),
                new OutputFunctionValue() {
                    OutputAction = handler,
                    MachineType = MachineType.Comprehensive });
        } //AddOutputFunctionPart

        StateMachineFunctionKey GetStateMachineFunctionKey(INPUT input, STATE state) {
            StateMachineFunctionKey key = new (FindInput(input), FindState(state));
            if (outputFunction.ContainsKey(key))
                throw new StateTransitionFunctionPopulationException(input, state);
            return key;
        } //GetStateMachineFunctionKey

        enum MachineType { Moore, Mealy, Comprehensive }
        class OutputFunctionValue {
            internal MooreMachineOutputAction<STATE, INPUT, OUTPUT> MooreMachineOutputAction { get; init;}
            internal MealyMachineOutputAction<STATE, INPUT, OUTPUT> MealyMachineOutputAction { get; init; }
            internal OutputAction<STATE, INPUT, OUTPUT> OutputAction { get; init; }
            internal MachineType MachineType { get; init; }
        } //OutputFunctionValue

        public override (bool success, string transitionComment) Signal(INPUT input) {
            (bool baseSuccess, string baseTransitionComment) = base.Signal(input);
            return (baseSuccess, baseTransitionComment);
        } //Signal

        class StateTransitionFunctionPopulationException : System.ApplicationException {
            internal StateTransitionFunctionPopulationException(INPUT input, STATE state)
                : base(DefinitionSet<STATE, INPUT, bool>.OutputFunctionPopulationExceptionMessage(input, state)) { }
        } //class StateTransitionFunctionPopulationException


        readonly Dictionary<OUTPUT, Output> outputDictionary = new();
        readonly Dictionary<StateMachineFunctionKey, OutputFunctionValue> outputFunction = new();

        internal class Output : Element<OUTPUT> {
            internal Output(string name, OUTPUT output) : base(name, output) {}
        } //class Input

    } //class Transducer

}
