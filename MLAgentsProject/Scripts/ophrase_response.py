# ophrase_response.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_template import RESPONSE_TEMPLATE, TASKS, RESPONSE_INSTR, RESPONSE_SYS
from ophrase_task import OphraseTask

class OphraseResponse(OphraseTask):
    def generate(self, original_text: str) -> List[Dict[str, Any]]:
        Log.debug(f"Generating responses for: {original_text}")
        results = []
        for task in TASKS.keys():
            response = self._generate(original_text, task, RESPONSE_TEMPLATE, RESPONSE_SYS, RESPONSE_INSTR)
            if Const.ERROR_KEY not in response:
                results.append(response)
            if len(results) >= 3:
                break
        return results
