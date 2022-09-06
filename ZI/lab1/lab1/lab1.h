#pragma once
#include <iostream>
#include <vector>
#include <array>
#include <random>
#include <map>

long long modExp(long long x, long long y, long long N);

std::array<long, 3> getExtendedGCD(int a, int n);

long long generateDiffieHellmanKey();

long long babyGiantStep(long long a, long long p, long long y);
