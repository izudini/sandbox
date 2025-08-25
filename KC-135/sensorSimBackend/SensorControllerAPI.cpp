#define SENSOR_CONTROLLER_EXPORTS
#include "SensorControllerAPI.h"
#include "TCPServer.h"
#include "Sensor.h"
#include <thread>
#include <memory>
#include <atomic>
#include <chrono>
#include <random>

static std::unique_ptr<TCPServer> tcpServer = nullptr;
static std::thread serverThread;
static std::atomic<bool> running = false;

void ServerThreadFunction() {
    if (!tcpServer) return;
    
    // Create some example sensors
    Sensor sensorA("A", "Wing_Left");
    Sensor sensorB("B", "Wing_Right");
    Sensor sensorC("C", "Nose");
    Sensor sensorD("Sensor 4", "Tail");
    
    // Add sensors to the server
    tcpServer->addSensor(sensorA);
    tcpServer->addSensor(sensorB);
    tcpServer->addSensor(sensorC);
    tcpServer->addSensor(sensorD);
    
    // Start the TCP server
    if (!tcpServer->startServer()) {
        running = false;
        return;
    }
    
    // Simulate sensor state changes
    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_int_distribution<> stateDist(0, 4);
    
    int counter = 0;
    while (running) {
        std::this_thread::sleep_for(std::chrono::seconds(1));
        
        // Occasionally change a random sensor state for demonstration
        if (counter % 5 == 0) {
            // SensorState newState = static_cast<SensorState>(stateDist(gen));
            // Simulate sensor state changes...
        }
        
        counter++;
    }
}

extern "C" {
    SENSOR_API bool StartSensorController() {
        if (running) {
            return false; // Already running
        }
        
        tcpServer = std::make_unique<TCPServer>(8080);
        running = true;
        
        serverThread = std::thread(ServerThreadFunction);
        
        return true;
    }
    
    SENSOR_API bool StopSensorController() {
        if (!running) {
            return false; // Not running
        }
        
        running = false;
        
        if (serverThread.joinable()) {
            serverThread.join();
        }
        
        tcpServer.reset();
        
        return true;
    }
    
    SENSOR_API bool IsRunning() {
        return running;
    }
}