# ophrase_util.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const

def post_process(text: str, results: List[Dict[str, Any]], response_prompts: List[str], proof_prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
    if isinstance(results, dict) and Const.ERROR_KEY in results:
        Log.error(Const.ERROR_MESSAGE)
        return results
    combined_responses = []
    for result in results:
        combined_responses.extend(result["response"])  # Ensure this key matches the actual response structure
    output = {
        Const.ORIGINAL_TEXT_KEY: text,
        Const.RESPONSES_KEY: combined_responses,
        Const.PROOFS_KEY: combined_responses  # Mirror the responses for proofs
    }
    if include_prompts:
        output[Const.PROMPTS_KEY] = {
            Const.RESPONSES_KEY: response_prompts,
            Const.PROOFS_KEY: proof_prompts
        }
    return output
