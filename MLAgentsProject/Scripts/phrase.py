import sys
import json
import argparse
import subprocess
import ollama

class Phrase:
    def __init__(self, model='llama3.1'):
        self.model = model
        self._check_ollama()
        self._pull_model()

    def _check_ollama(self):
        if not self._run_cmd(['ollama', '--version'], capture_output=True):
            raise Exception("Ollama not installed. Install it before running this script.")

    def _pull_model(self):
        if not self._run_cmd(['ollama', 'pull', self.model], capture_output=True):
            raise Exception(f"Failed to pull model {self.model}.")

    def _run_cmd(self, cmd, capture_output=False):
        try:
            result = subprocess.run(cmd, capture_output=capture_output, text=True, encoding='utf-8')
            return result if result.returncode == 0 else None
        except FileNotFoundError:
            return None

    def process(self, text, num, task, example, rules):
        prompt = (
            f"System: You are an expert in generating {task}. Provide a {task} for the following text in English only.\n"
            f"Rules: {rules}\n"
            f"Example: {example}\n"
            f"User: {text}\n"
            f"System: "
        )

        response = ollama.generate(prompt=prompt, model=self.model)
        if 'response' in response:
            try:
                variations = json.loads(response['response'].strip())
                if isinstance(variations, list) and all(isinstance(item, str) for item in variations):
                    return variations
                else:
                    return {"error": f"Unexpected format: {response['response']}"}
            except json.JSONDecodeError:
                return {"error": f"Failed to decode JSON: {response['response']}"}
        else:
            return {"error": f"Unexpected format: {response}"}

def main():
    parser = argparse.ArgumentParser(description="Phrase Generator")
    parser.add_argument("num", type=int, help="Number of variations to generate")
    parser.add_argument("text", type=str, help="Input string to generate variations for")

    args = parser.parse_args()

    examples = {
        "paraphrase": ["The cat sat on the mat.", "The feline rested on the rug."],
        "spelling": ["The color is red.", "The colour is red."],
        "synonym": ["She is happy.", "She is joyful."],
        "structure": ["She reads books.", "Books are read by her."],
        "speech": ["He is a fast runner.", "He runs quickly."],
        "voice": ["The cake was made by her.", "She made the cake."],
        "words": ["He ran to the store.", "He quickly ran to the store."],
        "combine": ["She went to the store. She bought milk.", "She went to the store and bought milk."],
        "clauses": ["Although it rained, they walked.", "They walked, although it rained."]
    }

    rules = (
        "Provide the task only, formatted as plain JSON, without any explanations, markdown, or additional formatting.\n"
        "Return only {num} complete sentences of the generated variations.\n"
        "Return only JSON like `[sentence_a, ...]` where each sentence is enclosed by double quotation marks.\n"
        "No explanations, no code, no markdown, only a single JSON array with {num} sentences.\n"
        "Do not return content surrounded with markdown triple backticks and markdown tags.\n"
    )

    generator = Phrase()

    try:
        result = {"original": args.text}
        for task, example in examples.items():
            result[task] = generator.process(args.text, args.num, task, json.dumps(example), rules)

        print(json.dumps(result, indent=2, separators=(',', ': ')))
    except Exception as e:
        print(json.dumps({"error": f"Error processing input: {e}"}, indent=2, separators=(',', ': ')))
        sys.exit(1)

if __name__ == "__main__":
    main()
