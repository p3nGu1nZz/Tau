from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const

def post_process(text: str, results: List[Dict[str, Any]], response_prompts: List[str], proof_prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
    if isinstance(results, dict) and Const.ERROR_KEY in results:
        Log.error(Const.ERROR_MESSAGE)
        return results
    combined_responses = []
    for result in results:
        combined_responses.extend(result['response'])
    output = {
        Const.ORIGINAL_TEXT_KEY: text,
        Const.RESPONSES_KEY: combined_responses,
        "proofs": combined_responses  # Mirror the responses for proofs
    }
    if include_prompts:
        output["response_prompts"] = response_prompts
        output["proof_prompts"] = proof_prompts
    return output
