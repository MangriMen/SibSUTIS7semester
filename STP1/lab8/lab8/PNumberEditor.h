#pragma once
#include <iostream>
#include "../../lab7/lab7/PNumber.h"

std::vector<std::string> split(std::string s, std::string delimiter) {
	size_t pos_start = 0, pos_end, delim_len = delimiter.length();
	std::string token;
	std::vector<std::string> res;

	while ((pos_end = s.find(delimiter, pos_start)) != std::string::npos) {
		token = s.substr(pos_start, pos_end - pos_start);
		pos_start = pos_end + delim_len;
		res.push_back(token);
	}

	res.push_back(s.substr(pos_start));
	return res;
}

class PNumberEditor
{
	std::string current_number;
	
	bool isNull() {
		return split(current_number, " ")[0] == "0";
	}

	std::string toggleNegative() {
		auto parsed = split(current_number, " ");
		if (parsed[0][0] == '-') {
			current_number.erase(0, 1);
		}
		else {
			current_number.insert(0, "-");
		}
	}
};
