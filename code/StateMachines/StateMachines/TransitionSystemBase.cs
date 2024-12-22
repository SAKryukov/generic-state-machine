/*
    Generic State Machine

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NotAStateAttribute : System.Attribute {}

    public class TransitionSystemBase<STATE> {

        public TransitionSystemBase(STATE initialState = default) {
            FieldInfo[] fields = typeof(STATE).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                if (field.GetCustomAttributes(typeof(NotAStateAttribute), inherit: false).Length > 0) continue;
                STATE value = (STATE)field.GetValue(null);
                State state = new(field.Name, value);
                stateDictionary.Add(value, state);
                if (value.Equals(initialState))
                    CurrentState = value;
            } //loop
            this.initialState = CurrentState;
        } //TransitionSystemBase

        private protected static bool Initialize<ELEMENT>(
                Dictionary<ELEMENT, Element<ELEMENT>> dictionary,
                ELEMENT initialElement = default)
        {
            FieldInfo[] fields = typeof(STATE).GetFields(BindingFlags.Static | BindingFlags.Public);
            bool found = false;
            foreach (var field in fields) {
                if (field.GetCustomAttributes(typeof(NotAStateAttribute), inherit: false).Length > 0) continue;
                ELEMENT element = (ELEMENT)field.GetValue(null);
                dictionary.Add(element, new Element<ELEMENT>(field.Name, element));
                if (element.Equals(initialElement))
                    found = true;
            } //loop
            return found;
        } //Initialize
        internal class Element<ELEMENT> {
            internal Element(string name, ELEMENT element) {
                Name = name; UnderlyingMember = element;
            } //Element
            internal string Name { get; init; }
            internal ELEMENT UnderlyingMember { get; init; }
        } //Element

        internal class State : Element<STATE> {
            internal State(string name, STATE underlyingMember) : base(name, underlyingMember) {}
            internal (bool isVisited, List<State> followingStates) digest = (false, new());
        } //class State

        public STATE CurrentState { get; private protected set; }

        public STATE ResetState() => // unconditional jump to initial state, ignoring the transition graph
            CurrentState = initialState;

        readonly STATE initialState;
        private protected readonly Dictionary<STATE, State> stateDictionary = new();

    } //class TransitionSystemBase

}
