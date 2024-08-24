@echo off
set script_dir=%~dp0
call %script_dir%..\venv\mlagents\Scripts\activate
python %script_dir%encoder.py %*
