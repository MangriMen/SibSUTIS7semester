import signal
import sys
from fiat_shamir import Server


def signal_handler(sig, frame):
    print('Exiting...')
    sys.exit(0)


def main():
    signal.signal(signal.SIGINT, signal_handler)

    path_to_db = "Fiat-Shamir.sqlite"
    Server("localhost", 35535, path_to_db)

    while True:
        pass


if __name__ == "__main__":
    main()
