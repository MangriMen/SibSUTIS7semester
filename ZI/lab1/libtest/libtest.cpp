#include <iostream>
#include "../lab1/lab1.cpp"

int main()
{
    int x = 0;
    int y = 0;
    std::cout << gcd(2, 4, x, y) << "\n";
    std::cout << x << " " << y;
}