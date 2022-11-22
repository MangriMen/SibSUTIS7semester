#include <iostream>
#include <fcntl.h>
#include <sys/mman.h>
#include <cstring>
#include <bitset>
#include <chrono>

using namespace std;

int main()
{
    int fd = open("./angel-mech.jpg", O_RDONLY);

    size_t n = 4194304ull;
    void* map = mmap(NULL, n, PROT_READ, MAP_PRIVATE, fd, NULL);

    if (map == MAP_FAILED) {
        cerr << "FAILED TO MAP FILE" << endl;
        return 1;
    }

    volatile char* ptr = (volatile char*)map;
    int tries = 1e6;

    auto start = chrono::high_resolution_clock::now();
    while (tries--) {
        ptr[0];
    }
    auto stop = chrono::high_resolution_clock::now();

    cout << chrono::duration_cast<chrono::microseconds>(stop - start).count()
         << endl;

    return 0;
}