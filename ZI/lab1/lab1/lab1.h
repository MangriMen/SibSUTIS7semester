#pragma once
#include <iostream>
#include <vector>
#include <array>
#include <random>
#include <map>

long long modExp(long long x, long long y, long long N);

long long modExp(long long a, long long x, long long p);

std::array<long long, 3> getExtendedGCD(long long a, long long n);

long long generateDiffieHellmanKey();

long long babyGiantStep(long long a, long long p, long long y);
