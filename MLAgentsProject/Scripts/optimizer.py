import warnings
warnings.filterwarnings("ignore", category=FutureWarning)
warnings.filterwarnings("ignore", category=UserWarning)

import sys
import json
import argparse
from sklearn.decomposition import PCA
from sklearn.preprocessing import StandardScaler, MinMaxScaler
import numpy as np
import os

class Optimizer:
    def __init__(self, n_components=3):
        self.pca = PCA(n_components=n_components)
        self.standard_scaler = StandardScaler()
        self.min_max_scaler = MinMaxScaler(feature_range=(-1, 1))

    def load_embeddings(self, vocab_file):
        with open(vocab_file, 'r') as f:
            vocab_dict = json.load(f)
        tokens = [token["Name"] for token in vocab_dict["tokens"]]
        embeddings = np.array([token["Vector"] for token in vocab_dict["tokens"]])
        return tokens, embeddings

    def standardize_embeddings(self, embeddings):
        return self.standard_scaler.fit_transform(embeddings)

    def renormalize_embeddings(self, embeddings):
        return self.min_max_scaler.fit_transform(embeddings)

    def determine_optimal_components(self, embeddings):
        pca = PCA()
        pca.fit(embeddings)
        eigenvalues = pca.explained_variance_
        n_components_kaiser = np.sum(eigenvalues > 1)
        return eigenvalues, max(n_components_kaiser, 1)  # Ensure at least 1 component

    def perform_pca(self, embeddings, n_components):
        pca = PCA(n_components=n_components)
        reduced_embeddings = pca.fit_transform(embeddings)
        return reduced_embeddings

    def save_reduced_embeddings(self, tokens, reduced_embeddings, output_file):
        reduced_vocab_dict = {"tokens": [{"Name": token, "Vector": reduced_embeddings[i].tolist()} for i, token in enumerate(tokens)]}
        with open(output_file, 'w') as f:
            json.dump(reduced_vocab_dict, f, indent=4)

    def optimize(self, input_file, opt_type='pca', components=3):
        data_dir = os.path.join(os.path.dirname(__file__), '..', 'Data')
        input_path = os.path.join(data_dir, input_file)
        tokens, embeddings = self.load_embeddings(input_path)
        
        # Standardize embeddings
        embeddings = self.standardize_embeddings(embeddings)
        
        eigenvalues, optimal_n_components = self.determine_optimal_components(embeddings)
        print(f'Optimal number of components based on Kaiser\'s rule: {optimal_n_components}')
        print(f'Eigenvalues: {eigenvalues}')
        
        if opt_type == 'pca':
            reduced_embeddings = self.perform_pca(embeddings, components)
            # Re-normalize the reduced embeddings
            reduced_embeddings = self.renormalize_embeddings(reduced_embeddings)
        else:
            reduced_embeddings = embeddings
        
        output_file = self.get_output_filename(input_path)
        self.save_reduced_embeddings(tokens, reduced_embeddings, output_file)

    def get_output_filename(self, input_file):
        base, ext = os.path.splitext(input_file)
        return f"{base}_opti{ext}"

def main():
    parser = argparse.ArgumentParser(description="Optimizer")
    parser.add_argument("input_file", type=str, help="Input JSON file containing embeddings")
    parser.add_argument("--type", type=str, default="pca", help="Type of optimization to perform (default: pca)")
    parser.add_argument("--components", type=int, default=3, help="Number of PCA components to reduce to")

    args = parser.parse_args()

    optimizer = Optimizer()

    try:
        optimizer.optimize(args.input_file, args.type, args.components)
        output_file = optimizer.get_output_filename(args.input_file)
        print(f'Reduced embeddings saved to {output_file}')
    except Exception as e:
        print(f"An error occurred: {e}", file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()