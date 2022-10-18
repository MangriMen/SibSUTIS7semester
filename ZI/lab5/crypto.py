from math import gcd
import random


def extendedGCD(a, b):
    if a <= 0 or b <= 0:
        raise ValueError("a and b must be greater than zero")

    u1, u2, u3 = a, 1, 0
    v1, v2, v3 = b, 0, 1
    while v1:
        q = u1 // v1
        t1, t2, t3 = u1 % v1, u2 - q * v2, u3 - q * v3
        u1, u2, u3 = v1, v2, v3
        v1, v2, v3 = t1, t2, t3
    return u1, u2, u3


def inverse(n, p):
    gcd, inv, _ = extendedGCD(n, p)
    assert gcd == 1
    return inv


def isPrime(n, trials=8):
    if n != int(n):
        return False
    n = int(n)
    # Miller-Rabin test for prime
    if n == 0 or n == 1 or n == 4 or n == 6 or n == 8 or n == 9:
        return False

    if n == 2 or n == 3 or n == 5 or n == 7:
        return True
    s = 0
    d = n - 1
    while d % 2 == 0:
        d >>= 1
        s += 1
    assert (2 ** s * d == n - 1)

    def trial_composite(a):
        if pow(a, d, n) == 1:
            return False
        for i in range(s):
            if pow(a, 2 ** i * d, n) == n - 1:
                return False
        return True

    for i in range(trials):  # number of trials
        a = random.randrange(2, n)
        if trial_composite(a):
            return False

    return True


def getPrimeInBounds(a, b):
    while True:
        p = random.randint(a, b)
        if isPrime(p):
            return p


def inverse(n, p):
    gcd, inv, _ = extendedGCD(n, p)
    assert gcd == 1
    return inv


def getCoprime(a):
    while True:
        b = random.randrange(2, a)
        if gcd(a, b) == 1:
            return b
