# ophrase_task.py

from typing import NamedTuple

class OphraseTask(NamedTuple):
    task_type: str
    instructions: str
    system_prompt: str
    template: str
