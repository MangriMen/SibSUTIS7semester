#include "pch.h"
#include "framework.h"
#include "lab2.hpp"
#include <fstream>

using namespace crypto;

namespace encryption {
	Shamir::Shamir(unsigned long long P) {
		this->P = P;

		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, INT_MAX - 4);
		std::array<long long, 3> EuclidResult;

		do {
			do {
				this->C = dist(rng) + 2;
				EuclidResult = Euclid::getExtendedGCD(C, P - 1);
			} while (EuclidResult[0] != 1);
			this->D = EuclidResult[2] + (P - 1);
		} while (C * D % (P - 1) != 1);
	}

	unsigned long long Shamir::generatePublicP() {
		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, INT_MAX - 4);

		unsigned long long P;
		do {
			P = dist(rng) + 2;
		} while (!DiffieHellman::isPrime(P));
		return P;
	}

	std::vector<long long> Shamir::ShamirCalcIteration(const std::string& path, std::vector<long long> prevX, int numOfIteration) {
		switch (numOfIteration) {
		case 1:
		{
			std::ifstream fileIn(path, std::ios::binary);
			if (!fileIn.is_open()) {
				throw std::runtime_error("Cannot open file: " + path);
			}

			std::vector<char> byteArray = std::vector<char>(std::istreambuf_iterator<char>(fileIn), std::istreambuf_iterator<char>());

			std::vector<long long> x1;

			for (char a : byteArray) {
				x1.push_back(FastMath::modExp(a, getC(), P));
			}
			return x1;
		}
		case 2:
		{

			std::vector<long long> x2;

			for (long long a : prevX) {
				x2.push_back(FastMath::modExp(a, getC(), P));
			}
			return x2;
		}
		case 3:
		{
			std::vector<long long> x3;

			for (long long a : prevX) {
				x3.push_back(FastMath::modExp(a, getD(), P));
			}
			return x3;
		}
		case 4:
		{
			std::vector<char> outFile(prevX.size());
			for (int i = 0; i < prevX.size(); i++) {
				outFile[i] = (char)FastMath::modExp(prevX[i], getD(), P);
			}
			std::ofstream fileOut(path, std::ios::binary);
			fileOut.write((char*)&outFile[0], outFile.size() * sizeof(char));
			return std::vector<long long>();
		}
		}
		return std::vector<long long>();
	}

	void Shamir::encryptFile(const std::string& filename)
	{
		long long P = Shamir::generatePublicP();

		Shamir A(P);
		//FileManipulation.KeysToFile("Shamir", "A", A.getC(), A.getD());

		Shamir B(P);
		//FileManipulation.KeysToFile("Shamir", "B", B.getC(), B.getD());

		std::vector<long long> x1 = A.ShamirCalcIteration(filename, std::vector<long long>(), 1);
		std::vector<long long>  x2 = B.ShamirCalcIteration("", x1, 2);
		std::vector<long long>  x3 = A.ShamirCalcIteration("", x2, 3);
		std::vector<long long>  x4 = B.ShamirCalcIteration(filename + ".enc", x3, 4);
	}

	long long Shamir::getC() {
		return C;
	}

	long long Shamir::getD() {
		return C;
	}

	long long ElGamal::P = 0;
	long long ElGamal::G = 0;
}
