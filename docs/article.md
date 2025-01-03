Enumeration-Based Generic State Machine{title}

Sergey A Kryukov{.author}

[Source code](https://GitHub.com/SAKryukov/generic-state-machine)<br/>
[API Documentation](https://SAKryukov.GitHub.io/generic-state-machine)

# Contents{no-toc}

@toc


# Enumeration Series

This article is the fifth article of the small series of articles on enumeration types:

1. [Enumeration Types do not Enumerate! Working around .NET and Language Limitations](https://www.CodeProject.com/Articles/129830/Enumeration-Types-do-not-Enumerate-Working-around)
1. [Human-readable Enumeration Meta-data](https://www.CodeProject.com/Articles/136181/Human-readable-Enumeration-Meta-data)
1. [Enumeration-based Command Line Utility](https://www.CodeProject.com/Articles/144349/Enumeration-based-Command-Line-Utility)
1. [Bitwise Enumeration Editor for PropertyGrid and Visual Studio](https://www.CodeProject.com/Articles/809357/Bitwise-Enumeration-Editor-for-PropertyGrid-and-Vi)
1. The present article

# Representation of a Transition System

Let's say, we have some system with three states, `A`, `B`, and `C`. Let's make a Cartesian square out of them:

<table>
  <tr>
  <td></td><td>A</td><td>B</td><td>C</td>
  <tr></tr>
  <td>A</td><td>×</td><td> </td><td> </td>
  <tr></tr>
  <td>B</td><td> </td><td>×</td><td> </td>
  <tr></tr>
  <td>C</td><td> </td><td> </td><td>×</td>
  </tr>
</table>

Circular relation `A ⭢ B ⭢ C ⭢ A ⭢ …`:

<table>
  <tr>
  <td></td><td>A</td><td>B</td><td>C</td>
  <tr></tr>
  <td>A</td> <td> </td> <td>×</td> <td> </td>
  <tr></tr>
  <td>B</td><td> </td><td></td> <td>×</td>
  <tr></tr>
  <td>C</td><td>×</td><td> </td><td> </td>
  </tr>
</table>



A *transition system* is a pair `(S, T)` where `S` is a set of states and `T`, the transition relation, is a subset of `S`.<br/>
We say that there is a transition from state `p` to state `q` if `(p, q) ∈ T`, and denote it `p ⭢ q`.

A *deterministic finite-state machine* or *deterministic finite-state acceptor* is a quintuple `(Σ, S, s₀, δ, F)`, where:

- `Σ`
- `S`
- `s₀`
- `δ` is the state-transition function: `δ: S ⨉ Σ ⭢ S`
- `F`

A *finite-state transducer* is a sextuple `(Σ, Γ, S, s₀, δ, ω)`, where:

- `Σ` is the input alphabet (a finite non-empty set of symbols)
- `Γ` is the output alphabet (a finite non-empty set of symbols)
- `S` is a finite non-empty set of states
- `s₀ ∈ S` is the initial state
- `δ` is the state-transition function: `δ: S ⨉ Σ ⭢ S`
- `ω` is the output function `ω: S ⨉ Σ ⭢ Γ (ω: S ⭢ Γ)`






@include(extensible-markdown.md)
