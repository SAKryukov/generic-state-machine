Zoo{title}

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestZoo)

The Zoo example represents the state machine representing the visitor location at the zoo. This is the transition graph:

![Zoo](zoo.svg)

Note that all the transitions are *undirected* except the transition between Flamingo and Exit, because entering the zoo area using the exit is not allowed.

The example demonstrates valid transitions and the attempt to perform some invalid transitions. In particular, each attempt to enter the Chimpanzee area shows the message "Chimpanzee area is temporarily closed".

The example also demonstrates [StateMachine.Labyrinth](index.html#heading-labyrinth), [StateMachine.LongestPaths](index.html#heading-longestpaths) ([NP-hard](https://en.wikipedia.org/wiki/NP-hardness)), [StateMachine.MaximumPaths](index.html#heading-maximumpaths) ([NP-hard](https://en.wikipedia.org/wiki/NP-hardness)), and [StateMachine.FindDeadEnds](index.html#heading-finddeadends).

This documentation is generated from the extended Markdown documentation using [Extensible Markdown](https://marketplace.visualstudio.com/items?itemName=sakryukov.extensible-markdown)
for Visual Studio Code.{.extensible-markdown}
