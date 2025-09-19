#include <iostream>
#include "../MathLibrary/MathLibrary.h"

#pragma comment(lib, "MathLibrary.lib")

int main() {
    std::cout << "=== DLL Import/Export Example ===" << std::endl;
    std::cout << "Library: " << GetLibraryName() << std::endl;
    std::cout << "Version: " << GetVersion() << std::endl;
    std::cout << std::endl;

    std::cout << "=== C-style Functions ===" << std::endl;
    int a = 10, b = 5;
    std::cout << "Add(" << a << ", " << b << ") = " << Add(a, b) << std::endl;
    std::cout << "Subtract(" << a << ", " << b << ") = " << Subtract(a, b) << std::endl;
    
    double x = 7.5, y = 2.5;
    std::cout << "Multiply(" << x << ", " << y << ") = " << Multiply(x, y) << std::endl;
    std::cout << "Divide(" << x << ", " << y << ") = " << Divide(x, y) << std::endl;
    std::cout << std::endl;

    std::cout << "=== C++ Class Example ===" << std::endl;
    Calculator calc;
    
    double result1 = calc.Add(15.0, 25.0);
    std::cout << "Calculator.Add(15.0, 25.0) = " << result1 << std::endl;
    std::cout << "Last result: " << calc.GetLastResult() << std::endl;
    
    double result2 = calc.Multiply(result1, 2.0);
    std::cout << "Calculator.Multiply(result, 2.0) = " << result2 << std::endl;
    std::cout << "Last result: " << calc.GetLastResult() << std::endl;
    
    double result3 = calc.Divide(result2, 4.0);
    std::cout << "Calculator.Divide(result, 4.0) = " << result3 << std::endl;
    std::cout << "Last result: " << calc.GetLastResult() << std::endl;

    std::cout << std::endl << "Press any key to exit..." << std::endl;
    std::cin.get();
    
    return 0;
}