#include <iostream>
#include "../lab1/lab1.cpp"

int main()
{
	std::random_device dev;
	std::mt19937 rng(dev());
	std::uniform_int_distribution<std::mt19937::result_type> dist(1, static_cast<long long>(1e9));

	auto modExp_result = modExp(dist(rng), dist(rng), dist(rng));
	cout << "Module Exp: " << modExp_result << "\n";

	auto gcd_result = getExtendedGCD(dist(rng), dist(rng));
	cout << "Extended GCD: " << gcd_result[0] << "; x: " << gcd_result[1] << " y: " << gcd_result[2] << "\n";

	auto generateDiffieHellmanKey_result = generateDiffieHellmanKey();
	cout << "Diffie Hellman: " << generateDiffieHellmanKey_result << "\n";

	auto babyGiantStep_result = babyGiantStep(dist(rng), dist(rng), dist(rng));
	cout << "Baby Giant Step: " << babyGiantStep_result << "\n";
}