#include <iostream>
#include <pthread.h>
#include <sys/neutrino.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>

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

void *send_msg(void *arg)
{
    while (true)
    {
        int tries = *((int *)arg);
        while (tries--)
        {
            MsgSend(Connection::benchmark, NULL, NULL, NULL, NULL);
        }
    }

    return (void *)0;
}

int main()
{
    const int TRIES = 1e6;

    Channel::benchmark = ChannelCreate(0);

    int fd = open("./mp_test", O_CREAT | O_RDWR, 0666);
    write(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    write(fd, &TRIES, sizeof(TRIES));
    close(fd);

    Connection::benchmark = ConnectAttach(0, 0, Channel::benchmark, 0, 0);
    pthread_t thread;

    pthread_create(&thread, NULL, send_msg, (void *)&TRIES);

    pthread_join(thread, NULL);

    pthread_cancel(thread);

    return 0;
}