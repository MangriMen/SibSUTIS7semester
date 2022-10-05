#include <iostream>
#include "../lab2/lab2.hpp"
#include <fstream>
#include <filesystem>

namespace fs = std::filesystem;

void shamir(const std::string& file) {
	unsigned long long P = encryption::Shamir::generatePublicP();

	encryption::Shamir A(P);
	encryption::Shamir B(P);

	Utils::writeBytesAsFile("output/shamir_key_P", std::vector<unsigned long long> { P });
	Utils::writeBytesAsFile("output/shamir_key_A", std::vector<unsigned long long> { A.getC(), A.getD() });
	Utils::writeBytesAsFile("output/shamir_key_B", std::vector<unsigned long long> { B.getC(), B.getD() });

	auto message = Utils::readFileAsBytes(file);

	auto x1 = std::get<std::vector<unsigned long long>>(A.ShamirCalcIteration(message, {}, 1));
	Utils::writeBytesAsFile("output/shamir_x1_" + file, x1);

	auto x2 = std::get<std::vector<unsigned long long>>(B.ShamirCalcIteration({}, x1, 2));
	Utils::writeBytesAsFile("output/shamir_x2_" + file, x2);

	auto x3 = std::get<std::vector<unsigned long long>>(A.ShamirCalcIteration({}, x2, 3));
	Utils::writeBytesAsFile("output/shamir_x3_" + file, x3);

	auto x4 = std::get<std::vector<char>>(B.ShamirCalcIteration({}, x3, 4));
	Utils::writeBytesAsFile("output/shamir_x4_" + file, x4);
}

void gamal(const std::string& file) {
	encryption::ElGamal::generateParameters();

	encryption::ElGamal A;
	encryption::ElGamal B;

	Utils::writeBytesAsFile("output/gamal_key_P", std::vector<unsigned long long> { encryption::ElGamal::getP(), encryption::ElGamal::getG() });
	Utils::writeBytesAsFile("output/gamal_key_A", std::vector<unsigned long long> { A.getC(), A.getD() });
	Utils::writeBytesAsFile("output/gamal_key_B", std::vector<unsigned long long> { B.getC(), B.getD() });

	std::vector<unsigned long long> encrypted = A.encryptFile(file, B.getD());
	Utils::writeBytesAsFile("output/gamal_enc_" + file, encrypted);

	auto decrypted = B.decryptFile("output/gamal_enc_" + file, A.getR());
	Utils::writeBytesAsFile("output/gamal_dec_" + file, decrypted);
}

void vernam(const std::string& file) {
	auto vernam = encryption::Vernam();

	auto encrypted = vernam.encryptFile(file);
	Utils::writeBytesAsFile("output/vernam_enc_" + file, encrypted);

	Utils::writeBytesAsFile("output/vernam_key", vernam.getSecretKey());

	auto decrypted = vernam.decryptFile("output/vernam_enc_" + file, vernam.getSecretKey());
	Utils::writeBytesAsFile("output/vernam_dec_" + file, decrypted);
}

void rsa(const std::string& file) {
	auto rsa = encryption::RSA();

	auto encrypted = rsa.encryptFile(file);
	Utils::writeBytesAsFile("output/rsa_enc_" + file, encrypted);

	Utils::writeBytesAsFile("output/rsa_key_public", rsa.getPublicKey());
	Utils::writeBytesAsFile("output/rsa_key_private", std::vector<unsigned long long> { rsa.getPrivateKey()});

	auto decrypted = rsa.decryptFile("output/rsa_enc_" + file);
	Utils::writeBytesAsFile("output/rsa_dec_" + file, decrypted);
}

int main()
{
	std::string folder = "output";
	if (!fs::is_directory(folder) || !fs::exists(folder)) {
		fs::create_directory(folder);
	}

	std::string filename = "Alexander.jpg";

	shamir(filename);
	gamal(filename);
	vernam(filename);
	rsa(filename);
}