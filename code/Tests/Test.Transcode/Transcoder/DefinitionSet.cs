/*
    Generic Transducer, test

    Copyright (C) 2024 by Sergey A Kryukov
    https://www.SAKryukov.org
    https://github.com/SAKryukov
*/
namespace StateMachines {

    static class DefinitionSet {

        internal const string inputSample = "Transcoder/Sample.json";  

        internal static class Regex {
            internal const string valueTypeFinder = "(\\s*?)({|\\[)|(\\\")";
            internal const string keyFinder = "(\\s*?)(\\\")([^\\\"]*?)(\\\")";
            internal const string stringValueFinder = "(.*?)(\\\")";
            internal const string valueFinder = "(.*?)[\\]\\s,}]";
        } //class Regex

        internal static class KeySet {
            internal const string objectStart = "{";
            internal const string objectEnd = "}";
            internal const string arrayStart = "[";
            internal const string arrayEnd = "]";
            internal const string next = ",";
            internal const string stringValueStart = "\"";
            internal const string keyValueSeparator = ":";
        } //class KeySet

    } //class DefinitionSet

}