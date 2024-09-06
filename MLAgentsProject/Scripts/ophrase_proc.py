from tenacity import retry, stop_after_attempt, wait_fixed
from loguru import logger as log
from typing import List, Dict, Any, Tuple
import json, subprocess as proc, ollama as oll
from ophrase_template import TEMPLATE, TASKS
from ophrase_config import INSTR, SYS
from ophrase_log import Log

class OphraseProcessor:
    def __init__(self, cfg):
        self.cfg = cfg
        self._log = log
        Log.setup(self.cfg.debug)

    def run_command(self, cmd: List[str], error_msg: str) -> None:
        try:
            result = proc.run(cmd, capture_output=True, text=True)
            if result.returncode != 0:
                self._log.error(result.stdout)
                raise Exception(error_msg)
        except FileNotFoundError:
            self._log.error(error_msg)
            raise Exception(error_msg)

    def post_process(self, text: str, results: List[Dict[str, Any]], prompts: List[str], include_prompts: bool) -> Dict[str, Any]:
        if isinstance(results, dict) and 'error' in results:
            return results
        combined_responses = []
        for result in results:
            combined_responses.extend(result['response'])
        output = {"original_text": text, "responses": combined_responses}
        if include_prompts:
            output["prompts"] = prompts
        return output

    def _gen(self, text: str, task: str) -> str:
        instr = INSTR
        return TEMPLATE.render(system=SYS, task=task, text=text, example=TASKS[task], instructions=instr, lang=self.cfg.lang)

    @retry(stop=stop_after_attempt(5), wait=wait_fixed(1))
    def _task(self, text: str, task: str) -> Dict[str, Any]:
        prompt = self._gen(text, task)
        self._log.debug(f"Prompt: {prompt}")
        self._log.debug('-' * 100)
        resp = oll.generate(prompt=prompt, model=self.cfg.model)
        self._log.debug(f"Response: {resp}")
        self._log.debug('-' * 100)
        resp_str = resp['response']
        self._log.debug(f"Response string: {resp_str}")
        resp_json = json.loads(resp_str)
        self._log.debug(f"Response JSON: {resp_json}")
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
        return [{"error": "Reached maximum retries without successful generation"}], prompts
