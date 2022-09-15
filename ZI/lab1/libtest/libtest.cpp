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
		1, static_cast<long long>(1e3));

	auto aM = dist(rng);
	auto xM = dist(rng);
	auto pM = dist(rng);
	cout << aM << " " << xM << " " << pM << "\n";
	auto modExp_result = FastMath::modExp(aM, xM, pM);
	cout << "Module Exp: " << modExp_result << "\n";

	auto gcd_result = Euclid::getExtendedGCD(dist(rng), dist(rng));
	cout << "Extended GCD: " << gcd_result[0] << "; x: " << gcd_result[1]
		<< " y: " << gcd_result[2] << "\n";

	auto generateDiffieHellmanKey_result = DiffieHellman::generateKey();
	cout << "Diffie Hellman: " << generateDiffieHellmanKey_result << "\n";

	auto a = 0ll;
	auto p = 0ll;
	auto y = dist(rng);
	do {
		a = dist(rng);
		p = dist(rng);
	} while (Euclid::getExtendedGCD(a, p)[0] != 1);

	auto babyGiantStep_result =
		BabyGiantStep::calculate(a, p, y);
	
	for (const auto& result : babyGiantStep_result) {
		if (y == FastMath::modExp(a, result, p)) {
			cout << "OK ";
		}
		else {
			//cout << "ERROR ";
			//return -1;
		}
	}

	cout << "Baby Giant Step: \n";

	cout << "S: " << babyGiantStep_result.size() << "\n";
	cout << "Press enter key to print results...\n";
	cin.get();
	for (const auto& result : babyGiantStep_result) {
		cout << result << " ";
	}
	cout << "\n";
}