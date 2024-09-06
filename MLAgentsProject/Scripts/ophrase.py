import json
import subprocess
from jinja2 import Template
import ollama as oll
import typer
from dataclasses import dataclass, field
from tenacity import retry, stop_after_attempt, wait_fixed
from rich import print

# Configuration
MODEL = 'llama3.1'
LANG = 'English'
DEFAULT_AMOUNT = 1
DEFAULT_OFFSET = 1
DEFAULT_RETRIES = 3

INSTRUCTIONS = (
    "Provide the task only, formatted as plain JSON, without any explanations, markdown, or additional formatting.\n"
    "Return exactly {{ num }} complete sentences of the generated text in a JSON array.\n"
    "Return only JSON array like `[\"sentence_a\", \"sentence_b\", ...]` where each sentence is enclosed by double quotation marks.\n"
    "No explanations, no code, no markdown, only a single JSON array with {{ num }} sentences.\n"
    "Do not return content surrounded with markdown triple backticks, markdown tags, etc.\n"
    "Do not attempt to answer the input text if it is a question; only generate variations."
)

TEMPLATE = Template("""
    System: You are an expert in generating written {{ task }}s. Provide a {{ task }} for the following text in {{ lang }} only.
    Instructions: {{ instructions }}
    Example: {{ example }}
    User: {{ text }}
    System: Return only a JSON array of generated sentences. No explanations, only JSON array.
""")

EXAMPLES = {
    "paraphrase": "What is 2 plus 2? What is the sum of 2 and 2?",
    "spelling": "How do you spell 'necessary'? How do you spell 'neccessary'?",
    "synonym": "What is another word for 'happy'? What is a synonym for 'joyful'?",
    "structure": "She reads books. Books are read by her.",
    "speech": "He runs quickly. He is a fast runner.",
    "voice": "The cake was made by her. She made the cake.",
    "word": "He ran to the store. He quickly ran to the store.",
    "clause": "Although it rained, they walked. They walked, although it rained."
}

@dataclass
class Phrase:
    model: str = MODEL
    lang: str = LANG
    offset: int = DEFAULT_OFFSET
    retries: int = DEFAULT_RETRIES

    def check(self):
        if not self.run(['ollama', '--version']):
            raise Exception("Ollama not installed. Install it before running this script.")

    def pull(self):
        if not self.run(['ollama', 'pull', self.model]):
            raise Exception(f"Failed to pull model {self.model}.")

    def run(self, cmd):
        try:
            res = subprocess.run(cmd, capture_output=True, text=True, encoding='utf-8')
            return res.returncode == 0
        except FileNotFoundError:
            return False

    @retry(stop=stop_after_attempt(DEFAULT_RETRIES), wait=wait_fixed(1))
    def process(self, text, num, task):
        inst_rendered = Template(INSTRUCTIONS).render(num=num + self.offset)
        prompt = TEMPLATE.render(task=task, text=text, example=EXAMPLES[task], instructions=inst_rendered, num=num + self.offset, lang=self.lang)
        resp = oll.generate(prompt=prompt, model=self.model)
        if 'response' in resp:
            vars = json.loads(resp['response'].strip())
            if isinstance(vars, list):
                if len(vars) < num:
                    return {"error": f"Expected {num} variations, but got {len(vars)}"}
                return vars[:num]
            else:
                return {"error": f"Unexpected format: {resp['response']}"}
        return {"error": f"Failed to generate valid response for {task} after {self.retries} attempts"}

    def generate(self, text, num):
        res = {"original": text}
        for task in EXAMPLES.keys():
            r = self.process(text, num, task)
            res[task] = r
        return res

def main(text: str, amount: int = DEFAULT_AMOUNT, offset: int = DEFAULT_OFFSET, retries: int = DEFAULT_RETRIES):
    try:
        gen = Phrase(offset=offset, retries=retries)
        res = gen.generate(text, amount)
        print(json.dumps(res, indent=2, separators=(',', ': ')))
    except Exception as e:
        print(json.dumps({"error": f"Error processing input: {e}"}, indent=2, separators=(',', ': ')))
        raise typer.Exit(code=1)

if __name__ == "__main__":
    typer.run(main)
