import sys


with open('data.dat', 'wb') as file:
    file.write(int(1).to_bytes(
        1, byteorder=sys.byteorder))
