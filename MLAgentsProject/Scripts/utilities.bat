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

set /A "START_TOTAL_MS=(%START_HOUR%*3600000 + %START_MIN%*60000 + %START_SEC%*1000 + %START_MS%)"
set /A "END_TOTAL_MS=(%END_HOUR%*3600000 + %END_MIN%*60000 + %END_SEC%*1000 + %END_MS%)"

set /A "DURATION_MS=%END_TOTAL_MS% - %START_TOTAL_MS%"

if %DURATION_MS% lss 0 (
    set /A "DURATION_MS=%DURATION_MS% + 86400000"
)

set /A "DURATION_SEC=%DURATION_MS% / 1000"

endlocal & set DURATION_SEC=%DURATION_SEC%
exit /b 0
