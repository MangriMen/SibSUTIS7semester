#include <iostream>
#include "../lab2/lab2.hpp"
#include <fstream>

void shamir() {
	//encryption::Shamir::encryptFile("test.txt");
}

void gamal() {
	encryption::ElGamal::ecnryptFile("angel-mech.jpg");
}

void vernam() {
	auto vernam = encryption::Vernam();
	auto vernam_encrypted = vernam.encryptFile("angel-mech.jpg");
	Utils::writeFileAsBytes("vernam_enc_angel-mech.jpg", vernam_encrypted);
	vernam.decryptFile("vernam_dec_angel-mech.jpg", vernam_encrypted, vernam.getSecretKey());
}

void rsa() {
	auto rsa = encryption::RSA();
	rsa.RSA_Initialize();

	auto filename = std::string("test.txt");

	auto encrypted = rsa.RSA_Encrypt(filename);
	Utils::writeFileAsBytes("rsa/rsa_enc_" + filename, encrypted);

	auto encrypted_data = Utils::readFileAsBytes<unsigned long long>("rsa/rsa_enc_" + filename);

	auto decrypted = rsa.RSA_Decrypt(encrypted_data);
	Utils::writeFileAsBytes("rsa/rsa_dec_" + filename, decrypted);
}

int main()
{
	shamir();
	gamal();
	vernam();
	rsa();
}