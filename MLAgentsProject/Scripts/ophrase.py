# ophrase.py

import json, argparse
from typing import List, Dict, Any, Tuple
from pydantic import ValidationError
from tenacity import retry, stop_after_attempt, wait_fixed
from rich.console import Console
from ophrase_config import Config
from ophrase_gen import OphraseGen
from ophrase_task import OphraseTask
from ophrase_log import Log
from ophrase_serializer import serialize_output
from ophrase_const import Const

console = Console()

class Ophrase:
    def __init__(self, cfg: Config):
        self.cfg = cfg
        self.task = OphraseTask(cfg)
        self.gen = OphraseGen(cfg, self.task)
        self._log = Log

    def check(self) -> None:
        self.task.run_command(['ollama', '--version'], Const.RUN_COMMAND_ERROR)

    def pull(self) -> None:
        self.task.run_command(['ollama', 'pull', self.cfg.model], f"{Const.PULL_COMMAND_ERROR} {self.cfg.model}.")

    def generate(self, text: str) -> Tuple[List[Dict[str, Any]], List[str]]:
        responses = self.gen.generate_responses(text)
        response_prompts = [r['prompt'] for r in responses]
        return responses, response_prompts

    def validate(self, text: str, responses: List[str]) -> List[str]:
        return self.gen.generate_proofs(text, responses)

@retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
def main_logic(text: str, debug: bool, include_prompts: bool) -> None:
    if not debug:
        Log.setup(debug)
    Log.debug(Const.STARTING_MAIN_FUNCTION)
    try:
        cfg = Config(debug=debug)
        task = OphraseTask(cfg)
        op = Ophrase(cfg)
        op.task = task  # Assign task to Ophrase instance
        op.check()
        res, response_prompts = op.generate(text)
        proofs = op.validate(text, [r["response"] for r in res])
        proof_prompts = response_prompts
        final_result = serialize_output(text, res, response_prompts, proof_prompts, include_prompts)
        print(json.dumps(final_result, indent=2, separators=(',', ': ')))
    except ValidationError as e:
        error_output = {Const.ERROR_KEY: f"{Const.VALIDATION_ERROR}{e}"}
        if debug:
            import traceback
            error_output["trace"] = traceback.format_exc()
        console.print(json.dumps(error_output, indent=2, separators=(',', ': ')), style="bold red")
    except Exception as e:
        error_output = {Const.ERROR_KEY: f"{Const.ERROR_PROCESSING_INPUT}{e}"}
        if debug:
            import traceback
            error_output["trace"] = traceback.format_exc()
        console.print(json.dumps(error_output, indent=2, separators=(',', ': ')), style="bold red")
        raise SystemExit(1)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=Const.ARG_DESCRIPTION)
    parser.add_argument(Const.ARG_TEXT, type=str, help=Const.ARG_TEXT_HELP)
    parser.add_argument(Const.ARG_DEBUG, action="store_true", help=Const.ARG_DEBUG_HELP)
    parser.add_argument(Const.ARG_PROMPT, action="store_true", help=Const.ARG_PROMPT_HELP)
    args = parser.parse_args()
    main_logic(args.text, args.debug, args.prompt)
