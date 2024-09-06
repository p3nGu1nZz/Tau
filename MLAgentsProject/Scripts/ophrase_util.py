from loguru import logger as log
from typing import List, Dict, Any

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
