#include "PythonIntegration.h"
#include <iostream>
#include <cstdlib>
#include <sstream>
#include <iomanip>
#include <Windows.h>

PythonVisualizer::PythonVisualizer() : m_isInitialized(false), m_dataFilePath("flight_data.csv"), m_visualizerStarted(false)
{
}

PythonVisualizer::~PythonVisualizer()
{
    if (m_isInitialized)
    {
        Shutdown();
    }
}

bool PythonVisualizer::Initialize()
{
    try 
    {
        // Clear any existing data file
        std::ofstream clearFile(m_dataFilePath, std::ofstream::trunc);
        if (clearFile.is_open())
        {
            clearFile << "entity_id,latitude,longitude,altitude,heading\n"; // CSV header
            clearFile.close();
        }
        
        m_isInitialized = true;
        std::cout << "Python visualizer data system initialized" << std::endl;
        return true;
    }
    catch (const std::exception& e)
    {
        std::cerr << "Failed to initialize Python visualizer: " << e.what() << std::endl;
        return false;
    }
}

void PythonVisualizer::StartVisualizerProcess()
{
    if (!m_visualizerStarted)
    {
        std::cout << "Starting Python map visualizer..." << std::endl;
        
        // Try to start the real-time Python visualizer in the background
        std::string command = "start \"NavSim Real-time Visualizer\" cmd /k \"cd /d " + std::string("..\\NavVisualiser") + " && python realtime_visualizer.py\"";
        int result = std::system(command.c_str());
        
        if (result == 0)
        {
            std::cout << "Python visualizer window should open shortly" << std::endl;
            m_visualizerStarted = true;
            
            // Give the visualizer time to start
            Sleep(2000);
        }
        else
        {
            std::cout << "Warning: Could not start Python visualizer. Make sure Python is installed and in PATH." << std::endl;
        }
    }
}

void PythonVisualizer::Shutdown()
{
    if (m_isInitialized)
    {
        WritePositionData();
        m_isInitialized = false;
        std::cout << "Python visualizer data system shutdown" << std::endl;
    }
}

void PythonVisualizer::UpdatePosition(const Position& position)
{
    if (!m_isInitialized)
    {
        return;
    }
    
    try
    {
        // Store position in history
        m_positionHistory.push_back(position);
        
        // Write latest position to CSV file immediately for real-time updates
        std::ofstream file(m_dataFilePath, std::ios::app);
        if (file.is_open())
        {
            file << position.GetEntityID() << "," 
                 << std::fixed << std::setprecision(6)
                 << position.GetLatitude() << "," 
                 << position.GetLongitude() << "," 
                 << position.GetAltitude() << "," 
                 << position.GetHeading() << std::endl;
            file.close();
        }
        
        std::cout << "Position updated: Entity " << position.GetEntityID() 
                  << " at (" << position.GetLatitude() << ", " << position.GetLongitude() << ")" << std::endl;
    }
    catch (const std::exception& e)
    {
        std::cerr << "Error updating position: " << e.what() << std::endl;
    }
}

void PythonVisualizer::Visualize()
{
    if (!m_isInitialized)
    {
        return;
    }
    
    // Start the visualizer if not already started
    if (!m_visualizerStarted)
    {
        StartVisualizerProcess();
    }
}

void PythonVisualizer::WritePositionData()
{
    if (m_positionHistory.empty())
    {
        return;
    }
    
    try
    {
        std::ofstream file(m_dataFilePath, std::ofstream::trunc);
        if (file.is_open())
        {
            file << "entity_id,latitude,longitude,altitude,heading\n";
            
            for (const auto& pos : m_positionHistory)
            {
                file << pos.GetEntityID() << "," 
                     << std::fixed << std::setprecision(6)
                     << pos.GetLatitude() << "," 
                     << pos.GetLongitude() << "," 
                     << pos.GetAltitude() << "," 
                     << pos.GetHeading() << std::endl;
            }
            file.close();
            std::cout << "Written " << m_positionHistory.size() << " positions to " << m_dataFilePath << std::endl;
        }
    }
    catch (const std::exception& e)
    {
        std::cerr << "Error writing position data: " << e.what() << std::endl;
    }
}