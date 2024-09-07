# ophrase_proof.py

from typing import List
from ophrase_log import Log
from ophrase_const import Const

class OphraseProof:
    def validate(self, original_text: str, responses: List[List[str]]) -> List[str]:
        Log.debug(f"Validating responses for: {original_text}")
        # Flatten the list of responses
        flat_responses = [item for sublist in responses for item in sublist]
        return flat_responses
