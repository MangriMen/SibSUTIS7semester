#include <iostream>
#include <vingraph.h>
#include <pthread.h>
#include <chrono>
#include <sys/neutrino.h>
#include <sys/netmgr.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>

using namespace std;
using namespace chrono;

struct Channel {
    static int benchmark;
};
int Channel::benchmark = 0;

struct Connection {
    static int benchmark;
};
int Connection::benchmark = 0;

struct Timer {
    static high_resolution_clock::time_point begin_time;
    static high_resolution_clock::time_point end_time;
};
high_resolution_clock::time_point Timer::begin_time
    = high_resolution_clock::now();
high_resolution_clock::time_point Timer::end_time
    = high_resolution_clock::now();

void* send_msg(void* arg)
{
    int tries = *((int*)arg);
    while (tries--) {
        MsgSend(Connection::benchmark, NULL, NULL, NULL, NULL);
    }
    return (void*)0;
}

int main()
{
    int pid = 0;
    int tries = 0;

    int fd = open("/net/EA3bf47/dev/shmem/mp_test", O_RDWR);
    read(fd, &pid, sizeof(pid));
    read(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    read(fd, &tries, sizeof(tries));
    close(fd);

    int nd = netmgr_strtond("EA3bf47", 0);
    Connection::benchmark = ConnectAttach(nd, pid, Channel::benchmark, 0, 0);

    pthread_t thread;
    pthread_create(&thread, NULL, send_msg, (void*)&tries);

    Timer::begin_time = high_resolution_clock::now();
    pthread_join(thread, NULL);
    Timer::end_time = high_resolution_clock::now();

    cout << (duration_cast<microseconds>(Timer::end_time - Timer::begin_time)
                 .count()
        / (double)tries)
         << " microseconds" << endl;

    pthread_cancel(thread);

    return 0;
}