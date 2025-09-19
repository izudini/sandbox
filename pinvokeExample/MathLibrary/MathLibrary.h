#pragma once

#ifdef MATHLIBRARY_EXPORTS
#define MATHLIBRARY_API __declspec(dllexport)
#else
#define MATHLIBRARY_API __declspec(dllimport)
#endif

extern "C" {
    MATHLIBRARY_API int Add(int a, int b);
    MATHLIBRARY_API int Subtract(int a, int b);
    MATHLIBRARY_API double Multiply(double a, double b);
    MATHLIBRARY_API double Divide(double a, double b);
    MATHLIBRARY_API int GetVersion();
    MATHLIBRARY_API const char* GetLibraryName();
}

class MATHLIBRARY_API Calculator {
public:
    Calculator();
    ~Calculator();
    
    double Add(double a, double b);
    double Subtract(double a, double b);
    double Multiply(double a, double b);
    double Divide(double a, double b);
    double GetLastResult() const;
    
private:
    double m_lastResult;
};