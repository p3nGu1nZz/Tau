set "PROJECT_ROOT=%~dp0\.."
set "RESOURCES_DIR=%PROJECT_ROOT%\resources"
set "PYTHON_INSTALLER=python-3.10.11-amd64.exe"
set "PYTHON_DIR=%PROJECT_ROOT%\python"
set "VENV_NAME=ml-agents-env"
set "VENV_DIR=%PYTHON_DIR%\venv\%VENV_NAME%"
set "LOG_FILE=%RESOURCES_DIR%\python_install.log"

if not exist "%RESOURCES_DIR%" (
    mkdir "%RESOURCES_DIR%"
)

echo Downloading Python installer...
curl -o "%RESOURCES_DIR%\%PYTHON_INSTALLER%" https://www.python.org/ftp/python/3.10.11/%PYTHON_INSTALLER%

echo Installing Python...
start /wait ""%RESOURCES_DIR%\%PYTHON_INSTALLER%"" /quiet InstallAllUsers=1 PrependPath=1 TargetDir="%PYTHON_DIR%" > "%LOG_FILE%" 2>&1

if %errorlevel% neq 0 (
    echo Python installation failed. Check the log file for details.
    exit /b %errorlevel%
)

if not exist "%PYTHON_DIR%\python.exe" (
    echo Python installation failed. Python executable not found.
    exit /b 1
)

echo Creating virtual environment...
"%PYTHON_DIR%\python.exe" -m venv "%VENV_DIR%"

if %errorlevel% neq 0 (
    echo Virtual environment creation failed. Check the Python installation.
    exit /b %errorlevel%
)

if not exist "%VENV_DIR%\Scripts\activate" (
    echo Virtual environment activation script not found. Check the virtual environment path.
    exit /b 1
)

echo Activating virtual environment...
call "%VENV_DIR%\Scripts\activate"

if %errorlevel% neq 0 (
    echo Virtual environment activation failed. Check the virtual environment path.
    exit /b %errorlevel%
)

echo Installation complete!
