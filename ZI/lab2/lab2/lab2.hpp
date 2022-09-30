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

	static class ElGamal {
	private:
		static long long P;
		static long long G;

		long C;
		long D;

		long R;

	public:
		static void generateParameters() {
			auto [P, g, key] = crypto::DiffieHellman::generateKey();
			setP(P);
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

		std::vector<long long> elGamalSendMessage(long D, std::string path) {
			std::random_device dev;
			std::mt19937_64 rng(dev());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, P - 3 - 1);

			long k = dist(rng) + 1;;
			std::ifstream fileIn(path, std::ios::binary);
			if (!fileIn.is_open()) {
				throw std::runtime_error("Cannot open file: " + path);
			}

			std::vector<char> byteArray = std::vector<char>(std::istreambuf_iterator<char>(fileIn), std::istreambuf_iterator<char>());
			std::vector<long long> E;

			long partD = crypto::FastMath::modExp(D, k, P);

			for (char b : byteArray) {
				E.push_back(crypto::FastMath::modExp((unsigned char)b * static_cast<unsigned long long>(partD), 1, P));
			}
			R = crypto::FastMath::modExp(G, k, P);
			return E;
		}

		void elGamalReceiveMessage(long R, std::vector<long long> E, std::string path) {
			std::vector<char> outFile(E.size());
			long partR = crypto::FastMath::modExp(R, P - 1 - C, P);

			for (int i = 0; i < E.size(); i++) {
				outFile[i] = (char)crypto::FastMath::modExp(E[i] * partR, 1, P);
			}

			std::ofstream fileOut(path, std::ios::binary);
			fileOut.write((char*)&outFile[0], outFile.size() * sizeof(char));
		}

		long getC() {
			return C;
		}

		long getD() {
			return D;
		}

		long getR() {
			return R;
		}

		static void setP(int p) {
			P = p;
		}

		static void setG(int g) {
			G = g;
		}

		static void ecnryptFile(const std::string& path) {
			ElGamal::generateParameters();

			ElGamal N;
			//FileManipulation.KeysToFile("ElGamal", "N", N.getC(), N.getD());

			ElGamal M;
			//FileManipulation.KeysToFile("ElGamal", "M", M.getC(), M.getD());

			std::vector<long long> E = N.elGamalSendMessage(M.getD(), path);

			std::ofstream fileOut("gamal_enc_" + path, std::ios::out | std::ios::binary);
			if (!fileOut.is_open()) {
				throw std::runtime_error("Cannot open file");
			}
			fileOut.write((char*)&E[0], E.size() * sizeof(E[0]));
			fileOut.close();

			M.elGamalReceiveMessage(N.getR(), E, "gamal_dec_" + path);
		}
	};

	class Vernam {
	private:
		std::vector<char> secretKey;

	public:
		std::vector<char> encryptFile(std::string path) {
			std::ifstream fileIn(path, std::ios::binary);
			if (!fileIn.is_open()) {
				throw std::runtime_error("Cannot open file: " + path);
			}

			std::vector<char> byteArray = std::vector<char>(std::istreambuf_iterator<char>(fileIn), std::istreambuf_iterator<char>());

			secretKey = std::vector<char>(byteArray.size());
			std::vector<char> encryptMessage = std::vector<char>(byteArray.size());

			std::random_device dev;
			std::mt19937_64 rng(dev());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, 255);

			for (int i = 0; i < byteArray.size(); i++) {
				secretKey[i] = (char)dist(rng);
				encryptMessage[i] = (char)(byteArray[i] ^ secretKey[i]);
			}
			return encryptMessage;
		}
		void decryptFile(std::string path, std::vector<char> encryptMessage, std::vector<char> key) {
			std::vector<char> decryptMessage = std::vector<char>(encryptMessage.size());
			for (int i = 0; i < encryptMessage.size(); i++) {
				decryptMessage[i] = (char)(encryptMessage[i] ^ key[i]);
			}

			std::ofstream fileOut(path, std::ios::binary);
			fileOut.write((char*)&decryptMessage[0], decryptMessage.size() * sizeof(char));
			fileOut.close();
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
			std::random_device dev;
			std::mt19937_64 rng(dev());
			std::uniform_int_distribution<std::mt19937_64::result_type> dist(0, 1e9);

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

			std::cout << "e: " << e << " d: " << d << " n:" << n << " eulerN: " << eulerN << "\n";
		};

		std::vector<unsigned long long> RSA_Encrypt(std::string filename)
		{
			auto data = Utils::readFileAsBytes(filename);
			std::vector<unsigned long long> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				short a = (unsigned char)(data[i]);
				message[i] = crypto::FastMath::modExp(a, e, n);
			}
			return message;
		}

		std::vector<char> RSA_Decrypt(std::vector<unsigned long long> data)
		{
			std::vector<char> message(data.size());
			for (size_t i = 0; i < message.size(); i++)
			{
				message[i] = (char)crypto::FastMath::modExp(data[i], d, n);
			}
			return message;
		}
	};
}