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
