import subprocess as proc
from loguru import logger as log
from typing import List, Dict, Any

def run_command(cmd: List[str], error_msg: str) -> None:
    try:
        result = proc.run(cmd, capture_output=True, text=True)
        if result.returncode != 0:
            log.error(result.stdout)
            raise Exception(error_msg)
    except FileNotFoundError:
        log.error(error_msg)
        raise Exception(error_msg)

def setup_logging(debug: bool) -> None:
    log.remove()
    if debug:
        log.add(lambda msg: print(msg, end=''), level="DEBUG")

def post_process(text: str, results: List[Dict[str, Any]], prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
    if isinstance(results, dict) and 'error' in results:
        return results
    combined_responses = []
    for result in results:
        combined_responses.extend(result['response'])
    output = {"original_text": text, "responses": combined_responses}
    if include_prompts:
        output["prompts"] = prompts
    return output
