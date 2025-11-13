@echo off
REM Generate C# classes from protobuf definitions

echo Generating C# classes from proto files...

REM Get the directory where this script is located
set SCRIPT_DIR=%~dp0

REM Change to the proto directory
cd /d "%SCRIPT_DIR%"

REM Generate C# classes - output to ../cSharp directory
protoc --csharp_out=../cSharp *.proto

if %ERRORLEVEL% EQU 0 (
    echo C# classes generated successfully in ../cSharp directory
) else (
    echo Error generating C# classes. Make sure protoc is installed and in your PATH.
    exit /b 1
)
