#include "python_interface.h"
#include <cstdlib>
#include <cstdio>
#include <unistd.h>
#include <sys/stat.h>

namespace navsim {

PythonInterface::PythonInterface() 
    : initialized(false), pythonScriptPath("nav_listener_bridge.py") {
}

PythonInterface::~PythonInterface() {
    cleanup();
}

bool PythonInterface::initialize() {
    if (initialized) {
        return true;
    }
    
    if (!setupPythonScript()) {
        std::cerr << "Failed to setup Python script" << std::endl;
        return false;
    }
    
    initialized = true;
    return true;
}

bool PythonInterface::setupPythonScript() {
    // Create a bridge Python script that will handle the NavListener calls
    std::ofstream scriptFile(pythonScriptPath);
    if (!scriptFile.is_open()) {
        std::cerr << "Failed to create Python bridge script" << std::endl;
        return false;
    }
    
    scriptFile << "#!/usr/bin/env python3\n";
    scriptFile << "import sys\n";
    scriptFile << "import os\n";
    scriptFile << "\n";
    scriptFile << "# Add algorithm src to path\n";
    scriptFile << "algorithm_path = os.path.join(os.path.dirname(__file__), '../algorithm/src')\n";
    scriptFile << "sys.path.insert(0, os.path.abspath(algorithm_path))\n";
    scriptFile << "\n";
    scriptFile << "try:\n";
    scriptFile << "    from nav_listener import NavListener\n";
    scriptFile << "    from position import Position\n";
    scriptFile << "except ImportError as e:\n";
    scriptFile << "    print(f'Import error: {e}', file=sys.stderr)\n";
    scriptFile << "    sys.exit(1)\n";
    scriptFile << "\n";
    scriptFile << "def main():\n";
    scriptFile << "    if len(sys.argv) != 6:\n";
    scriptFile << "        print('Usage: script.py entity_id latitude longitude altitude heading', file=sys.stderr)\n";
    scriptFile << "        sys.exit(1)\n";
    scriptFile << "    \n";
    scriptFile << "    try:\n";
    scriptFile << "        entity_id = int(sys.argv[1])\n";
    scriptFile << "        latitude = float(sys.argv[2])\n";
    scriptFile << "        longitude = float(sys.argv[3])\n";
    scriptFile << "        altitude = float(sys.argv[4])\n";
    scriptFile << "        heading = float(sys.argv[5])\n";
    scriptFile << "        \n";
    scriptFile << "        position = Position(entity_id, latitude, longitude, altitude, heading)\n";
    scriptFile << "        listener = NavListener()\n";
    scriptFile << "        listener.on_position_update(position)\n";
    scriptFile << "        \n";
    scriptFile << "    except Exception as e:\n";
    scriptFile << "        print(f'Error: {e}', file=sys.stderr)\n";
    scriptFile << "        sys.exit(1)\n";
    scriptFile << "\n";
    scriptFile << "if __name__ == '__main__':\n";
    scriptFile << "    main()\n";
    
    scriptFile.close();
    
    // Make the script executable
    chmod(pythonScriptPath.c_str(), 0755);
    
    return true;
}

void PythonInterface::callNavListener(const Position& position) {
    if (!initialized) {
        std::cerr << "Python interface not initialized" << std::endl;
        return;
    }
    
    executePythonScript(position);
}

void PythonInterface::executePythonScript(const Position& position) {
    // Build command to execute Python script
    std::ostringstream command;
    command << "python3 " << pythonScriptPath
            << " " << position.entity_id()
            << " " << std::fixed << position.latitude()
            << " " << std::fixed << position.longitude()
            << " " << std::fixed << position.altitude()
            << " " << std::fixed << position.heading()
            << " 2>/dev/null"; // Suppress stderr to avoid cluttering output
    
    // Execute the command
    int result = std::system(command.str().c_str());
    if (result != 0) {
        // Only print error if it's not just a missing module (which is expected initially)
        std::cerr << "Python script execution failed (code: " << result << ")" << std::endl;
    }
}

void PythonInterface::cleanup() {
    if (initialized) {
        // Remove the bridge script
        if (access(pythonScriptPath.c_str(), F_OK) == 0) {
            remove(pythonScriptPath.c_str());
        }
        initialized = false;
    }
}

} // namespace navsim