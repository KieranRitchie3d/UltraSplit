@echo off
setlocal
cd /d "%~dp0"
title Build UltraSplit 2.2

set "CSC64=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set "CSC32=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if exist "%CSC64%" (
  set "CSC=%CSC64%"
) else if exist "%CSC32%" (
  set "CSC=%CSC32%"
) else (
  echo ERROR: .NET Framework C# compiler not found.
  echo Enable or install .NET Framework 4.8.
  pause
  exit /b 1
)

if not exist "%~dp0UltraSplit2.cs" (
  echo ERROR: UltraSplit2.cs is missing.
  echo Extract the entire ZIP before building.
  pause
  exit /b 1
)

if not exist "%~dp0UltraSplit.ico" (
  echo ERROR: UltraSplit.ico is missing.
  echo Extract the entire ZIP before building.
  pause
  exit /b 1
)

echo Building UltraSplit2.exe...

"%CSC%" /nologo /target:winexe /optimize+ /platform:anycpu ^
 /win32manifest:"%~dp0app.manifest" ^
 /win32icon:"%~dp0UltraSplit.ico" ^
 /out:"%~dp0UltraSplit2.exe" ^
 /reference:System.dll ^
 /reference:System.Core.dll ^
 /reference:System.Drawing.dll ^
 /reference:System.Windows.Forms.dll ^
 /reference:System.Web.Extensions.dll ^
 "%~dp0UltraSplit2.cs"

if errorlevel 1 (
  echo.
  echo BUILD FAILED.
  pause
  exit /b 1
)

echo.
echo BUILD COMPLETE:
echo %~dp0UltraSplit2.exe
echo.
start "" "%~dp0UltraSplit2.exe"
