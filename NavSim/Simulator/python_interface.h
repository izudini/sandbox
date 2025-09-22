#pragma once

#include "position.pb.h"
#include <string>
#include <iostream>
#include <fstream>
#include <sstream>

namespace navsim {

class PythonInterface {
public:
    PythonInterface();
    ~PythonInterface();
    
    bool initialize();
    void callNavListener(const Position& position);
    void cleanup();

private:
    bool initialized;
    std::string pythonScriptPath;
    
    bool setupPythonScript();
    void executePythonScript(const Position& position);
};

} // namespace navsim