from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const

def post_process(text: str, results: List[Dict[str, Any]], prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
    if isinstance(results, dict) and Const.ERROR_KEY in results:
        Log.error(Const.ERROR_MESSAGE)
        return results
    combined_responses = []
    for result in results:
        combined_responses.extend(result['response'])
    output = {Const.ORIGINAL_TEXT_KEY: text, Const.RESPONSES_KEY: combined_responses}
    if include_prompts:
        output[Const.PROMPTS_KEY] = prompts
    return output
