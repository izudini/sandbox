#include <iostream>
#include <memory>
#include <vector>
#include "Resource.h"

// ============================================================================
// EXAMPLE 1: Passing shared_ptr by value
// This increments the reference count during the function call
// ============================================================================
void passByValue(std::shared_ptr<Resource> ptr) {
    std::cout << "  Inside passByValue - Reference count: " << ptr.use_count() << "\n";
    ptr->display();
}

// ============================================================================
// EXAMPLE 2: Passing shared_ptr by const reference
// This is more efficient - doesn't increment reference count
// Use when you don't need to modify the shared_ptr itself
// ============================================================================
void passByConstRef(const std::shared_ptr<Resource>& ptr) {
    std::cout << "  Inside passByConstRef - Reference count: " << ptr.use_count() << "\n";
    ptr->display();
    // Can still modify the object, just not the pointer itself
    // ptr->setValue(999); // This would work
}

// ============================================================================
// EXAMPLE 3: Passing shared_ptr by reference
// Allows modifying the shared_ptr itself (e.g., reset, reassign)
// ============================================================================
void passByRef(std::shared_ptr<Resource>& ptr) {
    std::cout << "  Inside passByRef - Reference count: " << ptr.use_count() << "\n";
    // Can modify the pointer itself
    ptr = std::make_shared<Resource>("Replaced Resource", 777);
}

// ============================================================================
// EXAMPLE 4: Returning a shared_ptr from a function
// ============================================================================
std::shared_ptr<Resource> createResource(const std::string& name, int value) {
    return std::make_shared<Resource>(name, value);
}

// ============================================================================
// EXAMPLE 5: Taking ownership - function stores the shared_ptr
// ============================================================================
class ResourceManager {
private:
    std::vector<std::shared_ptr<Resource>> resources_;

public:
    void addResource(std::shared_ptr<Resource> res) {
        std::cout << "  Adding resource to manager. Ref count: " << res.use_count() << "\n";
        resources_.push_back(res);
    }

    void displayAll() const {
        std::cout << "  ResourceManager contents:\n";
        for (const auto& res : resources_) {
            std::cout << "    ";
            res->display();
        }
    }

    size_t size() const { return resources_.size(); }
};

// ============================================================================
// EXAMPLE 6: Passing raw pointer when ownership is not transferred
// This is the preferred way when function doesn't need ownership
// ============================================================================
void processResource(Resource* res) {
    if (res) {
        std::cout << "  Processing resource via raw pointer:\n";
        std::cout << "    ";
        res->display();
    }
}

// ============================================================================
// Helper function to print separator
// ============================================================================
void printSeparator(const std::string& title) {
    std::cout << "\n" << std::string(70, '=') << "\n";
    std::cout << title << "\n";
    std::cout << std::string(70, '=') << "\n";
}

// ============================================================================
// MAIN FUNCTION - Demonstrates all examples
// ============================================================================
int main() {
    std::cout << "SHARED_PTR DEMONSTRATION\n";
    std::cout << "========================\n\n";

    // ------------------------------------------------------------------------
    // Demo 1: Basic shared_ptr creation and reference counting
    // ------------------------------------------------------------------------
    printSeparator("1. BASIC CREATION AND REFERENCE COUNTING");
    {
        std::cout << "Creating shared_ptr using make_shared:\n";
        std::shared_ptr<Resource> res1 = std::make_shared<Resource>("Resource1", 100);
        std::cout << "Reference count: " << res1.use_count() << "\n";

        std::cout << "\nCreating second shared_ptr pointing to same object:\n";
        std::shared_ptr<Resource> res2 = res1;
        std::cout << "res1 reference count: " << res1.use_count() << "\n";
        std::cout << "res2 reference count: " << res2.use_count() << "\n";

        std::cout << "\nLeaving scope - both pointers will be destroyed:\n";
    }
    std::cout << "Scope exited - object was destroyed when ref count reached 0\n";

    // ------------------------------------------------------------------------
    // Demo 2: Passing by value
    // ------------------------------------------------------------------------
    printSeparator("2. PASSING SHARED_PTR BY VALUE");
    {
        auto res = std::make_shared<Resource>("ValueTest", 200);
        std::cout << "Before function call - Reference count: " << res.use_count() << "\n";
        passByValue(res);
        std::cout << "After function call - Reference count: " << res.use_count() << "\n";
    }

    // ------------------------------------------------------------------------
    // Demo 3: Passing by const reference
    // ------------------------------------------------------------------------
    printSeparator("3. PASSING SHARED_PTR BY CONST REFERENCE (More Efficient)");
    {
        auto res = std::make_shared<Resource>("ConstRefTest", 300);
        std::cout << "Before function call - Reference count: " << res.use_count() << "\n";
        passByConstRef(res);
        std::cout << "After function call - Reference count: " << res.use_count() << "\n";
    }

    // ------------------------------------------------------------------------
    // Demo 4: Passing by reference (allows modification)
    // ------------------------------------------------------------------------
    printSeparator("4. PASSING SHARED_PTR BY REFERENCE (Allows Modification)");
    {
        auto res = std::make_shared<Resource>("RefTest", 400);
        std::cout << "Before function call:\n";
        res->display();
        passByRef(res);
        std::cout << "After function call (pointer was replaced):\n";
        res->display();
    }

    // ------------------------------------------------------------------------
    // Demo 5: Returning shared_ptr from function
    // ------------------------------------------------------------------------
    printSeparator("5. RETURNING SHARED_PTR FROM FUNCTION");
    {
        auto res = createResource("ReturnedResource", 500);
        std::cout << "Received shared_ptr from function. Ref count: " << res.use_count() << "\n";
        res->display();
    }

    // ------------------------------------------------------------------------
    // Demo 6: Shared ownership with ResourceManager
    // ------------------------------------------------------------------------
    printSeparator("6. SHARED OWNERSHIP (Multiple Owners)");
    {
        ResourceManager manager;

        std::cout << "Creating resource outside manager:\n";
        auto res1 = std::make_shared<Resource>("SharedResource1", 600);
        std::cout << "Ref count before adding to manager: " << res1.use_count() << "\n";

        manager.addResource(res1);
        std::cout << "Ref count after adding to manager: " << res1.use_count() << "\n";

        // Create another resource directly in the addResource call
        std::cout << "\nCreating resource directly in addResource call:\n";
        manager.addResource(std::make_shared<Resource>("SharedResource2", 700));

        std::cout << "\nManager now contains " << manager.size() << " resources.\n";
        manager.displayAll();

        std::cout << "\nres1 ref count before leaving scope: " << res1.use_count() << "\n";
        std::cout << "Leaving scope - res1 pointer destroyed but object survives (still owned by manager):\n";
    }
    std::cout << "After scope: object still alive because manager still owns it\n";

    // ------------------------------------------------------------------------
    // Demo 7: Passing raw pointer when ownership not needed
    // ------------------------------------------------------------------------
    printSeparator("7. PASSING RAW POINTER (No Ownership Transfer)");
    {
        auto res = std::make_shared<Resource>("RawPtrTest", 800);
        std::cout << "Ref count before passing raw pointer: " << res.use_count() << "\n";
        processResource(res.get());
        std::cout << "Ref count after passing raw pointer: " << res.use_count() << "\n";
    }

    // ------------------------------------------------------------------------
    // Demo 8: Using reset and checking for nullptr
    // ------------------------------------------------------------------------
    printSeparator("8. RESET AND NULLPTR CHECKING");
    {
        auto res = std::make_shared<Resource>("ResetTest", 900);
        std::cout << "Created shared_ptr. Is null? " << (res == nullptr ? "Yes" : "No") << "\n";
        std::cout << "Ref count: " << res.use_count() << "\n";

        std::cout << "\nCalling reset():\n";
        res.reset();
        std::cout << "After reset. Is null? " << (res == nullptr ? "Yes" : "No") << "\n";
        std::cout << "Ref count: " << res.use_count() << "\n";
    }

    // ------------------------------------------------------------------------
    // Demo 9: Multiple shared_ptrs and destruction order
    // ------------------------------------------------------------------------
    printSeparator("9. MULTIPLE SHARED_PTRS - DESTRUCTION ORDER");
    {
        std::vector<std::shared_ptr<Resource>> resources;

        std::cout << "Creating 3 resources and adding to vector:\n";
        resources.push_back(std::make_shared<Resource>("VectorRes1", 1001));
        resources.push_back(std::make_shared<Resource>("VectorRes2", 1002));
        resources.push_back(std::make_shared<Resource>("VectorRes3", 1003));

        std::cout << "\nVector size: " << resources.size() << "\n";
        std::cout << "Leaving scope - vector will be destroyed:\n";
    }
    std::cout << "All resources cleaned up\n";

    // ------------------------------------------------------------------------
    printSeparator("DEMONSTRATION COMPLETE");
    std::cout << "All resources have been properly cleaned up!\n\n";

    return 0;
}
