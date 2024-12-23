/*
    Generic State Machines

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using System.Collections.Generic;

    public delegate STATE AcceptorTransitionAction<STATE, INPUT> (STATE state, INPUT input);

    public class Acceptor<STATE, INPUT> : TransitionSystem<STATE> {

        public Acceptor(STATE initialState = default) : base(initialState = default) {
            Traverse<INPUT>((name, input) => {
                inputDictionary.Add(input, new Input(name, input));
            });
        } //Acceptor

        public void AddStateTransitionFunctionPart(
            INPUT input, STATE state,
            AcceptorTransitionAction<STATE, INPUT> handler)
        {
            var key = new StateMachineFunctionKey(FindInput(input), FindState(state));
            if (stateTransitionFunction.ContainsKey(key))
                throw new StateTransitionFunctionPopulationException(input, state);
            stateTransitionFunction.Add(key, new StateTransitionFunctionValue(handler));
        } //AddStateTransitionFunctionPart

        public string TransitionSignal(INPUT input) {
            StateMachineFunctionKey key = new(FindInput(input), FindState(CurrentState));
            if (stateTransitionFunction.TryGetValue(key, out StateTransitionFunctionValue part))
                if (part.Handler != null)
                    return TransitionTo(part.Handler(CurrentState, input));
            return
                DefinitionSet<STATE, INPUT, bool>.UndefinedStateTransitionFunction(CurrentState, input);
        } //TransitionSignal

        class StateTransitionFunctionPopulationException : System.ApplicationException {
            internal StateTransitionFunctionPopulationException(INPUT input, STATE state)
                : base(DefinitionSet<STATE, INPUT, bool>.StateTransitionFunctionPopulationExceptionMessage(input, state)) { }
        } //class StateTransitionFunctionPopulationException

        internal class StateMachineFunctionKey {
            internal StateMachineFunctionKey(Input input, State state) { State = state; Input = input;}
            internal Input Input { get; init; }
            internal State State { get; init; }
            public override bool Equals(object obj) {
                StateMachineFunctionKey keyObject = (StateMachineFunctionKey)obj;
                return Input.UnderlyingMember.Equals(keyObject.Input.UnderlyingMember)
                    && State.UnderlyingMember.Equals(keyObject.State.UnderlyingMember);   
            } //Equals
            public override int GetHashCode() =>
                Input.Name.GetHashCode() ^ State.Name.GetHashCode();
        } //StateMachineFunctionKey

        class StateTransitionFunctionValue {
            internal StateTransitionFunctionValue(AcceptorTransitionAction<STATE, INPUT> handler) { Handler = handler; }
            internal AcceptorTransitionAction<STATE, INPUT> Handler { get; init; }
        } //StateTransitionFunctionValue

        private protected Input FindInput(INPUT key) {
            if (inputDictionary.TryGetValue(key, out Input input))
                return input;
            else
                throw new InvalidInputException(key);
        } //FindState

        internal class Input : Element<INPUT> {
            internal Input(string name, INPUT input) : base(name, input) {}
        } //class Input

        internal class InvalidInputException : System.ApplicationException {
            internal InvalidInputException(INPUT input)
                : base(DefinitionSet<bool, INPUT, bool>.InvalidInputExceptionMessage(input)) { }
        } //class InvalidStateException

        private protected readonly Dictionary<INPUT, Input> inputDictionary = new();
        readonly Dictionary<StateMachineFunctionKey, StateTransitionFunctionValue> stateTransitionFunction = new();

    } //class Acceptor

}
