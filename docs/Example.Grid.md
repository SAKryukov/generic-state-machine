Grid{title}

[Source code](https://github.com/SAKryukov/generic-state-machine/tree/main/code/Tests/TestGrid)

The state transition graph can be represented as a matrix 6x4 with each neighboring state connected with an *undirected* valid state transition:

~~~
n00 n01 n02 n03 n04 n05
n10 n11 n12 n13 n14 n15
n20 n21 n22 n23 n24 n25
n30 n31 n32 n33 n34 n35
~~~

This example demonstrates that NP-hard problems can be pretty hard even for the 24 states.
The example demonstrates the calculation of the longest path between two states and the maximum number of possible paths.

Maximum number of paths between a pair of states is 5493.
Total number of paths: 1603536, longest path length: 23.

@include(extensible-markdown.md)
