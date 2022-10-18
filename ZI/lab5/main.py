from hashlib import sha3_256
from secrets import randbits
import sys
import crypto


class Server:
    _C: int

    N: int
    D: int

    users: dict
    database: dict

    def __init__(self) -> None:
        P = crypto.getPrimeInBounds(1 << 1023, (1 << 1024) - 1)
        Q = crypto.getPrimeInBounds(1 << 1023, (1 << 1024) - 1)

        self.N = P * Q
        phi = (P - 1) * (Q - 1)

        self.D = crypto.getCoprime(phi)

        gcd, c, _ = crypto.extendedGCD(self.D, phi)
        assert gcd == 1
        while c < 0:
            c += phi
        self._C = c

        self.users = {}
        self.database = {}

    def get_blank(self, name, user_result):
        if name in self.users.keys():
            print(f"{name} already voted")
            return -1
        self.users[name] = user_result
        return pow(user_result, self._C, self.N)

    def send_blank(self, name, n, s):
        hash = get_hash(n)
        calculated_hash = pow(s, self.D, self.N)
        if hash == calculated_hash:
            self.database[name] = (n, s)
            print(f"n: {n}")
            print(f"s: {s}")
            print(f"Blank from {name} accepted")
        else:
            print(f"n: {n}")
            print(f"s: {s}")
            print(f"Blank from {name} rejected")
            print(f"hash: {hash}")
            print(f"calc hash: {calculated_hash}")


def get_hash(num: int):
    return int.from_bytes(sha3_256(str(num).encode('ASCII')).digest(), byteorder=sys.byteorder)


def vote(name: str, vote: int, server: Server):
    # 1.
    rnd = randbits(512)
    n = rnd << 512 | vote
    # 2.
    r = crypto.getCoprime(server.N)
    # 3.
    h = get_hash(n)
    # 4.
    encoded_h = h * pow(r, server.D, server.N) % server.N
    # 5.
    encoded_s = server.get_blank(name, encoded_h)
    if encoded_s == -1:
        return
    # 6.
    s = encoded_s * crypto.inverse(r, server.N) % server.N
    # 7.
    server.send_blank(name, n, s)


def main() -> None:
    server = Server()

    vote("Alice", 1, server)
    vote("Alice", 42, server)
    vote("Bob", 42, server)


if __name__ == "__main__":
    main()
