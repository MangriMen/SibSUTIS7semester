#include <iostream>
#include <vingraph.h>
#include <pthread.h>
#include <chrono>
#include <sys/neutrino.h>
#include <unistd.h>

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

void* receive_msg(void* arg)
{
    int tries = *((int*)arg);
    while (tries--) {
        int msg_id = MsgReceive(Channel::benchmark, NULL, NULL, NULL);
        MsgReply(msg_id, NULL, NULL, NULL);
    }
    return (void*)0;
}

int main()
{
    Channel::benchmark = ChannelCreate(0);
    Connection::benchmark = ConnectAttach(0, 0, Channel::benchmark, 0, 0);

    int tries = 1e6;

    pthread_t thread_1;
    pthread_t thread_2;
    pthread_create(&thread_2, NULL, receive_msg, (void*)&tries);
    pthread_create(&thread_1, NULL, send_msg, (void*)&tries);

    Timer::begin_time = high_resolution_clock::now();
    pthread_join(thread_1, NULL);
    pthread_join(thread_2, NULL);
    Timer::end_time = high_resolution_clock::now();

    cout << duration_cast<microseconds>(Timer::end_time - Timer::begin_time)
                .count()
            / (double)tries
         << " microseconds" << endl;

    pthread_cancel(thread_1);
    pthread_cancel(thread_2);

    return 0;
}