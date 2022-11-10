#include <iostream>
#include <cstdlib>
#include <cstdio>
#include <ctime>
#include <cmath>
#include <pthread.h>
#include <unistd.h>
#include <vingraph.h>
#include "/root/labs/plates.h"
#include "utils.hpp"

using namespace std;

struct target_data {
    int loc1;
    int loc2;
    timespec start_time;
    timespec end_time;
    int height;
};

void* calcAndShootTarget(void* arg)
{
    target_data& data = *(target_data*)arg;

    int distance_between_locators = abs(data.loc2 - data.loc1);
    long long usec = diffTimeAsUSec(data.start_time, data.end_time);

    double target_speed = distance_between_locators / static_cast<double>(usec);

    double time_to_target = abs(MISSLE_SILO_X - data.loc2) / target_speed;
    double time_to_climb
        = pointsToUSec(MISSLE_SILO_Y - data.height, MISSLE_SPEED);

    long long fire_delay = time_to_target - time_to_climb;

    cout << fire_delay / USEC_IN_SEC << endl;

    timespec fire_time = data.end_time;
    fire_time.tv_sec += fire_delay / USEC_IN_SEC;
    fire_time.tv_nsec += (fire_delay % USEC_IN_SEC) * 1000;
    fire_time = normalizeTimespec(fire_time);

    clock_nanosleep(CLOCK_REALTIME, TIMER_ABSTIME, &fire_time, 0);
    putreg(RG_GUNS, GUNS_SHOOT);
}

void* leftSideHandler(void* arg)
{
    timespec start;
    timespec end;

    bool isSkip = false;

    while (true) {
        if (getreg(RG_LOCN) == 1 && getreg(RG_LOCW) == 3) {
            clock_gettime(CLOCK_REALTIME, &start);
            while (true) {
                usleep(1);
                clock_gettime(CLOCK_REALTIME, &end);
                long long usec = diffTimeAsUSec(start, end);
                if (usec > 2000000) {
                    cout << "Skip misdetection on left side" << endl;
                    isSkip = true;
                    break;
                }
                if (getreg(RG_LOCN) == 2 && getreg(RG_LOCW) == 3) {
                    clock_gettime(CLOCK_REALTIME, &end);
                    break;
                }
            }

            if (isSkip) {
                isSkip = false;
                continue;
            }

            target_data data;
            data.start_time = start;
            data.end_time = end;
            data.loc1 = XL1;
            data.loc2 = XL2;
            data.height = getreg(RG_LOCY);

            pthread_create(NULL, NULL, &calcAndShootTarget, (void*)&data);
        }
        usleep(1);
    }
}

void* rightSideHandler(void* arg)
{
    timespec start;
    timespec end;

    bool isSkip = false;

    while (true) {
        if (getreg(RG_LOCN) == 4 && getreg(RG_LOCW) == 3) {
            clock_gettime(CLOCK_REALTIME, &start);
            while (true) {
                usleep(1);
                clock_gettime(CLOCK_REALTIME, &end);
                long long usec = diffTimeAsUSec(start, end);
                if (usec > 2000000) {
                    cout << "Skip misdetection on right side" << endl;
                    isSkip = true;
                    break;
                }
                if (getreg(RG_LOCN) == 3 && getreg(RG_LOCW) == 3) {
                    clock_gettime(CLOCK_REALTIME, &end);
                    break;
                }
            }

            if (isSkip) {
                isSkip = false;
                continue;
            }

            target_data data;
            data.start_time = start;
            data.end_time = end;
            data.loc1 = XL4;
            data.loc2 = XL3;
            data.height = getreg(RG_LOCY);

            pthread_create(NULL, NULL, &calcAndShootTarget, (void*)&data);
        }
        usleep(1);
    }
}

int main()
{
    cout << endl;

    StartGame(2);

    pthread_t left_side;
    pthread_t right_side;

    pthread_create(&left_side, NULL, &leftSideHandler, NULL);
    pthread_create(&right_side, NULL, &rightSideHandler, NULL);

    pthread_join(left_side, NULL);
    pthread_join(right_side, NULL);

    EndGame();
    return 0;
}