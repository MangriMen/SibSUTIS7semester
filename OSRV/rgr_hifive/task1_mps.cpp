#include <iostream>
#include <pthread.h>
#include <sys/neutrino.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/mman.h>
#include <errno.h>
#include <string.h>

struct Channel
{
    static int benchmark;
};
int Channel::benchmark = 0;
struct Connection
{
    static int benchmark;
};
int Connection::benchmark = 0;
struct Timer
{
    static int64_t result;
};
int64_t Timer::result = 0;

const int TRIES = 1e6;
int64_t end_times[TRIES];

void *send_msg(void *arg)
{
    for (int64_t i = 0; i < TRIES; ++i)
    {
        MsgSend(Connection::benchmark, NULL, NULL, NULL, NULL);
        end_times[i] = ClockCycles();
    }

    return (void *)0;
}

int main()
{
    const int64_t CPU_FREQ = 4170000000ul;

    int shm_fd = shm_open("/myregion", O_CREAT | O_RDWR, S_IRUSR | S_IWUSR);
    if (shm_fd == -1)
    {
        std::cout << strerror(errno) << std::endl;
        return -1;
    }

    if (ftruncate(shm_fd, TRIES * sizeof(int64_t)) == -1)
    {
        std::cout << strerror(errno) << std::endl;
        return -1;
    }

    int64_t *begin_times = (std::int64_t *)mmap(0, TRIES * sizeof(int64_t), PROT_WRITE | PROT_READ, MAP_SHARED, shm_fd, 0);
    close(shm_fd);

    if (begin_times == MAP_FAILED)
    {
        std::cout << strerror(errno) << std::endl;
        return -1;
    }

    pid_t PID = 0;

    int fd = open("./mp_test", O_RDWR, 0666);
    read(fd, &PID, sizeof(PID));
    read(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    close(fd);

    Connection::benchmark = ConnectAttach(0, PID, Channel::benchmark, 0, 0);
    pthread_t thread;

    pthread_create(&thread, NULL, send_msg, NULL);

    pthread_join(thread, NULL);

    pthread_cancel(thread);

    for (int64_t i = 0; i < TRIES; ++i)
    {
        Timer::result += end_times[i] - begin_times[i];
    }

    std::cout << (Timer::result / (long double)TRIES) / (long double)CPU_FREQ << " seconds" << std::endl;

    return 0;
}