import json
import argparse
from typing import List, Dict, Any, Tuple  # Import the necessary types
from ophrase_const import Const
from ophrase_config import Config
from ophrase_manager import OphraseManager  # Import OphraseManager
from ophrase_main import OphraseMain  # Import OphraseMain

class Ophrase:
    def __init__(self, cfg: Config):
        self.manager = OphraseManager(cfg)  # Initialize OphraseManager

    def check(self) -> None:
        self.manager.check()

    def pull(self) -> None:
        self.manager.pull()

    def generate(self, text: str) -> Tuple[List[Dict[str, Any]], List[str]]:
        return self.manager.generate(text)

    def validate(self, text: str, responses: List[str]) -> List[str]:
        return self.manager.validate(text, responses)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=Const.ARG_DESCRIPTION)
    parser.add_argument(Const.ARG_TEXT, type=str, help=Const.ARG_TEXT_HELP)
    parser.add_argument(Const.ARG_DEBUG, action="store_true", help=Const.ARG_DEBUG_HELP)
    parser.add_argument(Const.ARG_PROMPT, action="store_true", help=Const.ARG_PROMPT_HELP)
    args = parser.parse_args()

    cfg = Config(debug=args.debug)
    ophrase = Ophrase(cfg)
    ophrase_main = OphraseMain(cfg)

    try:
        ophrase_main._run(args.text, args.debug, args.prompt)
    except Exception as e:
        error_output = {Const.ERROR_KEY: f"{Const.ERROR_PROCESSING_INPUT}{e}"}
        if args.debug:
            import traceback
            error_output["trace"] = traceback.format_exc()
        print(json.dumps(error_output, indent=2, separators=(',', ': ')))
        raise SystemExit(1)
