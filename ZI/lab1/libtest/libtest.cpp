#include "../lab1/lab1.hpp"
#include <array>
#include <iostream>
#include <random>

using namespace std;
using namespace crypto;

int main() {
	std::random_device dev;
	std::mt19937 rng(dev());
	std::uniform_int_distribution<std::mt19937::result_type> dist(
		1, static_cast<long long>(1e9));

	auto modExp_result = FastMath::modExp(dist(rng), dist(rng), dist(rng));
	cout << "Module Exp: " << modExp_result << "\n";

	auto gcd_result = Euclid::getExtendedGCD(dist(rng), dist(rng));
	cout << "Extended GCD: " << gcd_result[0] << "; x: " << gcd_result[1]
		<< " y: " << gcd_result[2] << "\n";

	auto generateDiffieHellmanKey_result = DiffieHellman::generateKey();
	cout << "Diffie Hellman: " << generateDiffieHellmanKey_result << "\n";

	auto a = dist(rng);
	auto p = dist(rng);

	while (Euclid::getExtendedGCD(a, p)[0] != 1 || !DiffieHellman::isPrime(a) || !DiffieHellman::isPrime(p)) {
		a = dist(rng);
		p = dist(rng);
	}

	auto x = dist(rng);
	auto y = FastMath::modExp(a, x, p);

	cout << "Baby Giant Step: " << "\n";
	auto babyGiantStep_result = BabyGiantStep::calculate(a, y, p);

	for (const auto& res : babyGiantStep_result) {
		cout << res << " ";
		if (y == FastMath::modExp(a, res, p)) {
			cout << "OK ";
		}
		else {
			cout << "ERROR ";
			return -1;
		}
		cout << "\n";
	}

	cout << "\n";
}