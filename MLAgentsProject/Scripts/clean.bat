@echo off
call "%~dp0setenv.bat"

echo Cleaning up virtual environment and temporary files...

if exist "%VENV_DIR%" (
    echo Removing virtual environment directory...
    rd /s /q "%VENV_DIR%"
    if %errorlevel% neq 0 (
        echo Failed to remove virtual environment directory.
        exit /b 1
    )
)

if exist "%TEMP_DIR%" (
    echo Removing temporary directory...
    rd /s /q "%TEMP_DIR%"
    if %errorlevel% neq 0 (
        echo Failed to remove temporary directory.
        exit /b 1
    )
)

echo Cleanup complete!
exit /b 0
