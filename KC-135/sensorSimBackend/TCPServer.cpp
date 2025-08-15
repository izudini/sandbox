#include "TCPServer.h"
#include <iostream>
#include <sstream>
#include <algorithm>
#include <cstring>

#ifdef _WIN32
    #pragma comment(lib, "ws2_32.lib")
#endif

TCPServer::TCPServer(int port) : port(port), serverSocket(-1), isRunning(false) {
#ifdef _WIN32
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        std::cerr << "WSAStartup failed" << std::endl;
    }
#endif
}

TCPServer::~TCPServer() {
    stopServer();
    
#ifdef _WIN32
    WSACleanup();
#endif
}

bool TCPServer::startServer() {
    // Create socket
    serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket < 0) {
        std::cerr << "Failed to create socket" << std::endl;
        return false;
    }

    // Set socket options to reuse address
    int opt = 1;
#ifdef _WIN32
    if (setsockopt(serverSocket, SOL_SOCKET, SO_REUSEADDR, (char*)&opt, sizeof(opt)) < 0) {
#else
    if (setsockopt(serverSocket, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt)) < 0) {
#endif
        std::cerr << "Failed to set socket options" << std::endl;
#ifdef _WIN32
        closesocket(serverSocket);
#else
        close(serverSocket);
#endif
        return false;
    }

    // Bind socket
    struct sockaddr_in serverAddr;
    memset(&serverAddr, 0, sizeof(serverAddr));
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) < 0) {
        std::cerr << "Failed to bind socket to port " << port << std::endl;
#ifdef _WIN32
        closesocket(serverSocket);
#else
        close(serverSocket);
#endif
        return false;
    }

    // Listen for connections
    if (listen(serverSocket, 5) < 0) {
        std::cerr << "Failed to listen on socket" << std::endl;
#ifdef _WIN32
        closesocket(serverSocket);
#else
        close(serverSocket);
#endif
        return false;
    }

    isRunning = true;
    
    // Start server thread for accepting clients
    serverThread = std::thread(&TCPServer::acceptClients, this);
    
    // Start status broadcasting thread
    statusThread = std::thread(&TCPServer::sendStatusMessages, this);
    
    std::cout << "TCP Server started on port " << port << std::endl;
    return true;
}

void TCPServer::stopServer() {
    if (!isRunning) return;
    
    isRunning = false;

    // Close server socket
    if (serverSocket >= 0) {
#ifdef _WIN32
        closesocket(serverSocket);
#else
        close(serverSocket);
#endif
        serverSocket = -1;
    }

    // Close all client sockets
    {
        std::lock_guard<std::mutex> lock(clientsMutex);
        for (int clientSocket : clientSockets) {
#ifdef _WIN32
            closesocket(clientSocket);
#else
            close(clientSocket);
#endif
        }
        clientSockets.clear();
    }

    // Wait for threads to finish
    if (serverThread.joinable()) {
        serverThread.join();
    }
    if (statusThread.joinable()) {
        statusThread.join();
    }

    std::cout << "TCP Server stopped" << std::endl;
}

void TCPServer::acceptClients() {
    while (isRunning) {
        struct sockaddr_in clientAddr;
        socklen_t clientAddrLen = sizeof(clientAddr);
        
        int clientSocket = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrLen);
        if (clientSocket < 0) {
            if (isRunning) {
                std::cerr << "Failed to accept client connection" << std::endl;
            }
            continue;
        }

        {
            std::lock_guard<std::mutex> lock(clientsMutex);
            clientSockets.push_back(clientSocket);
        }

        char clientIP[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &(clientAddr.sin_addr), clientIP, INET_ADDRSTRLEN);
        std::cout << "Client connected from " << clientIP << ":" << ntohs(clientAddr.sin_port) << std::endl;
    }
}

void TCPServer::sendStatusMessages() {
    while (isRunning) {
        std::string statusMessage = generateStatusMessage();
        
        {
            std::lock_guard<std::mutex> lock(clientsMutex);
            std::vector<int> disconnectedClients;
            
            for (int clientSocket : clientSockets) {
                int result = send(clientSocket, statusMessage.c_str(), statusMessage.length(), 0);
                if (result < 0) {
                    std::cout << "Client disconnected" << std::endl;
                    disconnectedClients.push_back(clientSocket);
                }
            }
            
            // Remove disconnected clients
            for (int disconnectedSocket : disconnectedClients) {
                removeDisconnectedClient(disconnectedSocket);
            }
        }
        
        // Sleep for 1 second (1Hz frequency)
        std::this_thread::sleep_for(std::chrono::seconds(1));
    }
}

void TCPServer::removeDisconnectedClient(int clientSocket) {
    auto it = std::find(clientSockets.begin(), clientSockets.end(), clientSocket);
    if (it != clientSockets.end()) {
        clientSockets.erase(it);
#ifdef _WIN32
        closesocket(clientSocket);
#else
        close(clientSocket);
#endif
    }
}

std::string TCPServer::generateStatusMessage() const {
    std::ostringstream oss;
    auto now = std::chrono::system_clock::now();
    auto time_t = std::chrono::system_clock::to_time_t(now);
    
    oss << "{";
    oss << "\"timestamp\":\"" << std::ctime(&time_t);
    // Remove newline from ctime
    std::string timestamp = oss.str();
    if (!timestamp.empty() && timestamp.back() == '\n') {
        timestamp.pop_back();
    }
    oss.str("");
    oss << "{";
    oss << "\"timestamp\":\"" << timestamp.substr(timestamp.find(':') + 1) << "\",";
    oss << "\"server_status\":\"running\",";
    oss << "\"connected_clients\":" << getClientCount() << ",";
    oss << "\"sensors\":[";
    
    for (size_t i = 0; i < sensors.size(); ++i) {
        if (i > 0) oss << ",";
        oss << "{";
        oss << "\"name\":\"" << sensors[i].getName() << "\",";
        oss << "\"location\":\"" << sensors[i].getLocation() << "\",";
        oss << "\"state\":\"";
        
        switch (sensors[i].getCurrentState()) {
            case SensorState::Off: oss << "Off"; break;
            case SensorState::Initializing: oss << "Initializing"; break;
            case SensorState::Operational: oss << "Operational"; break;
            case SensorState::Declaring: oss << "Declaring"; break;
            case SensorState::Degraded: oss << "Degraded"; break;
        }
        
        oss << "\"";
        oss << "}";
    }
    
    oss << "]";
    oss << "}\n";
    
    return oss.str();
}

void TCPServer::addSensor(const Sensor& sensor) {
    sensors.push_back(sensor);
}

bool TCPServer::hasClients() const {
    std::lock_guard<std::mutex> lock(clientsMutex);
    return !clientSockets.empty();
}

size_t TCPServer::getClientCount() const {
    std::lock_guard<std::mutex> lock(clientsMutex);
    return clientSockets.size();
}