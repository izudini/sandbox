#include "UDPSocketListener.h"
#include <iostream>
#include <cstring>

#ifdef _WIN32
    #pragma comment(lib, "ws2_32.lib")
#endif

UDPSocketListener::UDPSocketListener(int port) 
    : socketFd(-1), port(port), isListening(false) {
#ifdef _WIN32
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);
#endif
}

UDPSocketListener::~UDPSocketListener() {
    closeSocket();
#ifdef _WIN32
    WSACleanup();
#endif
}

bool UDPSocketListener::openSocket() {
    socketFd = socket(AF_INET, SOCK_DGRAM, 0);
    if (socketFd < 0) {
        std::cerr << "Failed to create socket" << std::endl;
        return false;
    }

    struct sockaddr_in serverAddr;
    memset(&serverAddr, 0, sizeof(serverAddr));
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(socketFd, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) < 0) {
        std::cerr << "Failed to bind socket to port " << port << std::endl;
#ifdef _WIN32
        closesocket(socketFd);
#else
        close(socketFd);
#endif
        socketFd = -1;
        return false;
    }

    isListening = true;
    listenerThread = std::thread(&UDPSocketListener::listenForMessages, this);
    
    std::cout << "UDP socket listening on port " << port << std::endl;
    return true;
}

void UDPSocketListener::closeSocket() {
    if (isListening) {
        isListening = false;
        
        if (listenerThread.joinable()) {
            listenerThread.join();
        }
    }
    
    if (socketFd >= 0) {
#ifdef _WIN32
        closesocket(socketFd);
#else
        close(socketFd);
#endif
        socketFd = -1;
    }
}

void UDPSocketListener::listenForMessages() {
    char buffer[1024];
    struct sockaddr_in clientAddr;
    socklen_t clientAddrLen = sizeof(clientAddr);

    while (isListening) {
        ssize_t bytesReceived = recvfrom(socketFd, buffer, sizeof(buffer) - 1, 0, 
                                        (struct sockaddr*)&clientAddr, &clientAddrLen);
        
        if (bytesReceived > 0) {
            buffer[bytesReceived] = '\0';
            std::string message(buffer);
            
            std::lock_guard<std::mutex> lock(queueMutex);
            messageQueue.push(message);
        }
    }
}

bool UDPSocketListener::hasMessages() const {
    std::lock_guard<std::mutex> lock(queueMutex);
    return !messageQueue.empty();
}

std::string UDPSocketListener::getNextMessage() {
    std::lock_guard<std::mutex> lock(queueMutex);
    if (messageQueue.empty()) {
        return "";
    }
    
    std::string message = messageQueue.front();
    messageQueue.pop();
    return message;
}

size_t UDPSocketListener::getQueueSize() const {
    std::lock_guard<std::mutex> lock(queueMutex);
    return messageQueue.size();
}