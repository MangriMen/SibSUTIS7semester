#include <iostream>
#include "../lab1/lab1.h"

using namespace std;

int main()
{
	auto modExp_result = modExp(1, 2, 3);
	cout << "Module Exp: " << modExp_result << "\n";

	auto gcd_result = getExtendedGCD(2, 31);
	cout << "Extended GCD: " << gcd_result[0] << "; x: " << gcd_result[1] << " y: " << gcd_result[2] << "\n";

	for (int i = 0; i < 100000; i++) {
		auto generateDiffieHellmanKey_result = generateDiffieHellmanKey();
		cout << "Diffie Hellman: " << generateDiffieHellmanKey_result << "\n";
	}

	auto babyGiantStep_result = babyGiantStep(3, 31, 7);
	cout << "Baby Giant Step: " << babyGiantStep_result << "\n";
}