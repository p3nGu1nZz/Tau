# ThinkingDatasetProject

Welcome to the ThinkingDatasetProject! This project aims to create a high-quality dataset focusing on thinking, reasoning, and reflection around various economic case studies. By leveraging advanced AI models and techniques, we intend to generate and refine content that can be used to train and evaluate machine learning models, particularly in the realm of economics and decision-making.

## Project Overview

The ThinkingDatasetProject is part of the larger TauProject, designed to explore and innovate in the field of artificial intelligence. This sub-project focuses on generating a specialized dataset to support AI in understanding and reasoning about complex economic scenarios.

## Objectives

- **Generate High-Quality Dataset**: Create a comprehensive dataset that includes economic case studies with detailed annotations.
- **Leverage Semantic Kernel**: Utilize Semantic Kernel and Ollama connectors to streamline the process of data generation and refinement.
- **Evaluate Models**: Develop an evaluation system to score fine-tuned models against baseline models, ensuring high performance and accuracy.
- **Continuous Iteration**: Refine and update the dataset based on feedback and new insights, maintaining a high standard of quality.

## Project Structure

- **Raw Data Ingestion**: Collect initial data and store it in a raw format.
- **Data Cleaning and Preprocessing**: Remove duplicates, handle missing values, and normalize data.
- **Seed Generation**: Generate seed objects using predefined keywords.
- **Cable Creation**: Combine multiple seed objects to generate cables.
- **Case Study Generation**: Use cables to generate detailed case studies.
- **Distillation to Standard Format**: Refine case studies into a standard dataset format.
- **Final Dataset Preparation**: Ensure the dataset is ready for model training.
- **Model Training and Evaluation**: Train models using the prepared dataset and evaluate their performance.
- **Continuous Iteration**: Continuously refine and update the dataset.

## Key Components

- **Semantic Kernel**: Integrates with the Ollama connector to utilize local models for efficient data generation and processing.
- **Ollama Connector**: Provides optimized and quantized models for fast and efficient content generation.
- **Evaluation System**: Scores models against baseline models to ensure high performance.

## How to Use

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/p3nGu1nZz/Tau.git
   cd Tau/ThinkingDatasetProject
   ```

2. Install necessary packages:
   ```bash
   dotnet add package Microsoft.SemanticKernel
   dotnet add package Microsoft.SemanticKernel.Connectors.Ollama --version 1.25.0-alpha
   ```
   
   see `https://www.nuget.org/packages/Microsoft.SemanticKernel.Connectors.Ollama` for more information on the connector.
   
3. Configure and run the project:
   dotnet run

### Interaction

- **Generate Data**: Use the integrated models to generate economic case studies.
- **Evaluate Models**: Run the evaluation system to score and compare models.

## Contributing

We welcome contributions! Feel free to fork the repository and submit pull requests. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- **Microsoft Semantic Kernel**: For providing a robust framework for AI model integration.
- **Ollama**: For their optimized and quantized models.
