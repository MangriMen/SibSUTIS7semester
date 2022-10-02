#pragma once
#include <iostream>
#include <random>
#include <array>
#include <numeric>
#include <list>
#include <fstream>
#include "../../lab1/lab1/lab1.hpp"
#include "Utils.h"

namespace encryption {
	class Shamir {
	private:
		unsigned long long P;
		unsigned long long C;
		unsigned long long D;

	public:
		Shamir(unsigned long long P);

		static unsigned long long generatePublicP();

		std::vector<long long> ShamirCalcIteration(const std::string& path, std::vector<long long> prevX, int numOfIteration);

		static void encryptFile(const std::string&);

		long long getC();
		long long getD();
	};

	class ElGamal {
	private:
		static unsigned long long P;
		static unsigned long long G;

		unsigned long long C;
		unsigned long long D;

		unsigned long long R;

	public:
		static void generateParameters() {
			auto [newP, g, key] = crypto::DiffieHellman::generateKey();
			setP(newP);
			setG(g);
		}

		ElGamal() {
			std::random_device dev;
			std::mt19937_64 rng(dev());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, P - 2 - 1);

			C = dist(rng) + 1;
			D = crypto::FastMath::modExp(G, C, P);
			this->R = 0;
		}

		std::vector<unsigned long long> encryptMessage(const std::vector<char>& message, unsigned long long D) {
			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, P - 3 - 1);

			unsigned long long k = dist(rng) + 1;
			unsigned long long partD = crypto::FastMath::modExp(D, k, P);

			std::vector<unsigned long long> encryptedMessage(message.size());
			for (size_t i = 0; i < encryptedMessage.size(); i++)
			{
				encryptedMessage[i] = crypto::FastMath::modExp((unsigned char)message[i] * static_cast<unsigned unsigned long long>(partD), 1, P);
			}
			R = crypto::FastMath::modExp(G, k, P);
			return encryptedMessage;
		}

		std::vector<char> decryptMessage(std::vector<unsigned long long> message, unsigned long long R) {
			unsigned long long partR = crypto::FastMath::modExp(R, P - 1 - C, P);

			std::vector<char> decryptedMessage(message.size());
			for (int i = 0; i < message.size(); i++) {
				decryptedMessage[i] = (char)crypto::FastMath::modExp(message[i] * partR, 1, P);
			}
			return decryptedMessage;
		}

		std::vector<unsigned long long> encryptFile(std::string filename, unsigned long long D) {
			auto data = Utils::readFileAsBytes(filename);
			return encryptMessage(data, D);
		}

		std::vector<char> decryptFile(std::string filename, unsigned long long R) {
			auto data = Utils::readFileAsBytes<unsigned long long>(filename);
			return decryptMessage(data, D);
		}

		unsigned long long getC() {
			return C;
		}

		unsigned long long getD() {
			return D;
		}

		unsigned long long getR() {
			return R;
		}

		static void setP(unsigned long long p) {
			P = p;
		}

		static void setG(unsigned long long g) {
			G = g;
		}
	};

	class Vernam {
	private:
		std::vector<char> secretKey;

	public:
		std::vector<char> encryptMessage(const std::vector<char>& message) {
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

		std::vector<char> decryptMessage(const std::vector<char>& message, const std::vector<char>& key) {
			std::vector<char> decryptedMessage = std::vector<char>(message.size());
			for (int i = 0; i < message.size(); i++) {
				decryptedMessage[i] = (char)(message[i] ^ key[i]);
			}
			return decryptedMessage;
		}

		std::vector<char> encryptFile(std::string file) {
			std::vector<char> data = Utils::readFileAsBytes(file);
			return encryptMessage(data);
		}

		std::vector<char> decryptFile(std::string file, const std::vector<char>& key) {
			auto data = Utils::readFileAsBytes(file);
			return decryptMessage(data, key);
		}

		std::vector<char> getSecretKey() {
			return secretKey;
		}
	};

	class RSA {
	private:
		unsigned long long e;
		unsigned long long d;
		unsigned long long n;

	public:
		void RSA_Initialize()
		{
			std::mt19937_64 rng((std::random_device())());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, static_cast<unsigned long long>(1e9));

			unsigned long long p;
			unsigned long long q;
			do {
				p = dist(rng);
				q = dist(rng);
			} while (p == q || !crypto::DiffieHellman::isPrime(p) || !crypto::DiffieHellman::isPrime(q));

			n = p * q;

			unsigned long long eulerN = (p - 1) * (q - 1);

			std::uniform_int_distribution<std::mt19937_64::result_type> distE(7, eulerN);

			do {
				e = distE(rng);
			} while (crypto::Euclid::getExtendedGCD(e, eulerN)[0] != 1 || !crypto::DiffieHellman::isPrime(e));

			d = crypto::Euclid::getExtendedGCD(e, eulerN)[1];
		};

		std::vector<unsigned long long> RSA_Encrypt(std::string file)
		{
			auto data = Utils::readFileAsBytes(file);
			std::vector<unsigned long long> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				short a = (unsigned char)(data[i]);
				message[i] = crypto::FastMath::modExp(a, e, n);
			}
			return message;
		}

		std::vector<char> RSA_Decrypt(std::string file)
		{
			auto data = Utils::readFileAsBytes<unsigned long long>(file);
			std::vector<char> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				message[i] = (char)crypto::FastMath::modExp(data[i], d, n);
			}
			return message;
		}
	};
}