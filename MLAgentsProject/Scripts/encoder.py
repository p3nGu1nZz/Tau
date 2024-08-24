import warnings
warnings.filterwarnings("ignore", category=FutureWarning)
warnings.filterwarnings("ignore", category=UserWarning)

import sys
import json
import argparse
from sentence_transformers import SentenceTransformer

class TextEncoder:
    def __init__(self, model_name='all-MiniLM-L6-v2'):
        self.model = SentenceTransformer(model_name)
        self.max_length = self.model.get_max_seq_length()

    def encode(self, token):
        tokens = self.model.tokenize(token)
        if len(tokens) > self.max_length:
            raise ValueError(f"Input message exceeds maximum length of {self.max_length} tokens: '{token}'")
        
        embeddings = self.model.encode([token])[0]
        result = {
            "Token": token,
            "Embeddings": embeddings.tolist()
        }
        return result

def main():
    parser = argparse.ArgumentParser(description="Text Encoder")
    parser.add_argument("input_string", type=str, help="Input string to encode")

    args = parser.parse_args()

    input_string = args.input_string

    encoder = TextEncoder()

    try:
        result = encoder.encode(input_string)
        json_output = json.dumps(result, separators=(',', ':'))
        print(json_output)
    except ValueError as ve:
        error_output = json.dumps({"error": str(ve)}, separators=(',', ':'))
        print(error_output)
        sys.exit(1)
    except Exception as e:
        error_output = json.dumps({"error": f"Error processing input: {e}"}, separators=(',', ':'))
        print(error_output)
        sys.exit(1)

if __name__ == "__main__":
    main()
