#ifndef RESOURCE_H
#define RESOURCE_H

#include <string>
#include <iostream>

/**
 * Sample Resource class to demonstrate shared_ptr behavior
 * This class tracks construction and destruction to show when objects are cleaned up
 */
class Resource {
private:
    std::string name_;
    int value_;

public:
    // Constructor
    Resource(const std::string& name, int value);

    // Destructor - will show when the object is actually destroyed
    ~Resource();

    // Getters
    std::string getName() const;
    int getValue() const;

    // Setters
    void setValue(int value);

    // Display information
    void display() const;
};

#endif // RESOURCE_H
