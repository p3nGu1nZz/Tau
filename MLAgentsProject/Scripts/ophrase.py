import json, subprocess as proc, ollama as oll, argparse
from jinja2 import Template as T
from pydantic import BaseModel, Field, ValidationError
from tenacity import retry, stop_after_attempt, wait_fixed
from loguru import logger as log

# Config
MODEL, LANG = 'llama3.1', 'English'
DEFAULT_AMOUNT, DEFAULT_OFFSET, DEFAULT_RETRIES = 3, 4, 5
INSTRUCTIONS = (
    "Provide the task as plain JSON, no explanations or markdown.\n"
    "Return exactly {{ amount }} sentences in a JSON array.\n"
    "Only one JSON array, e.g., [\"sentence_a\", \"sentence_b\", ...]\n"
    "Sentences must be in double quotes.\n"
    "No markdown or code.\n"
    "Do not answer the input; only generate variations.\n"
    "No explanations; only a JSON array with {{ amount }} sentences.\n"
    "For spelling tasks, do not provide the spelling; only variations.\n"
    "Match the punctuation of the input text (e.g., questions, exclamations).\n"
    "If the User text is a question, generate {{ amount }} sentences rhetorically as a question."
)
SYSTEM = "You are an expert writing system that generates {{ task }}s. Provide a {{ task }} for the following text in {{ lang }}."
PROMPT = (
    "System: {{ system }}\n"
    "Instructions: {{ instructions }}\n"
    "Example: {{ example }}\n"
    "User: {{ text }}\n"
    "System: Return only a JSON array of sentences. No explanations, only JSON array."
)
EXAMPLES = {
    "paraphrase": "What is 2 plus 2? What is the sum of 2 and 2?",
    "spelling": "How do you spell 'necessary'? How do you spell 'neccessary'?",
    "synonym": "What is another word for 'happy'? What is a synonym for 'joyful'?"
}
TEMPLATE = T(PROMPT)

class Config(BaseModel):
    model: str = Field(default=MODEL)
    lang: str = Field(default=LANG)
    offset: int = Field(default=DEFAULT_OFFSET)
    retries: int = Field(default=DEFAULT_RETRIES)
    debug: bool = Field(default=False)

class Ophrase:
    def __init__(self, cfg: Config):
        self.cfg = cfg
        log.remove()
        if self.cfg.debug:
            log.add(lambda msg: print(msg, end=''), level="DEBUG")

    def check_reqs(self):
        pass  # Skipping pip-check-reqs check for now

    def check(self):
        self._run(['ollama', '--version'], "Ollama not installed. Install it before running this script.")

    def pull(self):
        self._run(['ollama', 'pull', self.cfg.model], f"Failed to pull model {self.cfg.model}.")

    def _run(self, cmd, error_msg):
        try:
            result = proc.run(cmd, capture_output=True, text=True)
            if result.returncode != 0:
                self._log(result.stdout, error=True)
                raise Exception(error_msg)
        except FileNotFoundError:
            self._log(error_msg, error=True)
            raise Exception(error_msg)

    def _gen(self, text, amount, task):
        instructions = T(INSTRUCTIONS).render(amount=amount)
        return TEMPLATE.render(system=SYSTEM, task=task, text=text, example=EXAMPLES[task], instructions=instructions, amount=amount, lang=self.cfg.lang)

    def _log(self, message, error=False):
        if error:
            log.error(message)
        elif self.cfg.debug:
            log.debug(message)

    @retry(stop=stop_after_attempt(DEFAULT_RETRIES), wait=wait_fixed(1))
    def _task(self, text, amount, task):
        prompt = self._gen(text, amount, task)
        self._log(f"Prompt: {prompt}")
        self._log('-' * 100)
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        self._log(f"Response: {resp}")
        self._log('-' * 100)
        response_text = json.loads(resp['response'])
        return {"prompt": prompt, "response": response_text}

    def generate(self, text, amount):
        results = []
        tasks = list(EXAMPLES.keys())
        for task in tasks:
            result = self._task(text, amount, task)
            if 'error' not in result:
                results.append(result)
            if len(results) >= amount:
                return results[:amount]
        return {"error": "Reached maximum retries without successful generation"}

    def post_process(self, results):
        if isinstance(results, dict) and 'error' in results:
            return results
        return results

@retry(stop=stop_after_attempt(3), wait=wait_fixed(1))
def main(text: str, debug: bool):
    if not debug:
        log.remove()
    log.debug("Starting main function")
    try:
        cfg = Config(debug=debug)
        op = Ophrase(cfg)
        op.check()
        res = op.generate(text, DEFAULT_AMOUNT)
        final_result = op.post_process(res)
        print(json.dumps(final_result, indent=2, separators=(',', ': ')))
    except ValidationError as e:
        log.error(f"Validation error: {e}")
    except Exception as e:
        log.error(f"Error processing input: {e}")
        raise SystemExit(1)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Ophrase script")
    parser.add_argument("text", type=str, help="Input text")
    parser.add_argument("--debug", action="store_true", help="Enable debug logging")
    args = parser.parse_args()
    main(args.text, args.debug)
