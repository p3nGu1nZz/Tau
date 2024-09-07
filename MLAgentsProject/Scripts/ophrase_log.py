# ophrase_log.py

from loguru import logger as log

class Log:
    DEBUG = "DEBUG"
    INFO = "INFO"
    WARNING = "WARNING"
    ERROR = "ERROR"
    CRITICAL = "CRITICAL"

    @staticmethod
    def setup(debug: bool) -> None:
        log.remove()
        if debug:
            log.add(lambda msg: print(msg, end=''), level=Log.DEBUG)

    @staticmethod
    def debug(message: str) -> None:
        log.debug(message)

    @staticmethod
    def info(message: str) -> None:
        log.info(message)

    @staticmethod
    def warning(message: str) -> None:
        log.warning(message)

    @staticmethod
    def error(message: str) -> None:
        log.error(message)

    @staticmethod
    def critical(message: str) -> None:
        log.critical(message)
