@echo off
echo Setting up Python dependencies for NavSim
echo ==========================================

echo.
echo Installing required Python packages...
pip install -r requirements.txt

if %errorlevel% neq 0 (
    echo ERROR: Failed to install Python dependencies
    echo Please make sure Python and pip are installed and in PATH
    pause
    exit /b 1
)

echo.
echo Python setup completed successfully!
echo You can now run the NavSim example with full map visualization.
echo.
pause