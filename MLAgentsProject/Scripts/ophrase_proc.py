from tenacity import retry, stop_after_attempt, wait_fixed
from typing import List, Dict, Any, Tuple
import json, subprocess as proc, ollama as oll
from ophrase_template import TEMPLATE, TASKS
from ophrase_config import INSTR, SYS
from ophrase_log import Log
from ophrase_util import post_process

class OphraseProcessor:
    def __init__(self, cfg):
        self.cfg = cfg
        Log.setup(self.cfg.debug)

    def run_command(self, cmd: List[str], error_msg: str) -> None:
        try:
            result = proc.run(cmd, capture_output=True, text=True)
            if result.returncode != 0:
                Log.error(result.stdout)
                raise Exception(error_msg)
        except FileNotFoundError:
            Log.error(error_msg)
            raise Exception(error_msg)

    def _gen(self, text: str, task: str) -> str:
        instr = INSTR
        return TEMPLATE.render(system=SYS, task=task, text=text, example=TASKS[task], instructions=instr, lang=self.cfg.lang)

    @retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
    def _task(self, text: str, task: str) -> Dict[str, Any]:
        prompt = self._gen(text, task)
        Log.debug(f"Prompt: {prompt}")
        Log.debug('-' * 100)
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        Log.debug(f"Response: {resp}")
        Log.debug('-' * 100)
        resp_str = resp['response']
        Log.debug(f"Response string: {resp_str}")
        resp_json = json.loads(resp_str)
        Log.debug(f"Response JSON: {resp_json}")
        return {"prompt": prompt, "response": resp_json}

    def generate(self, text: str) -> Tuple[List[Dict[str, Any]], List[str]]:
        results, prompts = [], []
        for task in TASKS.keys():
            result = self._task(text, task)
            if 'error' not in result:
                results.append(result)
                prompts.append(result['prompt'])
            if len(results) >= 3:
                return results[:3], prompts
        return results, prompts
