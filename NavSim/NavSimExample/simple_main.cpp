#include "SimpleNavEntity.h"
#include <iostream>

int main()
{
    std::cout << "NavSim Simple Example" << std::endl;
    std::cout << "=====================" << std::endl;
    
    // Create and initialize the navigation entity
    SimpleNavEntity entity;
    entity.Initialize();
    
    // Run the demo
    entity.StartDemo();
    
    // Cleanup
    entity.Shutdown();
    
    std::cout << "Press Enter to exit..." << std::endl;
    std::cin.get();
    
    return 0;
}