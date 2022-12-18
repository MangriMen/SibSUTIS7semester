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

const int TRIES = 1e6;

int64_t begin_times[TRIES];
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

void *receive_msg(void *arg)
{
    for (int64_t i = 0; i < TRIES; ++i)
    {
        int msg_id = MsgReceive(Channel::benchmark, NULL, NULL, NULL);

        begin_times[i] = ClockCycles();
        const int64_t CPU_FREQ = 4170000000ul;

        Channel::benchmark = ChannelCreate(0);
        Connection::benchmark = ConnectAttach(0, 0, Channel::benchmark, 0, 0);
        pthread_t thread_1;
        pthread_t thread_2;

        pthread_create(&thread_1, NULL, send_msg, NULL);
        pthread_create(&thread_2, NULL, receive_msg, NULL);

        pthread_join(thread_1, NULL);
        pthread_join(thread_2, NULL);

        pthread_cancel(thread_1);
        pthread_cancel(thread_2);

        for (int64_t i = 0; i < TRIES; ++i)
        {
            Timer::result += end_times[i] - begin_times[i];
        }

        std::cout << (Timer::result / (long double)TRIES) / (long double)CPU_FREQ << " seconds" << std::endl;

        return 0;
    }