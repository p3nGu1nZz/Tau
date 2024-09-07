# ophrase_proof.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_template import PROOF_TEMPLATE, TASKS, PROOF_INSTR, PROOF_SYS
from ophrase_task import OphraseTask

class OphraseProof(OphraseTask):
    def validate(self, original_text: str, responses: List[str]) -> List[str]:
        Log.debug(f"Validating responses for: {original_text}")
        valid_responses = []
        for response in responses:
            proof_response = self._generate(response, "paraphrase", PROOF_TEMPLATE, PROOF_SYS, PROOF_INSTR)
            valid_responses.append(proof_response["response"])  # Ensure this key matches the actual response structure
        return valid_responses
