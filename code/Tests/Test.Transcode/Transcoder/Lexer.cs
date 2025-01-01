/*
    Generic Transducer, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/
namespace StateMachines {
    using System.Text.RegularExpressions;

    enum Signal { Key, ValueStart, Value, StringValueStart, StringValue, Object, Array, Next, LineEnd, End }

    class Lexer {

        readonly Regex valueTypeFinder = new(DefinitionSet.Regex.valueTypeFinder);
        readonly Regex keyFinder = new(DefinitionSet.Regex.keyFinder);
        readonly Regex stringValueFinder = new(DefinitionSet.Regex.stringValueFinder);
        readonly Regex valueFinder = new(DefinitionSet.Regex.valueFinder);

        internal (Signal signal, string value) FindObjectStart(string line, ref int position) {
            Match match = valueTypeFinder.Match(line[position..]);
            if (match.Captures.Count < 1)
                return (Signal.ValueStart, null);
            string text = match.Captures[0].Value.Trim();
            position += text.Length;
            return text switch {
                DefinitionSet.KeySet.objectStart => (Signal.Object, null),
                DefinitionSet.KeySet.arrayStart => (Signal.Array, null),
                DefinitionSet.KeySet.stringValueStart => (Signal.StringValueStart, null),
                _ => (Signal.ValueStart, null),
            };
        } //FindObjectStart

        internal string GetSimpleObject(string line, ref int position, bool isString) {
            Match match = isString 
                ? stringValueFinder.Match(line[position..])
                : valueFinder.Match(line[position..]);
            string text = match.Groups.Count > 1 ? match.Groups[1].Value : null;
            if (!string.IsNullOrEmpty(text))
                position += text.Length;
            return text;
        } //if

        internal string GetKey(string line, ref int position) {
            Match match = keyFinder.Match(line[position..]);
            return null;
        } //GetKey

    } //class Lexer;

}
