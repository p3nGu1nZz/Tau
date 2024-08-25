@echo off
call "%~dp0setenv.bat"

set "START_TIME=%time%"

if "%1"=="--clean" (
    call :run_clean
    goto :exit
)

if not exist "%TEMP_DIR%" (
    mkdir "%TEMP_DIR%"
)

echo Starting setup process...
call :check_python
if %errorlevel% neq 0 goto :error

call :install_python
if %errorlevel% neq 0 goto :error

call :create_venv
if %errorlevel% neq 0 goto :error

call "%ACTIVATE_SCRIPT%"
if %errorlevel% neq 0 goto :error

call :upgrade_pip
if %errorlevel% neq 0 goto :error

call :install_pytorch
if %errorlevel% neq 0 goto :error

call :check_ml_agents
if %errorlevel% neq 0 goto :error

call :install_ml_agents
if %errorlevel% neq 0 goto :error

call :clean_temp
if %errorlevel% neq 0 goto :error

call "%DEACTIVATE_SCRIPT%"
if %errorlevel% neq 0 goto :error

set "END_TIME=%time%"
call "%~dp0utilities.bat" :calculate_duration "%START_TIME%" "%END_TIME%"

echo Installation complete!
goto :exit

:run_clean
echo Running clean script...
call "%CLEAN_SCRIPT%"
if %errorlevel% neq 0 goto :error
goto :exit

:check_python
echo Checking Python version...
for /f "tokens=2 delims= " %%i in ('python --version 2^>^&1') do set PYTHON_VERSION=%%i
echo Detected Python version: %PYTHON_VERSION%
if "%PYTHON_VERSION%"=="3.10.11" (
    echo Python 3.10.11 is already installed.
    exit /b 0
) else (
    echo Python 3.10.11 is not installed.
    exit /b 1
)

:install_python
if "%PYTHON_VERSION%"=="3.10.11" (
    exit /b 0
)
echo Downloading Python installer...
curl -o "%TEMP_DIR%\%PYTHON_INSTALLER%" https://www.python.org/ftp/python/3.10.11/%PYTHON_INSTALLER%

if %errorlevel% neq 0 (
    echo Python download failed.
    goto :error
)

echo Installing Python...
if exist "%TEMP_DIR%\%PYTHON_INSTALLER%" (
    start /wait "" "%TEMP_DIR%\%PYTHON_INSTALLER%" /quiet InstallAllUsers=1 PrependPath=1 TargetDir="%PYTHON_DIR%"
    if %errorlevel% neq 0 (
        echo Python installation failed.
        goto :error
    )
) else (
    echo Error: missing %PYTHON_INSTALLER%
    goto :error
)

if not exist "%PYTHON_DIR%\python.exe" (
    echo Python installation failed. Python executable not found.
    goto :error
)
exit /b 0

:create_venv
echo Creating virtual environment...
echo Using Python executable: "%PYTHON_DIR%\python.exe"
echo Virtual environment path: "%VENV_DIR%"
"%PYTHON_DIR%\python.exe" -m venv "%VENV_DIR%"

if %errorlevel% neq 0 (
    echo Virtual environment creation failed with error code %errorlevel%.
    goto :error
)

if not exist "%VENV_DIR%\Scripts\activate.bat" (
    echo Virtual environment activation script not found.
    goto :error
)
exit /b 0

:upgrade_pip
echo Upgrading pip...
"%VENV_DIR%\Scripts\python.exe" -m pip install --upgrade pip

if %errorlevel% neq 0 (
    echo Pip upgrade failed with error code %errorlevel%.
    goto :error
)
exit /b 0

:install_pytorch
echo Installing PyTorch...
"%VENV_DIR%\Scripts\python.exe" -m pip install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121

if %errorlevel% neq 0 (
    echo PyTorch installation failed with error code %errorlevel%.
    goto :error
)
exit /b 0

:check_ml_agents
echo Checking if ml-agents directory exists...
if not exist "%ML_AGENTS_DIR%" (
    echo ml-agents directory does not exist. Please clone the ml-agents repository into the project root.
    goto :error
)
exit /b 0

:install_ml_agents
echo Installing ml-agents packages...
cd "%ML_AGENTS_DIR%"
"%VENV_DIR%\Scripts\python.exe" -m pip install "%ML_AGENTS_ENVS_INSTALL%"
if %errorlevel% neq 0 (
    echo ml-agents-envs installation failed with error code %errorlevel%.
    goto :error
)
"%VENV_DIR%\Scripts\python.exe" -m pip install "%ML_AGENTS_INSTALL%"
if %errorlevel% neq 0 (
    echo ml-agents installation failed with error code %errorlevel%.
    goto :error
)
exit /b 0

:clean_temp
echo Cleaning up temporary files...
rd /s /q "%TEMP_DIR%"

if %errorlevel% neq 0 (
    echo Cleanup failed with error code %errorlevel%.
    goto :error
)
exit /b 0

:error
echo An error occurred. Exiting setup.
exit /b 1

:exit
exit /b 0

:eof
