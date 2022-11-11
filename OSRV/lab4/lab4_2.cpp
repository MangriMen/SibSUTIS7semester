#include <iostream>
#include <cstdio>
#include <ctime>
#include <unistd.h>
#include <vingraph.h>
#include "/root/labs/plates.h"
#include "utils.hpp"

using namespace std;

int main()
{
    StartGame(1);

    timespec start;
    timespec end;
    timespec fire_time;

    while (true) {
        usleep(1);
        if (getreg(RG_LOCN) == 1 && getreg(RG_LOCW) == 3) {
            clock_gettime(CLOCK_REALTIME, &start);
            while (true) {
                usleep(1);
                if (getreg(RG_LOCN) == 2 && getreg(RG_LOCW) == 3) {
                    clock_gettime(CLOCK_REALTIME, &end);
                    break;
                }
            }

            int distance_between_locators = XL2 - XL1;
            long long usec = diffTimeAsUSec(start, end);

            double target_speed
                = distance_between_locators / static_cast<double>(usec);

            double time_to_target = (MISSLE_SILO_X - XL2) / target_speed;
            double time_to_climb
                = pointsToUSec(MISSLE_SILO_Y - getreg(RG_LOCY), MISSLE_SPEED);

            long long fire_delay = time_to_target - time_to_climb;

            fire_time = end;
            fire_time.tv_sec += fire_delay / USEC_IN_SEC;
            fire_time.tv_nsec += (fire_delay % USEC_IN_SEC) * 1000;
            fire_time = normalizeTimespec(fire_time);
            clock_nanosleep(CLOCK_REALTIME, TIMER_ABSTIME, &fire_time, 0);

            putreg(RG_GUNS, GUNS_SHOOT);
        }
    }

    EndGame();
    return 0;
}