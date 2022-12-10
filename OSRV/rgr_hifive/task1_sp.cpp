#include <iostream>
#include <pthread.h>
#include <sys/neutrino.h>
#include <unistd.h>

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

void *send_msg(void *arg)
{
    int tries = *((int *)arg);
    while (tries--)
    {
        MsgSend(Connection::benchmark, NULL, NULL, NULL, NULL);
    }

    return (void *)0;
}

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
    const int TRIES = 1e6;
    const int64_t CPU_FREQ = 4170000000ul;

    Channel::benchmark = ChannelCreate(0);
    Connection::benchmark = ConnectAttach(0, 0, Channel::benchmark, 0, 0);
    pthread_t thread_1;
    pthread_t thread_2;

    pthread_create(&thread_1, NULL, send_msg, (void *)&TRIES);
    pthread_create(&thread_2, NULL, receive_msg, (void *)&TRIES);

    pthread_join(thread_1, NULL);
    pthread_join(thread_2, NULL);

    pthread_cancel(thread_1);
    pthread_cancel(thread_2);

    std::cout << (Timer::result / (long double)TRIES) / (long double)CPU_FREQ << " seconds" << std::endl;

    return 0;
}