#ifndef UDP_SOCKET_LISTENER_H
#define UDP_SOCKET_LISTENER_H

#include <string>
#include <queue>
#include <thread>
#include <mutex>
#include <atomic>

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

class UDPSocketListener {
private:
#ifdef _WIN32
    SOCKET socketFd;
#else
    int socketFd;
#endif
    int port;
    std::queue<std::string> messageQueue;
    mutable std::mutex queueMutex;
    std::thread listenerThread;
    std::atomic<bool> isListening;
    
    void listenForMessages();

public:
    UDPSocketListener(int port);
    ~UDPSocketListener();
    
    bool openSocket();
    void closeSocket();
    bool hasMessages() const;
    std::string getNextMessage();
    size_t getQueueSize() const;
};

#endif // UDP_SOCKET_LISTENER_H