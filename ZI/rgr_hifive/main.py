import secrets
import random
from typing import Dict, List
import crypto
import pprint


class Client:
    _p: int
    _q: int
    n: int

    def __init__(self, file_name: str) -> None:
        self.graph_data = read_file(file_name)

    def _get_random_colors_transposition(self):
        shuffled_colors = list(set(
            self.graph_data["vertices_colors"].values()))
        random.shuffle(shuffled_colors)

        return {
            key: shuffled_colors[value % len(shuffled_colors)]
            for key, value in self.graph_data["vertices_colors"].items()
        }

    def _get_prime_for_vertex(self, vertices_colors):
        salted = []
        for vertex in range(1, self.graph_data["vertices_count"] + 1):
            r = crypto.getPrimeInBounds(2147483648, 4294967296)

            color = vertices_colors[vertex]

            bin_list = list(bin(r))
            bin_list[-1] = bin_list[-2] = "0"
            new_r = int(''.join(bin_list), 2) | (color - 1)

            salted.append(new_r)

        return salted

    def _get_vertexes_keys(self):
        return [
            getRsa() for _ in range(1, self.graph_data["vertices_count"] + 1)
        ]

    def _get_encoded_vertexes_color(self, prime_vertexes, vertexes_keys):
        return [
            pow(r, key[2], key[0])
            for r, key in zip(prime_vertexes, vertexes_keys)
        ]

    def _get_edge_from_server(self, edge, vertexes_keys):
        return (vertexes_keys[edge[0] - 1][1], vertexes_keys[edge[1] - 1][1])


class Server:

    Nv: List[int]
    dv: List[int]
    Zv: List[int]

    def __init__(self, file_name) -> None:
        self.Nv = []
        self.dv = []
        self.Zv = []
        self.graph_data = read_file(file_name)

    def _get_data_from_client(self, Nv, dv, Zv):
        self.Nv = Nv
        self.dv = dv
        self.Zv = Zv

        return self._get_random_edge()

    def _get_random_edge(self):
        i = -1
        j = -1
        while True:
            i = secrets.randbelow(self.graph_data["vertices_count"] + 1)

            while (j := secrets.randbelow(self.graph_data["vertices_count"]) +
                   1) == i:
                pass

            if i in self.graph_data["graph"] and j in self.graph_data["graph"]:
                if self.graph_data["graph"][i][j] == 1:
                    break

        self.i, self.j = i, j
        return (i, j)

    def _get_vertexes_from_client(self, cv):
        Zv1 = (pow(self.Zv[self.i - 1], cv[0], self.Nv[self.i - 1]))
        Zv2 = (pow(self.Zv[self.j - 1], cv[1], self.Nv[self.j - 1]))

        Zv1_color = Zv1 & 0b11
        Zv2_color = Zv2 & 0b11

        if (Zv1_color) == (Zv2_color):
            return -1

        return 0


def read_file(file_name):
    vertices_count: int = 0
    edges_count: int = 0
    graph: Dict[int, Dict[int, int]] = {}
    vertices_colors: Dict[int, int] = {}

    with open(file_name, 'r') as file:
        raw_data = file.readlines()

        vertices_count = int(raw_data[0][0])
        edges_count = int(raw_data[0][2])

        for i in range(1, vertices_count + 1):
            graph[i] = {}
            for j in range(1, vertices_count + 1):
                graph[i][j] = 0

        for edge in range(2, edges_count + 2):
            v1 = int(raw_data[edge][0])
            v2 = int(raw_data[edge][2])

            graph[v1][v2] = 1
            graph[v2][v1] = 1

        for color in range(3 + edges_count, 3 + edges_count + vertices_count):
            vertices_colors[int(raw_data[color][0])] = int(raw_data[color][2])

    return {
        "vertices_count": vertices_count,
        "edges_count": edges_count,
        "graph": graph,
        "vertices_colors": vertices_colors
    }


def getRsa():
    P = crypto.getPrimeInBounds(1 << 255, (1 << 256) - 1)
    Q = crypto.getPrimeInBounds(1 << 255, (1 << 256) - 1)

    N = P * Q
    phi = (P - 1) * (Q - 1)

    d = crypto.getCoprime(phi)

    gcd, c, _ = crypto.extendedGCD(d, phi)
    assert gcd == 1
    while c < 0:
        c += phi

    return (N, c, d)


def main():
    graph_file = "graph2.txt"

    alice = Client(graph_file)
    bob = Server(graph_file)

    tries = 15

    for current_try in range(tries):
        # 1
        transposed_colors = alice._get_random_colors_transposition()
        # print(f"{transposed_colors=}")

        # 2
        rv = alice._get_prime_for_vertex(transposed_colors)

        # print("rv=")
        # for color in rv:
        #     print(bin(color))

        # 3
        vertex_keys = alice._get_vertexes_keys()
        # print("vertex_keys")
        # pprint.pprint(vertex_keys)

        # 4
        Zv = alice._get_encoded_vertexes_color(rv, vertex_keys)
        # print("Zv")
        # for encoded_color in Zv:
        #     print(encoded_color)

        Nv, _, dv = zip(*vertex_keys)

        edge = bob._get_data_from_client(Nv, dv, Zv)
        # print(f"{edge=}")

        cv = alice._get_edge_from_server(edge, vertex_keys)
        # print(f"{cv=}")

        if bob._get_vertexes_from_client(cv) == -1:
            print("Alice is sus")
            break

        print(f"Try #{current_try + 1} passed!")

    if tries == 0:
        print("Alice was not sus")


if __name__ == '__main__':
    main()
