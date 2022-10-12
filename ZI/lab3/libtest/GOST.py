from hashlib import md5, sha256
from pathlib import Path
import random
import secrets
import sys


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


def main():
    folder = "output"
    Path(folder).mkdir(parents=True, exist_ok=True)

    filename = "angel-mech.jpg"

    message_hash = get_file_hash(filename)

    a, q, p, private_key, public_key = generate_parameters()
    r, s = sign(a, q, p, private_key, message_hash)

    with open(f'{folder}/gost_public_keys', 'wb') as file:
        file.write(a.to_bytes(1024 // 8, byteorder=sys.byteorder))
        file.write(p.to_bytes(1024 // 8, byteorder=sys.byteorder))
        file.write(q.to_bytes(256 // 8, byteorder=sys.byteorder))

    with open(f'{folder}/gost_signed', 'wb') as file:
        file.write(r.to_bytes(256 // 8, byteorder=sys.byteorder))
        file.write(s.to_bytes(256 // 8, byteorder=sys.byteorder))

    print("GOST: sign is ", end='')
    print("correct" if validate_sign(a, p, q, r, s,
          public_key, message_hash) else "incorrect")


def get_file_hash(path):
    with open(path, 'rb') as file:
        return int.from_bytes(sha256(file.read()).digest(), byteorder=sys.byteorder)


def generate_parameters():
    q = getPrimeInBounds(1 << 255, (1 << 256) - 1)

    while True:
        b = random.randint((1 << 1023) // q, ((1 << 1024) - 1) // q)
        if isPrime(p := b * q + 1):
            break

    while True:
        g = random.randrange(2, p - 1)
        if (a := pow(g, b, p)) > 1 and (pow(a, q, p) == 1):
            break

    x = secrets.randbelow(q - 2) + 1
    y = pow(a, x, p)

    return a, q, p, x, y


def sign(a, q, p, x, message_hash):
    assert 0 < message_hash < q

    while True:
        k = secrets.randbelow(q - 2) + 1
        if (r := pow(a, k, p) % q) == 0:
            continue
        if (s := (k * message_hash + x * r) % q) != 0:
            break

    return r, s


def validate_sign(a, p, q, r, s, y, message_hash):
    if not (0 < r < q):
        return False
    if not (0 < s < q):
        return False

    inversed_h = inverse(message_hash, q)
    u1 = s * inversed_h % q
    u2 = -r * inversed_h % q
    v = pow(a, u1, p) * pow(y, u2, p) % p % q

    return v == r


if __name__ == "__main__":
    main()
