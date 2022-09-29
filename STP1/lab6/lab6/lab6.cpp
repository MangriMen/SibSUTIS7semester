#include <iostream>
#include "Complex.h"

int main()
{
	auto cplx = -Complex(1, 2);

	std::cout << cplx - Complex(10, 20);
}