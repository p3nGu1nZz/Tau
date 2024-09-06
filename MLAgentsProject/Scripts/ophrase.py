import json, argparse
from loguru import logger as log
from typing import List, Dict, Any, Tuple
from pydantic import ValidationError
from tenacity import retry, stop_after_attempt, wait_fixed
from ophrase_config import Config
from ophrase_proc import OphraseProcessor

class Ophrase:
    def __init__(self, cfg: Config):
        self.cfg = cfg
        self.processor = OphraseProcessor(cfg)
        self._log = log

    def check(self) -> None:
        self.processor.run_command(['ollama', '--version'], "Ollama not installed. Install it before running this script.")

    def pull(self) -> None:
        self.processor.run_command(['ollama', 'pull', self.cfg.model], f"Failed to pull model {self.cfg.model}.")

    def generate(self, text: str) -> Tuple[List[Dict[str, Any]], List[str]]:
        return self.processor.generate(text)

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
        final_result = op.processor.post_process(text, res, prompts, include_prompts)
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
