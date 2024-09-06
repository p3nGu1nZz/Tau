from pydantic import BaseModel, Field
from ophrase_template import INSTR, SYS, PROMPT, EXAMPLES

MODEL = 'llama3.1'
LANG = 'English'

class Config(BaseModel):
    model: str = Field(default=MODEL)
    lang: str = Field(default=LANG)
    offset: int = Field(default=1)
    retries: int = Field(default=5)
    debug: bool = Field(default=False)
