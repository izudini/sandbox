@echo off
echo Testing Simple NavSim Build
echo ===========================

echo.
echo Step 1: Checking Visual Studio installation...
where cl.exe >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Visual Studio compiler not found in PATH
    echo Please run this from "Developer Command Prompt for VS"
    pause
    exit /b 1
)
echo Visual Studio compiler found.

echo.
echo Step 2: Building NavSim.dll...
cd NavSim
cl /LD /DNAVSIM_EXPORTS SimpleNavEntity.cpp /Fe:NavSim.dll
if %errorlevel% neq 0 (
    echo ERROR: Failed to build NavSim.dll
    pause
    exit /b 1
)
echo NavSim.dll built successfully.

echo.
echo Step 3: Building NavSimExample.exe...
cd ..\NavSimExample
cl simple_main.cpp /I..\NavSim ..\NavSim\NavSim.lib /Fe:NavSimExample.exe
if %errorlevel% neq 0 (
    echo ERROR: Failed to build NavSimExample.exe
    pause
    exit /b 1
)
echo NavSimExample.exe built successfully.

echo.
echo Step 4: Running example...
NavSimExample.exe
if %errorlevel% neq 0 (
    echo ERROR: Failed to run example
    pause
    exit /b 1
)

echo.
echo Build and test completed successfully!
pause