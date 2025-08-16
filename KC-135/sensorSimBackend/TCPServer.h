#ifndef TCP_SERVER_H
#define TCP_SERVER_H

#include <string>
#include <vector>
#include <thread>
#include <mutex>
#include <atomic>
#include <chrono>
#include "Sensor.h"

#ifdef _WIN32
    #define WIN32_LEAN_AND_MEAN
    #include <windows.h>
    #include <winsock2.h>
    #include <ws2tcpip.h>
#else
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
#endif

class TCPServer {
private:
#ifdef _WIN32
    SOCKET serverSocket;
#else
    int serverSocket;
#endif
    int port;
#ifdef _WIN32
    std::vector<SOCKET> clientSockets;
#else
    std::vector<int> clientSockets;
#endif
    mutable std::mutex clientsMutex;
    std::thread serverThread;
    std::thread statusThread;
    std::atomic<bool> isRunning;
    std::vector<Sensor> sensors;
    
    void acceptClients();
    void sendStatusMessages();
#ifdef _WIN32
    void removeDisconnectedClient(SOCKET clientSocket);
#else
    void removeDisconnectedClient(int clientSocket);
#endif
    std::string generateStatusMessage() const;

public:
    TCPServer(int port);
    ~TCPServer();
    
    bool startServer();
    void stopServer();
    void addSensor(const Sensor& sensor);
    bool hasClients() const;
    size_t getClientCount() const;
};

#endif // TCP_SERVER_H