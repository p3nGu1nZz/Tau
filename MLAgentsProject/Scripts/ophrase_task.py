# ophrase_task.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
import subprocess as proc

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
