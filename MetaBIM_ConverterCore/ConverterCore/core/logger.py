import datetime
import inspect

class Logger:

    # Static global switch
    debug_mode = True
    debug_filname = False

    @staticmethod
    def log(message: str, level: str = "INFO"):
        if not Logger.debug_mode and level == "DEBUG":
            return

        # Timestamp
        timestamp = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

        # Get caller file and line number
        frame = inspect.currentframe().f_back
        filename = inspect.getframeinfo(frame).filename.split("/")[-1]
        lineno = inspect.getframeinfo(frame).lineno

        # Format message
        if Logger.debug_filname:
            print(f"[{timestamp}] [{level}]\n({filename}:{lineno})\n{message}")
        else:
            print(f"[{timestamp}] [{level}] {message}")

    @staticmethod
    def info(message: str):
        Logger.log(message, "INFO")

    @staticmethod
    def warn(message: str):
        Logger.log(message, "WARN")

    @staticmethod
    def error(message: str):
        Logger.log(message, "ERROR")

    @staticmethod
    def debug(message: str):
        Logger.log(message, "DEBUG")
