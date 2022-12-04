#include <iostream>
#include <vingraph.h>
#include <pthread.h>
#include <chrono>
#include <string.h>
#include <sys/neutrino.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <errno.h>
#include <stdio.h>

struct Channel
{
    static int benchmark;
};
int Channel::benchmark = 0;

struct Timer
{
    static int64_t result;
};
int64_t Timer::result = 0;

void *receive_msg(void *arg)
{
    int64_t begin_time = 0;
    int64_t end_time = 0;

    int tries = *((int *)arg);
    while (tries--)
    {
        begin_time = ClockCycles();
        int msg_id = MsgReceive(Channel::benchmark, NULL, NULL, NULL);
        end_time = ClockCycles();

        Timer::result += end_time - begin_time;

        MsgReply(msg_id, NULL, NULL, NULL);
    }
    return (void *)0;
}

int main()
{
    const int64_t CPU_FREQ = 4170000000ul;
    int tries = 0;

    int fd = open("./mp_test", O_RDWR);
    read(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    read(fd, &tries, sizeof(tries));
    close(fd);

    pthread_t thread;
    pthread_create(&thread, NULL, receive_msg, (void *)&tries);

    pthread_join(thread, NULL);

    pthread_cancel(thread);

    std::cout << (Timer::result / (long double)tries) / (long double)CPU_FREQ << " seconds" << std::endl;

    return 0;
}