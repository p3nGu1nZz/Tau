See discussions, stats, and author profiles for this publication at: https://www.researchgate.net/publication/2267817

Communicating Hierarchical State Machines

Conference Paper  in  Lecture Notes in Computer Science · May 1999

DOI: 10.1007/3-540-48523-6_14 · Source: CiteSeer

CITATIONS
123

3 authors, including:

Mihalis Yannakakis

Columbia University

308 PUBLICATIONS   35,437 CITATIONS

SEE PROFILE

READS
871

All content following this page was uploaded by Mihalis Yannakakis on 25 May 2016.

The user has requested enhancement of the downloaded file.

Communicating Hierarchical State Machines

Ra jeev Alur

, Sampath Kannan

, and Mihalis Yannakakis

?

??

? ? ?

Abstract. Hierarchical state machines are (cid:12)nite state machines whose

states themselves can be other machines. In spite of their popularity in

many modeling tools for software design, very little is known concern-

ing their complexity and expressiveness. In this paper, we study these

questions for hierarchical state machines as well as for communicating

hierarchical state machines, that is, (cid:12)nite state machines extended with

both hierarchy and concurrency. We present a comprehensive set of re-

sults characterizing (1) the complexity of the reachability, emptiness and

universality problems, (2) the complexity of the language inclusion and

equivalence problems, and (3) the succinctness relationships between dif-

ferent types of machines.

1 Introduction

Finite state machines (FSMs) are widely used in the modeling of systems in var-

ious areas. Descriptions using FSMs are useful to represent the (cid:13)ow of control

(as opposed to data manipulation) and are amenable to formal analysis such

as model checking [CE81,CK96,Hol97,VW86]. In the simplest setting, an FSM

consists of a labeled graph whose vertices correspond to system states and edges

correspond to system transitions. In practice, to describe complex systems us-

ing FSMs, several extensions are useful such as communicating FSMs in which

the system is described by a collection of FSMs that operate concurrently and

synchronize with one another periodically,

There is a rich body of theoretical results concerning complexity and ex-

pressiveness of state machines. By labeling the edges of an FSM with alphabet

symbols and by introducing initial and (cid:12)nal states, FSMs can be used to de-

(cid:12)ne regular languages. Analysis problems of interest include emptiness of the

language, model checking with respect to temporal requirements, and inclu-

sion and equivalence of the languages of two machines. For a single FSM, the

complexity of some problems depends on whether the machine is determinis-

tic or not. For instance, language equivalence is NL-complete for deterministic

machines but Pspace-complete for nondeterministic ones (cf. [HU79]). Intro-

ducing concurrency, that is, considering communicating FSMs, usually costs an

?

??

? ? ?

Department of Computer and Information Science, University of Pennsylvania, and

Bell Laboratories, Lucent Technologies. Email: alur@cis.upenn.edu. Supported in

part by NSF CAREER award CCR-9734115 and by the DARPA grant NAG2-1214.

Department of Computer and Information Science, University of Pennsylvania.

Email: kannan@cis.upenn.edu.

Bell Laboratories, Lucent Technologies. Email: mihalis@research.bell-labs.com.

extra exponential. For instance, simple reachability for communicating FSMs

is Pspace-complete, while trace equivalence is Expspace-complete [HKV97].

Insight into the complexity of analysis problems is also provided by the suc-

cinctness a(cid:11)orded by various features. For instance, nondeterministic machines

are exponentially more succinct than deterministic ones (cf. [HU79]), concurrent

machines are exponentially more succinct than the sequential ones, and con-

current nondeterministic machines are doubly-exponentially more succinct than

deterministic FSMs [DH94].

While the impact of features such as nondeterminism and concurrency on

complexity and expressiveness of (cid:12)nite-state machines is well understood, there

is almost no literature on understanding the impact of introducing hierarchy

in state machines. In hierarchical (nested) FSMs, the states of an FSM can be

ordinary states or superstates which are FSMs themselves. The notion of hier-

archical FSMs was popularized by the introduction of Statecharts [Har87],

and exists in various ob ject-oriented software development methodologies such

as Room [SGW94] and the Uni(cid:12)ed Modeling Language (Uml [BJR97]). Hier-

archical state machines have two descriptive advantages over ordinary FSMs.

First, superstates o(cid:11)er a convenient structuring mechanism that allows us to

specify systems by stepwise re(cid:12)nement, and to view it at di(cid:11)erent levels of gran-

ularity. Second, by allowing sharing of component FSMs, we need to specify

components only once and then can reuse them in di(cid:11)erent contexts, leading to

modularity and succinct system representations. In a recent paper, it is shown

that the succinctness o(cid:11)ered by sequential hierarchical state machines comes at

no cost: the reachability problem for sequential hierarchical state machines can

be solved without constructing the equivalent (cid:13)attened ordinary FSM, and is

P-complete [AY98]. In this paper, we investigate the reachability question for

communicating hierarchical state machines, language equivalence problems for

hierarchical and communicating hierarchical state machines, and succinctness

issues.

Our (cid:12)rst set of results concern algorithms for reachability in presence of both

the concurrency and hierarchy constructs. While reachability of a single hierar-

chical state machine (HSM) can be solved in linear time [AY98], we show that

the product of two HSMs is di(cid:14)cult to analyze and has a Pspace-complete

reachability problem. For communicating hierarchical state machines (CHM)

that allow arbitrary nesting of the concurrency and hierarchy constructs, we

show the reachability problem to be Expspace-complete. This shows that hier-

archy, in presence of concurrency, costs an extra exponential. Then, we proceed

to identify a restriction on the use of the two constructs that avoids this extra

cost. In wel l-structured CHMs, communication among hierarchical components

is allowed only at the top level, and for such machines we show that one needs

to pay only for concurrency: the reachability problem is Pspace-complete as in

the case of communicating FSMs.

Our second set of results concern checking equivalence of languages of two ma-

chines. For a sequential HSM, while language emptiness is P-complete, we show

checking universality to be Expspace-complete. It follows that checking lan-

guage inclusion and equivalence are both Expspace-complete problems. In the

case of ordinary FSMs, deterministic machines have polynomial-time complexity

for universality, inclusion, and equivalence, while the corresponding problems are

Pspace-complete for nondeterministic ones. This motivates us to consider these

problems for deterministic HSMs: while universality becomes easy (P-complete),

language inclusion is Pspace-complete, and language equivalence can be solved

in Pspace (a matching lower bound remains an open question). When we con-

sider communicating hierarchical machines, the costs due to concurrency and

hierarchy add up: all of universality, language inclusion, and language equiva-

lence are 2Expspace-complete.

Finally, we consider succinctness a(cid:11)orded by the hierarchical construct. Start-

ing with deterministic FSMs, we know that nondeterminism can add exponen-

tial succinctness. We show that hierarchy can also add exponential succinctness.

Furthermore, nondeterministic FSMs and deterministic HSMs are incompara-

ble extensions: both can be exponentially more succinct with respect to each

other. Allowing both nondeterminism and hierarchy gives double-exponential

succinctness with respect to deterministic FSMs, and curiously enough, this

double-exponential gap exists between nondeterministic HSMs and determinis-

tic HSMs also. Allowing nondeterminism, hierarchy, as well as concurrency, gives

triple-exponential succinctness with respect to deterministic FSMs (or even de-

terministic HSMs), and double-exponential succinctness with respect to nonde-

terministic FSMs (or even nondeterministic HSMs)!

2 Communicating Hierarchical State Machines

There are many variants of de(cid:12)nitions of (cid:12)nite-state machines. We choose a

de(cid:12)nition in which edges are labeled with alphabet symbols. For simplicity, we

restrict ourselves to a single initial and a single (cid:12)nal state, but generalization

to multiple initial and (cid:12)nal states poses no technical problems. A (cid:12)nite-state

machine (FSM) consists of a (cid:12)nite set Q of states, a (cid:12)nite alphabet (cid:6) , an initial

state q

2 Q, a (cid:12)nal state q

2 Q, a set ! (cid:18) Q (cid:2) (cid:6) (cid:2) Q of transitions. Given a

I

F

word (cid:26) = (cid:27)

(cid:27)

(cid:1) (cid:1) (cid:1) (cid:27)

over the alphabet (cid:6) , an accepting run of the FSM M over

0

1

n

(cid:26) is a sequence q

! q

! (cid:1) (cid:1) (cid:1)

! q

such that q

equals the initial state q

,

0

1

n+1

0

(cid:27)

(cid:27)

(cid:27)

0

1

n

I

q

equals the (cid:12)nal state q

, and for 0 (cid:20) i (cid:20) n, (q

; (cid:27)

; q

) is a transition of

n+1

i

i

i+1

F

M . The set of words (cid:26) 2 (cid:6)

over which M has an accepting run is called the

(cid:3)

language of M , denoted L(M ).

Now we proceed to add two features: concurrency and hierarchy. For concur-

rency, a machine is composed of a set of component machines which synchronize

on transitions labeled with common alphabet symbols. For hierarchy, the states

of a machine can be other machines. Such states are popularly known as super-

states. The meaning of such a hierarchical de(cid:12)nition is obtained by recursively

substituting each superstate by the machine associated with it. Hierarchical de(cid:12)-

nitions allow sharing of patterns. The following formal de(cid:12)nition allows arbitrary

nesting of the concurrency and hierarchy constructs. A communicating hierar-

chical state machine (CHM) is one of the following three forms:

Base case: An FSM (Q; (cid:6) ; q

; q

; !) is a CHM.

I

F

Concurrency: If M

; M

; : : : M

are CHMs then M

kM

k (cid:1) (cid:1) (cid:1) kM

is a CHM.

1

2

k

1

2

k

Hierarchy: If M is a (cid:12)nite set of CHMs, N = (Q; (cid:6) ; q

; q

; !) is an FSM with

I

F

states Q, and (cid:22) is a labeling function (cid:22) : Q 7! M that associates each state

q 2 Q with a CHM in M, then the triple (N ; M; (cid:22)) is a CHM.

A CHM of the form M

kM

k (cid:1) (cid:1) (cid:1) kM

is called a product expression , and each M

1

2

k

i

is called a component of the product expression. A CHM of the form (N ; M; (cid:22))

is called a hierarchical expression , the FSM N is called the top-level of the hierar-

chical expression, and each CHM in M is called a component of the hierarchical

expression.

M

M

M

1

2

a

a

M

M

3

4

Fig. 1. A schematic sample communicating hierarchical state machine

As an illustration, see Figure 1 that shows a partial CHM. The machine M

is product of two hierarchical expressions M

and M

. The states of M

and M

1

2

1

2

are mapped to lower-level component machines. For instance, one state of M

is

1

mapped to M

, which in turn is a product of two machines, while another state

3

of M

is mapped to the ordinary FSM M

. Notice the sharing: machine M

is

1

4

4

associated with states of both M

and M

.

1

2

The semantics is de(cid:12)ned by mapping each CHM M to an FSM [[M ]]:

Base case: If M is an FSM, then [[M ]] equals M .

Concurrency: If M is the product expression M

kM

k (cid:1) (cid:1) (cid:1) kM

, then [[M ]] is

1

2

k

obtained by taking the product of the FSMs [[M

]] corresponding to the

i

I

F

components. Formally, suppose [[M

]] = (Q

; (cid:6)

; q

; q

; !

) for each i. Then,

i

i

i

i

i

i

{ The state-space of [[M ]] is Q

(cid:2) (cid:1) (cid:1) (cid:1) (cid:2) Q

.

1

k

{ The alphabet (cid:6) of [[M ]] is (cid:6)

[ (cid:1) (cid:1) (cid:1) [ (cid:6)

.

1

k

{ The initial state of [[M ]] is hq

; : : : q

i.

1

k

I

I

{ The (cid:12)nal state of [[M ]] is hq

; : : : q

i.

1

k

F

F

{ For a symbol (cid:27) 2 (cid:6) , [[M ]] has a (cid:27) -labeled transition from hq

; : : : q

i

1

k

to hw

; : : : w

i i(cid:11) for every i such that (cid:27) 2 (cid:6)

, the FSM [[M

]] has a

1

k

i

i

transition (q

; (cid:27); w

), and for every i such that (cid:27) 62 (cid:6)

, w

= q

.

i

i

i

i

i

Hierarchy: If M = (N ; M; (cid:22)) with top-level FSM N = (Q; (cid:6) ; q

; q

; !) then

I

F

{ A state of [[M ]] is of the form (q ; w) where q 2 Q and w is a state of the

FSM [[(cid:22)(q)]] associated with q .

{ A symbol (cid:27) belongs to the alphabet of [[M ]] if (cid:27) belongs to the alphabet

(cid:6) of the top-level FSM N , or (cid:27) belongs to the alphabet of [[(cid:22)(q)]] for

some q 2 Q.

I

{ The initial state of [[(cid:22)(q

)]] is the initial state of [[M ]].

{ The (cid:12)nal state of [[(cid:22)(q

)]] is the (cid:12)nal state of [[M ]].

F

{ [[M ]] has two types of transitions

0

(cid:15) For a transition (q ; (cid:27); q

) of the top-lavel FSM N , [[M ]] has a (cid:27) -labeled

transition from the (cid:12)nal state of [[(cid:22)(q)]] to the initial state of [[(cid:22)(q

)]].

0

(cid:15) For q 2 Q, if (w; (cid:27); w

) is a transition of [[(cid:22)(q)]] then ((q ; w); (cid:27); (q ; w

))

0

0

is a transition of [[M ]].

Now we can associate a language with each CHM: the language L(M ) of a CHM

M is the same as the language L([[M ]]) of the FSM associated with M .

A CHM can be represented by a DAG. A terminal node corresponds to the

base case, and has an associated FSM. An internal node may correspond to a

product expression or a hierarchical expression. A node for a product expression

is labeled with the operator k, and its children are the components of the prod-

uct. Note that the concurrency operator is associative. Consequently, we assume

that the immediate children of a product expression are not themselves product

expressions. A node for a hierarchical expression has the top-level FSM associ-

ated with it, and the edges connect the states of the top-level FSM with other

nodes. When counting the size of the description of a CHM, we consider the

size of this DAG representation. For instance, in Figure 1, the size of the FSM

M

is counted only once, even though it appears multiple times. Two important

4

parameters of this DAG representation are width and depth: width of a CHM

M is the maximum number of components in the product nodes, and depth is

the length of the longest path in the DAG.

A CHM M is called a hierarchical state machine (HSM) if it does not con-

tain any product expression. Thus, HSMs do not involve any concurrency. To

illustrate the power of HSMs, we show that HSMs can count with exponential

succinctness. The alphabet in this example consists of a single symbol (cid:27) . It is

easy to construct a sequence of HSMs M

; M

; : : : M

as follows. M

is an FSM

1

2

n

1

with L(M

) = f(cid:27)g, and for i > 1, M

is a hierarchical expression with two su-

1

i

perstates, each mapped to M

so that L(M

) = L(M

) (cid:1) (cid:27) (cid:1) L(M

). Thus,

i(cid:0)1

i

i(cid:0)1

i(cid:0)1

the language L(M

) of the HSM M

contains precisely the string of length 2

(cid:0) 1.

i

i

i

3 Reachability

The reachability problem for a CHM M is to determine if the (cid:12)nal state of [[M ]]

is reachable from the initial state of [[M ]]. Alternatively, this can be viewed as

checking if the language L(M ) is empty. The reachability problem for ordinary

FSMs is in NL. Introducing hierarchy in FSMs comes at a minimal cost: the

reachability problem for HSMs can be solved in linear time, and is P-complete

[AY98]. On the other hand, introducing concurrency in FSMs is expensive: the

reachability problem for product of FSMs is Pspace-complete. Now we want

to study the impact of the combination of the two constructs. We begin by

considering product of hierarchical state machines.

For ordinary FSMs, the product of two FSMs has a quadratic blow-up.

In [AY98], it is shown that the product of an HSM with an FSM can be con-

structed to yield an HSM with quadratic number of states. As the next result

shows, the product of two HSMs cannot be constructed e(cid:14)ciently.

Before we present the result, consider the sequence P

; P

; : : : P

of HSMs

0

1

n

over the alphabet f0; 1; #g. P

is an FSM with L(P

) = f#g, and for i > 0,

0

0

P

is a hierarchical expression with two superstates, each mapped to P

, so

i

i(cid:0)1

that L(P

) is the union of 0 (cid:1) L(P

) (cid:1) 0 and 1 (cid:1) L(P

) (cid:1) 1. Thus, the language

i

i(cid:0)1

i(cid:0)1

accepted by P

is f w # w

j w 2 f0; 1g

g. It is worth noting that the language

R

n

n

n

f w # w j w 2 f0; 1g

g cannot be de(cid:12)ned succinctly by HSMs (in fact, we

can prove that every HSM accepting this language must be of exponential size).

Intuitively, an HSM can be viewed as a push-down automaton with bounded

stack size.

Recall that for pushdown automata, emptiness of single automaton can be

solved in polynomial-time, but emptiness of intersection of two pushdown au-

tomata is undecidable. In the case of HSMs, emptiness of a single HSM is linear-

time, but emptiness of the intersection of two HSMs is Pspace-complete.

Proposition 1. Reachability problem for M

kM

, where M

and M

are HSMs,

1

2

1

2

is Pspace-complete.

1

In CHMs, the concurrency and hierarchy operators are arbitrarily nested, and

the product components can synchronize with each other at di(cid:11)erent levels of

hierarchy. These features make the reachability problem signi(cid:12)cantly di(cid:14)cult to

solve. In fact, to solve the reachability problem for a CHM M , one cannot do

better than the obvious solution of constructing the (cid:13)attened FSM [[M ]] (locally),

and applying the standard reachability algorithm to it.

Proposition 2. For a CHM M , the number of states of [[M ]] is O(n

), where

m

d

each FSM in M has at most n states, M has width d > 1 and depth m.

This implies that reachability problem for a CHM can be solved in time O(n

),

m

d

that is, doubly-exponential in the depth in the worst case. The precise complexity

class is Expspace.

Theorem 1. Reachability problem for CHMs is Expspace-complete.

Since reachability problem for CHMs is Expspace-hard, we wish to identify

subclasses with lower complexity. We proceed to de(cid:12)ne well-structured CHMs

1

A detailed version with proofs can be obtained from the authors.

where arbitrary levels of hierarchy of product components cannot synchronize

with each other. A CHM M is said to be wel l structured if for every product

expression M

in M , if a proper component M

of M

is a hierarchical expres-

0

00

0

sion, then for every proper component N of M

and every component N

of

00

0

0

0

M

, the alphabet of [[N

]] is disjoint from the alphabet of [[N ]]. Note that the

alphabet plays a crucial role in the de(cid:12)nition of the concurrency operator since

components synchronize on common symbols. The restriction to well structured

machines ensures that if two (or more) hierarchical machines are composed to-

gether, then they can synchronize only at the top level. For example, in Figure 1,

a is a common symbol to the two components M

and M

. Well-structuredness

1

2

requires that a is used to label transitions of only the top-level FSMs of M

and

1

M

, and cannot appear, for instance, in M

or M

. The reachability problem for

2

3

4

well structured CHMs is Pspace-complete, where the exponential cost is only

due to concurrency, but not hierarchy.

Theorem 2. The reachability problem for a wel l structured CHM M can be

solved in time O(k (cid:1) n

), where k is the number of operators in M , each FSM in

d

M has at most n states, and d is its width.

4 Language Equivalence

In this section, we consider equivalence problem for hierarchical state machines

and communicating hierarchical state machines. Each HSM M de(cid:12)nes the lan-

guage L(M ). The emptiness problem for HSMs is to determine if L(M ) is empty,

and can be solved in P using the reachability algorithm. The universality prob-

lem for HSMs is to determine if the complement of L(M ) is empty. This problem

turns out to be much harder:

Theorem 3. The universality problem for HSMs is Expspace-complete.

Two HSMs M

and M

are trace equivalent if their languages are identical. This

1

2

problem, and also the language inclusion problem, have the same complexity as

the universality problem:

Theorem 4. The trace equivalence problem for HSMs is Expspace-complete.

For ordinary FSMs, problems such as universality, inclusion, and equivalence,

are much easier if we consider deterministic variants. The results for HSMs show

somewhat subtle distinctions between these problems. Recall that an FSM is de-

terministic if for every state q and every symbol (cid:27) , there is at most one (cid:27) -labeled

transition with source q . An HSM M is deterministic if [[M ]] is deterministic. This

entails two requirements: each base FSM and each top-level FSM of a hierarchi-

cal expression is deterministic, and furthermore, if a state q of a top-level FSM

of a hierarchical expression has a (cid:27) -labeled outgoing transition, then the (cid:12)nal

state of the FSM associated with q has no (cid:27) -labeled outgoing transition. The

latter condition ensures determinism concerning exiting lower-level FSMs.

Emptiness Intersection Universality Inclusion Equivalence

FSM

NL

NL

Pspace

Pspace

Pspace

Det FSM

NL

NL

NL

NL

NL

HSM

P

Pspace

Expspace Expspace Expspace

Det HSM

P

Pspace

P

Pspace

2 Pspace

CHM Expspace Expspace 2Expspace 2Expspace 2Expspace

Fig. 2. Summary of complexity results

Theorem 5. For deterministic HSMs, the universality problem is P-complete,

and the language inclusion problem is Pspace-complete.

For deterministic HSMs, trace equivalence can be solved in Pspace, but its

complexity is sandwiched somewhere between that of the P-complete universality

problem and the Pspace-complete language inclusion problem. It remains open

to determine its exact complexity.

Finally, we establish that the universality, language inclusion, and language

equivalence problems for communicating hierarchical machines are complete for

double-exponential space.

Theorem 6. For CHMs, the universality, language inclusion, and language equiv-

alence problems are 2Expspace-complete.

The results concerning HSMs and CHMs are summarized in Figure 2. The table

lists the known complexity bounds for ordinary FSMs also for comparison.

5 Succinctness

In this section we are concerned with the expressive power of the hierarchical

construct, both in presence and absence of the concurrency construct. To discuss

expressive power, we will look at families of languages L = fL

jn = 1; 2; : : : ; g

n

and consider the number of states #(L; n) needed by a machine that accepts L

n

in some particular formalism. Where necessary we adopt the baroque practice of

denoting the formalism by a superscript for #(L; n). Thus #

(L; n) denotes

F SM

the number of states necessary and su(cid:14)cient for an FSM to recognize L

. We

n

will say that formalism F

can be exponentially more succinct than formalism

1

F

if there is a family of languages L for which #

(L; n) = O(log[#

(L; n)]).

2

F 1

F 2

While hierarchical state machines de(cid:12)ne regular languages, they can be much

more succinct than ordinary FSMs. Note that there is an exponential translation

from HSMs to FSMs, and the translation preserves determinism. Recall that

nondeterministic FSMs can be exponentially more succinct than deterministic

ones. The following proposition is established by showing that the language

L

= fw # w

j w 2 f0; 1g

g can be recognized by a deterministic HSM of

n

R

n

linear size, while any nondeterministic FSM to recognize L

must be at least

n

exponential in n.

Proposition 3. Deterministic HSMs can be exponential ly more succinct than

nondeterministic FSMs.

The next result establishes that, while nondeterminism introduces exponen-

tial succinctness for ordinary FSM, it introduces doubly-exponential succinct-

ness in presence of hierarchy. The proof uses the language L

= fw j w

=

n

i

w

for some ig to establish the gap:

n

i+2

Proposition 4. Nondeterministic HSMs are doubly-exponential ly more succinct

than deterministic HSMs.

Note that the gap between nondeterministic HSMs and nondeterministic FSMs

is singly exponential. It is interesting to note that nondeterminism and hierarchy

are incomparable extensions, which is established using the language L

= fw j

n

w

= w

for some ig.

i

i+n

Proposition 5. Nondeterministic FSMs can be exponential ly more succinct than

deterministic HSMs.

Finally, we consider the expressive power added by concurrency and show that

this is signi(cid:12)cant. When unrestricted use of concurrency and hierarchy is allowed,

we get double-exponential succinctness compared to both FSMs and HSMs, and

triple-exponential succinctness compared to the deterministic variants of FSMs

and HSMs. The proof uses the language

L

= fw

#w

# (cid:1) (cid:1) (cid:1) #w

j

jw

j = 2

for each i and w

= w

for some i; j g:

n

0

1

k

i

i

j

n

Proposition 6. Communicating hierarchical machines can be doubly-exponential ly

more succinct than nondeterministic HSMs (and nondeterministic FSMs), and

triply-exponential ly more succinct than deterministic HSMs (and deterministic

FSMs).

The various succinctness claims are summarized in Figure 3.

CHM

HSM

2exp

2exp

1exp

3exp

1exp

Det HSM

FSM

1exp

1exp

1exp

Det FSM

Fig. 3. Summary of succinctness results

6 Conclusions

In this paper, we have answered several questions concerning complexity and

succinctness of hierarchical state machines. As the summary (cid:12)gures at the end

of each section indicate, the complexity bounds do not change in a uniform

way with many peculiarities. In terms of practice, the Expspace-hardness of

the reachability problem in presence of unrestricted use of hierarchy and concur-

rency constructs suggests that modeling languages should enforce well-structured

use of the constructs to lower the complexity of the analysis problems. The re-

sults concerning the reachability problem should carry over to the more general

question of model checking of linear-time requirements. We have presented a

comprehensive picture that characterizes the power and complexity of sequen-

tial and communicating hierarchical state machines. A few questions concerning

subclasses of communicating hierarchical machines, such as the precise complex-

ity of equivalence of deterministic CHMs, remain to be resolved.

References

[AY98] R. Alur and M. Yannakakis. Model checking of hierarchical state machines.

In Proc. Sixth FSE, pp. 175{188. 1998.

[BJR97] G. Booch, I. Jacobson, and J. Rumbaugh. Uni(cid:12)ed Modeling Language User

Guide. Addison Wesley, 1997.

[CE81] E.M. Clarke and E.A. Emerson. Design and synthesis of synchronization

skeletons using branching time temporal logic. In Proc. Workshop on Logic

of Programs, LNCS 131, pp. 52{71. Springer-Verlag, 1981.

[CK96] E.M. Clarke and R.P. Kurshan. Computer-aided veri(cid:12)cation. IEEE Spectrum,

33(6):61{67, 1996.

[DH94] D. Drusinsky and D. Harel. On the power of bounded concurrency i: (cid:12)nite

automata. Journal of the ACM, 41(3), 1994.

[Har87] D. Harel. Statecharts: A visual formalism for complex systems. Science of

Computer Programming, 8:231{274, 1987.

[HKV97] D. Harel, O. Kupferman, and M.Y. Vardi. On the complexity of verifying

concurrent transition systems. In CONCUR, LNCS 1243, pp. 258{272. 1997.

[Hol97] G.J. Holzmann. The model checker spin. IEEE Trans. on Software Engi-

neering, 23(5):279{295, 1997.

[HU79]

J.E. Hopcroft and J.D. Ullman. Introduction to Automata Theory, Languages,

and Computation. Addison-Wesley, 1979.

[SGW94] B. Selic, G. Gullekson, and P.T. Ward. Real-time object oriented modeling

and design. J. Wiley, 1994.

[VW86] M.Y. Vardi and P. Wolper. An automata-theoretic approach to automatic

program veri(cid:12)cation. In Proc. First LICS, pp. 332{344, 1986.

View publication stats