#pragma once

#include "Position.h"
#include "PythonIntegration.h"
#include <thread>
#include <atomic>
#include <memory>

#ifdef NAVSIM_EXPORTS
#define NAVSIM_API __declspec(dllexport)
#else
#define NAVSIM_API __declspec(dllimport)
#endif

class NAVSIM_API NavEntity
{
public:
    NavEntity();
    ~NavEntity();

    void Initialize();
    void Update();
    void Shutdown();
    void simulateFlight(const Position& start, const Position& destination, int speed_mph);
    void stopFlight();
    void Start();
    void StartWithVisualizer();

private:
    void simulatingFlight();
    void simulatingFlightWithVisualizer();
    bool m_isInitialized;
    Position currentPosition;
    Position m_destination;
    int m_speed_mph;
    int simulationRate_Hz;
    std::thread m_simulationThread;
    std::atomic<bool> m_isSimulating;
    std::unique_ptr<PythonVisualizer> m_visualizer;
    bool m_useVisualizer;
};