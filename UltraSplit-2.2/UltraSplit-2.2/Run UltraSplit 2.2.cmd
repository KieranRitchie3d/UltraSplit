@echo off
cd /d "%~dp0"
if not exist "UltraSplit2.exe" (
  call "Build UltraSplit 2.2.cmd"
  exit /b
)
start "" "UltraSplit2.exe"
