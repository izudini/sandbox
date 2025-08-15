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
    int serverSocket;
    int port;
    std::vector<int> clientSockets;
    mutable std::mutex clientsMutex;
    std::thread serverThread;
    std::thread statusThread;
    std::atomic<bool> isRunning;
    std::vector<Sensor> sensors;
    
    void acceptClients();
    void sendStatusMessages();
    void removeDisconnectedClient(int clientSocket);
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