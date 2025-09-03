@echo off
REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\YMS2023_Info" >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    REG DELETE "HKEY_LOCAL_MACHINE\SOFTWARE\YMS2023_Info" /f
    echo レジストリキーを削除しました。
) ELSE (
    echo レジストリキーが見つかりませんでした。
)
pause