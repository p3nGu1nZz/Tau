# ophrase_main.py

from tenacity import retry, stop_after_attempt, wait_fixed
from rich.console import Console
from ophrase_log import Log
from ophrase_serializer import serialize_output
from ophrase_const import Const
from ophrase_config import Config
from ophrase_task import OphraseTask
from ophrase import Ophrase  # Import Ophrase from ophrase

console = Console()

@retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
def main_logic(text: str, debug: bool, include_prompts: bool) -> None:
    if not debug:
        Log.setup(debug)
    Log.debug(Const.STARTING_MAIN_FUNCTION)
    try:
        cfg = Config(debug=debug)
        task = OphraseTask(cfg)
        op = Ophrase(cfg, task)
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
