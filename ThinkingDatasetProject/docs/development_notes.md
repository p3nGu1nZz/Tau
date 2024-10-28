# Development Journal

## [2024-10-28]
- Created `ThinkingDatasetProject` directory.
- Installed Semantic Kernel and Ollama connector.
- Updated `.csproj` to target .NET 8.0.
- Cleared NuGet cache and did a clean build.
- Successfully restored and built the project with all dependencies.
- Discussed and planned the use of Serilog for logging with color output for readability.
- Explored the use of MediatR for an event-driven architecture to ensure loose coupling and scalability.
- Next steps: Start developing unit tests for basic Ollama functionality.

# System Design and Phases

## Phase 1: Setup and Configuration
- **Project Initialization**: Created and configured the `ThinkingDatasetProject` directory.
- **Install Dependencies**: Installed Semantic Kernel and Ollama connector.
- **Setup README**: Created and finalized the README.md to outline the project goals, objectives, and setup instructions.

## Phase 2: Data Pipeline Development
- **Raw Data Ingestion**: Collected initial raw data sources and set up mechanisms for raw data storage.
- **Data Cleaning and Preprocessing**: Implemented methods to remove duplicates, handle missing values, and normalize data.
- **Seed Generation**: Defined and generated seed objects using predefined keywords.

## Phase 3: Data Enrichment and Case Study Creation
- **Cable Creation**: Combined multiple seed objects to generate cables.
- **Case Study Generation**: Used cables to generate detailed case studies with injected data points.
- **Distillation to Standard Format**: Refined case studies into a standard dataset format (e.g., `input_text`, `features`, `context`, `target`, etc.).

## Phase 4: Model Training and Evaluation
- **Final Dataset Preparation**: Validated data integrity, balanced classes, and split data into training and testing sets.
- **Model Training**: Trained baseline and fine-tuned models using the prepared dataset.
- **Evaluation System Development**: Implemented an evaluation system to score models and compare fine-tuned models against baseline models.

## Phase 5: Continuous Improvement
- **Feedback Loop**: Gathered user and system feedback, iteratively refined and updated the dataset and models.
- **Documentation**: Maintained comprehensive documentation of methods, changes, and updates.
- **Community Engagement**: Engaged with the community for contributions and collaboration, opened issues and pull requests for enhancements.

# Notes on Data Structure

## Case Study Data Structure
- **Case ID**: Unique identifier for the case study.
- **Title**: The title of the case study.
- **Introduction**: Brief overview of the case study.
- **Background**: Detailed background information on the scenario.
- **Narrative**: Description of the situation and main issues.
- **Stakeholders**: List of key people involved, with a brief description of their relevance.
- **Challenges**: Main challenges faced in the scenario.
- **Mission**: Main goal to be achieved.
- **Directive**: Instructions on what needs to be evaluated or decided.
- **Objectives**: Specific goals that need to be met.
- **Data**: Relevant data points presented as a list of strings.
- **Problem Statement**: Main problem to be analyzed.
- **Questions**: Thought-provoking questions related to the problem.
- **Strategy**: Proposed strategy to address the problem.
- **Decision**: Decision made by the stakeholders.
- **Reasoning**: Detailed reasoning behind the decision.
- **Reflections**: Points for reflection on the ethical implications and importance of sustainability.
- **Outcomes**: Results of the decision.
- **Conclusion**: Summarize the impact and importance of the decision.
- **Ethical Considerations**: Any ethical dilemmas or considerations.
- **Alternative Perspectives**: Different viewpoints from stakeholders.
- **Chain of Thought**: Step-by-step reasoning process leading to the decision, to be wrapped in <thought> tags within the input field.
- **Input**: Comprehensive markdown document including all prior fields except for reasoning, reflections, outcomes, and conclusion.
- **Output**: Combined field that includes reasoning, reflections, outcomes, and conclusion.

# Next Actions
- Develop unit tests to prototype basic Ollama functionality.
- Verify chat completion and text generation using Ollama.
- Configure Serilog for colorful logging output.
- Set up configuration management using `Microsoft.Extensions.Configuration`.
- Integrate MediatR for a robust event system.
- Document and refine the case study generation pipeline.
