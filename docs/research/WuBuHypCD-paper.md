# WuBu Nesting: An Adaptive Multi-Scale Nested Geometric Framework with Tangent Space Rotations, Relative Geometry, Level Descriptors, and Dynamic Flows

**(Comprehensive Conceptual Paper)**

**Abstract**

Modeling the intricacies of real-world data often necessitates capturing a confluence of complex characteristics: deep multi-scale hierarchical organizations, intrinsic rotational symmetries or transformations between structural levels, dynamic evolution within scales, and varying degrees of uncertainty or regional influence. Existing geometric deep learning paradigms, while powerful, often specialize. Standard Euclidean models struggle with hierarchical data, traditional hyperbolic models excel at single-level hierarchies but typically lack integrated rotational mechanics and multi-scale adaptivity, and quaternion-based models handle rotations efficiently but usually lack inherent hierarchical or multi-scale geometric structure. To bridge these gaps and provide a more holistic geometric inductive bias, we introduce **WuBu Nesting (層疊嵌套 - céngdié qiàn​tào: "layered nesting")**, a novel and comprehensive conceptual framework. WuBu Nesting features a recursively nested structure of hyperbolic spaces ($\mathbb{H}^{n_1}_{c_1,s_1} \supset \mathbb{H}^{n_2}_{c_2,s_2} \supset \dots$), where the dimensionality ($n_i$), curvature ($c_i > 0$), and relative scale ($s_i > 0$) of each hyperbolic "bubble" can be learned, allowing the geometry to adapt dynamically to data complexity and distribution. Within each level $i$, the framework accommodates learnable **Boundary Sub-Manifolds** (e.g., parameterized point sets representing "circles" or landmarks) symbolizing distinct substructures or feature clusters relevant at that scale. Crucially, transitions between adjacent levels ($i \rightarrow i+1$) are orchestrated via sophisticated, learnable transformations operating within the flat **Euclidean tangent spaces** ($T_p(\mathbb{H}^{n_i}) \cong \mathbb{R}^{n_i}$) associated with the hyperbolic manifolds. These inter-level transformations are deliberately decomposed into a learned **Rotation** component ($R_i$, implemented efficiently via Quaternions for 4D or general SO($n_i$) matrices) applied *simultaneously* to the primary data representation, the representations of boundary manifolds (mapped to tangent vectors), and a novel **learnable Level Descriptor Vector ($\vec{ld}_i$)** intrinsic to the source level. This rotation is followed by a learnable **non-rotational Mapping** ($\tilde{T}_i$) that adjusts features and potentially dimensionality, producing transformed vectors in the target tangent space ($T_o(\mathbb{H}^{n_{i+1}})$). From these transformed vectors, we compute **Relative Vectors ($\vec{d}_{i+1, j, k}$)** between the primary representation and the boundary representations, explicitly encoding rotation-aware spatial relationships in the target tangent space. Furthermore, each level $i$ possesses a learnable **Level Spread Parameter ($\sigma_i$)**, representing characteristic uncertainty or density, which is passed as context to the next level. The framework also allows for **Intra-Level Tangent Flow ($F_i$)**, a learnable dynamic transformation applied within the tangent space during a level's internal processing step, modeling localized evolution or adjustment. The inputs informing the processing at level $i+1$ thus include the primary representation mapped back into $\mathbb{H}^{n_{i+1}}$, the computed relative vectors $\{\vec{d}_{i+1, j, k}\}$, the transformed level descriptor $\vec{ld}_{i+1}$, and the contextual spread parameter $\sigma_i$. This rich, multi-faceted information stream allows WuBu Nesting to capture scale-aware, rotation-informed, dynamic, and density-sensitive relationships, offering an exceptionally flexible and powerful geometric framework adaptable to the profound complexity of real-world data exhibiting intertwined hierarchical, rotational, dynamic, and uncertain characteristics.

## 1. Introduction

The quest for effective data representation lies at the heart of machine learning. Standard deep learning architectures, while achieving remarkable success, predominantly operate within the confines of Euclidean geometry. This geometric choice, however, imposes limitations when modeling data imbued with strong intrinsic structures not naturally suited to flat spaces. A prominent example is hierarchical data, such as taxonomies, phylogenetic trees, complex molecules, articulated objects, or parse trees, where relationships exhibit a natural parent-child structure. Embedding such hierarchies into Euclidean space often incurs significant distortion, as the space's polynomial volume growth struggles to accommodate the exponential expansion of nodes typically found in trees [39].

Hyperbolic geometry, characterized by its constant negative curvature and exponential volume growth relative to radius, offers a mathematically elegant and practically effective solution for embedding hierarchical structures with significantly lower distortion [39, 31, 15]. Models leveraging spaces like the Poincaré disk or ball ($\mathbb{H}^n$) have demonstrated substantial benefits in tasks ranging from graph embedding and natural language processing [19, 22] to computer vision [31, 15, 1] and category discovery [42]. These successes underscore the power of aligning the model's geometric inductive bias with the data's underlying structure.

However, many real-world systems exhibit complexities beyond a single, static hierarchy. Firstly, hierarchies themselves can be **nested**: structures contain sub-structures which themselves possess internal hierarchies (e.g., a molecule composed of domains, composed of secondary structures, composed of residues). Secondly, components within these structures often possess **intrinsic orientations**, and transformations between different levels or viewpoints frequently involve **rotations**. For instance, analyzing articulated objects requires understanding part hierarchies alongside their relative orientations and movements, while modeling protein interactions involves recognizing hierarchical domains and their rotational alignment during docking. Existing hyperbolic models typically focus on embedding a single hierarchy level within a single hyperbolic space of fixed curvature and lack native, efficient mechanisms for modeling rotations or adaptively handling multiple scales of hierarchy.

Conversely, **Quaternions** [43] provide an exceptionally compact and computationally efficient algebra for representing and manipulating rotations, particularly in 3D and 4D. Quaternion Neural Networks (QNNs) [44, 45] have leveraged this power for tasks involving orientation and 3D data, demonstrating parameter efficiency and improved performance. However, QNNs typically operate within Euclidean spaces and lack the intrinsic capacity for hierarchical embedding offered by hyperbolic geometry. Combining different geometries via product spaces (e.g., $\mathbb{R}^n \times \mathbb{S}^m \times \mathbb{H}^k$) [46] offers increased capacity by arranging spaces in parallel, but does not directly address nested hierarchies or integrated rotational transformations *between* hierarchical levels.

This paper introduces **WuBu Nesting (層疊嵌套)**, a comprehensive conceptual framework meticulously designed to bridge these gaps. WuBu Nesting aims to unify adaptive multi-scale hierarchical representation with explicit modeling of rotational dynamics, dynamic evolution, and regional uncertainty within a single, cohesive geometric structure. Instead of a single hyperbolic space or a parallel product manifold, WuBu Nesting proposes a nested "Russian doll" architecture comprising recursively embedded hyperbolic manifolds. The key innovations, detailed extensively in this paper, are:

1.  **Adaptive Nested Hyperbolic Geometry:** The core structure is a sequence of nested hyperbolic spaces, $\mathbb{H}^{n_1}_{c_1, s_1} \supset \mathbb{H}^{n_2}_{c_2, s_2} \supset \dots$. Critically, the dimensionality ($n_i$), curvature ($c_i > 0$), and a relative scale parameter ($s_i > 0$, influencing the zoom/density) of each hyperbolic "bubble" can be learnable parameters, allowing the overall geometry to dynamically adapt its capacity and structure to the specific data distribution and complexity.
2.  **Boundary Sub-Manifolds:** Each hyperbolic level $\mathbb{H}^{n_i}$ can host learnable, lower-dimensional **Boundary Sub-Manifolds** ($B_{i,j}$), such as sets of points representing hyperbolic disks ("circles") or other landmark configurations. These symbolize distinct substructures, components, or feature clusters pertinent to the scale represented by level $i$.
3.  **Tangent Space Transitions:** Transitions between levels ($i \rightarrow i+1$) are mediated not directly in the curved hyperbolic spaces, but within their associated **Euclidean Tangent Spaces** ($T_p(\mathbb{H}^{n_i}) \cong \mathbb{R}^{n_i}$). This allows leveraging the well-understood properties and operations of Euclidean vector spaces for complex transformations.
4.  **Explicit Tangent Space Rotations ($R_i$):** A core component of the inter-level transition is a learnable **Rotation** $R_i$. This rotation operates within the tangent space $T_o(\mathbb{H}^{n_i})$ and can be implemented using efficient quaternion multiplication (if $n_i=4$) or general SO($n_i$) rotation matrices (parameterized appropriately).
5.  **Simultaneous Transformation:** The learned rotation $R_i$ is applied *consistently and simultaneously* to the tangent vector representing the main data point ($v_i$), the tangent vectors representing the boundary manifolds ($v_{b_{i,j,k}}$), and a learnable Level Descriptor Vector ($\vec{ld}_i$). This ensures that the relative orientations of all relevant features are preserved and correctly transformed into the rotated frame.
6.  **Non-Rotational Mapping ($\tilde{T}_i$):** Following the rotation, a learnable **non-rotational mapping** $\tilde{T}_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_{i+1}})$ is applied. This mapping handles potential dimension changes ($n_i \rightarrow n_{i+1}$), applies non-linear feature transformations (e.g., using MLPs), and prepares the vectors for the target tangent space of the next level. The full inter-level tangent transformation is thus $T_{i \rightarrow i+1} = \tilde{T}_i \circ R_i$.
7.  **Relative Vector Generation ($\vec{d}_{i+1}$):** After the full tangent space transformation ($T_{i \rightarrow i+1}$), **Relative Vectors** ($\vec{d}_{i+1, j, k} = v_{i+1} - v''_{b_{i,j,k}}$) are computed in the target tangent space $T_o(\mathbb{H}^{n_{i+1}})$. These vectors explicitly encode the spatial relationship between the transformed primary data representation ($v_{i+1}$) and the transformed boundary representations ($v''_{b_{i,j,k}}$), capturing geometry informed by the inter-level rotation.
8.  **Learnable Level Descriptor Vector ($\vec{ld}_i$):** Each level $i$ possesses an intrinsic, learnable **Level Descriptor Vector** $\vec{ld}_i \in T_o(\mathbb{H}^{n_i})$. This vector, transformed alongside other features to $\vec{ld}_{i+1}$ for the next level, potentially captures scale-specific anisotropy, a dominant feature direction, or other characteristic geometric properties of the level itself.
9.  **Learnable Level Spread Parameter ($\sigma_i$):** Each level $i$ is associated with a learnable scalar **Level Spread Parameter** $\sigma_i > 0$, representing the characteristic "atmosphere," uncertainty radius, or density falloff at that scale. This parameter is passed as contextual information to the subsequent level ($i+1$).
10. **Intra-Level Tangent Flow ($F_i$):** The framework allows for a learnable **Intra-Level Tangent Flow** field $F_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_i})$, applied during the internal processing stage of level $i$. This mechanism can model scale-specific dynamics, adjustments, or "orbital" transformations within the level's tangent space representation before output.
11. **Rich Hierarchical Information Flow:** The processing module within level $i+1$ receives a comprehensive set of inputs: the primary representation mapped into $\mathbb{H}^{n_{i+1}}$, the set of relative vectors $\{\vec{d}_{i+1, j, k}\}$, the transformed level descriptor $\vec{ld}_{i+1}$, and the contextual spread parameter $\sigma_i$ from level $i$. This rich input stream allows the model to make decisions based on position, relative structure, orientation, level characteristics, and source level uncertainty.

We hypothesize that this deeply integrated, multi-faceted geometric structure – encompassing nested adaptivity, explicit boundaries, tangent space rotations, relative geometry, level descriptors, spread context, and intra-level dynamics – provides an exceptionally powerful and flexible inductive bias. WuBu Nesting is proposed as a foundational framework capable of modeling complex real-world systems where hierarchical, rotational, dynamic, and uncertainty characteristics are inextricably intertwined.

## 2. Related Work

The WuBu Nesting framework draws inspiration from, and aims to synthesize concepts across, several distinct areas of geometric deep learning and representation learning.

### 2.1 Hyperbolic Deep Learning

The seminal work of Nickel and Kiela [39] demonstrated the aptitude of hyperbolic geometry, specifically the Poincaré ball model, for embedding hierarchical data structures like taxonomies with significantly lower distortion compared to Euclidean counterparts. This spurred a wave of research exploring hyperbolic geometry for various machine learning tasks. Key developments include:
*   **Hierarchical Embeddings:** Further applications in embedding trees [31], graphs [15], and ontologies [1].
*   **Hyperbolic Neural Networks:** Defining analogues of standard neural network operations within hyperbolic space, such as fully connected layers (Gyroplane layers) [19], attention mechanisms [22], and convolutions [ref: Hyperbolic CNNs, if available]. Ganea et al. [19] provided foundational work on hyperbolic neural networks using the gyrovector space formalism.
*   **Computer Vision:** Applying hyperbolic embeddings to image classification [31], image retrieval [15], object detection [ref: Hyperbolic Object Detection], semantic segmentation [ref: Hyperbolic Semantic Segmentation], and category discovery (e.g., HypCD [42]), often showing benefits where hierarchical part relationships or semantic hierarchies are relevant.

**Critique & WuBu Distinction:** While foundational, these methods predominantly utilize a *single* hyperbolic space with a *fixed* curvature. They typically lack mechanisms for handling *nested* hierarchies adaptively and do not incorporate explicit modeling of *rotational* transformations or the other novel components introduced in WuBu Nesting (boundaries, relative vectors, descriptors, spread, flow). HypCD [42], for example, shows the benefit of a single hyperbolic space for GCD but doesn't employ nesting or explicit rotations.

### 2.2 Quaternion Neural Networks (QNNs)

Quaternions [43], a four-dimensional normed division algebra extending complex numbers, offer a highly efficient representation for 3D rotations. QNNs [44, 45] leverage this property:
*   **Parameter Efficiency:** Quaternion-valued weights and operations can significantly reduce the number of parameters compared to equivalent real-valued networks for tasks involving 3D/4D structure or rotations.
*   **Rotational Equivariance/Invariance:** QNNs can be designed to better respect rotational symmetries.
*   **Applications:** Primarily successful in areas like 3D computer vision, robotics, signal processing, and physics simulations where orientation and rotation are critical.

**Critique & WuBu Distinction:** QNNs operate primarily in Euclidean space (or spaces easily representable with quaternions). They lack the intrinsic geometric bias for hierarchical embedding provided by hyperbolic spaces. WuBu Nesting incorporates rotational modeling (potentially using quaternions when $n_i=4$) but does so within a *tangent space transition* mechanism *between nested hyperbolic levels*, thus integrating rotation with adaptive multi-scale hierarchy.

### 2.3 Product Manifolds and Multi-Scale Approaches

To combine the strengths of different geometries, some approaches utilize **Product Manifolds** [46], creating spaces like $\mathbb{R}^n \times \mathbb{S}^m \times \mathbb{H}^k$.
*   **Increased Capacity:** Allows simultaneous representation in spaces with different inductive biases (e.g., Euclidean for attributes, Spherical for directions, Hyperbolic for hierarchy).
*   **Parallel Structure:** Geometries are typically arranged in parallel; information is processed within each component space and then aggregated.

Traditional **Multi-Scale Methods** in deep learning (e.g., feature pyramids in vision, wavelet transforms) typically operate in Euclidean space, extracting features at different spatial resolutions or frequency bands.

**Critique & WuBu Distinction:** Product manifolds offer parallel capacity but do not inherently model the *nested*, "Russian doll" structure proposed by WuBu Nesting. Transitions and interactions between the different geometric components in product spaces are often handled via simple concatenation or aggregation, lacking the sophisticated, rotation-aware tangent space transformations of WuBu. Standard multi-scale methods lack the specific geometric biases of hyperbolic spaces and the integrated rotational modeling. WuBu Nesting proposes a fundamentally different architecture based on *recursive embedding* and *geometrically meaningful transitions*, integrating hierarchy, scale, rotation, dynamics, and uncertainty in a deeply unified manner.

In summary, WuBu Nesting distinguishes itself by proposing a novel synthesis: an *adaptive, nested hyperbolic* structure providing multi-scale hierarchy, combined with *explicit tangent space rotations and mappings* for handling orientation changes between levels, further enriched by *learnable boundary manifolds, relative vectors, level descriptors, spread parameters, and intra-level flows* to capture unprecedented geometric detail and dynamics.

## 3. The WuBu Nesting Framework

WuBu Nesting offers a recursive, multi-layered geometric architecture where data representations are progressively refined through a series of nested hyperbolic "bubbles." Transitions between these bubbles are orchestrated in their associated Euclidean tangent spaces, incorporating learnable rotations, mappings, and the generation of rich geometric features like relative vectors, while also considering level-specific descriptors, spread, and internal dynamics.

### 3.1. Conceptual Architecture

The core concept envisions data flowing through a hierarchy of processing stages, each associated with a hyperbolic space $\mathbb{H}^{n_i}_{c_i, s_i}$. An initial encoding maps the input data into the tangent space of the outermost hyperbolic level. Within each level $i$, the representation undergoes processing which may involve an intra-level tangent flow $F_i$. To transition to the next, deeper level $i+1$, the representation (along with boundary manifold representations and the level descriptor vector) is mapped to the tangent space $T_o(\mathbb{H}^{n_i})$ via the logarithmic map. Here, a learned rotation $R_i$ is applied simultaneously to all these vectors. Subsequently, a learnable non-rotational mapping $\tilde{T}_i$ transforms these rotated vectors into the tangent space $T_o(\mathbb{H}^{n_{i+1}})$ of the next level, potentially changing dimensionality. In this target tangent space, relative vectors ($\vec{d}_{i+1, j, k}$) are computed between the main transformed vector and the transformed boundary vectors. The main transformed vector $v_{i+1}$, the relative vectors $\{\vec{d}_{i+1, j, k}\}$, the transformed level descriptor $\vec{ld}_{i+1}$, and the spread parameter $\sigma_i$ from the source level collectively form the input for processing within level $i+1$. The main vector $v_{i+1}$ is typically mapped into the hyperbolic ball $\mathbb{H}^{n_{i+1}}$ using the exponential map for hyperbolic operations within that level. This process repeats recursively through the nested levels. Finally, information aggregated across relevant levels and tangent spaces is used for the final task prediction.

```mermaid
graph TD;
    A[Input Data] --> B(Initial Euclidean Encoding);
    B --> C{Map to Tangent Space T_o(H1)};

    subgraph "Level 1: Outer (H_n1, c1, s1)"
        style L1 fill:#D6C0FF,stroke:#9966FF,stroke-width:2px
        C -- Tangent Input v1_in --> Proc1{Intra-Ball Processing L1};
        subgraph "Level 1 Params & State"
             LD1(Learnable Desc. ld1);
             Sigma1(Learnable Spread σ1);
             Flow1(Learnable Flow F1);
             BM1{Boundary Manifolds B1jk};
             BM1 --> BM1_P(Points {b_1jk} in H1);
        end
        %% Processing uses v1_in, context from previous (if any), applies F1
        Proc1 -- Internal State --> D(Hyperbolic Rep x1_out in H1);
        D --> F[LogMap: x1_out -> v1_out in T_o(H1)]; %% Main Tangent Out
        BM1_P --> FBT1(LogMap: {b_1jk} -> {v_b1jk} in T_o(H1)); %% Boundary Tangent Out
    end

    subgraph "Inter-Level Transformation T(1->2)"
        direction TB
        F -- Main Tangent v1_out --> R1{Rotate v1_out, {v_b1jk}, ld1 by R1};
        FBT1 -- Boundary Tangents {v_b1jk} --> R1;
        LD1 -- Level Desc. ld1 --> R1;
        R1 -- Rotated v1', {v_b1jk'}, ld1' --> T1(Apply Map ~T1);
        T1 -- Main Tangent v2 --> V2{Target Tangent T_o(H2)};
        T1 -- Transformed Boundary Tangents {v_b1jk''} --> V2;
        T1 -- Transformed Level Desc. ld2 --> V2;
        V2 -- Compute d2jk = v2 - v_b1jk'' --> VectorsD2(Relative Vectors {d2jk});
        Sigma1 -- Pass Context --> Ctx2(Context for L2: σ1); %% Spread passed as context
    end

    %% Connect Transform Output to Next Level
    V2 -- Main Tangent v2 --> I1{Input Tangent for L2};
    VectorsD2 -- Relative Vectors {d2jk} --> Proc2_Input;
    V2 -- Level Desc. ld2 --> Proc2_Input;
    Ctx2 -- Spread Context σ1 --> Proc2_Input;

    subgraph "Level 2: Middle (H_n2, c2, s2)"
        style L2 fill:#F5F0FF,stroke:#D6C0FF,stroke-width:2px
        I1 --> MapToH2(Optional ExpMap: v2 -> x2_in in H2);
        Proc2_Input(Gather Inputs) --> Proc2{Intra-Ball Processing L2};
        MapToH2 --> Proc2; %% Primary representation
         subgraph "Level 2 Params & State"
             LD2(Learnable Desc. ld2_param);
             Sigma2(Learnable Spread σ2);
             Flow2(Learnable Flow F2);
             BM2{Boundary Manifolds B2jk};
             BM2 --> BM2_P(Points {b_2jk} in H2);
        end
         %% Processing uses v2 (or x2_in), {d2jk}, ld2, σ1, applies F2
        Proc2 -- Internal State --> J1(Hyperbolic Rep x2_out in H2);
        J1 --> L1[LogMap: x2_out -> v2_out in T_o(H2)]; %% Main Tangent Out
        BM2_P --> FBT2(LogMap: {b_2jk} -> {v_b2jk} in T_o(H2)); %% Boundary Tangent Out
    end

    subgraph "Inter-Level Transformation T(2->3)"
        direction TB
        L1 -- Main Tangent v2_out --> R2{Rotate v2_out, {v_b2jk}, ld2_param by R2};
        FBT2 -- Boundary Tangents {v_b2jk} --> R2;
        LD2 -- Level Desc. ld2_param --> R2;
        R2 -- Rotated v2', {v_b2jk'}, ld2' --> T2(Apply Map ~T2);
        T2 -- Main Tangent v3 --> V3{Target Tangent T_o(H3)};
        T2 -- Transformed Boundary Tangents {v_b2jk''} --> V3;
        T2 -- Transformed Level Desc. ld3 --> V3;
        V3 -- Compute d3jk = v3 - v_b2jk'' --> VectorsD3(Relative Vectors {d3jk});
        Sigma2 -- Pass Context --> Ctx3(Context for L3: σ2); %% Spread passed as context
    end

     %% Connect Transform Output to Next Level
    V3 -- Main Tangent v3 --> M1{Input Tangent for L3};
    VectorsD3 -- Relative Vectors {d3jk} --> Proc3_Input;
    V3 -- Level Desc. ld3 --> Proc3_Input;
    Ctx3 -- Spread Context σ2 --> Proc3_Input;

    M1 --> Proc3_Input(Gather Inputs);
    Proc3_Input --> N[Intra-Ball Processing L3 ...];
    N --> O{Aggregate Information};
    O --> P[Final Projection / Task Head];
    P --> Q[Output];

    %% Styling (Add styles for new elements)
    classDef level1 fill:#D6C0FF,stroke:#9966FF,stroke-width:1px;
    classDef level2 fill:#F5F0FF,stroke:#D6C0FF,stroke-width:1px;
    classDef level3 fill:#E6E6FF,stroke:#C0C0FF,stroke-width:1px;
    classDef boundary fill:#E7DAFF, stroke:#9966FF, stroke-dasharray: 5 5, stroke-width:1px;
    classDef rotation fill:#FFD700,stroke:#B8860B,stroke-width:2px;
    classDef tangent fill:#FFF,stroke:#BBB,stroke-dasharray: 2 2, stroke-width:1px;
    classDef ball fill:#EFE,stroke:#AEA, stroke-width:1px;
    classDef transform fill:#FFE,stroke:#DDA, stroke-width:1px;
    classDef vector_gen fill:#ADD8E6, stroke:#4682B4, stroke-width:1px;
    classDef leveldesc fill:#FFFACD,stroke:#BDB76B,stroke-width:1px, stroke-dasharray: 3 3;
    classDef spread fill:#FAFAD2,stroke:#BDB76B,stroke-width:1px, stroke-dasharray: 1 1;
    classDef flow fill:#E0FFFF,stroke:#008B8B,stroke-width:1px, stroke-dasharray: 4 4;
    classDef processing fill:#F0FFF0,stroke:#2E8B57, stroke-width:1.5px;

    class Proc1, Proc2, N processing;
    class BM1, BM2 boundary;
    class R1, R2 rotation;
    class C, F, FBT1, V2, L1, FBT2, V3, M1 tangent; class I1 tangent; %% Inputs to levels
    class D, BM1_P, J1, BM2_P, MapToH2 ball; %% Hyperbolic states
    class T1, T2 transform;
    class VectorsD2, VectorsD3 vector_gen;
    class LD1, LD2 leveldesc;
    class Sigma1, Sigma2, Ctx2, Ctx3 spread;
    class Flow1, Flow2 flow;
    class Proc2_Input, Proc3_Input processing; %% Input gathering nodes
```
**Figure 1:** Conceptual Architecture of the Comprehensive WuBu Nesting Framework. This diagram illustrates the flow through nested hyperbolic levels ($\mathbb{H}^{n_i}_{c_i, s_i}$) with adaptive parameters. It highlights key components: learnable Boundary Manifolds ($B_{ijk}$), Level Descriptors ($\vec{ld}_i$), Level Spreads ($\sigma_i$), and Intra-Level Tangent Flows ($F_i$). Inter-level transitions involve tangent space mapping (LogMap), simultaneous Rotation ($R_i$) of primary, boundary, and descriptor vectors, followed by a Mapping ($\tilde{T}_i$). Relative Vectors ($\vec{d}_{i+1}$) are computed in the target tangent space. The next level's processing utilizes the transformed primary vector ($v_{i+1}$ or $x_{i+1}$), relative vectors ($\vec{d}_{i+1}$), transformed descriptor ($\vec{ld}_{i+1}$), and contextual spread ($\sigma_i$).

### 3.2. Component Details

We now elaborate on each distinct component of the WuBu Nesting framework.

#### 3.2.1 Nested Hyperbolic Spaces & Adaptive Geometry
The foundational structure is a sequence of **nested hyperbolic spaces**. We typically employ the **Poincaré Ball model** for each level $i$, denoted $\mathbb{H}^{n_i}_{c_i, s_i}$.
*   **Nesting:** The embedding conceptually proceeds from an outer, potentially lower-curvature space $\mathbb{H}^{n_1}$ to progressively deeper, potentially higher-curvature or differently scaled spaces $\mathbb{H}^{n_2}, \mathbb{H}^{n_3}, \dots$. This nesting allows the model to capture hierarchical structure across multiple scales.
*   **Dimensionality ($n_i$):** The dimension $n_i$ of the hyperbolic space at level $i$ can vary between levels. This allows the model to allocate representational capacity differently across the hierarchy. $n_i$ could be a hyperparameter or potentially learned/selected via architecture search.
*   **Curvature ($c_i$):** The curvature parameter $c_i > 0$ (where the manifold curvature is typically $-c_i^2$) determines the "steepness" of the geometry at level $i$. Higher curvature leads to faster volume growth and potentially better embedding of deep hierarchies within that level. $c_i$ can be a fixed hyperparameter per level or, more powerfully, a **learnable parameter**, allowing the model to adapt the geometry's intensity at each scale. Learning requires careful optimization (e.g., using projected gradient descent or Riemannian optimization) to keep $c_i > 0$.
*   **Scale ($s_i$):** We introduce a **learnable positive scale parameter** $s_i > 0$ for each level $i$. This parameter acts as a "zoom factor" modulating the relationship between the tangent space and the hyperbolic ball, typically incorporated into the scale-aware exponential and logarithmic maps. Conceptually, a scale-aware exponential map might resemble $\text{exp}_{o,s_i}^{c_i}(v) = \tanh\left(s_i \cdot \frac{\sqrt{c_i}\|v\|}{2}\right) \frac{v}{\sqrt{c_i}\|v\|}$ (Note: precise derivation needed for consistency with a corresponding metric). Learning $s_i$ allows the model to control the effective density or spatial extent represented within each level's tangent space mapping. Minimum value clamping is necessary for stability.

#### 3.2.2 Boundary Sub-Manifolds ($B_{i,j}$)
To explicitly model substructures or landmark features within a given scale, each level $\mathbb{H}^{n_i}$ can host a set of learnable **Boundary Sub-Manifolds** $B_{i,j}$.
*   **Representation:** These are conceptually lower-dimensional manifolds embedded within $\mathbb{H}^{n_i}$. A practical implementation often involves parameterizing them using a set of characteristic **learnable points** $\{b_{i,j,k}\} \subset \mathbb{H}^{n_i}$. For example, a few points could define the location and extent of a hyperbolic disk ("circle") or simply act as landmarks. These points $b_{i,j,k}$ are model parameters, potentially initialized near the origin or boundary and learned via backpropagation.
*   **Purpose:** They represent distinct components, parts, feature clusters, or reference frames relevant at the scale defined by level $i$. Their relative positions to the main data representation $x_i$ become important features.
*   **Transformation:** For inter-level transitions, these points $b_{i,j,k}$ are mapped to tangent vectors $v_{b_{i,j,k}}$ using the LogMap, then rotated by $R_i$ and mapped by $\tilde{T}_i$ alongside the primary representation.

#### 3.2.3 Tangent Space Logic
A cornerstone of WuBu Nesting is that complex transformations, particularly rotations and mappings between potentially different dimensions, occur within the **Euclidean tangent spaces** associated with the hyperbolic levels.
*   **Mapping To/From:** The **Logarithmic Map** ($\text{Log}_{p,s_i}^{c_i}: \mathbb{H}^{n_i} \to T_p(\mathbb{H}^{n_i})$) projects points from the hyperbolic ball to the tangent space at a reference point $p$ (often the origin $o$), incorporating the scale $s_i$ and curvature $c_i$. The **Exponential Map** ($\text{exp}_{p,s_i}^{c_i}: T_p(\mathbb{H}^{n_i}) \to \mathbb{H}^{n_i}$) performs the inverse projection. Robust implementations (e.g., from libraries like `geoopt`) are crucial.
*   **Operations:** Within the tangent space $T_p(\mathbb{H}^{n_i}) \cong \mathbb{R}^{n_i}$, standard Euclidean vector operations (addition, subtraction, scalar multiplication, linear transformations, rotations, MLPs) can be applied.

#### 3.2.4 Tangent Space Rotations ($R_i$)
To explicitly model orientational changes between hierarchical levels, a learnable **Rotation** $R_i$ is applied in the tangent space $T_o(\mathbb{H}^{n_i})$ during the $i \rightarrow i+1$ transition.
*   **Implementation:**
    *   If $n_i = 4$, $R_i$ can be efficiently implemented using **unit quaternion multiplication**. A general SO(4) rotation can be parameterized by two unit quaternions $p, q$ acting as $v' = p \cdot v \cdot q$. These unit quaternions (8 parameters constrained to the sphere $S^3 \times S^3$) are learned.
    *   If $n_i \neq 4$, $R_i$ is implemented using **rotation matrices** from the Special Orthogonal group SO($n_i$). These matrices $R_i \in \mathbb{R}^{n_i \times n_i}$ satisfy $R_i^T R_i = I$ and $\det(R_i)=1$. They are learned parameters, typically parameterized using techniques that ensure they remain on the SO($n_i$) manifold during optimization (e.g., using the matrix exponential map from the Lie algebra $\mathfrak{so}(n_i)$, or using orthogonal parameterizations like Cayley maps or projections [ref: Orthogonal Matrix param methods]).
*   **Simultaneous Application:** $R_i$ is applied to the main tangent vector $v_i$, all boundary tangent vectors $v_{b_{i,j,k}}$, and the level descriptor vector $\vec{ld}_i$ originating from level $i$.

#### 3.2.5 Non-Rotational Mapping ($\tilde{T}_i$)
Following the rotation $R_i$, a learnable **non-rotational mapping** $\tilde{T}_i$ is applied to the rotated tangent vectors.
*   **Purpose:** This component handles feature transformation, non-linear interactions, and dimensionality changes between levels ($n_i \rightarrow n_{i+1}$).
*   **Implementation:** $\tilde{T}_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_{i+1}})$ can be implemented using various standard neural network layers operating on vectors, such as:
    *   Multi-Layer Perceptrons (MLPs).
    *   Linear projections (if only dimension change is needed).
    *   QuaternionLinear layers (if $n_i, n_{i+1}$ are divisible by 4, potentially using only the non-rotational part of the quaternion transform).
    *   Other specialized layers as appropriate.
*   **Output:** Produces the final tangent vectors $v_{i+1}$, $v''_{b_{i,j,k}}$, and $\vec{ld}_{i+1}$ in the target tangent space $T_o(\mathbb{H}^{n_{i+1}})$.

#### 3.2.6 Relative Vector Generation ($\vec{d}_{i+1, j, k}$)
After the full tangent space transformation $T_{i \rightarrow i+1} = \tilde{T}_i \circ R_i$, **Relative Vectors** are computed directly in the target Euclidean tangent space $T_o(\mathbb{H}^{n_{i+1}})$.
*   **Computation:** $\vec{d}_{i+1, j, k} = v_{i+1} - v''_{b_{i,j,k}}$
*   **Purpose:** These vectors explicitly encode the geometric relationship (displacement and direction) between the primary data representation and the boundary substructures *after* accounting for the learned rotation and mapping between levels. They provide rich, orientation-aware structural information.
*   **Usage:** The set of relative vectors $\{\vec{d}_{i+1, j, k}\}$ is passed as input to the processing stage of the next level, $\mathbb{H}^{n_{i+1}}$.

#### 3.2.7 Learnable Level Descriptor Vector ($\vec{ld}_i$)
Each level $i$ possesses an intrinsic **Learnable Level Descriptor Vector** $\vec{ld}_i$.
*   **Representation:** $\vec{ld}_i \in T_o(\mathbb{H}^{n_i}) \cong \mathbb{R}^{n_i}$ is a learnable parameter vector, initialized (e.g., randomly, radially, or zero) and optimized alongside other model parameters.
*   **Purpose:** This vector aims to capture characteristic geometric properties of level $i$ itself, independent of the specific input data instance. It might learn to represent a preferred orientation, a direction of maximum variance within the level, an axis of symmetry, or some other scale-specific anisotropic feature.
*   **Transformation:** $\vec{ld}_i$ is treated similarly to other feature vectors during the inter-level transition: it is rotated by $R_i$ ($\vec{ld}'_i = R_i(\vec{ld}_i)$) and then mapped by $\tilde{T}_i$ ($\vec{ld}_{i+1} = \tilde{T}_i(\vec{ld}'_i)$).
*   **Usage:** The transformed vector $\vec{ld}_{i+1}$ is passed as input to the processing stage of level $i+1$, providing context about the learned geometric characteristics of the source level $i$.

#### 3.2.8 Learnable Level Spread Parameter ($\sigma_i$)
Each level $i$ is associated with a **Learnable Level Spread Parameter** $\sigma_i$.
*   **Representation:** A learnable positive scalar parameter $\sigma_i > 0$. Learning requires ensuring positivity (e.g., parameterizing $\log \sigma_i$ or using a softplus activation).
*   **Purpose:** Represents the characteristic "atmosphere," radius of influence, uncertainty measure, or density falloff associated with representations at scale $i$. A large $\sigma_i$ might indicate broader clusters or higher uncertainty at that level.
*   **Transformation & Usage:** $\sigma_i$ is typically passed directly as a scalar **contextual input** to the processing stage of the next level $i+1$. It does not usually undergo the rotation/mapping transform itself. The processing module at level $i+1$ can use $\sigma_i$ to modulate its computations, for example, by adjusting attention weights, scaling features, or simply using it as an additional input feature.

#### 3.2.9 Intra-Level Tangent Flow ($F_i$)
To model dynamics or adjustments *within* a scale, each level $i$ can incorporate a learnable **Intra-Level Tangent Flow** field $F_i$.
*   **Representation:** A learnable function $F_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_i})$ operating within the tangent space of level $i$. It could be parameterized as:
    *   An MLP predicting a displacement: $F_i(v) = \text{MLP}_i(v)$. The flowed vector is then $v_{flowed} = v + F_i(v)$.
    *   A linear transformation: $F_i(v) = M_i v$, where $M_i$ is a learnable matrix. $v_{flowed} = F_i(v)$.
    *   More complex flows like Neural ODEs could also be considered.
*   **Purpose:** Models characteristic evolution, refinement, or "orbital" adjustment of the representation pertinent to the scale $i$. It allows the representation to shift within its local geometric context before potentially being passed to the next level.
*   **Placement:** $F_i$ is applied as part of the `Intra-Ball Processing` module within level $i$. It typically acts on a tangent space representation derived from the hyperbolic state within that level.

#### 3.2.10 Hierarchical Information Flow
The design ensures a rich flow of information between levels. The input to the `Intra-Ball Processing` module of level $i+1$ comprises:
*   The primary tangent vector $v_{i+1}$ (potentially mapped to $x_{i+1} = \text{exp}_{o,s_{i+1}}^{c_{i+1}}(v_{i+1})$ for hyperbolic operations).
*   The set of relative tangent vectors $\{\vec{d}_{i+1, j, k}\}$, encoding rotation-aware structure.
*   The transformed Level Descriptor tangent vector $\vec{ld}_{i+1}$, encoding source level characteristics.
*   The scalar Level Spread parameter $\sigma_i$ from the source level, encoding source level uncertainty/density.
The `Intra-Ball Processing` module (which may itself apply the flow $F_{i+1}$) can then utilize this comprehensive set of inputs (e.g., via concatenation followed by projection, attention mechanisms where relative vectors/descriptors act as keys/values, or using $\sigma_i$ to modulate activity) to compute the refined representation $x_{i+1}^{out}$ for that level.

#### 3.2.11 Scale-Aware Aggregation
To produce a final output for a downstream task, information from multiple levels of the WuBu Nesting hierarchy often needs to be aggregated.
*   **Mechanism:** Representations from different levels (e.g., the output tangent vectors $v_i^{out}$, relevant relative vectors $\vec{d}_{i,j,k}$, or level descriptors $\vec{ld}_i$) need to be brought into a common space. This might involve applying the inverse tangent space transformations ($T^{-1}_{j \rightarrow j+1}$) to map deeper representations back towards the outermost tangent space $T_o(\mathbb{H}^{n_1})$, or mapping all relevant vectors to a separate Euclidean output space.
*   **Strategies:** Once in a common space, aggregation can occur via:
    *   Concatenation followed by a final processing network (e.g., MLP).
    *   Attention mechanisms, allowing the model to weigh the importance of features from different scales and geometric components.
    *   Pooling operations (e.g., max or mean pooling).

## 4. Mathematical Formulation (Conceptual)

Let's outline the conceptual mathematical flow for a single step from level $i$ to level $i+1$.

**Inputs to Level $i$ Processing:**
*   Primary tangent vector from previous transition: $v_i^{in} \in T_o(\mathbb{H}^{n_i})$.
*   Set of relative tangent vectors from previous transition: $\{\vec{d}_{i, j, k}\} \subset T_o(\mathbb{H}^{n_i})$.
*   Transformed level descriptor from previous level: $\vec{ld}_i^{in} \in T_o(\mathbb{H}^{n_i})$.
*   Contextual spread parameter from previous level: $\sigma_{i-1} \in \mathbb{R}^+$.

**Parameters specific to Level $i$:**
*   Curvature $c_i$, Scale $s_i$.
*   Boundary points $\{b_{i,j,k}\} \subset \mathbb{H}^{n_i}_{c_i, s_i}$.
*   Level Descriptor Vector $\vec{ld}_i^{param} \in T_o(\mathbb{H}^{n_i})$.
*   Level Spread Parameter $\sigma_i \in \mathbb{R}^+$.
*   Intra-Level Tangent Flow function $F_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_i})$.

**A. Intra-Level Processing within Level $i$:**
1.  **Map Primary Input to Hyperbolic (Optional):** $x_i^{in} = \text{exp}_{o,s_i}^{c_i}(v_i^{in})$.
2.  **Internal Hyperbolic/Tangent Operations:** This is the core of the `Intra-Ball Processing` module. It uses $x_i^{in}$ (or $v_i^{in}$), $\{\vec{d}_{i, j, k}\}$, $\vec{ld}_i^{in}$, and $\sigma_{i-1}$ to compute an intermediate state. This might involve hyperbolic operations (like gyrovector additions, distance calculations) or mapping internal states to the tangent space.
3.  **Apply Intra-Level Flow (Tangent Space):** Let $v_{intermediate}$ be a relevant tangent space representation derived during step 2. Apply the flow:
    $$ v_{flowed} = v_{intermediate} + F_i(v_{intermediate}) \quad (\text{e.g., additive flow})$$
    or $ v_{flowed} = F_i(v_{intermediate}) \quad (\text{e.g., transformative flow})$.
4.  **Generate Level Output State:** Based on $v_{flowed}$ and other internal computations, determine the final hyperbolic state for this level, $x_i^{out} \in \mathbb{H}^{n_i}_{c_i, s_i}$.

**B. Inter-Level Transition ($i \rightarrow i+1$):**
1.  **Map Features to Tangent Space $T_o(\mathbb{H}^{n_i})$:**
    *   Primary Output: $v_i^{out} = \text{Log}_{o,s_i}^{c_i}(x_i^{out})$.
    *   Boundary Points: $v_{b_{i,j,k}} = \text{Log}_{o,s_i}^{c_i}(b_{i,j,k})$.
    *   Level Descriptor (Parameter): $\vec{ld}_i = \vec{ld}_i^{param}$.

2.  **Apply Learned Rotation $R_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_i})$:**
    *   $v'^{out}_i = R_i(v_i^{out})$.
    *   $v'_{b_{i,j,k}} = R_i(v_{b_{i,j,k}})$.
    *   $\vec{ld}'_i = R_i(\vec{ld}_i)$.
    (Where $R_i$ is quaternion multiplication or SO($n_i$) matrix multiplication).

3.  **Apply Mapping Transform $\tilde{T}_i: T_o(\mathbb{H}^{n_i}) \to T_o(\mathbb{H}^{n_{i+1}})$:**
    *   $v_{i+1} = \tilde{T}_i(v'^{out}_i)$.
    *   $v''_{b_{i,j,k}} = \tilde{T}_i(v'_{b_{i,j,k}})$.
    *   $\vec{ld}_{i+1} = \tilde{T}_i(\vec{ld}'_i)$.
    (These vectors are now in the target tangent space $T_o(\mathbb{H}^{n_{i+1}})$).

4.  **Generate Relative Vectors in $T_o(\mathbb{H}^{n_{i+1}})$:**
    $$ \vec{d}_{i+1, j, k} = v_{i+1} - v''_{b_{i,j,k}} $$

5.  **Gather Inputs for Level $i+1$ Processing:** The inputs passed to the next level's processing module are:
    *   $v_{i+1}$ (Primary tangent vector for level $i+1$).
    *   $\{\vec{d}_{i+1, j, k}\}$ (Set of relative vectors).
    *   $\vec{ld}_{i+1}$ (Transformed level descriptor).
    *   $\sigma_i$ (Spread parameter *from level $i$*).

This process repeats for the transition from level $i+1$ to $i+2$, and so on.

*(Note: The exact formulation requires careful derivation of scale-aware hyperbolic maps and metrics, appropriate parameterizations for rotations and flows, and stable implementations.)*

## 5. Potential Applications

The comprehensive nature of the WuBu Nesting framework, integrating multi-scale hierarchy, rotation, relative geometry, level-specific characteristics, and dynamics, makes it potentially suitable for a wide range of complex modeling tasks:

*   **Computer Vision:**
    *   **Articulated Object Understanding:** Modeling complex objects like humans or animals, where nested part hierarchies (limbs, digits) combine with rotational joint movements (handled by $R_i$) and potentially part-specific dynamics (modeled by $F_i$). Boundary manifolds ($B_{i,j}$) could represent keypoints or parts, relative vectors ($\vec{d}$) their configuration. Level descriptors ($\vec{ld}_i$) could capture part symmetry or orientation bias.
    *   **Scene Analysis with Viewpoint Changes:** Representing scenes with nested object structures where viewpoint transformations involve rotations ($R_i$) applied across scales. Spread ($\sigma_i$) could model ambiguity or scale uncertainty.
    *   **Robotic Vision & Interaction:** Representing robot configurations (hierarchy of links/joints) and their interaction with complex, structured environments, involving both physical rotations and potential dynamic adjustments ($F_i$).

*   **Molecular Biology & Cheminformatics:**
    *   **Protein Structure & Function:** Modeling proteins with hierarchical structures (domains, secondary structures, residues). Rotations ($R_i$) are crucial for conformational changes and docking. Boundary manifolds could represent active sites or key residues. Relative vectors can capture precise spatial arrangements. Level descriptors might encode chirality or domain orientation. Spread could model flexibility or ensemble variation. Tangent flow could model local folding dynamics.
    *   **Drug Discovery & Docking:** Representing molecules and protein pockets hierarchically, using rotations for alignment scoring. Spread ($\sigma_i$) could model docking pose uncertainty.

*   **Robotics & Control:**
    *   **Hierarchical Planning & Control:** Representing complex tasks decomposed into sub-tasks at different scales (nesting). Physical robot movements involve rotations ($R_i$). Intra-level flows ($F_i$) could model local trajectory refinements or impedance control adaptations. Level descriptors might represent tool orientation.
    *   **State Representation:** Encoding complex robot states (e.g., manipulators with complex grippers) and their interaction with the environment.

*   **Knowledge Graph Representation:**
    *   **Complex Ontologies:** Embedding knowledge graphs with deep hierarchical category structures (nesting) and potentially relational orientations or types (captured partly by $R_i$ or $\vec{ld}_i$). Boundary manifolds could represent salient entity types within a hierarchy level.

*   **Generative Models:**
    *   **Structured Data Generation:** Creating complex, structured data like 3D shapes, molecules, or scenes with inherent hierarchical consistency, controlled orientations, and potentially learned dynamic variations ($F_i$). The adaptive geometry ($c_i, s_i$) could allow generation of structures with varying complexity.

*   **Time Series Analysis:**
    *   **Hierarchical Processes:** Modeling time series with multi-scale temporal patterns where dynamics ($F_i$) and state transitions ($T_{i \rightarrow i+1}$) are key. Rotation might model phase shifts or periodic components.

## 6. Implementation Considerations & Strategy

Implementing the full WuBu Nesting framework presents significant technical challenges, demanding careful attention to mathematical rigor, numerical stability, and computational efficiency.

*   **Mathematical Rigor:**
    *   **Scale-Aware Maps/Metrics:** Formal derivation and implementation of consistent, differentiable scale-aware exponential maps, logarithmic maps, and associated hyperbolic metrics are required.
    *   **Tangent Space Consistency:** Ensuring reference points for tangent spaces are handled consistently during transitions.
    *   **Rotation Parameterization:** Choosing and implementing stable, differentiable parameterizations for SO($n_i$) matrices (e.g., using Lie algebra exponentiation, Cayley maps, or iterative projections) or unit quaternions.
    *   **Flow Parameterization:** Defining suitable parameterizations for the intra-level tangent flows ($F_i$) that are expressive yet stable.

*   **Numerical Stability:**
    *   **Hyperbolic Operations:** Standard hyperbolic operations (LogMap, ExpMap, Möbius addition, distances) can suffer from instability near the boundary of the Poincaré ball or with large vector norms. Robust implementations require careful handling of edge cases, numerical precision (e.g., float64), gradient clipping, vector norm clipping before ExpMap (as in HypCD), and potentially re-parameterizations.
    *   **Tangent Space Operations:** While Euclidean, large rotations or complex MLP mappings within $\tilde{T}_i$ or $F_i$ can still lead to exploding/vanishing gradients. Normalization techniques (LayerNorm, BatchNorm adapted for tangent spaces) are crucial.
    *   **Curvature/Scale Learning:** Keeping learned curvatures $c_i$ and scales $s_i$ strictly positive requires constraints or specific parameterizations (e.g., learning $\log c_i, \log s_i$).

*   **Computational Cost:**
    *   **Multiple Levels:** Each level adds computational overhead for both intra-ball processing and inter-level transformations.
    *   **Boundary Representations:** A large number of boundary points $b_{i,j,k}$ increases the number of vectors to be transformed and the number of relative vectors to compute.
    *   **Complex Transformations:** SO($n_i$) operations (especially for high $n_i$), MLP-based mappings $\tilde{T}_i$, and potentially complex flows $F_i$ add significant computational load.

*   **Component Design & Interaction:**
    *   **Boundary Representation:** Defining effective parameterizations for boundary manifolds beyond simple point sets might be necessary for some applications.
    *   **Flow Field Design:** Choosing the right complexity and functional form for $F_i$ is critical – too simple might be ineffective, too complex might be unstable or hard to train.
    *   **Information Fusion:** Designing the `Intra-Ball Processing` modules to effectively utilize the rich input stream (primary vector, relative vectors, descriptor, spread) is key. Attention mechanisms seem promising but add complexity.

*   **Optimization:**
    *   **Complex Loss Landscape:** The model involves numerous learnable geometric parameters ($c_i, s_i$, boundary points, rotation parameters, $\vec{ld}_i, \sigma_i$, flow parameters) likely resulting in a highly non-convex and complex loss landscape.
    *   **Optimization Algorithms:** Standard optimizers like Adam might struggle. Riemannian optimization methods (e.g., Riemannian Adam/SGD [4]) might be necessary, especially for optimizing parameters directly on manifolds (like $c_i, s_i$, rotation parameters).
    *   **Initialization & Regularization:** Careful initialization strategies for all components and appropriate regularization techniques (e.g., penalizing extreme curvatures/scales, regularizing rotation parameters, dropout) will be vital for successful training.

**Incremental Implementation Strategy:**
Given the complexity, a staged approach is highly recommended:
1.  **Foundation (2 Levels, Fixed Geometry):** Start with a fixed 2-level structure ($\mathbb{H}^{n_1} \supset \mathbb{H}^{n_2}$) with fixed $n_i, c_i, s_i=1$. Focus on implementing stable tangent space transitions with LogMap/ExpMap and a basic rotation ($R_1$, e.g., SO(3) or Quat if $n_1=4$) and mapping ($\tilde{T}_1$, e.g., linear projection).
2.  **Adaptive Geometry:** Introduce learnable scales $s_i$ and curvatures $c_i$, using robust parameterizations and potentially Riemannian optimizers.
3.  **Boundaries & Relative Vectors:** Implement learnable boundary points $\{b_{i,j,k}\}$ and the computation/passing of relative vectors $\{\vec{d}_{i+1}\}$. Modify the level processing to utilize these vectors.
4.  **Level Descriptors & Spread:** Add learnable $\vec{ld}_i$ vectors and $\sigma_i$ scalars. Integrate their transformation and contextual passing. Update level processing to use them.
5.  **Intra-Level Flow:** Introduce the learnable tangent flow $F_i$ within the `Intra-Ball Processing` modules. Start with simpler parameterizations (e.g., linear).
6.  **Multi-Level Expansion:** Gradually increase the number of levels, carefully monitoring stability and performance.
7.  **Refinement & Optimization:** Develop advanced aggregation strategies, explore different component parameterizations, and fine-tune optimization hyperparameters and regularization.

## 7. Discussion & Future Work

WuBu Nesting, as presented, is a highly ambitious conceptual framework aiming to unify multiple desirable geometric properties within a single deep learning architecture. Its potential lies in providing a much richer and more flexible inductive bias than currently available methods, potentially leading to breakthroughs in modeling complex systems where hierarchy, orientation, scale, dynamics, and uncertainty are all crucial aspects.

**Potential Impact:**
*   **Unified Geometric Modeling:** Offers a path towards a single model architecture capable of handling diverse geometric complexities simultaneously.
*   **Improved Representation Learning:** The explicit modeling of these geometric features could lead to more robust, interpretable, and efficient representations, particularly for structured data.
*   **New Modeling Capabilities:** Enables tackling problems previously difficult due to the lack of suitable geometric biases (e.g., complex protein dynamics, fine-grained articulated object interaction).
*   **Interpretability:** Components like boundary manifolds, relative vectors, level descriptors, and spread parameters might offer more interpretable insights into the model's internal workings compared to monolithic black-box models.

**Limitations & Challenges:**
*   **Complexity:** The framework is significantly more complex than standard models, posing substantial implementation, training, and debugging challenges.
*   **Computational Cost:** The computational demands are likely to be high, potentially limiting scalability to very large datasets or deep hierarchies without significant optimization or hardware acceleration.
*   **Data Requirements:** Training such a complex model might require large amounts of appropriately structured data to learn all the geometric parameters meaningfully.
*   **Theoretical Analysis:** A thorough theoretical understanding of the properties of this nested, adaptive, dynamic geometry (e.g., stability, convergence guarantees, expressivity) is currently lacking and requires significant future research.
*   **Interpretability vs. Complexity:** While some components offer interpretability, the interactions between all parts might become highly complex and difficult to fully dissect.

**Future Work:**
1.  **Formal Mathematical Development:** Rigorous derivation of scale-aware hyperbolic geometry, consistent tangent space mappings, and stable parameterizations for all components.
2.  **Robust Implementation:** Development of numerically stable and efficient implementations, potentially leveraging libraries like `geoopt`, `PyTorch Geometric`, or specialized CUDA kernels.
3.  **Component Variations:** Exploring different parameterizations for boundary manifolds (beyond points), intra-level flows (e.g., Neural ODEs), and inter-level mappings ($\tilde{T}_i$).
4.  **Optimization Strategies:** Developing tailored optimization techniques, including adaptive learning rates, Riemannian methods, and regularization strategies specifically designed for this complex geometric landscape.
5.  **Empirical Validation:** Rigorous testing on diverse benchmark datasets across the potential application domains to demonstrate the practical benefits and limitations compared to existing methods.
6.  **Theoretical Analysis:** Investigating the theoretical properties of the WuBu Nesting geometry, such as its embedding capacity, distortion characteristics, and convergence behavior.
7.  **Architecture Search:** Exploring methods to automatically determine the optimal number of levels, dimensions $n_i$, and potentially the types of components needed for a given task.
8.  **Visualization Tools:** Creating tools to visualize the learned nested structures, boundary manifolds, descriptor vectors, and flows to aid understanding and debugging.

## 8. Conclusion

WuBu Nesting is presented as a novel, comprehensive conceptual framework for deep geometric learning. By uniquely integrating **adaptively nested hyperbolic spaces**, **explicit boundary sub-manifolds**, **tangent space rotations and mappings**, **relative vector computations**, **learnable level descriptors**, **contextual level spread parameters**, and **intra-level tangent flows**, it aims to provide an unprecedentedly rich geometric inductive bias. The framework is designed to capture the complex interplay of multi-scale hierarchy, orientation, relative structure, scale-specific characteristics, density/uncertainty, and local dynamics often found in real-world data. While significant theoretical and implementation challenges remain, WuBu Nesting offers a promising and potentially transformative direction for developing next-generation deep learning models capable of understanding and generating data with profound geometric complexity. Future work will focus on formalizing the underlying mathematics, developing robust implementations, and empirically validating the framework's potential across diverse scientific and engineering domains.

## References

## References

[1] Atigh, M. G., Schoep, J., Acar, E., Van Noord, N., & Mettes, P. (2022). Hyperbolic image segmentation. *CVPR*.

[4] Becigneul, G., & Ganea, O. E. (2019). Riemannian adaptive optimization methods. *ICLR*.

[15] Ermolov, A., Mirvakhabova, L., Khrulkov, V., Sebe, N., & Oseledets, I. (2022). Hyperbolic vision transformers: Combining improvements in metric learning. *CVPR*.

[19] Ganea, O., Bécigneul, G., & Hofmann, T. (2018). Hyperbolic neural networks. *NeurIPS*.

[22] Gulcehre, C., Denil, M., Malinowski, M., Razavi, A., Pascanu, R., Hermann, K. M., ... & de Freitas, N. (2019). Hyperbolic attention networks. *ICLR*.

[31] Khrulkov, V., Mirvakhabova, L., Ustinova, E., Oseledets, I., & Lempitsky, V. (2020). Hyperbolic image embeddings. *CVPR*.

[39] Nickel, M., & Kiela, D. (2017). Poincaré embeddings for learning hierarchical representations. *NeurIPS*.

[42] Liu, Y., He, Z., & Han, K. (2025). Hyperbolic Category Discovery. *arXiv preprint arXiv:2504.06120*. *(Note: Placeholder date/ID)*

[43] Hamilton, W. R. (1866). *Elements of quaternions*. Longmans, Green, & Company.

[44] Parcollet, T., Morchid, M., Bousquet, P. M., Dufour, R., Linarès, G., & De Mori, R. (2019). Quaternion recurrent neural networks. *ICLR*.

[45] Grassucci, E., Comminiello, D., & Uncini, A. (2021). Quaternion neural networks: State-of-the-art and research challenges. *IEEE Transactions on Neural Networks and Learning Systems*.

[46] Gu, A., Sala, F., Gunel, B., & Ré, C. (2019). Learning Semantic Representations using Diffusion Kernels. *NeurIPS*.

[50] Ungar, A. A. (2008). *Gyrovector spaces and gyrovector space theory*. Springer.

[51] Nickel, M., & Kiela, D. (2018). Learning continuous hierarchies in the Lorentz model of hyperbolic geometry. *ICML*.

[52] Tifrea, A., Bécigneul, G., & Ganea, O. E. (2019). Poincaré Glove: Hyperbolic word embeddings. *ICLR*.

[53] Chami, I., Ying, R., Ré, C., & Leskovec, J. (2019). Hyperbolic graph convolutional neural networks. *NeurIPS*.

[54] Zhang, Y., Wang, X., Shi, C., & Ye, Y. (2021). Hyperbolic graph attention network. *WWW*.

[55] Long, Y., Liu, Y., & Yu, F. (2021). Hyperbolic graph neural networks: A review. *arXiv preprint arXiv:2108.08990*.

[56] Skopek, O., Ganea, O. E., & Bécigneul, G. (2020). Mixed-curvature variational autoencoders. *ICLR*.

[57] Guo, W., Chen, Z., & Chang, B. (2021). Learning mixed-curvature representations in products of model spaces. *arXiv preprint arXiv:2110.10119*.

[58] Zhou, Y., et al. (2019). On the continuity of rotation representations in neural networks. *CVPR*.

[59] Weiler, M., Hamprecht, F. A., & Storath, M. (2018). Learning steerable filters for rotation equivariant CNNs. *CVPR*.

[60] Cohen, T., & Welling, M. (2016). Group equivariant convolutional networks. *ICML*.

[61] Falorsi, L., et al. (2020). Explorations in geometric deep learning. *arXiv preprint arXiv:2007.07203*.

[62] Lezama, J., Qiu, Q., & Sapiro, G. (2017). Riemannian stochastic optimization methods for non-convex matrix completion. *arXiv preprint arXiv:1701.00306*.

[63] Chen, R. T. Q., Rubanova, Y., Bettencourt, J., & Duvenaud, D. K. (2018). Neural ordinary differential equations. *NeurIPS*.

[64] Grathwohl, W., et al. (2018). FFJORD: Free-form continuous dynamics for scalable reversible generative models. *ICLR*.

[65] Dupont, E., Doucet, A., & Teh, Y. W. (2019). Augmented neural ODEs. *NeurIPS*.

[66] Mathieu, E., Le Lan, C., Maddison, C. J., Tomioka, R., & Teh, Y. W. (2020). Riemannian continuous normalizing flows. *NeurIPS*.

[67] Davidson, T. R., Falorsi, L., De Cao, N., Kipf, T., & Tomczak, J. M. (2018). Hyperspherical variational auto-encoders. *NeurIPS*.

[68] Skopek, O., et al. (2021). Geometric bayesian deep learning: A review. *arXiv preprint arXiv:2107.08957*.

[69] Bronstein, M. M., Bruna, J., LeCun, Y., Szlam, A., & Vandergheynst, P. (2017). Geometric deep learning: going beyond euclidean data. *IEEE Signal Processing Magazine*.

[70] Bronstein, M. M., Bruna, J., Cohen, T., & Veličković, P. (2021). Geometric deep learning: Grids, groups, graphs, geodesics, and gauges. *arXiv preprint arXiv:2104.13478*.

[71] Kochurov, M., et al. (2020). Geoopt: Riemannian Optimization in PyTorch. *GitHub Repository*. `https://github.com/geoopt/geoopt`
