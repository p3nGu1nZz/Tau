# ophrase_gen.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_template import TASKS, RESPONSE_TEMPLATE, RESPONSE_SYS, RESPONSE_INSTR, PROOF_TEMPLATE, PROOF_SYS, PROOF_INSTR
import ollama as oll
import json

class OphraseGen:
    def __init__(self, cfg, task):
        self.cfg = cfg
        self.task = task

    def generate_responses(self, original_text: str) -> List[Dict[str, Any]]:
        Log.debug(f"Generating responses for: {original_text}")
        results = []
        for task in TASKS.keys():
            response = self.task.generate(original_text, task, RESPONSE_TEMPLATE, RESPONSE_SYS, RESPONSE_INSTR)
            if Const.ERROR_KEY not in response:
                results.append(response)
            if len(results) >= 3:
                break
        return results

    def generate_proofs(self, original_text: str, responses: List[str]) -> List[str]:
        Log.debug(f"Generating proofs for: {original_text}")
        valid_responses = []
        for response in responses:
            proof_response = self.task.generate(response, "paraphrase", PROOF_TEMPLATE, PROOF_SYS, PROOF_INSTR)
            valid_responses.append(proof_response["response"])  # Ensure this key matches the actual response structure
        return valid_responses
