#include <iostream>
#include "../lab2/lab2.hpp"
#include <fstream>
#include <filesystem>

namespace fs = std::filesystem;

void shamir(const std::string& file) {
	//encryption::Shamir::encryptFile(file);
}

void gamal(const std::string& file) {
	encryption::ElGamal::generateParameters();

	encryption::ElGamal N;
	encryption::ElGamal M;

	Utils::writeBytesAsFile("output/gamal_key_N", std::vector<unsigned long long> { N.getC(), N.getD() });
	Utils::writeBytesAsFile("output/gamal_key_M", std::vector<unsigned long long> { N.getC(), N.getD() });

	std::vector<unsigned long long> encryptedMessage = N.encryptFile(file, M.getD());
	Utils::writeBytesAsFile("output/gamal_enc_" + file, encryptedMessage);

	auto decrypted_data = M.decryptFile("output/gamal_enc_" + file, N.getR());
	Utils::writeBytesAsFile("output/gamal_dec_" + file, decrypted_data);
}

void vernam(const std::string& file) {
	auto vernam = encryption::Vernam();

	auto encrypted = vernam.encryptFile(file);
	Utils::writeBytesAsFile("output/vernam_enc_" + file, encrypted);
	Utils::writeBytesAsFile("output/vernam_key", vernam.getSecretKey());

	auto decrypted_data = vernam.decryptFile("output/vernam_enc_" + file, vernam.getSecretKey());
	Utils::writeBytesAsFile("output/vernam_dec" + file, decrypted_data);
}

void rsa(const std::string& file) {
	auto rsa = encryption::RSA();
	rsa.RSA_Initialize();

	auto encrypted = rsa.RSA_Encrypt(file);
	Utils::writeBytesAsFile("output/rsa_enc_" + file, encrypted);

	auto decrypted = rsa.RSA_Decrypt("output/rsa_enc_" + file);
	Utils::writeBytesAsFile("output/rsa_dec_" + file, decrypted);
}

int main()
{
	std::string folder = "output";
	if (!fs::is_directory(folder) || !fs::exists(folder)) {
		fs::create_directory(folder);
	}

	std::string filename = "eng.txt";

	shamir(filename);
	gamal(filename);
	vernam(filename);
	rsa(filename);
}