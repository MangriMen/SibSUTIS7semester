#include <iostream>
#include <cstdlib>
#include <cstring>
#include <sys/mman.h>

int main()
{
    const int ARR_LENGTH = 5;

    // 4096 is a minimal value
    int *test1 = (int *)mmap(0, sizeof(int) * ARR_LENGTH, PROT_WRITE | PROT_READ, MAP_SHARED | MAP_ANON, -1, 0);

    for (int i = 0; i < ARR_LENGTH; i++)
    {
        test1[i] = i;
    }

    for (int i = 0; i < ARR_LENGTH; i++)
    {
        std::cout << test1[i] << std::endl;
    }

    // PROT_WRITE write + read
    // PROT_READ read
    // PROT_EXEC mark as executable memory (does nothing apperently)

    int *test2 = (int *)mmap(0, 4096, PROT_WRITE | PROT_EXEC, MAP_SHARED | MAP_ANON, -1, 0);
    memset(test2, 0xc3, 4096);
    ((void (*)(void))test2)();

    std::cout << "test2 executed correctly" << std::endl;

    try
    {
        int *test3 = (int *)mmap(0, 4096, PROT_WRITE, MAP_SHARED | MAP_ANON, -1, 0);
        memset(test3, 0xc3, 4096);
        ((void (*)(void))test3)();
    }
    catch (...)
    {
        std::cout << "test3 failed" << std::endl;
    }

    return 0;
}
