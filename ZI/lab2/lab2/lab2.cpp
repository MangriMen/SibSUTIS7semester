#include "pch.h"
#include "framework.h"
#include "lab2.hpp"
#include <fstream>

using namespace crypto;

namespace encryption {
	// Shamir
	unsigned long long Shamir::generatePublicP() {
		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type>dist(1, static_cast<unsigned long long>(1e9));

		unsigned long long P;

		do {
			P = dist(rng);
		} while (!crypto::DiffieHellman::isPrime(P));

		return P;
	}

	Shamir::Shamir(unsigned long long P) {
		this->P = P;

		std::mt19937_64 rng((std::random_device())());
		std::uniform_int_distribution<std::mt19937_64::result_type>dist(1, static_cast <unsigned long long>(1e9));
		std::array<long long, 3> EuclidResult;

		do {
			do {
				this->C = dist(rng);
				EuclidResult = crypto::Euclid::getExtendedGCD(C, P - 1);
			} while (EuclidResult[0] != 1);
			this->D = EuclidResult[2] + (P - 1);
		} while (C * D % (P - 1) != 1);
	}

	std::variant<std::vector<unsigned long long>, std::vector<char>> Shamir::ShamirCalcIteration(const std::vector<char>& message, std::vector<unsigned long long> prevX = {}, int numOfIteration = 0) {
		switch (numOfIteration) {
		case 1:
		{
			std::vector<unsigned long long> x1;

			for (const auto& a : message) {
				auto letter = static_cast<unsigned char>(a);
				x1.push_back(crypto::FastMath::modExp(letter, getC(), P));
			}
			return x1;
		}
		case 2:
		{

			std::vector<unsigned long long> x2;

			for (unsigned long long a : prevX) {
				x2.push_back(crypto::FastMath::modExp(a, getC(), P));
			}
			return x2;
		}
		case 3:
		{
			std::vector<unsigned long long> x3;

			for (unsigned long long a : prevX) {
				x3.push_back(crypto::FastMath::modExp(a, getD(), P));
			}
			return x3;
		}
		case 4:
		{
			std::vector<char> outFile(prevX.size());
			for (int i = 0; i < prevX.size(); i++) {
				outFile[i] = (char)crypto::FastMath::modExp(prevX[i], getD(), P);
			}
			return outFile;
		}
		default:
			return std::vector<unsigned long long>();
		}
	}

	unsigned long long Shamir::getC() {
		return C;
	}

	unsigned long long Shamir::getD() {
		return D;
	}

	void Shamir::setCD(const std::vector<unsigned long long>& keys) {
		assert(keys.size() >= 2);
		C = keys[0];
		D = keys[1];
	}
	// Shamir

	// El Gamal
	unsigned long long ElGamal::P = 0;
	unsigned long long ElGamal::G = 0;

	void ElGamal::generateParameters() {
		auto [newP, g, key] = crypto::DiffieHellman::generateKey();
		setP(newP);
		setG(g);

		assert(crypto::DiffieHellman::isPrime(newP));
		assert(crypto::DiffieHellman::isPrime(g));
	}

	void ElGamal::setP(unsigned long long p) {
		P = p;
	}

	void ElGamal::setG(unsigned long long g) {
		G = g;
	}

	ElGamal::ElGamal() {
		std::random_device dev;
		std::mt19937_64 rng(dev());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(2, P - 2);

		C = dist(rng);
		D = crypto::FastMath::modExp(G, C, P);
		this->R = 0;
	}

	std::vector<unsigned long long> ElGamal::encryptMessage(const std::vector<char>& message, unsigned long long D) {
		std::mt19937_64 rng((std::random_device())());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(1, P - 2);

		unsigned long long k = dist(rng);
		R = crypto::FastMath::modExp(G, k, P);

		unsigned long long partD = crypto::FastMath::modExp(D, k, P);

		std::vector<unsigned long long> encryptedMessage(message.size());
		for (size_t i = 0; i < encryptedMessage.size(); i++)
		{
			const auto letter = static_cast<unsigned char>(message[i]);
			encryptedMessage[i] = crypto::FastMath::modExp(letter * partD, 1, P);
		}
		return encryptedMessage;
	}

	std::vector<char> ElGamal::decryptMessage(std::vector<unsigned long long> message, unsigned long long R) {
		unsigned long long partR = crypto::FastMath::modExp(R, P - 1 - C, P);
		std::vector<char> decryptedMessage(message.size());
		for (int i = 0; i < message.size(); i++) {
			decryptedMessage[i] = (char)crypto::FastMath::modExp(message[i] * partR, 1, P);
		}
		return decryptedMessage;
	}

	std::vector<unsigned long long> ElGamal::encryptFile(std::string filename, unsigned long long D) {
		auto data = Utils::readFileAsBytes(filename);
		return encryptMessage(data, D);
	}

	std::vector<char> ElGamal::decryptFile(std::string filename, unsigned long long R) {
		auto data = Utils::readFileAsBytes<unsigned long long>(filename);
		return decryptMessage(data, R);
	}

	unsigned long long ElGamal::getC() {
		return C;
	}

	unsigned long long ElGamal::getD() {
		return D;
	}

	unsigned long long ElGamal::getR() {
		return R;
	}

	void ElGamal::setCD(const std::vector<unsigned long long>& keys) {
		assert(keys.size() >= 2);
		C = keys[0];
		D = keys[1];
	}
	// El Gamal

	// Vernam
	std::vector<char> Vernam::encryptMessage(const std::vector<char>& message) {
		std::mt19937_64 rng((std::random_device())());
		std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, 255);

		std::vector<char> encryptedMessage = std::vector<char>(message.size());
		secretKey = std::vector<char>(message.size());
		for (int i = 0; i < message.size(); i++) {
			secretKey[i] = (char)dist(rng);
			encryptedMessage[i] = (char)(message[i] ^ secretKey[i]);
		}
		return encryptedMessage;
	}

	std::vector<char> Vernam::decryptMessage(const std::vector<char>& message, const std::vector<char>& key) {
		std::vector<char> decryptedMessage = std::vector<char>(message.size());
		for (int i = 0; i < message.size(); i++) {
			decryptedMessage[i] = (char)(message[i] ^ key[i]);
		}
		return decryptedMessage;
	}

	std::vector<char> Vernam::encryptFile(std::string file) {
		std::vector<char> data = Utils::readFileAsBytes(file);
		return encryptMessage(data);
	}

	std::vector<char> Vernam::decryptFile(std::string file, const std::vector<char>& key) {
		auto data = Utils::readFileAsBytes(file);
		return decryptMessage(data, key);
	}

	std::vector<char> Vernam::getSecretKey() {
		return secretKey;
	}
	void Vernam::setSecretKey(const std::vector<char>& key) {
		secretKey = key;
	}
	// Vernam
}
