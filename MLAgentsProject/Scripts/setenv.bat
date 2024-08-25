@echo off

set "PROJECT_ROOT=%~dp0\.."
set "TEMP_DIR=%PROJECT_ROOT%\.temp"
set "PYTHON_INSTALLER=python-3.10.11-amd64.exe"
set "PYTHON_DIR=%USERPROFILE%\.python\Python310"
set "VENV_NAME=ml-agents"
set "VENV_DIR=%PROJECT_ROOT%\venv\%VENV_NAME%"
set "ACTIVATE_SCRIPT=%~dp0activate.bat"
set "DEACTIVATE_SCRIPT=%~dp0deactivate.bat"
set "CLEAN_SCRIPT=%~dp0clean.bat"
set "ML_AGENTS_DIR=%PROJECT_ROOT%\ml-agents"
set "ML_AGENTS_ENVS_INSTALL=%ML_AGENTS_DIR%\ml-agents-envs"
set "ML_AGENTS_INSTALL=%ML_AGENTS_DIR%\ml-agents"
