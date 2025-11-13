#include "Resource.h"

Resource::Resource(const std::string& name, int value)
    : name_(name), value_(value) {
    std::cout << "  [Constructor] Resource '" << name_ << "' created\n";
}

Resource::~Resource() {
    std::cout << "  [Destructor] Resource '" << name_ << "' destroyed\n";
}

std::string Resource::getName() const {
    return name_;
}

int Resource::getValue() const {
    return value_;
}

void Resource::setValue(int value) {
    value_ = value;
}

void Resource::display() const {
    std::cout << "  Resource: " << name_ << ", Value: " << value_ << "\n";
}
