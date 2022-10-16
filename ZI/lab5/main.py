from hashlib import sha256, sha3_256, sha3_512
from secrets import randbelow, randbits
import sys
import crypto
import rsa


class Server:
    _C: int

    N: int
    D: int

    users: dict
    database: dict

    def __init__(self) -> None:
        public_key, private_key = rsa.newkeys(2048)

        self.N = public_key.n
        self.D = public_key.e
        self._C = private_key.e

        self.users = {}
        self.database = {}

    def receive(self, user_name, user_result):
        self.users[user_name] = user_result
        return pow(user_result, self._C, self.N)

    def receive_s(self, user_name, message):
        hash = get_hash(message[0])
        a = pow(message[1], self.D, self.N)
        if hash == pow(message[1], self.D, self.N):
            self.database[user_name] = message


def get_hash(num: int):
    return int.from_bytes(sha3_256(str(num).encode('ASCII')).digest(), byteorder=sys.byteorder)


def main() -> None:
    server = Server()

    # 1.
    rnd = randbits(512)
    vote = 1
    n = vote

    # 2.
    while True:
        r = crypto.getPrimeInBounds(1, 100)
        if crypto.extendedGCD(r, server.N)[0] == 1:
            break

    # 3.
    h = get_hash(n)

    # 4.
    encoded_h = (h * pow(r, server.D)) % server.N

    # 5.
    encoded_s = server.receive("alice", encoded_h)

    # 6.
    s = encoded_s * crypto.inverse(r, server.N) % server.N

    # 7.
    server.receive_s("alice", (n, s))

    print(server.database)


if __name__ == "__main__":
    main()
