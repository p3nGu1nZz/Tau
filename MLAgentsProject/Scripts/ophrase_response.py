# ophrase_response.py

from typing import List, Dict, Any
from ophrase_log import Log
from ophrase_const import Const
from ophrase_gen import OphraseGen  # Updated import

class OphraseResponse:
    def __init__(self, cfg, task):
        self.gen = OphraseGen(cfg,