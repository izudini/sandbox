#include <iostream>
#include <chrono>
#include <thread>
#include <random>
#include "SensorState.h"
#include "Sensor.h"
#include "UDPSocketListener.h"
#include "TCPServer.h"

int main() {
    std::cout << "SensorController started" << std::endl;
    
    // Create TCP server on port 8080
    TCPServer tcpServer(8080);
    
    // Create some example sensors
    Sensor sensorA("A", "Wing_Left");
    Sensor sensorB("B", "Wing_Right");
    Sensor sensorC("C", "Nose");
    Sensor sensorD("Sensor 4", "Tail");
    
    // Add sensors to the server
    tcpServer.addSensor(sensorA);
    tcpServer.addSensor(sensorB);
    tcpServer.addSensor(sensorC);
    tcpServer.addSensor(sensorD);
    
    // Start the TCP server
    if (!tcpServer.startServer()) {
        std::cerr << "Failed to start TCP server" << std::endl;
        return 1;
    }
    
    // Simulate sensor state changes
    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_int_distribution<> stateDist(0, 4);
    
    std::cout << "TCP Server running on port 8080" << std::endl;
    std::cout << "Broadcasting sensor status at 1Hz..." << std::endl;
    std::cout << "Press Ctrl+C to stop" << std::endl;
    
    // Keep the main thread alive and occasionally change sensor states
    int counter = 0;
    while (true) {
        std::this_thread::sleep_for(std::chrono::seconds(5));
        
        // Occasionally change a random sensor state for demonstration
        if (counter % 4 == 0) {
            SensorState newState = static_cast<SensorState>(stateDist(gen));
            // This would need sensor state modification methods in a real implementation
            std::cout << "Simulating sensor state changes..." << std::endl;
        }
        
        counter++;
    }
    
    return 0;
}