/*
    Generic State Machines

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/

namespace StateMachines {
    using BindingFlags = System.Reflection.BindingFlags;
    using FieldInfo = System.Reflection.FieldInfo;

    public abstract class ExcludeAttribute : System.Attribute {}

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NotAStateAttribute : ExcludeAttribute {}
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NotAnAlphabetElementAttribute : ExcludeAttribute {}

    public abstract class TransitionSystemBase<STATE> {

        internal protected delegate void TraverseHandler<ELEMENT>(string name, ELEMENT element);
        internal protected void Traverse<ELEMENT>(TraverseHandler<ELEMENT> handler) {
            FieldInfo[] fields = typeof(ELEMENT).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                if (field.GetCustomAttributes(typeof(ExcludeAttribute), inherit: false).Length > 0)
                    continue;
                ELEMENT element = (ELEMENT)field.GetValue(null);
                handler(field.Name, element);
            } //loop
        } //Traverse

        internal class Element<ELEMENT> {
            internal Element(string name, ELEMENT element) {
                Name = name; UnderlyingMember = element;
            } //Element
            internal string Name { get; init; }
            internal ELEMENT UnderlyingMember { get; init; }
        } //Element

    } //class TransitionSystemBase

}
