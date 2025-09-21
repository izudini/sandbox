#pragma once

#ifdef NAVSIM_EXPORTS
#define NAVSIM_API __declspec(dllexport)
#else
#define NAVSIM_API __declspec(dllimport)
#endif

class NAVSIM_API SimpleNavEntity
{
public:
    SimpleNavEntity();
    ~SimpleNavEntity();
    
    void Initialize();
    void Shutdown();
    void StartDemo();
    
private:
    bool m_isInitialized;
};