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
            proof_responses = self._generate(response, "paraphrase", PROOF_TEMPLATE, PROOF_SYS, PROOF_INSTR)
            if self._is_valid_response(original_text, proof_responses['response']):
                valid_responses.append(response)
            else:
                Log.debug(f"Invalid response: {response}")
        return valid_responses

    def _is_valid_response(self, original_text: str, proof_responses: List[str]) -> bool:
        # Simple validation logic: check if the proof responses contain key words from the original text
        original_words = set(original_text.lower().split())
        for proof_response in proof_responses:
            response_words = set(proof_response.lower().split())
            common_words = original_words.intersection(response_words)
            if len(common_words) > 0:
                return True
        return False
