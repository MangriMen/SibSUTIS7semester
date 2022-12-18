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

const int TRIES = 1e6;
int64_t *begin_times = NULL;

void *receive_msg(void *arg)
{
    for (int64_t i = 0; i < TRIES; ++i)
    {
        int msg_id = MsgReceive(Channel::benchmark, NULL, NULL, NULL);

        begin_times[i] = ClockCycles();
        MsgReply(msg_id, NULL, NULL, NULL);
    }

    return (void *)0;
}

int main()
{
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

    begin_times = (std::int64_t *)mmap(0, TRIES * sizeof(int64_t), PROT_WRITE | PROT_READ, MAP_SHARED, shm_fd, 0);
    close(shm_fd);

    if (begin_times == MAP_FAILED)
    {
        std::cout << strerror(errno) << std::endl;
        return -1;
    }

    pid_t PID = getpid();
    Channel::benchmark = ChannelCreate(0);

    int fd = open("./mp_test", O_CREAT | O_RDWR);
    write(fd, &PID, sizeof(PID));
    write(fd, &Channel::benchmark, sizeof(Channel::benchmark));
    close(fd);

    pthread_t thread;
    pthread_create(&thread, NULL, receive_msg, NULL);

    pthread_join(thread, NULL);

    pthread_cancel(thread);

    return 0;
}