import json, subprocess as proc, ollama as oll, argparse
from jinja2 import Template as T
from tenacity import retry, stop_after_attempt, wait_fixed
from loguru import logger as log
from typing import List, Tuple, Dict, Any
from ophrase_config import Config, INSTR, SYS, PROMPT, EXAMPLES

TEMPLATE = T(PROMPT)

class Ophrase:
    def __init__(self, cfg: Config):
        self.cfg = cfg
        log.remove()
        if self.cfg.debug:
            log.add(lambda msg: print(msg, end=''), level="DEBUG")
        self._log = log

    def check(self) -> None:
        self._run(['ollama', '--version'], "Ollama not installed. Install it before running this script.")

    def pull(self) -> None:
        self._run(['ollama', 'pull', self.cfg.model], f"Failed to pull model {self.cfg.model}.")

    def _run(self, cmd: List[str], error_msg: str) -> None:
        try:
            result = proc.run(cmd, capture_output=True, text=True)
            if result.returncode != 0:
                self._log.error(result.stdout)
                raise Exception(error_msg)
        except FileNotFoundError:
            self._log.error(error_msg)
            raise Exception(error_msg)

    def _gen(self, text: str, task: str) -> str:
        instr = T(INSTR).render(amount=3)
        return TEMPLATE.render(system=SYS, task=task, text=text, example=EXAMPLES[task], instructions=instr, lang=self.cfg.lang)

    @retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
    def _task(self, text: str, task: str) -> Dict[str, Any]:
        prompt = self._gen(text, task)
        self._log.debug(f"Prompt: {prompt}")
        self._log.debug('-' * 100)
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        self._log.debug(f"Response: {resp}")
        self._log.debug('-' * 100)
        resp_str = resp['response']
        self._log.debug(f"Response string: {resp_str}")
        resp_json = json.loads(resp_str)
        self._log.debug(f"Response JSON: {resp_json}")
        return {"prompt": prompt, "response": resp_json}

    def generate(self, text: str) -> Tuple[List[Dict[str, Any]], List[str]]:
        results, prompts = [], []
        for task in EXAMPLES.keys():
            result = self._task(text, task)
            if 'error' not in result:
                results.append(result)
                prompts.append(result['prompt'])
            if len(results) >= 3:
                return results[:3], prompts
        return [{"error": "Reached maximum retries without successful generation"}], prompts

    def post_process(self, text: str, results: List[Dict[str, Any]], prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
        if isinstance(results, dict) and 'error' in results:
            return results
        combined_responses = []
        for result in results:
            combined_responses.extend(result['response'])
        output = {"original_text": text, "responses": combined_responses}
        if include_prompts:
            output["prompts"] = prompts
        return output

@retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
def main(text: str, debug: bool, include_prompts: bool) -> None:
    if not debug:
        log.remove()
    log.debug("Starting main function")
    try:
        cfg = Config(debug=debug)
        op = Ophrase(cfg)
        op.check()
        res, prompts = op.generate(text)
        final_result = op.post_process(text, res, prompts, include_prompts)
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
    parser.add_argument("--prompt", action="store_true", help="Include prompts in the output JSON")
    args = parser.parse_args()
    main(args.text, args.debug, args.prompt)
