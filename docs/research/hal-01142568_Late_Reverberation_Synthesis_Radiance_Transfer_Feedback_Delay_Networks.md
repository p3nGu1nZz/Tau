Late Reverberation Synthesis: From Radiance Transfer
to Feedback Delay Networks
Bai Hequn, Gael Richard, Laurent Daudet

To cite this version:

Bai Hequn, Gael Richard, Laurent Daudet. Late Reverberation Synthesis: From Radiance Transfer
to Feedback Delay Networks. 2015. ￿hal-01142568￿

HAL Id: hal-01142568

https://hal.science/hal-01142568v1

Preprint submitted on 15 Apr 2015

HAL is a multi-disciplinary open access
archive for the deposit and dissemination of sci-
entific research documents, whether they are pub-
lished or not. The documents may come from
teaching and research institutions in France or
abroad, or from public or private research centers.

L’archive ouverte pluridisciplinaire HAL, est
destinée au dépôt et à la diffusion de documents
scientifiques de niveau recherche, publiés ou non,
émanant des établissements d’enseignement et de
recherche français ou étrangers, des laboratoires
publics ou privés.

Late Reverberation Synthesis: From Radiance Transfer to
Feedback Delay Networks

Hequn Bai,1∗ Gaël Richard,1 Laurent Daudet2

1Institut Mines-Télécom, Télécom ParisTech, CNRS-LTCI,
37-39 rue Dareau, 75014 Paris, France
2Institut Langevin, ESPCI ParisTech, Univ. Paris Diderot and CNRS UMR7587,
1 rue Jussieu, 75005 Paris, France

∗To whom correspondence should be addressed; E-mail: hequn.bai@telecom-paristech.fr.

In room acoustic modeling, Feedback Delay Networks (FDN) are known to efﬁciently

model late reverberation due to their capacity to generate exponentially decaying dense

impulses. However, this method relies on a careful tuning of the different synthesis pa-

rameters, either estimated from a pre-recorded impulse response from the real acoustic

scene, or set manually from experience. In this paper we present a new method, which

still inherits the efﬁciency of the FDN structure, but aims at linking the parameters

of the FDN directly to the geometry setting. This relation is achieved by studying the

sound energy exchange between each delay line using the acoustic Radiance Transfer

Method (RTM). Experimental results show that the late reverberation modeled by this

method is in good agreement with the virtual geometry setting.

Introduction

The propagation of a sound within an enclosed space undergoes several reﬂections and diffraction at

walls and obstacles. For a listener in this room, this creates a speciﬁc coloring, that depends on the

position of both sound source and listener.

In many application domains, there is a clear interest in

1

having efﬁcient means to simulate perceptually, if not physically, this sound propagation. For example, in

applications involving virtual reality (video games, serious games, movies...), it is of utmost importance

to give to the listener the impression of being immersed in the acoustic scene, sometimes in a dynamic

way (1, 2). In other applications, such as music recording in a studio, it may be desired to enrich the

original acoustic space in order to simulate a larger, perceptually more pleasant stage.

The time-domain representation of such room-dependent sound propagation, between a sound source

and a receiver, is classically characterized by the Room Impulse Response (RIR). Plenacoustic functions

(3–5) extend this notion by representing the evolution of RIRs as a function of the three-dimensional

spatial positions of source and receiver. Static acoustic scenes, neglecting their environmental variations

such as temperature and humidity changes, can be considered as linear and time-invariant (LTI) systems,

which can thus be uniquely characterized by their plenacoustic functions. Room impulse responses are

classically decomposed into two parts, that provide distinct perceptual cues: the Early Reﬂections (ER)

and the Late Reverberation (LR) (6). ER helps in localization and conveys spatial information about

the environment, such as the shape of the room (7), while LR enhances immersion, contributing to the

perception of the size of the environment, its amount of furnishing and type of absorptivity (6).

Many techniques have been proposed to compute RIRs in order to simulate the sound propagation

within a room. They can be roughly divided into geometric and statistical approaches.

On the one hand, geometric approaches rely on physical principles. For example, the Image Source

Method (ISM) (8, 9) computes specular reﬂection paths by considering a set of virtual sources, which

are positioned at mirrored locations with respect to each wall. The ray tracing method (10) computes

sound rays outgoing from the source through the acoustic scene, and collects them at the receiver. The

beam tracing method (11) classiﬁes reﬂection paths from a source by recursively tracing the pyramidal

sound beams. A tree structure can be used to represent the beam path from the source to the full space

enclosure (12). Due to the fact that computation time increases rapidly with the reﬂection order, these

geometrical-based methods are mostly used to model the low-order echoes for early reﬂections. To

2

overcome this limitation, the Radiance Transfer Method (RTM), originating from the light transport

algorithm, efﬁciently models the diffuse reﬂections of RIRs and the sound energy decay of the LR (13,

14). The RTM decouples the sound source and receiver from the sound propagation, by precomputing a

linear operator that deﬁnes how a sound emitted from wall surface patches affects the radiance of other

surfaces. For example, in (15), the so-called acoustic form factor is derived for diffuse surface reﬂections,

in arbitrary polyhedral rooms. However, the main limitation of the RTM is its computational cost :

depending on the complexity of the geometry, it may require hundreds to thousands of convolutions at

run-time, which hinders real-time implementation even with alternative implementations in the frequency

domain (16–18).

On the other hand, statistical methods (also termed perceptually-based methods) use global acous-

tic parameters, such as reverberation time, to model the main perceptual aspects of reverberation. In

his landmark paper (19), Schroeder introduced a novel paradigm based on recursive comb ﬁlters and

allpass ﬁlters to simulate the multiple echoes. This scheme was later improved by Moorer (20), and

then generalized into a structure well suited for artiﬁcial reverberation called Feedback Delay Networks

(FDNs)” (21). This structure is characterized by a set of delay lines connected in a feedback loop through

a feedback matrix. Jot further studied the FDNs and developed associated techniques for designing good

quality reverberators (22). Several extensions were proposed (23), including a binaural reverberator (24).

Other approaches also exist, such as the recently proposed sparse velvet noise reverberator (25) and the

artiﬁcial reverberator in the frequency domain to model the spectral magnitude decay (26). A compre-

hensive overview of other artiﬁcial reverberators is given in (27). It is however known that, given the

structure of a statistical reverberator, it can be a rather tedious and empirical task to set all parame-

ters (28), including the delay lengths, attenuation factors and feedback matrix. Besides, these statistical

methods sometimes make use of measured impulse responses, extracting some global acoustic param-

eters to tune the artiﬁcial reverberator, thereby ignoring ﬁne details of the actual room size/shape or

material absorptivity. The work in (29) linked the physics-based digital waveguide mesh (DWM) model

3

with the feedback delay networks to design a scalable physics-based artiﬁcial reverberator. In a recent

study (30), a genetic algorithm was used to automatically search an optimal setting of the parameters for

a length-4 FDN, given the measured impulse responses.

The goal of this paper is to combine the two classes of approaches for the synthesis of room reverber-

ation. First, we highlight some links between RTM and FDN before introducing a new geometry-based

statistical method. Our approach inherits the computational efﬁciency of the FDN structure, but aims at

linking the parameters of the FDN directly to the geometry setting. Such a relation is obtained by study-

ing the sound energy exchange between each delay line, using the acoustic Radiance Transfer Method

(RTM). In other words, the main principle of this hybrid approach, called FDN-RTM, is to relate a FDN

statistical model to the physical room layout, without actual RIR measurements.

The paper is organized as follows.

In section 1 we further describe the links between the RTM

and FDN approaches and then recall the basics of the acoustic radiance transfer method and the main

computation procedure. In section 2 we propose a new geometrical-based statistical modeling paradigm,

and design the corresponding parameter estimation procedure. Experimental results are given in section

3. Finally, some conclusions and discussions are suggested in section 4.

1 Radiance Transfer Method

1.1 Links between RTM and FDN

It can be noticed that, in the radiance transfer method, the iteration of energy exchanges between surface

patches can also be regarded as feedback loops in a feedback delay network, as illustrated on Fig. 1.

Here, each delay line represents sound reﬂection on one surface patch, and the elements of the feedback

matrix A contain both an attenuation factor aij and a time delay dij. The sound propagation between

wall surface patches are represented as delay lines in feedback delay loops. It is then of practical interest

to combine these two methods, using a feedback delay network as the synthesis structure, while feedback

characteristics are estimated by the acoustic transfer property in the room. The proposed room geometry-

4

(cid:1827)

(cid:1845)(cid:2871)

(cid:1845)(cid:2869)

(cid:1845)(cid:2870)

(cid:1845)(cid:2869)
(cid:1845)(cid:2870)
(cid:1845)(cid:2871)

Figure 1: Relation between radiance transfer method and feedback delay networks.

based statistical method then uses delay lines to represent the group of diffuse reﬂections between room

surfaces. For the FDN, the convolution of the room impulse response with the dry signal is replaced by

N 2 + kN multiplications and a few additions, where N is the number of delay lines. Thus, in another

way, it can also be regarded as an approximated but computationally efﬁcient structure of the radiance

transfer method.

1.2 Principles of the radiance transfer method

Although some extensions to non-diffuse reﬂections exist (31, 32), the most commonly used radiance

transfer method is based on the assumption that all boundaries are diffusely reﬂecting, governed by

Lambert’s law. Analytically, the radiation density of an inﬁnitesimal wall element, x, is deﬁned as its

initial radiation density I0(x, t), plus the contribution of the radiation density from all other wall elements

of the enclosure S at some earlier time I(x′, t

|x−x′|
c

−

), where c is the sound velocity in normal condition.

The analytical acoustic radiance transfer model can be described as:

I(x, t) = I0(x, t) +

R(x, x′, t)I(x′, t

ZS

x′

x
|

−
c

−

)dx′,

|

(1)

where R(x, x′, t) is the reﬂection kernel which describes how the outgoing radiation from point x′ at

time t inﬂuences the radiation density at point x after propagation.

To numerically simulate the radiation exchange, the room surface S is discretized into M small

5

planar patches. The radiation density of patch i after n diffuse reﬂections can be written in discretized

form as:

I (n)
i

(t) = I (n−1)
i

(t) + ρi

M

Xj=1,j6=i

Fi,jI (n−1)
j

(t

ri,j
c

).

−

(2)

where ri,j is the average distance between patch i and j, and Fi,j is a form factor which is the integral of

f (x, x′) over patch i and j, deﬁning the fraction of energy leaving patch j and incident on patch i. The

total radiation density from patch i can be written as:

Ii(t) =

∞

Xn=0

I (n)
i

(t).

where I (0)

i

(t) is the initial radiation density of patch i, distributed directly from the sound source.

The analytical expression of the form factor Fi,j is derived by (15) as in Eqn. (4):

Fi,j =

Vi,j
Aj ZSi ZSj

cos θ cos θ′
πd2

dSdS′,

(3)

(4)

where Vi,j is the visibility factor between patch i and j, Aj is the area of patch j, d is the distance

between the points of integration on Si and Sj, as shown on Fig. 2. Together with the diffuse coefﬁcient

ρ and the patch-to-patch propagation delay, patch-to-patch responses are formed, which correspond to

the reﬂection kernel R(x, x′, t).

At initial stage, the sound energy from the source is distributed to each surface patch, forming the

so-called initial shooting matrix. Then, the sound energy is propagated within the enclosure and is

exchanged between surface patches. After high-order reﬂections, the resulting responses are gathered

from each surface patch to the listener, using the ﬁnal gathering matrix. It is worth noting that the high-

order reﬂection response is independent from the positions of the source and the listener. This property

makes this method suitable for dynamic scene rendering, when both source and listener move, as only

initial shooting and ﬁnal gathering matrices require a real-time update.

Several studies have shown that the radiance transfer method is effective in predicting the rever-

beration characteristics of a given room. In (15), predictions were made in three spheric enclosures of

6

(cid:1856)(cid:1845)(cid:1314)

(cid:2016)(cid:1314)

(cid:1845)(cid:3036)

(cid:1856)

(cid:1845)(cid:3037)

(cid:2016)

(cid:1856)(cid:1845)

Figure 2: Illustration of the form factor for diffuse reﬂections.

varying size and absorptivity. Prediction results show that the radiance transfer method simulates im-

pulse responses with almost the same acoustic characteristic values, including reverberation time (RT),

steady-state sound-pressure and the average radiation density, compared with analytical solutions (33).

In (14), further investigations were made by comparing with real measured data in a squash court, a class-

room and an ofﬁce. Experimental results have shown that the purely diffused radiance transfer method

can predict the room sound ﬁeld with good accuracy, and, in general, better than the prediction made

by purely specular ray tracing methods. In the rest of this paper, we therefore take RTM as the baseline

method for modeling late reverberation.

2 Geometry-Based Statistical Model

In this section, a new geometrical-based statistical model is presented, named FDN-RTM. The name

of the model comes from two parts. In the one hand, it borrows the feedback delay networks (FDN)

structure for its efﬁcient implementation and ﬂexible parameter settings. In the other hand, the order-1

patch-to-patch responses from the radiance transfer method are used to estimate the parameters of the

FDN structure, in order to synthesize a realistic impulse response as expected from the speciﬁed room

7

geometry.

2.1 Relation between FDN and RTM

The basic idea of radiance transfer method is to divide the room surface into patches, and to model how

the outgoing radiance from one patch will affect the outgoing radiance of all the other patches after

order-1 diffuse reﬂection. High order reﬂections can also be achieved. This is similar to the structure of

feedback delay networks where the signal from one delay line will be sent to other delay lines according

to the feedback matrix, in order to increase the energy mixture between them. If we replace each patch-

to-patch interaction by a delay line, and set the feedback matrix with the corresponding form factors, we

can, theoretically, construct a huge FDN with hundreds to thousands of delay lines, which then would be

equivalent to the radiance transfer model.

However, in most cases of late reverberation, the trend of the energy decay is sufﬁcient to perceive

the dimension and layout of the environment. Thus, we can ignore the precise model of the delay and

amplitude of each pulse in the room impulse response, and only focus on modeling the global energy

exchange statistics. This suggests that a simpliﬁed RTM model can be used, by grouping the patch-to-

patch energy interactions, in order to reduce the number of delay lines. The parameter of each delay

line is determined by the average value of the patch-to-patch delays of the corresponding group and the

accumulated surface absorption factors.

The range and distribution of the delay lengths can also guide the choice of delay units in the FDN

structure. To illustrate this, we have computed the distribution of occurrence and energy for different

delays of the non-convex room displayed on Fig. 3 - Room 1. This room is modeled using 5000 patches.

The distribution of occurrence and energy is shown in Fig. 4. Note that the distribution of energy

for different delays is the product of the distribution of their occurrence with the energy transported

by the corresponding delays. Although the occurrence of short delays is low, patch-to-patch sound

8

Room 1: non−convex

Room 2: rectangular

Room 4: gallery

3

2

1

0
0

2

4

6

6

8

2

1

0
0

0

2

4

2

4

0

3

2

1

2
1
0
0

Room 3: corridor

5

10

2

1

0

15

5

0
−10

0

0

10

−10

3m.
Figure 3: The experimental geometries. Room 1: non-convex room of outside dimension 8m
corridor of dimension
Room 2:
16m
5.1m. It should be noted that
2m
the triangular patches do not illustrate the real patching density during the computation of the RTM
algorithm.

rectangular room of dimension 4.5m
2m. Room 4: gallery of outside dimension 19.6m

Room 3:

17.7m

2.5m.

3m

6m

×

×

×

×

×

×

×

×

interactions with short delays convey important energy, mainly because short delays normally come

from neighboring patches and transport considerably more energy than longer delays, according to Eqn.

(4). A closer look on Fig. 4 reveals that both the occurrence distribution and the energy distribution have

small peaks at around 390, 540, 800 and 1050 sample delays, which correspond to distances of 3m, 4m,

6m and 8m, respectively. Considering the dimensions of the room, these peaks reveal some of its acoustic

modes. These distributions can be exploited to appropriately choose delay units which are related to the

dimension and shape of the room. Optimal clustering algorithms (such as K-means) can also be used to

ﬁnd the optimal quantization of the delays based on their occurrence distribution. In some special cases

the modes are even more easily observed, such as for the long corridor room displayed on Fig. 3 - Room

3. The delay distribution, as shown in Fig. 5, has a strong peak at around 250 delay samples, which

corresponds to 2m, the width of the corridor. In such cases, the delays can be chosen around the strong

peak values to model the acoustic modes.

2.2 System structure

The proposed reverberator is illustrated in Fig. 6. It consists of a FDN-RTM for late reverberation, as

shown in the upper frame, and a series of early reﬂections, as shown in the lower frame.

The structure of the FDN-RTM is similar to the feedback delay network proposed in (34), except that

9

e
c
n
e
r
r
u
c
c
o

f
o

e
g
a
t
n
e
c
r
e
p

8

6

4

2

0

−3

x 10

distribution of occurrence for different delays

0

500

1000

1500

distribution of energy for different delays

0.015

0.01

0.005

y
g
r
e
n
e

f
o

e
g
a
t
n
e
c
r
e
p

0

0

500

1000

1500

delay [samples]

Figure 4: Distribution of occurrence and energy for different patch-to-patch delays due to diffuse reﬂec-
tions for Room 1 (non-convex room). Top: histogram of occurrence. Bottom: histogram of energy. The
dotted lines illustrate some peaks of the distributions. The sampling rate is 44.1kHz.

for each delay line, an initial delay unit and a post delay unit are added before and after the recursive

circuit, respectively. Each delay line represents a group of order-1 patch-to-patch sound reﬂections,

where the delay unit is determined by the mean patch-to-patch delay of each group. The initial delay

unit is the mean delay from the sound source to the grouped patches, and the post delay unit is the mean

delay from the grouped patches to the receiver. Since the dry input sound has to pass through the initial

delay, the post delay and at least once through the patch-to-patch delays, the output reverberant sound

actually models sound reﬂections starting from the second order. Thus, the ﬁrst order sound reﬂections

have to be modeled and added using the FIR ﬁlter in the lower frame.

Let ℘n denote the set of patch-to-patch sound interactions in group n. Then, if the total number of

groups is N , the corresponding feedback delay network will have N delay lines as depicted on Fig. 6.

2.3 Parameter estimation

The parameters of proposed FDN-RTM are estimated as follows using the order-1 patch-to-patch inter-

actions in the radiance transfer model.

10

e
c
n
e
r
r
u
c
c
o

f
o

e
g
a
t
n
e
c
r
e
p

y
g
r
e
n
e

f
o

e
g
a
t
n
e
c
r
e
p

0.01

0.008

0.006

0.004

0.002

0

0

0.015

0.01

0.005

0

0

distribution of occurrence for different delays

500

1000

1500

2000

distribution of energy for different delays

500

1000

1500

2000

delay [samples]

Figure 5: Distribution of occurrence and energy for different patch-to-patch delays due to diffuse reﬂec-
tions for Room 3 (long corridor of dimension 16m
2m). Top: histogram of occurrence. Bottom:
×
histogram of energy. The dotted lines highlight the main peak of the distributions. The sampling rate is
44.1kHz.

2m

×

1. Feedback matrix

The element amn in the feedback matrix actually describes the proportion of energy which is transported

by the patch-to-patch interactions within group m, that will be diffusely reﬂected and go to some other

patch-to-patch interactions in group n, during an order-1 diffuse reﬂection.

Let us denote as ℓm the total energy transported by the patch-to-patch energy exchange in group m,

and ℓm,n the total energy received by group n because of the diffuse reﬂections of group m. They can be

estimated from the form factors Fi,j in the RTM model as:

ℓm =

Fi,j

Xi→j∈℘m

ℓm,n =

Xi→j∈℘m Xj→k∈℘n

Fi,jFj,k

11

(5)

(6)

Late
reverberation

(cid:3)

(cid:1853)(cid:2869)(cid:2869) (cid:1710) (cid:1853)(cid:2869)(cid:3015)
(cid:1853)(cid:2869)(cid:2869) (cid:1710) (cid:1853)(cid:2869)(cid:3015)

(cid:1709)
(cid:1709)

(cid:1709)
(cid:1709)

(cid:1712)
(cid:1712)
(cid:1853)(cid:3015)(cid:2869) (cid:485) (cid:1853)(cid:3015)(cid:3015)
(cid:1853)(cid:3015)(cid:2869) (cid:485) (cid:1853)(cid:3015)(cid:3015)

(cid:2879)(cid:3029)(cid:3117)

(cid:2879)(cid:3029)(cid:3118)

(cid:2879)(cid:3029)(cid:3119)

(cid:1709)

(cid:2879)(cid:3029)(cid:3263)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:1854)(cid:2869)

(cid:1854)(cid:2870)

(cid:1854)(cid:2871)
(cid:1854)
(cid:1709)

(cid:1854)(cid:3015)
(cid:1854)

(cid:1876)(cid:4666)(cid:1866)(cid:4667)

  Early reflections

(cid:2879)(cid:3031)(cid:3117)

(cid:2879)(cid:3031)(cid:3118)

(cid:2879)(cid:3031)(cid:3119)

(cid:1709)

(cid:2879)(cid:3031)(cid:3263)(cid:3117)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:2879)(cid:3031)(cid:3294)

(cid:1878)
(cid:2879)(cid:3045)(cid:3117)

(cid:1878)

(cid:1709)

(cid:2879)(cid:3045)(cid:3260)

(cid:1878)

(cid:1834)(cid:2869)(cid:4666)(cid:1878)(cid:4667)

(cid:1834)(cid:2870)(cid:4666)(cid:1878)(cid:4667)

(cid:1834)(cid:2871)(cid:4666)(cid:1878)(cid:4667)
(cid:1709)

(cid:1834)(cid:3015)(cid:4666)(cid:1878)(cid:4667)

(cid:1871)(cid:2868)

(cid:1832)(cid:2869)(cid:4666)(cid:1878)(cid:4667)
(cid:1709)

(cid:1832)(cid:3012)(cid:4666)(cid:1878)(cid:4667)

(cid:2879)(cid:3030)(cid:3117)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:1878)

(cid:2879)(cid:3030)(cid:3118)

(cid:2879)(cid:3030)(cid:3119)

(cid:1709)
(cid:2879)(cid:3030)(cid:3263)

(cid:1855)(cid:2869)

(cid:1855)(cid:2870)

(cid:1855)(cid:2871)
(cid:1855)
(cid:1709)

(cid:1855)(cid:3015)
(cid:1855)

(cid:1877)(cid:4666)(cid:1866)(cid:4667)

Figure 6: The system structure of the FDN-RTM method.

The feedback coefﬁcient amn is deﬁned as:

amn =

ℓm,n
ℓm

subject to

N

Xn=1

amn = 1.

(7)

(8)

According to the energy conservation principle, Eqn. (8) will be satisﬁed if the form factors are

computed with ideal precision. Using the form-factor calculation in (15), the curved surface integration

is simpliﬁed to planar surface integration where the mean distance and angle are used as approximation.

When using such approximated form factors, each row only approximately sums to 1.

Note that the radiance transfer method models the propagation of the sound energy within an enclo-

sure, and thus is based on sound intensity. The feedback coefﬁcients estimated by the radiance transfer

method are also energy-based. Since the intended input of a FDN is the sound pressure of the source

signal and the intended output is the sound pressure of the reverberant signal, FDN parameters must be

12

designed to process sound pressure. With unit outgoing energy from group ℘m, the outgoing energy of

group ℘n due to the radiation from ℘m is amn. In real sound propagation this amount of energy amn may

contains multiple pulses. In the FDN-RTM model, this group of pulses are represented by a single pulse

with sound pressure of √amn, which contains the equal total sound energy. Thus, in order to synthesize

the sound pressure impulse response from the FDN-RTM structure with equivalent energy, the elements

in the feedback matrix need to be set to the square-root of the energy-based values.

√a11
...
√aN 1

A = 



· · ·
. . .

· · ·

√a1N
...
√aN N



.




(9)

The signs of these entries can then be chosen according to the procedure described in section 2.5.

2. Attenuation ﬁlters

The attenuation ﬁlters Hn(z) in traditional FDNs are low-pass ﬁlters, which model the frequency-

dependent absorption of the surface material and air (35). In the case of FDN-RTM, the attenuations

are energy-weighted mean reﬂection coefﬁcients of all the emitting patches within the group:

¯θ s
n =

Xi→j∈℘n

Fi,jθ s
i

(10)

where ¯θ s

n is the mean reﬂection coefﬁcient for frequency band s of group n, θ s
i

is the material reﬂection

coefﬁcient of patch i for frequency band s, and Fi,j the corresponding form factor.

In the Sabine formula the concept of absorption area is the equivalent absorption weighted by the

area of each material. In Eqn. (10), the absorption area of each patch is also expressed, but is hidden

in the form factor, where large patches emit more energy than small patches and thus their absorption

coefﬁcients take more importance in the summation.

13

(cid:1876)(cid:4666)(cid:1866)(cid:4667)

(cid:1877)(cid:4666)(cid:1866)(cid:4667)
(cid:2868)

(cid:2016)

(cid:1854)(cid:3043)

(cid:2879)(cid:2869)

(cid:1878)

Figure 7: Implementation of ﬁrst order low-pass ﬁlter.

(cid:4666)(cid:3398)(cid:4667)

For the sake of simplicity, the frequency-dependent absorption of the surface material is modeled

here as a ﬁrst order low-pass ﬁlter, as shown in Fig. 7. Higher order ﬁlters can be used if better quality is

required.

The frequency response of the ﬁrst order low-pass ﬁlter has a ratio (1

bp)/(1 + bp) between zero

−

and Nyquist frequencies. Thus, the factor bp can be estimated using the mean reﬂection coefﬁcients of

materials at zero and Nyquist frequencies as:

¯θ f s
n
¯θ 0
n

=

1
bp
−
1 + bp

.

(11)

However, acoustic absorptions of materials are often only available from 125 Hz until 4000 Hz or

8000 Hz. To determine the low-pass ﬁlter, the frequency-dependent gain of the ﬁlter in Fig. 7 is used:

2 =
H(ejω)
k
k

(1
1 + bp2

−
−

bp)2
2bpcos(ω)

(12)

Then, the absorptions coefﬁcients at 125 Hz and 4000 Hz or 8000 Hz can be used to determine the

low-pass ﬁlter coefﬁcient bp.

3. Delay units

In FDN-based artiﬁcial reverberators, the delay lengths are usually chosen as mutual prime numbers

around the mean free path given by:

14

¯d = 4V /S

(13)

where V is the total volume of the room, and S is total surface area enclosing the room. In (36) the

delays of delay units are also suggested to be distributed over a ratio of 1 : 1.5.

For the FDN-RTM, the mean delay length ¯dn for the nth delay line is calculated as the energy-

weighted mean delay length for all the patch-to-patch delays within that group:

i→j∈℘n Fi,jdi,j
i→j∈℘n Fi,j

¯dn = P
P

.

(14)

The delay lengths calculated using Eqn. (14) represent the true energy decay rate for each group, but

may not be the best combination for sound quality. For good sound synthesis quality, it is desirable to

adjust the delay lengths to be mutual prime numbers. For example, if we want to model the frequency

modes shown in Fig. 4, some of the delay units can be chosen as prime numbers around the peaks of

the delay distributions. Alternatively, we can choose the delay values that minimize the average error to

represent the delay distribution. In order to keep the energy decaying rate unchanged, the attenuation of

the delay line need to be modiﬁed accordingly when its length is adjusted:

θ′ = ¯θd′/ ¯d,

(15)

where ¯d is the mean delay length for a certain delay line calculated by Eqn. (14), ¯θ is the mean attenuation

by order-1 reﬂection, d′ is the adjusted delay length and θ′ is the attenuation factor that need to be

modiﬁed accordingly.

4. Pre-mixing and post-mixing coefﬁcients

We name z−b1, ..., z−b4 the pre-mixing delays, and b1, ..., b4 the gains in Fig. 6, and relate them to the

initial shooting matrix used in the radiance transfer model. The sound starting from the source is ﬁrst

15

reﬂected at the boundary into the designed groups. The delays z−b1, ..., z−b4 are chosen as the average

integer number of such initial delays. The coefﬁcients b1, ..., b4 are the amounts of energy emitted from

the source which go to each group after the initial shooting.

The post-mixing delays z−c1, ..., z−c4 and gains c1, ..., c4 are estimated from the ﬁnal gathering ma-

trix in a similar way.

2.4 Stability of the feedback delay networks

Although the parameters estimated above satisfy the energy-conservation principle, the feedback loops

in the feedback delay networks impose further constraints to keep the system stable.

Let G = ΘA where A is the feedback matrix and Θ is the attenuation coefﬁcient matrix. Using a

matrix form, the system equation of Fig. 6 becomes

where D is the delay matrix

Y = D(X + GY )

D =

z−d1








0

z−d2

. . .

0

.








z−dN

The iterative feedback given in Eqn. (16) can be rewritten as

Y = DX + DGDX + DGDGDX + . . . ,

= DX + D(GD)X + D(GD)2X + D(GD)3X + . . . .

If we decompose G using an eigenvalue decomposition,

G = P ΛP ′,

(16)

(17)

(18)

(19)

where the diagonal elements of Λ are the eigenvalues, and P is the eigenmatrix, the high order reﬂections

16

in Eqn. (18) are expressed as

D(GD)nX = D(P ΛP ′D)nX

= DP Λ P ′DP Λ P ′DP Λ P ′D . . . X

(20)

Since the product of the delay matrix D, the eigenmatrix P and its inverse (underlined in Eqn. (20)

)do not change the energy of the reﬂected sound, in order to make the system stable, it is sufﬁcient to

keep the largest eigenvalue in Λ smaller than 1. Note that the Λ gathers the eigenvalues of the product of

the feedback matrix and the reﬂection coefﬁcients matrix.

The non-negative matrix A, estimated using the radiance transfer method, has its dominant eigen-

value usually much larger than 1, which contains the DC component of its non-negative entries. There-

fore, the square-root matrix cannot be directly used as feedback matrix. It is known that real unitary

matrices, whose columns are mutually orthogonal, have their eigenvalues lying on the unit circle and

thus are feasible feedback matrices for lossless feedback systems. Switching the signs of certain entries

(+1 and

−

1) does not alter the energy of the system, but introduces phase inversions of the room impulse

response. Therefore, by intentionally designing the grouping scheme and switching the signs of some

entries as below, near orthogonal matrices are expected to be found for feedback delay networks.

2.5 Even-energy grouping scheme

The grouping scheme can be designed in different ways, with various factors under consideration. This

section gives an example of a grouping scheme with even energy for each group. Such a grouping scheme

gives near uniform-entry matrices which can be transformed to mutually orthogonal Hadamard matrices.

Scaled Hadamard matrices are widely used in feedback delay networks. Their entries are either

+1/√N or

−

1/√N , where N is the number of delay lines. The eigenvectors of such Hadamard matrices

all lie on the unit circle and the absolute value of the eigenvalues are constant and equal to 1.

There are several ways to construct Hadamard matrices. If H2k−1 is a Hadamard matrix of dimension

2k−1

×

2k−1, then the Hadamard matrix of dimension 2k

2k can be constructed from H2k−1 as:

×

17

Since the form factors contains several hidden geometry information in theirs values (for example,

H2k = 


H2k−1



.

H2k−1



(21)

−

H2k−1 H2k−1

the patch size, distance to patches and angles), patch-to-patch interactions with similar form factors are

likely to be obtained from neighboring or symmetric positions in the room. Also, because of the isotropic

scattering property of each patch, form factors with neighboring or symmetric positions are likely to have

similar scattering behaviors to other patches. Thus, in order to obtain a uniform-entry feedback matrix,

we design a grouping scheme which assigns form factors with similar scattering behaviors to different

groups. Their similarity in scattering behavior is indicated by the energy that they transport. The detailed

procedure is described below. Since it is not a closed-form solution, the grouping scheme listed here is

just an approach to obtain an approximated mutually orthogonal matrix.

The form factors are ﬁrst sorted into descending order by their transported energy. We assume that

the sorted energy is nearly linearly descending with the same decreasing step. We take the ﬁrst N form

factors and assign each of them to different groups, where N is the order of the desired feedback matrix.

Then for the N + 1 to 2N form factors, we do the same, assigning each of them to different group, but

in a circular order. An example of the assigning order for N = 4 is shown in Fig. 8.

A grouping scheme designed in this way guarantees that both the total transmitted energy in each

group and the scattered energy to the other groups are almost evenly distributed, which results in a near

uniform-entry feedback matrix. Such a matrix is then square-rooted and the signs of the entries are

switched according to the signs of the Hadamard matrix. The resulting matrix is an approximation of the

Hadamard matrix, which makes the feedback system stable and lossless, while still related to the main

physical phenomena.

2.6 FDN-RTM algorithm

The different steps of FDN-RTM algorithm are summarized in Fig. 9:

18

y
g
r
e
n
E

0.7

0.6

0.5

0.4

0.3

0.2

0.1

0

0

Even−Energy Grouping Scheme

Bin 1

Bin 2

Group 1
Group 2
Group 3
Group 4

Bin 3

Bin 4

2

4

6

8

10

12

14

16

18

Index

Figure 8: Even-energy group assignment scheme for FDN-RTM with four delay lines. The samples with
the same color denote that they are assigned to the same group.

1. Decompose the room geometry into patches.

2. Calculate the diffuse form factors for all patch-to-patch reﬂections using Eqn. (4). Calculate the

distribution of the patch-to-patch delays, and choose the delay units around the distribution peaks,

or quantized delay values which minimize the average error.

3. Sort the form factors based on the transported energy. Assign them to N groups as shown in Fig.

8.

4. Estimate the FDN-RTM parameters as detailed in Section 2.3: the feedback matrix using Eqns.

(5-8), the attenuation factors for each delay line using Eqn. (10), the low-pass ﬁlter coefﬁcients

using Eqn. (11) or (12), and the average delay length using Eqn. (14).

5. Square-root the feedback matrix and switch the signs of the entries according to the signs of the

Hadamard matrix.

19

1.  Geometry decomposition

2.  Diffuse form factor calculation

Delay units

3.  Even-energy grouping

4.  FDN parameters calculation

Initial/post delay & attenuations,

low-pass filter coefficients

5.  Sign switching

6.  Adjusting reflection factors

Feedback matrix

Reflection factors

Figure 9: Process of ﬁnding the parameters of the FDN-RTM algorithm.

6. Using Eqn. (15), adjust the attenuation factors of each delay line based on the average delay length

(in step 4) and the chosen delay units (in step 2).

3 Experimental Results

3.1 Experiment setup

We use both the proposed FDN-RTM and the traditional radiance transfer method, taken as reference,

to model the diffuse late reverberation of a set of 4 rooms with different shapes and dimensions. The

experimental geometries are displayed on Fig. 3.

Room 1 is a non-convex room with dimension 8m

×
0.9. Room 2 is a rectangular room with dimension 4.5m

×
3m

6m

3m and uniform reﬂection coefﬁcients are

×
2m. Room 4 is a more complex gallery geometry. The main shape of the gallery

×

2.5m. Room 3 is a long corridor with

dimension 16m

2m

×
is a rectangular room with dimension 19.6m

×

17.7m

×

×

5.1m, with four pillars and a few small box-

shaped or cones-shaped obstacles on the ﬂoor. The triangular mesh of the geometry is ﬁrst simpliﬁed

20

to 325 triangles in order to remove the ﬁne details of the architecture, and then decomposed into 435

patches with variable size for the computation of the patch-to-patch interactions. The wall reﬂection

factors of the four rooms are shown in Table 1.

Table 1: Wall reﬂection coefﬁcients settings of the experimental geometries.

Room index
1. Non-convex
2. Rectangular
3. Corridor
4. Gallery

8m
4.5m
16m

19.6m

Room size
6m
×
×
3m
×
×
2m
×
×
17.7m

×

×

3m
2.5m
2m

5.1m

ﬂoor

ceiling wall

0.4

0.9
0.8
0.9
0.75

0.7

3.2 Simulation results of Room 1

We use Room 1, the non-convex room, as an example to show the parameter estimation process for a

FDN-RTM of order 4 and its modeling results. The source and the receiver are both assumed to be

located at position [3, 3, 1.5]. The surfaces are divided into 164 patches, with uniform patch size of 1m2.

The delay units are chosen as the prime number around the peaks of the delay distribution. The feed-

back matrix is estimated using the even-energy grouping scheme, and is near orthogonal with eigenvalues

of amplitudes around 1 (0.997, 1.003, 1.001, 0.999).

Fig. 10 shows the synthesized reverberation (normalized) for room 1 using 4 delay lines. Thanks

to the feedback structure, the reﬂection density of simulated echoes increases rapidly as time increases,

which gives sufﬁcient echo density for late reverberation. The amplitudes of the echoes decrease smoothly

with an exponential rate. Listening tests of synthesized signal for speech and music samples shows sat-

isfying sound quality with smooth reverberant sound.

3.3 Comparison with radiance transfer method

The parameters of FDN-RTM are estimated from the Radiance Transfer Model. Thus, it is of interest to

see if the FDN-RTM models the energy decay in a similar way as the Radiance Transfer Method. We

21

Late reverberation for Room 1

0.2

0.4

0.6

0.8

1

Time [s]

1

0.5

0

e
s
n
o
p
s
e
r

l

e
s
u
p
m

I

−0.5

0

]

B
d
[

e
s
n
o
p
s
e
r

y
c
n
e
u
q
e
r
F

−5

−10

−15

−20

5

10

15

20

Frequency [KHz]

Figure 10: Room impulse response synthesized for Room 1 with FDN-RTM using four delay lines.

use the energy decay curve to compare these two methods.

Fig. 11 shows the energy decay curves of the impulse responses synthesized using FDN-RTM and

RTM for the four rooms. It is shown that the impulse responses estimated by FDN-RTM is exponentially

decreasing, which is shown as linearly decreasing on a logarithmic scale (dB). This is due to the fact

that the estimated feedback matrix is near unitary and system poles are of near unit amplitude. The

decay rates of FDN-RTM agree well with those simulated using RTM, which shows that FDN-RTM can

be used as an alternative method to RTM in application cases where the late reverberation is assumed

diffusely reﬂected and only the general decay rate is of interest to the perception.

Table 2 compares the reverberation time T60 estimated by the RTM (reference), FDN-RTM and

Sabine formula for the four rooms.

The reverberation times estimated by FDN-RTM agree well with those estimated by RTM and Sabine

formula1. However, the reverberation times estimated by FDN-RTM are slightly longer. The main

reason is that the feedback matrix estimated using even-energy grouping scheme has some eigenvalues

1The T60 estimated by Sabine formula for Room 4 is an approximated value where the volume of the room is simpliﬁed by

considering only the volume of the main shoe-box, the four pillars and the windows.

22

0

]

B
d
[

−20

−40

−60

0

0

]

B
d

[

−20

−40

−60

0

Room 1

Room 2

FND−RTM
RTM

0

−20

−40

FND−RTM
RTM

0.5

Room 3

FND−RTM
RTM

1

−60

0

0.5

1

Room 4

FND−RTM
RTM

0

−20

−40

0.5
Time [s]

1

−60

0

0.5
Time [s]

1

Figure 11: Energy decay curves of the impulse responses synthesized using FDN-RTM and RTM. The
x-axis is kept within [0, 1]s and the y-axis is within the range of [0,

60]dB.

−

slightly higher than the others, which makes the corresponding delay lines decay slower. For the four

rooms, both RTM and FDN-RTM predict similar T60 with limited deviation from those predicted by the

Sabine formula. While achieving comparable simulation results, FDN-RTM requires a much smaller

computation time than the RTM during the pre-computation stage and the real-time rendering. Table 3

compares the computation times for both methods on a standard PC.

Here, the pre-computation time for the RTM includes the decomposition of the geometry, the calcu-

lation of the form factors and the computation of high order patch-to-patch responses. The rendering for

the RTM includes the generation of the room impulse response and the convolution of the RIR with the

dry signal. The pre-computation for FDN-RTM method includes the decomposition of the geometry, the

calculation of form factors, the even-energy grouping scheme and the parameters estimation. The ren-

dering for FDN-RTM method includes the feedback loops of the input signal, which requires N 2 + kN

23

Table 2: Prediction of reverberation time using the RTM, FDN-RTM and Sabine formula. The reverber-
ation times are in seconds, and the errors are in percentage.

Room Sabine RTM FDN-

(s)
1.178
0.246
0.758
0.861

(s)
1.078
0.217
0.743
0.778

1
2
3
4

Error to
Error to
RTM (s) Sabine (%) RTM (%)
5.8
7.3
1.0
1.4

1.110
0.228
0.785
0.849

2.9
3.2
5.7
9.1

Table 3: Computation time consumed by RTM and FDN-RTM (4 delay lines) to compute the late rever-
beration of a one-second room impulse response. The machine used for this simulation has a 2.8GHz
CPU and 8G RAM.

FDN-RTM

Render Pre-comp. Render
(s)
1.93
0.42
0.46
13.34

(s)
30.4
7.1
8.0
85.8

(s)
0.07
0.09
0.09
0.09

Room Number of

patches
164
64
68
435

1
2
3
4

RTM
Pre-comp.

1 h 18 min
13 min
14 min
14 h 51 min

24

multiplications and some additions/delays during the feedback loops, as shown in Fig. 6, where N = 4

is the number of delay lines in this simulation.

To render a one-second signal, FDN-RTM uses less than 100ms during run-time, 2

3 order of

−

magnitudes less than needed by the RTM. This makes this geometry-based reverberator suitable for real-

time processing.

3.4 Order of delay lines

The FDN-RTM can be regarded as a simpliﬁed and efﬁcient implementation of the RTM using only a few

delay lines, while the original RTM uses several hundreds to thousands of patch-to-patch interactions.

By increasing the number of delay lines, the sound quality should be increased, at the price of a higher

computation load. The computation time for FDN-RTM using 4 to 16 delay lines is shown in Table 4. It

can be seen that the computation time increases almost linearly with the number of delay lines.

Table 4: Computation time consumed by FND-RTM using 4, 8 and 16 delay lines to compute the late
reverberation of a one-second room impulse response.

Number of delay lines
Computation time [s]

4
0.078

8
0.187

16
0.476

Informal listening tests2 using FDN-RTM for speech, singing voice and drum signals reveal that,

with the increase of delay lines, the sound quality has noticeable improvement, especially for drum and

speech signals. The light metallic sound for N = 4 with high reﬂection coefﬁcient (θ > 0.9) is reduced

when N = 16, with a smoother reverberant sound.

3.5 Frequency-dependent absorptivity

Instead of using uniﬁed absorption coefﬁcients, here the walls of Room 1 are assigned with frequency-

dependent absorption materials, as shown in Table 5.

2The sound examples can be found at http://perso.telecom-paristech.fr/~hbai/demo_ASLP2015/.

25

Table 5: Material absorption coefﬁcients from 125Hz to 4000Hz for Room 1.

Surfaces
Floor
Ceiling

Material
Carpet on concrete
Plaster

Wall (type 1) Curtain 1 and concrete
Wall (type 2) Curtain 2 and concrete

125Hz
0.02
0.02
0.14
0.03

250Hz
0.06
0.03
0.35
0.04

500Hz
0.14
0.04
0.55
0.11

1000Hz
0.37
0.05
0.72
0.17

2000Hz
0.60
0.06
0.70
0.24

4000Hz
0.65
0.08
0.65
0.35

Room 1

0.2

0.4

0.6

0.8

1

Time [s]

1

0.5

0

e
s
n
o
p
s
e
r
e
s
u
p
m

l

I

−0.5

0

0

−5

−10

−15

−20

−25

]

B
d

[

e
s
n
o
p
s
e
r

y
c
n
e
u
q
e
r
F

5

10

15

20

Frequency [KHz]

Figure 12: Impulse response of frequency-dependent absorption.

The absorption coefﬁcients at 125Hz and 4000Hz are combined to determine the low-pass ﬁlter

coefﬁcient bp. The synthesized room impulse response with frequency low-pass character is shown in

Fig. 12.

The frequency-dependent reverberation is well modeled, as seen on the energy decay shown in Fig.

13, where the T60 at zero frequency is about 1.15s, while at 22.05kHz it is only 0.4s. Still, the ﬁrst-order

low-pass ﬁlters cannot model precisely the complex frequency-dependency absorption of real rooms.

Higher order IIR ﬁlters can be used for more accurate results.

26

Figure 13: Frequency-dependent energy decay for Room 1 modeled with the low-pass ﬁlter.

4 Conclusions

In this paper we propose a new method which implements a simpliﬁed radiance transfer method using

the Feedback Delay Network structure. It inherits the accuracy of the physical model of the acoustic

radiance transfer method, at a much lower computational load. The sound quality approaches that of

the radiance transfer method, with the increase of delay lines. This method can simulate room acoustic

characteristics, and synthesize virtual reverberation in real time, taking into account the physical char-

acteristics of the simulated room. Further extensions can investigate the grouping scheme, in order to

obtain a more ﬂexible group design, and possibly to introduce spatial information for each group.

27

References

1. T. Lokki, L. Savioja, R. Väänänen, J. Huopaniemi, T. Takala, IEEE Computer Graphics and Appli-

cations 22, 49 (2002).

2. T. Funkhouser, P. Min, I. Carlbom, Proceedings of the 26th annual conference on Computer graphics

and interactive techniques (ACM Press/Addison-Wesley Publishing Co., 1999), pp. 365–374.

3. T. Ajdler, L. Sbaiz, M. Vetterli, IEEE Transactions on Signal Processing 54, 3790 (2006).

4. R. Mignot, G. Chardon, L. Daudet, SPIE Optical Engineering+ Applications (International Society

for Optics and Photonics, 2011), pp. 813808–813808.

5. L. Bianchi, D. Markovic, F. Antonacci, A. Sarti, S. Tubaro, Workshop on Application of Signal

Processing to Audio and Acoustics (WASPAA) (IEEE, 2013), pp. 1–4.

6. H. Kuttruff, Room acoustics (CRC Press, 2009).

7. I. Dokmani´c, R. Parhizkar, A. Walther, Y. M. Lu, M. Vetterli, Proceedings of the National Academy

of Sciences 110, 12186 (2013).

8. J. B. Allen, D. A. Berkley, The Journal of the Acoustical Society of America 65, 943 (1979).

9. J. Borish, The Journal of the Acoustical Society of America 75, 1827 (1984).

10. A. Krokstad, S. Strom, S. Sørsdal, Journal of Sound and Vibration 8, 118 (1968).

11. P. S. Heckbert, P. Hanrahan, ACM SIGGRAPH Computer Graphics 18, 119 (1984).

12. T. Funkhouser, et al., Proceedings of the 25th annual conference on Computer graphics and inter-

active techniques (ACM, 1998), pp. 21–32.

13. G. I. Koutsouris, J. Brunskog, C.-H. Jeong, F. Jacobsen, The Journal of the Acoustical Society of

America 133, 3963 (2013).

28

14. M. Hodgson, E.-M. Nosal, The Journal of the Acoustical Society of America 120, 808 (2006).

15. E.-M. Nosal, M. Hodgson, I. Ashdown, The Journal of the Acoustical Society of America 116, 970

(2004).

16. S. Siltanen, T. Lokki, L. Savioja, Acta Acustica united with Acustica 95, 106 (2009).

17. L. Antani, A. Chandak, M. Taylor, D. Manocha, IEEE Transactions on Visualization and Computer

Graphics 18, 261 (2012).

18. L. Antani, A. Chandak, L. Savioja, D. Manocha, ACM Transactions on Graphics (TOG) 31, 7 (2012).

19. M. R. Schroeder, Journal of the Audio Engineering Society 10, 219 (1962).

20. J. A. Moorer, Computer music journal pp. 13–28 (1979).

21. J. Stautner, M. Puckette, Computer Music Journal 6, 52 (1982).

22. J.-M. Jot, A. Chaigne, Audio Engineering Society Convention 90 (Audio Engineering Society, 1991).

23. L. Dahl, J.-M. Jot, Proceedings of the COST G-6 Conference on Digital Audio Effects (DAFX-00),

Verona, Italy (2000).

24. F. Menzer, Audio Engineering Society Conference: 40th International Conference: Spatial Audio:

Sense the Sound of Space (Audio Engineering Society, 2010).

25. B. Holm-Rasmussen, H.-M. Lehtonen, V. Välimäki, Proceedings of the COST G-6 Conference on

Digital Audio Effects (DAFX-13) (2013).

26. E. Vickers, J.-L. L. Wu, P. G. Krishnan, R. N. K. Sadanandam, Proceedings of the AES 121th Con-

vention (2006).

27. V. Valimaki, J. D. Parker, L. Savioja, J. O. Smith, J. S. Abel, IEEE Transactions on Audio, Speech,

and Language Processing 20, 1421 (2012).

29

28. D. Rocchesso, J. O. Smith, IEEE Transactions on Speech and Audio Processing 5, 51 (1997).

29. E. De Sena, H. Hacihabiboglu, Z. Cvetkovic, Audio Engineering Society Conference: 41st Interna-

tional Conference: Audio for Games (Audio Engineering Society, 2011).

30. M. Chemistruck, K. Marcolini, W. Pirkle, Audio Engineering Society Convention 133 (Audio Engi-

neering Society, 2012).

31. S. Siltanen, T. Lokki, S. Kiminki, L. Savioja, The Journal of the Acoustical Society of America 122,

1624 (2007).

32. H. Bai, G. Richard, L. Daudet, Workshop on Applications of Signal Processing to Audio and Acous-

tics (WASPAA) (IEEE, 2013), pp. 1–4.

33. M. Carroll, C. Chien, The Journal of the Acoustical Society of America 62, 1442 (1977).

34. J.-M. Jot, A. Chaigne, Audio Engineering Society Convention 90 (Audio Engineering Society, 1991).

35. J. Huopaniemi, V. Valimaki, M. Karjalainen, R. Vaananen, Proceedings of the International Com-

puter Music Conference, Thessaloniki, Hellas (The International Computer Music Association,

1997), pp. 200–203.

36. Z. Raﬁi, B. Pardo, Proceedings of the 10th International Society for Music Information Retrieval

Conference (ISMIR) (2009), pp. 285–290.

30