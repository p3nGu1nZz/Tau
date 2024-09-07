# ophrase_serializer.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const

def serialize_output(text: str, results: List[Dict[str, Any]], response_prompts: List[str], proof_prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
    if isinstance(results, dict) and Const.ERROR_KEY in results:
        Log.error(Const.ERROR_MESSAGE)
        return results
    combined_responses = []
    for result in results:
        combined_responses.extend(result["response"])
    output = {
        Const.ORIGINAL_TEXT_KEY: text,
        Const.RESPONSES_KEY: combined_responses,
        Const.PROOFS_KEY: combined_responses
    }
    if include_prompts:
        output[Const.PROMPTS_KEY] = {
            Const.RESPONSES_KEY: response_prompts,
            Const.PROOFS_KEY: proof_prompts
        }
    return output
