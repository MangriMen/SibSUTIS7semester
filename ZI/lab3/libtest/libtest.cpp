#include <iostream>
#include <fstream>
#include <filesystem>
#include "../lab3/lab3.hpp"

namespace fs = std::filesystem;

void rsa(const std::string& file) {
	auto message = Utils::readFileAsBytes(file);

	auto rsa = encryption::RSA();
	std::string publicKeyPath = "output/rsa_key_public";
	std::string privateKeyPath = "output/rsa_key_private";

	if (!fs::exists(publicKeyPath) || !fs::exists(privateKeyPath)) {
		Utils::writeBytesAsFile(publicKeyPath, rsa.getPublicKey());
		Utils::writeBytesAsFile(privateKeyPath, std::vector<unsigned long long> { rsa.getPrivateKey()});
	}
	else {
		rsa.setPublicKey(Utils::readFileAsBytes<uint64_t>(publicKeyPath));
		rsa.setPrivateKey(Utils::readFileAsBytes<uint64_t>(privateKeyPath)[0]);
	}

	auto sign = sign::RSA::signMessage(rsa.getPublicKey(), message);
	Utils::writeBytesAsFile("output/rsa_signed", sign);

	auto signFromFile = Utils::readFileAsBytes<uint64_t>("output/rsa_signed");
	bool isRightSign = sign::RSA::validateSign(rsa.getPublicKey(), rsa.getPrivateKey(), message, signFromFile);

	std::cout << "RSA: " << (isRightSign ? "sign is correct" : "sign is incorrect") << "\n";
}

void gamal(const std::string& file) {
	auto message = Utils::readFileAsBytes(file);

	std::string PKeyPath = "output/gamal_key_P";
	std::string AKeyPath = "output/gamal_key_A";
	std::string BKeyPath = "output/gamal_key_B";

	encryption::ElGamal::generateParameters();

	encryption::ElGamal A;
	encryption::ElGamal B;

	if (fs::exists(PKeyPath) && fs::exists(AKeyPath) && fs::exists(BKeyPath)) {
		auto commonKey = Utils::readFileAsBytes<uint64_t>(PKeyPath);
		encryption::ElGamal::setP(commonKey[0]);
		encryption::ElGamal::setG(commonKey[1]);
		A.setCD(Utils::readFileAsBytes<uint64_t>(AKeyPath));
		B.setCD(Utils::readFileAsBytes<uint64_t>(BKeyPath));
	}

	auto sign = sign::ElGamal::signMessage(A, B, message);
	Utils::writeBytesAsFile("output/gamal_signed", sign);

	if (!fs::exists(AKeyPath) || !fs::exists(BKeyPath)) {
		Utils::writeBytesAsFile(PKeyPath, std::vector<unsigned long long> { encryption::ElGamal::getP(), encryption::ElGamal::getG() });
		Utils::writeBytesAsFile(AKeyPath, std::vector<unsigned long long> { A.getC(), A.getD() });
		Utils::writeBytesAsFile(BKeyPath, std::vector<unsigned long long> { B.getC(), B.getD() });
	}

	auto signFromFile = Utils::readFileAsBytes<uint64_t>("output/gamal_signed");
	bool isRightSign = sign::ElGamal::validateSign(A, B, message, sign);

	std::cout << "El Gamal: " << (isRightSign ? "sign is correct" : "sign is incorrect") << "\n";
}

//void GOST(const std::string& file) {
//	auto message = Utils::readFileAsBytes(file);
//
//	auto [p, q, a] = sign::GOST_P34_10_94::generateParameters();
//	sign::GOST_P34_10_94 GOST_1(p, q, a);
//
//	auto sign = GOST_1.signMessage(message);
//	Utils::writeBytesAsFile("output/GOST_signed", sign);
//
//	sign::GOST_P34_10_94 GOST_2(p, q, a);
//	auto signFromFile = Utils::readFileAsBytes<uint64_t>("output/GOST_signed");
//	bool isRightSign = GOST_2.validateSign(message, sign);
//
//	std::cout << "GOST: " << (isRightSign ? "sign is correct" : "sign is incorrect") << "\n";
//}

int main()
{
	std::string folder = "output";
	if (!fs::is_directory(folder) || !fs::exists(folder)) {
		fs::create_directory(folder);
	}

	std::string filename = "eng.txt";

	rsa(filename);
	gamal(filename);
	//GOST(filename);
}
