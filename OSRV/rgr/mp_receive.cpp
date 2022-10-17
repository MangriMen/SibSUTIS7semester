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

using namespace std;
using namespace chrono;

struct Channel {
    static int benchmark;
};
int Channel::benchmark = 0;

void* receive_msg(void* arg)
{
    while (true) {
        int tries = *((int*)arg) + 1;
        while (tries--) {
            int msg_id = MsgReceive(Channel::benchmark, NULL, NULL, NULL);
            MsgReply(msg_id, NULL, NULL, NULL);
        }
    }
    return (void*)0;
}

int main()
{
    int pid = getpid();
    Channel::benchmark = ChannelCreate(0);
    int tries = 1e6;

    int fd = open("/dev/shmem/mp_test", O_CREAT | O_RDWR, 0666);
    write(fd, &pid, sizeof(pid));
    write(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    write(fd, &tries, sizeof(tries));
    close(fd);

    pthread_t thread;
    pthread_create(&thread, NULL, receive_msg, (void*)&tries);
    pthread_join(thread, NULL);
    pthread_cancel(thread);

    return 0;
}