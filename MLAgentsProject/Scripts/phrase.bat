@echo off
call "%~dp0setenv.bat"
call "%ACTIVATE_SCRIPT%" >nul 2>&1
python "%~dp0phrase.py" %*
