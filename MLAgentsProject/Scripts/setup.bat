@echo off
call "%~dp0setenv.bat"

set "START_TIME=%time%"

if "%1"=="--clean" (
    call "%~dp0clean.bat"
    exit /b 0
)

if not exist "%TEMP_DIR%" (
    mkdir "%TEMP_DIR%"
)

set "FORCE_INSTALL=0"
set "SKIP_INSTALL=0"
if "%1"=="--force" (
    set "FORCE_INSTALL=1"
)

echo Starting setup process...
if "%FORCE_INSTALL%"=="0" (
    call :check_python
    if %errorlevel% neq 0 goto :exit
)

call :install_python
if %errorlevel% neq 0 goto :exit

if "%FORCE_INSTALL%"=="1" (
    if exist "%VENV_DIR%" (
        echo Found existing virtual environment.
        echo Removing existing virtual environment...
        rd /s /q "%VENV_DIR%"
        if %errorlevel% neq 0 goto :exit
    )
    call :create_venv
    if %errorlevel% neq 0 goto :exit
) else (
    call :check_venv
    if %errorlevel% neq 0 (
        call :create_venv
        if %errorlevel% neq 0 goto :exit
    ) else (
        set "SKIP_INSTALL=1"
    )
)

call "%ACTIVATE_SCRIPT%"
if %errorlevel% neq 0 goto :exit

if "%SKIP_INSTALL%"=="0" (
    call :upgrade_pip
    if %errorlevel% neq 0 goto :exit

    call :install_pytorch
    if %errorlevel% neq 0 goto :exit
) else (
    echo Skipping pip upgrade and PyTorch installation as everything is already installed.
)

call :check_ml_agents
if %errorlevel% neq 0 goto :exit

if "%FORCE_INSTALL%"=="1" (
    call :install_ml_agents
    if %errorlevel% neq 0 goto :exit
) else (
    echo Skipping ml-agents installation as everything is already installed.
)

call :clean_temp
if %errorlevel% neq 0 goto :exit

call "%DEACTIVATE_SCRIPT%"
if %errorlevel% neq 0 goto :exit

set "END_TIME=%time%"
call :calculate_duration "%START_TIME%" "%END_TIME%"

echo Installation complete!
goto :eof

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
if "%FORCE_INSTALL%"=="0" if "%PYTHON_VERSION%"=="3.10.11" (
    exit /b 0
)
echo Downloading Python installer...
curl -o "%TEMP_DIR%\%PYTHON_INSTALLER%" https://www.python.org/ftp/python/3.10.11/%PYTHON_INSTALLER%

if %errorlevel% neq 0 (
    echo Python download failed.
    goto :exit
)

echo Installing Python...
if exist "%TEMP_DIR%\%PYTHON_INSTALLER%" (
    start /wait "" "%TEMP_DIR%\%PYTHON_INSTALLER%" /quiet InstallAllUsers=1 PrependPath=1 TargetDir="%PYTHON_DIR%"
    if %errorlevel% neq 0 (
        echo Python installation failed.
        goto :exit
    )
) else (
    echo Error: missing %PYTHON_INSTALLER%
    goto :exit
)

if not exist "%PYTHON_DIR%\python.exe" (
    echo Python installation failed. Python executable not found.
    goto :exit
)
exit /b 0

:check_venv
echo Checking if virtual environment exists...
if exist "%VENV_DIR%" (
    echo Virtual environment already exists.
    exit /b 0
) else (
    echo Virtual environment does not exist.
    call :create_venv
    if %errorlevel% neq 0 goto :exit
    call :upgrade_pip
    if %errorlevel% neq 0 goto :exit
    exit /b 0
)

:create_venv
echo Creating virtual environment...
echo Using Python executable: "%PYTHON_DIR%\python.exe"
echo Virtual environment path: "%VENV_DIR%"
"%PYTHON_DIR%\python.exe" -m venv "%VENV_DIR%"

if %errorlevel% neq 0 (
    echo Virtual environment creation failed.
    goto :exit
)

if not exist "%VENV_DIR%\Scripts\activate" (
    echo Virtual environment activation script not found.
    goto :exit
)
exit /b 0

:upgrade_pip
echo Upgrading pip...
python -m pip install --upgrade pip

if %errorlevel% neq 0 (
    echo Pip upgrade failed.
    goto :exit
)
exit /b 0

:install_pytorch
echo Installing PyTorch...
pip install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121

if %errorlevel% neq 0 (
    echo PyTorch installation failed.
    goto :exit
)
exit /b 0

:check_ml_agents
echo Checking if ml-agents directory exists...
if not exist "%ML_AGENTS_DIR%" (
    echo ml-agents directory does not exist. Please clone the ml-agents repository into the project root.
    goto :exit
)
exit /b 0

:install_ml_agents
echo Installing ml-agents packages...
cd "%ML_AGENTS_DIR%"
python -m pip install "%ML_AGENTS_ENVS_INSTALL%"
if %errorlevel% neq 0 (
    echo ml-agents-envs installation failed.
    goto :exit
)
python -m pip install "%ML_AGENTS_INSTALL%"
if %errorlevel% neq 0 (
    echo ml-agents installation failed.
    goto :exit
)
exit /b 0

:clean_temp
echo Cleaning up temporary files...
rd /s /q "%TEMP_DIR%"

if %errorlevel% neq 0 (
    echo Cleanup failed.
    goto :exit
)
exit /b 0

:calculate_duration
setlocal
set "START=%~1"
set "END=%~2"

set /A "START_HOUR=%START:~0,2%"
set /A "START_MIN=%START:~3,2%"
set /A "START_SEC=%START:~6,2%"
set /A "START_MS=%START:~9,2%"

set /A "END_HOUR=%END:~0,2%"
set /A "END_MIN=%END:~3,2%"
set /A "END_SEC=%END:~6,2%"
set /A "END_MS=%END:~9,2%"

set /A "START_TOTAL_SEC=(%START_HOUR%*3600 + %START_MIN%*60 + %START_SEC%)"
set /A "END_TOTAL_SEC=(%END_HOUR%*3600 + %END_MIN%*60 + %END_SEC%)"

set /A "DURATION_SEC=%END_TOTAL_SEC% - %START_TOTAL_SEC%"

if %DURATION_SEC% lss 0 (
    set /A "DURATION_SEC=%DURATION_SEC% + 86400"
)

echo Setup completed in %DURATION_SEC% seconds.
endlocal
exit /b 0

:exit
echo An error occurred. Exiting setup.
exit /b 1

:eof
