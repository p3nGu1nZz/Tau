# ophrase_task.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_template import TASKS
import subprocess as proc
import ollama as oll
import json

class OphraseTask:
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

    def generate(self, text: str, task: str, template, system_prompt, instructions) -> Dict[str, Any]:
        prompt = template.render(system=system_prompt, task=task, text=text, example=TASKS[task], instructions=instructions, lang=self.cfg.lang)
        Log.debug(f"Prompt: {prompt}")
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        Log.debug(f"Response: {resp}")
        Log.debug(Const.PROMPT_SEPARATOR)
        resp_str = resp['response']
        Log.debug(f"Response string: {resp_str}")
        resp_json = json.loads(resp_str)
        Log.debug(f"Response JSON: {resp_json}")
        return {"prompt": prompt, "response": resp_json}
