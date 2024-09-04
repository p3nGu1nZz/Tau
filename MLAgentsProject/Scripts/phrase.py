#import warnings
#warnings.filterwarnings("ignore", category=FutureWarning)
#warnings.filterwarnings("ignore", category=UserWarning)

import sys
import json
import argparse
import torch
from transformers import LlamaTokenizer, LlamaForCausalLM

class PhraseGenerator:
    def __init__(self, model_name='openlm-research/open_llama_7b_v2'):
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.tokenizer = LlamaTokenizer.from_pretrained(model_name)
        self.model = LlamaForCausalLM.from_pretrained(model_name).to(self.device)

    def generate_variations(self, text, num_variations, task):
        variations = []
        for _ in range(num_variations):
            prompt = f"System: You are an expert in generating {task}. Please provide a {task} for the following text.\nUser: {text}\nSystem:"
            input_ids = self.tokenizer(prompt, return_tensors="pt").input_ids.to(self.device)
            output = self.model.generate(input_ids, max_length=128, num_return_sequences=1)
            generated_text = self.tokenizer.decode(output[0], skip_special_tokens=True)
            variations.append(generated_text.split("System:")[-1].strip())
        return variations

def main():
    parser = argparse.ArgumentParser(description="Phrase Generator")
    parser.add_argument("num_variations", type=int, help="Number of variations to generate")
    parser.add_argument("input_string", type=str, help="Input string to generate variations for")

    args = parser.parse_args()

    num_variations = args.num_variations
    input_string = args.input_string

    generator = PhraseGenerator()

    try:
        paraphrases = generator.generate_variations(input_string, num_variations, "paraphrase")
        spelling_variants = generator.generate_variations(input_string, num_variations, "spelling variant")

        result = {
            "original": input_string,
            "paraphrases": paraphrases,
            "spelling_variants": spelling_variants
        }

        json_output = json.dumps(result, separators=(',', ':'))
        print(json_output)
    except Exception as e:
        error_output = json.dumps({"error": f"Error processing input: {e}"}, separators=(',', ':'))
        print(error_output)
        sys.exit(1)

if __name__ == "__main__":
    main()
