from contextlib import closing
from email import message
import json
import secrets
import socket
from threading import Thread
import threading
from types import SimpleNamespace
import crypto
import sqlite3


class Server:
    _p: int
    _q: int
    n: int

    clients: dict
    db_connection: sqlite3.Connection
    server_socket: socket.socket

    server_socket_thread: Thread

    client_connections: list[threading.Thread]

    def __init__(self, ip: str, port: int, path_to_db: str) -> None:
        self._p = 0
        self._q = 0
        self.n = 0

        self.clients = dict()
        self.client_connections = []

        self._generate_parameters()
        self._init_connection_to_DB(path_to_db)
        self._init_server_socket(ip, port)

    def __del__(self):
        self.db_connection.close()
        self.server_socket.close()
        self.server_socket_thread.join()

    def _init_connection_to_DB(self, path_to_db) -> None:
        self.db_connection = sqlite3.connect(
            path_to_db, check_same_thread=False)
        with closing(self.db_connection.cursor()) as cur:
            cur.execute("DROP TABLE IF EXISTS Users;")
            cur.execute(
                "CREATE TABLE IF NOT EXISTS Users(name CHAR(20) UNIQUE, public_key INT);")

    def _init_server_socket(self, ip, port):
        self.server_socket = socket.socket()
        self.server_socket.bind((ip, port))
        self.server_socket.listen(2)

        self.server_socket_thread = threading.Thread(
            target=self._handle_connections, daemon=True)
        self.server_socket_thread.start()

        print(f'Server started on {ip}:{port}')

    def _handle_connections(self):
        while True:
            conn, address = self.server_socket.accept()

            self.client_connections.append(threading.Thread(
                target=self._handle_user, args=(conn, address)))
            self.client_connections[-1].start()

    def _handle_user(self, conn: socket.socket, address):
        tries_count = 20
        conn.send(json.dumps({"data": self.n}).encode('utf-8'))

        while True:
            if tries_count == -1:
                conn.send(json.dumps({'data': "logged"}).encode('utf-8'))

                data = conn.recv(4096).decode('utf-8')
                data = conn.recv(4096).decode('utf-8')
                if not data:
                    break
                data_json = json.loads(data)

                print(data_json)

                if data_json['data'] == "ok":
                    break

            data = conn.recv(4096).decode('utf-8')
            if not data:
                break
            data_json = json.loads(data)

            self.set_user_x(data_json['name'], data_json['data'])
            message = {"data": self.set_user_e_and_get_it(data_json['name'])}
            conn.send(json.dumps(message).encode('utf-8'))

            data = conn.recv(4096).decode('utf-8')
            if not data:
                break
            data_json = json.loads(data)

            message = {"data": self.validate_user_try(
                data_json['name'], data_json['data'])}
            conn.send(json.dumps(message).encode('utf-8'))

            tries_count -= 1

    def _generate_parameters(self) -> None:
        self._p = crypto.getPrimeInBounds(1000, 1e9)
        self._q = crypto.getPrimeInBounds(1000, 1e9)
        self.n = self._p * self._q

        with open('server_public_n.txt', 'w') as file_out:
            file_out.write(str(self.n))

    def _get_user(self, name: str) -> SimpleNamespace:
        if name not in self.clients:
            self.clients[name] = SimpleNamespace()
        return self.clients[name]

    def set_user_x(self, name: str, x: int):
        user = self._get_user(name)
        user.x = x

    def set_user_e_and_get_it(self, name: str):
        user = self._get_user(name)
        user.e = secrets.randbelow(2)
        return user.e

    def validate_user_try(self, name: str, answer: int):
        user = self._get_user(name)

        with closing(self.db_connection.cursor()) as cur:
            cur.execute(
                'SELECT public_key FROM Users WHERE name = ?;', [name])
            user_public_key = cur.fetchone()[0]

        user_y2 = (pow(answer, 2) % self.n)
        calc_y2 = (user.x * pow(user_public_key, user.e)) % self.n

        return user_y2 == calc_y2


class Client:
    s: int
    V: int

    n: int
    r: int

    tries_count: int

    name: str
    db_connection: sqlite3.Connection

    def __init__(self, path_to_db: str, name: str, ip: str, port: int) -> None:
        self.name = name
        self.n = 0

        self._init_client_socket(ip, port)
        self.n = self._get_n_from_server()

        self._generateParameters()
        self._init_connection_to_DB(path_to_db)
        self._writePublicKeyToDB()

    def _init_connection_to_DB(self, path_to_db) -> None:
        self.db_connection = sqlite3.connect(path_to_db)
        with closing(self.db_connection.cursor()) as cur:
            cur.execute(
                "CREATE TABLE IF NOT EXISTS Users(name CHAR(20) UNIQUE, public_key INT);")

    def _writePublicKeyToDB(self):
        with closing(self.db_connection.cursor()) as cur:
            cur.executemany(
                f"INSERT INTO Users VALUES(?, ?);", [(self.name, self.V)])
            self.db_connection.commit()

    def _init_client_socket(self, ip: str, port: int):
        self.client_socket = socket.socket()
        self.client_socket.connect((ip, port))

    def _get_n_from_server(self):
        return json.loads(self.client_socket.recv(4096))['data']

    def _generateParameters(self):
        while True:
            self.s = crypto.getPrimeInBounds(1000, 1e9)
            if crypto.extendedGCD(self.s, self.n)[0] == 1:
                break
        self.V = pow(self.s, 2) % self.n

    def connect(self, verbose=False):
        tries = 0
        isValid = True
        isConnected = False

        while isValid:
            message = {"name": self.name, "data": self.generate_r()}
            self.client_socket.send(json.dumps(message).encode('utf-8'))

            data = self.client_socket.recv(4096)
            if not data:
                return False
            data_json = json.loads(data)

            if data_json['data'] == None:
                break
            elif data_json['data'] == "logged":
                isConnected = True
                self.client_socket.send(json.dumps(
                    {'data': 'ok'}).encode('utf-8'))
                break

            e = data_json['data']
            message = {"name": self.name, "data": self.generate_y(e)}
            self.client_socket.send(json.dumps(message).encode('utf-8'))

            data = self.client_socket.recv(4096)
            if not data:
                return False
            data_json = json.loads(data)

            if not data_json['data']:
                return False

            if verbose:
                print(f'Try #{tries} - {"success" if isValid else "error"}')

            tries += 1

        self.client_socket.close()

        return isConnected

    def generate_r(self):
        self.r = crypto.getPrimeInBounds(1, self.n - 1)
        return pow(self.r, 2) % self.n

    def generate_y(self, e: int):
        self.y = self.r if e == 0 else (self.r * self.s) % self.n
        return self.y
