# Tau 2.0 Specification Revision: Collected References

This document compiles references identified or provided in the `docs/research/` directory, organized according to the categories in `Research.md`. It serves as a starting point for the comprehensive bibliography required for the Tau 2.0 revision.

*(Librarian Note: This list is based *only* on the initially provided markdown files. Extensive external searching is required to fulfill all requests in `Research.md`.)*

---

## 1. SO(3) Action Space

*   **Foundational Concepts (Rotations):**
    *   Hamilton, W. R. (1866). *Elements of quaternions*. Longmans, Green, & Company. (Cited in `04_fast_math...md`)
    *   Shoemake, K. (1985). Animating Rotation with Quaternion Curves. *ACM SIGGRAPH Computer Graphics*, 19(3), 245–254. (Cited in `04_fast_math...md`)
    *   *Note:* `04_fast_math...md` provides context on rotation matrices (SO(3), SO(4)) and Quaternions for transformations, relevant background but not specific to *action spaces* in RL.
*   **Empirical Studies / Benchmarks (Action Spaces):** **Missing – Librarian requests user/librarian to locate.** *(Assistant Clarification: Seeking theoretical and empirical studies on SO(3)/Lie groups as action spaces in RL, multimodal retrieval, or semantic embedding)*.
*   **Information Capacity / Theory (Action Spaces):** **Missing.** *(Assistant Clarification: Seeking info-theoretic analyses)*.
*   **Mapping Mechanisms (SO(3) to Vocabulary):** **Missing.** *(Assistant Clarification: Seeking mapping mechanisms, critiques)*.
*   **Critiques / Alternatives:** **Missing.**

## 2. Amplituhedron-Inspired Loss

*   **No references found in provided documents.** **Librarian requests user/librarian to locate mathematical physics and ML application papers.** *(Assistant Clarification: Seeking papers on amplituhedron geometry, computational representations, and applications to optimization, regularization, or geometric loss functions in ML. Connection intended to be more than analogical)*.

## 3. WuBu Nesting and Riemannian Optimization

*   **Nested/Hyperbolic Geometry:**
    *   Nickel, M., & Kiela, D. (2017). Poincaré embeddings for learning hierarchical representations. *NeurIPS*. (Cited in `WuBuHypCD-paper.md`)
    *   Ganea, O., Bécigneul, G., & Hofmann, T. (2018). Hyperbolic neural networks. *NeurIPS*. (Cited in `WuBuHypCD-paper.md`)
    *   Khrulkov, V., et al. (2020). Hyperbolic image embeddings. *CVPR*. (Cited in `WuBuHypCD-paper.md`)
    *   Liu, Y., He, Z., & Han, K. (2025). Hyperbolic Category Discovery. *arXiv preprint arXiv:2504.06120*. (Cited in `WuBuHypCD-paper.md`)
    *   Ungar, A. A. (2008). *Gyrovector spaces and gyrovector space theory*. Springer. (Cited in `WuBuHypCD-paper.md`)
    *   Nickel, M., & Kiela, D. (2018). Learning continuous hierarchies in the Lorentz model of hyperbolic geometry. *ICML*. (Cited in `WuBuHypCD-paper.md`)
    *   Chami, I., et al. (2019). Hyperbolic graph convolutional neural networks. *NeurIPS*. (Cited in `WuBuHypCD-paper.md`)
    *   *Note:* `WuBuHypCD-paper.md` itself serves as the primary conceptual document for WuBu Nesting.
*   **Tangent Space / Rotations:**
    *   See Section 1 references (Hamilton, Shoemake).
    *   *Note:* `WuBuHypCD-paper.md` describes tangent space transitions, Log/Exp maps, and SO(n)/Quaternion rotations conceptually.
*   **Riemannian Optimization:**
    *   Becigneul, G., & Ganea, O. E. (2019). Riemannian adaptive optimization methods. *ICLR*. (Cited in `WuBuHypCD-paper.md`)
    *   Kochurov, M., et al. (2020). Geoopt: Riemannian Optimization in PyTorch. *GitHub Repository*. `https://github.com/geoopt/geoopt` (Cited in `WuBuHypCD-paper.md`)
    *   Lezama, J., Qiu, Q., & Sapiro, G. (2017). Riemannian stochastic optimization methods for non-convex matrix completion. *arXiv preprint arXiv:1701.00306*. (Cited in `WuBuHypCD-paper.md`)
    *   *Note: Further search needed for stabilization, convergence proofs, and practical codebases.* *(Assistant Clarification: No known public codebases for WuBu, seeking partial/related work)*.

## 4. Core Component Interactions

*   **No specific references found for RL + Geometry + Loss integration.** **Librarian requests user/librarian to locate.** *(Assistant Clarification: Prioritize empirical studies, benchmarks, ablation protocols for modular systems)*.

## 5. cHFSM Dynamics

*   **Foundational Concepts:**
    *   Alur, R., Kannan, S., & Yannakakis, M. (1999). Communicating Hierarchical State Machines. In *Lecture Notes in Computer Science* (Vol. 1644, pp. 169-183). Springer. (Source of `10_..._CHSM.md`)
    *   Harel, D. (1987). Statecharts: A visual formalism for complex systems. *Science of Computer Programming*, 8(3), 231-274. (Cited in `10_..._CHSM.md`)
    *   *Note:* `10_..._CHSM.md` covers complexity and succinctness but lacks detailed algorithms for *dynamic* aspects.
*   **Algorithmic Descriptions (Dynamic):** **Missing.**
*   **Tractability / Optimization:** **Missing.**
*   **Implementations / Pseudocode:** **Missing.** *(Assistant Clarification: No known public codebases for dynamic cHFSM, seeking partial/related work)*.

## 6. Data Classification, UCI, and Meta-Meme Framework

*   **Foundational Concepts:**
    *   Rawson, K. (n.d.). From Chaos to Order: The Universal Comprehensive Integrated Data Framework for Data. (Source of `03_From_Chaos_to_Order...md`)
    *   *Note:* `03_From_Chaos_to_Order...md` provides the core description and cites inspirations like LCC, DDC, MeSH, ISO 2788, TGN, Linnaean system, IUPAC.
*   **Critiques / Integration Analyses:** **Missing.**
*   **Graph Storage / Ontology Management:** **Missing.**

## 7. Holographic Projections and Transformations

*   **Foundational Concepts:**
    *   Newell, A. C., Schlüper, B., & Davis, R. J. (n.d.). Holographic projection to an arbitrary plane from spherical near-field measurements. (Source of `01AN-holo-projection.md`)
    *   *Note:* `01AN-holo-projection.md` provides a specific technical application and cites related works in antenna measurement diagnostics.
*   **Broader Applications / AI Critiques:** **Missing.**

## 8. Fast Math Optimizations

*   **Foundational Concepts & Applications:**
    *   Rawson, K. (2025). Accelerating AI Systems with Fast Math Techniques: Scalable Solutions in Matrix Algebra. (Source of `04_fast_math...md`)
    *   Carmack, J. (1999). Fast Inverse Square Root: A Quake III Algorithm Analysis. *Online resource*. (Cited in `04_fast_math...md`)
    *   Taylor, B. (1715). Methodus Incrementorum Directa et Inversa. *Philosophical Transactions of the Royal Society*, 1, 111–171. (Cited in `04_fast_math...md`)
    *   Volder, J. E. (1969). The CORDIC Algorithm: Hardware for Fast Trigonometric Computation. *IEEE*. (Cited in `04_fast_math...md`)
    *   Gupta, T., & Patel, J. (2012). Efficient Lookup Table-Based Sine and Cosine Approximations for Embedded Systems. *Journal of Embedded Computing*, 9(4), 225–231. (Cited in `04_fast_math...md`)
    *   Trefethen, L. N. (2013). *Approximation Theory and Approximation Practice*. SIAM. (Cited in `04_fast_math...md`)
    *   Vaswani, A., et al. (2017). Attention Is All You Need. *NeurIPS*. (Cited in `04_fast_math...md`)
    *   Goodfellow, I., Bengio, Y., & Courville, A. (2016). *Deep Learning*. MIT Press. (Cited in `04_fast_math...md`)
    *   Su, J., et al. (2021). RoFormer: Enhanced Transformer with Rotary Position Embedding. *arXiv preprint arXiv:2104.09864*. (Cited in `04_fast_math...md`)
*   **Empirical Studies / Trade-offs:** **Missing.**

## 9. General and Supporting Research

*   **Geometric Deep Learning Overviews:**
    *   Bronstein, M. M., Bruna, J., LeCun, Y., Szlam, A., & Vandergheynst, P. (2017). Geometric deep learning: going beyond euclidean data. *IEEE Signal Processing Magazine*. (Cited in `WuBuHypCD-paper.md`)
    *   Bronstein, M. M., Bruna, J., Cohen, T., & Veličković, P. (2021). Geometric deep learning: Grids, groups, graphs, geodesics, and gauges. *arXiv preprint arXiv:2104.13478*. (Cited in `WuBuHypCD-paper.md`)
*   **Benchmarks / Validation / Data Requirements:** **Missing.**

---

**Action Required:**  
- Items marked **Missing** require user/librarian to locate, acquire, and annotate for integration into the Tau 2.0 revision.
- Please provide any additional context, keywords, or author names for targeted searches, especially for Amplituhedron-inspired loss and SO(3) action space in RL/ML. *(Librarian Note: Assistant provided some context)*.

---
