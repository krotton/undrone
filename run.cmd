@echo off
echo Building Undrone...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed!
    exit /b %errorlevel%
)
echo Launching game...
godot --path "%~dp0."
