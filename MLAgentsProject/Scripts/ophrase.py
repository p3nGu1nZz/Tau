import json, ollama as oll, argparse
from tenacity import retry, stop_after_attempt, wait_fixed
from loguru import logger as log
from typing import List, Tuple, Dict, Any
from pydantic import ValidationError
from ophrase_config import Config, INSTR, SYS, EXAMPLES
from ophrase_template import TEMPLATE
from ophrase_util import run_command, setup_logging, post_process

class Ophrase:
    def __init__(self, cfg: Config):
        self.cfg = cfg
        setup_logging(self.cfg.debug)
        self._log = log

    def check(self) -> None:
        run_command(['ollama', '--version'], "Ollama not installed. Install it before running this script.")

    def pull(self) -> None:
        run_command(['ollama', 'pull', self.cfg.model], f"Failed to pull model {self.cfg.model}.")

    def _gen(self, text: str, task: str) -> str:
        instr = INSTR
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
        final_result = post_process(text, res, prompts, include_prompts)
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
