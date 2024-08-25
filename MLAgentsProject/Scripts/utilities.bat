@echo off

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
