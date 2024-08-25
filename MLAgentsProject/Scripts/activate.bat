@echo off
call "%~dp0setenv.bat"

echo Activating virtual environment...
call "%VENV_DIR%\Scripts\activate.bat"

if %errorlevel% neq 0 (
    echo Virtual environment activation failed.
    exit /b 1
)

echo Virtual environment activated.
