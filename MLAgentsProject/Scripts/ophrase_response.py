# ophrase_response.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_template import RESPONSE_TEMPLATE, TASKS, RESPONSE_INSTR, RESPONSE_SYS
import ollama as oll
import json
import subprocess as proc

class OphraseResponse:
    def __init__(self, cfg):
        self.cfg = cfg

    def run_command(self, cmd: List[str], error_msg: str = Const.RUN_COMMAND_ERROR) -> None:
        try:
            result = proc.run(cmd, capture_output=True, text=True)
            if result.returncode != 0:
                Log.error(result.stdout)
                raise Exception(error_msg)
        except FileNotFoundError:
            Log.error(error_msg)
            raise Exception(error_msg)

    def generate(self, original_text: str) -> List[Dict[str, Any]]:
        Log.debug(f"Generating responses for: {original_text}")
        results = []
        for task in TASKS.keys():
            response = self._generate_response(original_text, task)
            if Const.ERROR_KEY not in response:
                results.append(response)
            if len(results) >= 3:
                break
        return results

    def _generate_response(self, text: str, task: str) -> Dict[str, Any]:
        prompt = RESPONSE_TEMPLATE.render(system=RESPONSE_SYS, task=task, text=text, example=TASKS[task], instructions=RESPONSE_INSTR, lang=self.cfg.lang)
        Log.debug(f"Response Prompt: {prompt}")
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        Log.debug(f"Response: {resp}")
        Log.debug(f"Response string: {resp['response']}")
        resp_json = json.loads(resp['response'])
        Log.debug(f"Response JSON: {resp_json}")
        return {"prompt": prompt, "response": resp_json}
