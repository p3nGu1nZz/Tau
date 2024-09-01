# Tau LLM Unity ML Agents Project

Welcome to the Tau LLM Unity ML Agents project! This project aims to build a Language Model (LLM) from scratch using Unity, ML-Agents, and Sentence Transformers.

## Table of Contents

- Introduction
- Features
- Installation
- Usage
- Project Structure
- Contributing
- License
- Acknowledgements

## Introduction

The Tau LLM Unity ML Agents project is an innovative initiative to create an intelligent chatbot using reinforcement learning and natural language processing techniques. By leveraging Unity's ML-Agents toolkit and Sentence Transformers, we aim to develop a robust and efficient language model.

## Features

- **Reinforcement Learning**: Train agents using Unity ML-Agents.
- **Natural Language Processing**: Utilize Sentence Transformers for language understanding.
- **Customizable Environments**: Create and modify training environments in Unity.
- **Scalable Training**: Support for training on multiple environments and configurations.
- **PostBuildProcessor**: Streamlined build process to run training commands in a production build outside of Unity.
- **Handling Unknown Words**: Robust handling of unknown words using the `all-MiniLM-L6-v2` model from Hugging Face.
- **Training Speed Optimization**: Techniques to speed up the training process, including granular reward systems and network configuration adjustments.
- **Custom Scripting Language**: TauLang for automating repetitive tasks and generating synthetic data.

## Installation

### Prerequisites

- Unity 2021.3 or later
- Python 3.8 or later
- ML-Agents Toolkit
- Sentence Transformers

### Steps

1. **Clone the repository**:
    ```bash
    git clone https://github.com/yourusername/tau-llm-unity-ml-agents.git
    cd Tau\MLAgentsProject
    ```

2. **Install Python dependencies**:
    ```bash
    .\Scripts\setup.bat
    ```

3. **Open the project in Unity**:
    - Launch Unity Hub.
    - Click on "Add" and select the project directory.

4. **Set up ML-Agents**:
    - Follow the ML-Agents installation guide.

5. **Clone ML-Agents Repository**:
    - Clone the ML-Agents repository from GitHub:
    ```bash
    git clone --branch develop https://github.com/Unity-Technologies/ml-agents.git
    ```

6. **Add ML-Agents Packages Locally**:
    - Open the Unity Package Manager.
    - Click on the "+" button and select "Add package from disk...".
    - Navigate to the cloned `ml-agents` repository and select the `com.unity.ml-agents` directory.
    - Repeat the process for the `com.unity.ml-agents.extensions` directory.

## Usage

### Training the Model

1. **Configure the training environment**:
    - Modify the `config.yaml` file to set up your training parameters.

2. **Start training**:
    ```bash
    mlagents-learn .\config\tau_agent_sac.yaml --run-id=tau_agent_sac_A1 --env .\Build\ --torch-device cuda --timeout-wait 300 --force
    ```

    - **Parameters**:
        - `.\config\tau_agent_sac.yaml`: Path to the configuration file.
        - `--run-id=tau_agent_sac_A1`: Unique identifier for the training run.
        - `--env .\Build\`: Path to the build directory.
        - `--torch-device cuda`: Specifies the use of a CUDA-enabled GPU for training.
        - `--timeout-wait 300`: Sets the timeout wait time.
        - `--force`: Forces the training to start even if there are warnings.

3. **Monitor training**:
    - Use TensorBoard to visualize training metrics:
    ```bash
    tensorboard --logdir results
    ```

### Running Inference

1. **Load the trained model in Unity**:
    - Drag and drop the trained model file into the Unity project.

2. **Test the agent**:
    - Run the Unity scene to see the agent in action.

## Project Structure

- `Assets/`: Unity project assets.
- `Scripts/`: Custom scripts for agents and environment setup.
- `Models/`: Trained models and configurations.
- `configs/`: Configuration files for training.
- `results/`: Training results and logs.
- `Data/`: Directory used to store the model's runtime data and training data for the database and other purposes.

## Contributing

We welcome contributions from the community! Please read our contributing guidelines for more information.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgements

- Unity ML-Agents Toolkit
- Sentence Transformers
