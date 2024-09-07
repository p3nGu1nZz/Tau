# ophrase_prompt.py

from typing import NamedTuple

class OphrasePrompt(NamedTuple):
    task_type: str
    instructions: str
    system_prompt: str
    template: str
