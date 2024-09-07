# ophrase_config.py

from pydantic import BaseModel, Field
from ophrase_template import RESPONSE_INSTR, RESPONSE_SYS, PROOF_INSTR, PROOF_SYS, TASKS
from ophrase_const import Const

MODEL = Const.MODEL_DEFAULT
LANG = Const.LANG_DEFAULT

class Config(BaseModel):
    model: str = Field(default=MODEL)
    lang: str = Field(default=LANG)
    offset: int = Field(default=Const.OFFSET_DEFAULT)
    retries: int = Field(default=Const.RETRIES_DEFAULT)
    debug: bool = Field(default=Const.DEBUG_DEFAULT)
