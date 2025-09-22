#pragma once

#include "position.pb.h"
#include "python_interface.h"
#include <vector>
#include <memory>
#include <thread>

namespace navsim {

class Simulator {
public:
    Simulator();
    ~Simulator();
    
    void start(const Position& start, const Position& destination, int speed_mph);

private:
    void runSimulation();
    double calculateDistance(const Position& pos1, const Position& pos2) const;
    Position calculateIntermediatePosition(const Position& start, const Position& end, double progress) const;
    
    int simulationFrequency_hz;
    int speedMph;
    Position startPosition;
    Position destinationPosition;
    Position currentPosition;
    std::thread simulationThread;
    bool simulationRunning;
    PythonInterface pythonInterface;
};

} // namespace navsim