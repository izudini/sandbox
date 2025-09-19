#include "MathLibrary.h"
#include <string>

extern "C" {
    int Add(int a, int b) {
        return a + b;
    }

    int Subtract(int a, int b) {
        return a - b;
    }

    double Multiply(double a, double b) {
        return a * b;
    }

    double Divide(double a, double b) {
        if (b != 0.0) {
            return a / b;
        }
        return 0.0;
    }

    int GetVersion() {
        return 100;
    }

    const char* GetLibraryName() {
        static const char* name = "MathLibrary v1.0";
        return name;
    }
}

Calculator::Calculator() : m_lastResult(0.0) {
}

Calculator::~Calculator() {
}

double Calculator::Add(double a, double b) {
    m_lastResult = a + b;
    return m_lastResult;
}

double Calculator::Subtract(double a, double b) {
    m_lastResult = a - b;
    return m_lastResult;
}

double Calculator::Multiply(double a, double b) {
    m_lastResult = a * b;
    return m_lastResult;
}

double Calculator::Divide(double a, double b) {
    if (b != 0.0) {
        m_lastResult = a / b;
    } else {
        m_lastResult = 0.0;
    }
    return m_lastResult;
}

double Calculator::GetLastResult() const {
    return m_lastResult;
}