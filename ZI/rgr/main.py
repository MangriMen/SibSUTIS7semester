from socket import socket
from fiat_shamir import Server, Client


def main():
    path_to_db = "Fiat-Shamir.sqlite"

    alice = Client(path_to_db, "Alice", "localhost", 35535)
    bob = Client(path_to_db, "Bob", "localhost", 35535)

    print("Alice " + "Сonnected successfully" if alice.connect(True)
          else "Error when login")
    print("Bob " + "Сonnected successfully" if bob.connect(True)
          else "Error when login")


if __name__ == "__main__":
    main()
