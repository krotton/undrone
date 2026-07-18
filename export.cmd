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
godot --headless --export-release "Windows Desktop" build/Undrone.exe
