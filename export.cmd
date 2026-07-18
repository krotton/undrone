@echo off
echo Building .NET project...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b %errorlevel%
)
echo Creating build directory...
if not exist build mkdir build
echo Exporting Undrone standalone...
godot --headless --path "%~dp0." --export-release "Windows Desktop" "%~dp0build/Undrone.exe"
