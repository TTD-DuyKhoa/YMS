@echo off
REG QUERY "HKEY_LOCAL_MACHINE\SOFTWARE\YMS2023_Info" >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    REG DELETE "HKEY_LOCAL_MACHINE\SOFTWARE\YMS2023_Info" /f
    echo ���W�X�g���L�[���폜���܂����B
) ELSE (
    echo ���W�X�g���L�[��������܂���ł����B
)
pause