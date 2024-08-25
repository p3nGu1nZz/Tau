@echo off
call "%~dp0setenv.bat"

echo Deactivating virtual environment...
call "%VENV_DIR%\Scripts\deactivate.bat"

if %errorlevel% neq 0 (
    echo Virtual environment deactivation failed.
    exit /b 1
)

echo Virtual environment deactivated.
