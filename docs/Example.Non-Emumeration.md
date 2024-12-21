Non-Enumeration Example{title}

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/Test.Non-Emumeration)

The example demonstrate the case of using a non-enumeration type for the generic `STATE` type of a state machine.

~~~{lang=C#}
<span class="_custom-word_ highlighter">StateMachine</span>&lt;<span class="type keyword highlighter">double</span>&gt; stateMachine = <span class="keyword highlighter">new</span>(<span class="type keyword highlighter">double</span>.<span class="literal keyword highlighter">NaN</span>);
~~~

The type `double` has 6 public static fields: `NegativeInfinity`, `MinValue`, `Epsilon`, `MaxValue`, `PositiveInfinity`, and `NaN`. They are used as the states.

Note that the default `double` value is not usable, because this is `0`, not corresponding the any of the `double` public static fields. Therefore, the call to the [constructor](index.html#heading-public-constructor) specifies `NaN` as an initial state.

State transition graph:

~~~
&minus;&#x221E; &#x21D4; MinValue &#x21D4; Epsilon &#x21D4; MaxValue &#x21D4; +&#x221E;;
MinValue &#x21D4; MaxValue
NaN &#x21D2; to any other state, but all the transition to NaN are invalid
~~~

This documentaton is generated from the extended Markdown documentation using [Extensible Markdown](https://marketplace.visualstudio.com/items?itemName=sakryukov.extensible-markdown)
for Visual Studio Code.{.extensible-markdown}

<script src="https://SAKryukov.github.io/publications/code/source-code-decorator.js"></script>
