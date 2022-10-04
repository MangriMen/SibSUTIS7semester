#pragma once
#include <iostream>
#include <vector>
#include <fstream>

class Utils {
public:
	static std::vector<char> readFileAsBytes(std::string path) {
		std::ifstream file(path, std::ios::in | std::ios::binary);
		if (!file.is_open()) {
			throw std::runtime_error("Cannot open file: " + path);
		}
		auto data = std::vector<char>(std::istreambuf_iterator<char>(file), std::istreambuf_iterator<char>());
		file.close();
		return data;
	}

	template <typename T>
	static std::vector<T> readFileAsBytes(std::string path) {
		std::ifstream file(path, std::ios::in | std::ios::binary);
		if (!file.is_open()) {
			throw std::runtime_error("Cannot open file: " + path);
		}

		file.ignore(std::numeric_limits<std::streamsize>::max());
		std::streamsize length = file.gcount();
		file.clear();
		file.seekg(0, std::ios_base::beg);

		std::vector<T> data(length / sizeof(T));
		if (data.size() > 0) {
			file.read((char*)&data[0], data.size() * sizeof(T));
		}
		file.close();

		return data;
	}

	template <typename T>
	static void writeBytesAsFile(std::string path, std::vector<T> data) {
		std::ofstream file(path, std::ios::out | std::ios::binary);
		if (!file.is_open()) {
			throw std::runtime_error("Cannot open file: " + path);
		}
		if (data.size() > 0) {
			file.write((char*)&data[0], data.size() * sizeof(T));
		}
		file.close();
	}
};