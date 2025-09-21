#pragma once

#include "Position.h"
#include <string>
#include <fstream>
#include <vector>

#ifdef NAVSIM_EXPORTS
#define NAVSIM_API __declspec(dllexport)
#else
#define NAVSIM_API __declspec(dllimport)
#endif

class NAVSIM_API PythonVisualizer
{
public:
    PythonVisualizer();
    ~PythonVisualizer();
    
    bool Initialize();
    void Shutdown();
    void UpdatePosition(const Position& position);
    void Visualize();
    void StartVisualizerProcess();
    
private:
    bool m_isInitialized;
    std::string m_dataFilePath;
    std::vector<Position> m_positionHistory;
    bool m_visualizerStarted;
    
    void WritePositionData();
};