from loguru import logger as log

def setup_logging(debug: bool) -> None:
    log.remove()
    if debug:
        log.add(lambda msg: print(msg, end=''), level="DEBUG")
