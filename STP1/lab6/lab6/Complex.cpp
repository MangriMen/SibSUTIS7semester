#include "Complex.h"

std::ostream& operator <<(std::ostream& os, const Complex& complex)
{
	os << complex.toString();
	return os;
}
