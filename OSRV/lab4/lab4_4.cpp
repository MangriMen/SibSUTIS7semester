#include <iostream>
#include <array>
#include <vector>
#include <set>
#include <cstdlib>
#include <cstdio>
#include <ctime>
#include <cmath>
#include <algorithm>
#include <pthread.h>
#include <unistd.h>
#include <vingraph.h>
#include "/root/labs/plates.h"
#include <sys/mman.h>
#include "utils.hpp"

using namespace std;

struct loc_group_data {
    int loc1_id;
    int loc1_x;
    int loc2_id;
    int loc2_x;

    loc_group_data(int loc1_id, int loc1_x, int loc2_id, int loc2_x)
        : loc1_id(loc1_id)
        , loc1_x(loc1_x)
        , loc2_id(loc2_id)
        , loc2_x(loc2_x)
    {
    }
};

struct target_data {
    int loc1;
    int loc2;
    timespec start_time;
    timespec end_time;
    int height;

    target_data(
        int loc1, int loc2, timespec start_time, timespec end_time, int height)
        : loc1(loc1)
        , loc2(loc2)
        , start_time(start_time)
        , end_time(end_time)
        , height(height)

    {
    }
};

struct rcm_target_data {
    int rcm_id;
    int x;
    int y;
    double speed;

    rcm_target_data(int rcm_id, int x, int y, double speed)
        : rcm_id(rcm_id)
        , x(x)
        , y(y)
        , speed(speed)
    {
    }
};

const int UFO_WIDTH = 3;
int current_rcm = 0;

void* processRCM(void* arg)
{
    rcm_target_data* data = (rcm_target_data*)arg;

    timespec time_to_turn;
    clock_gettime(CLOCK_REALTIME, &time_to_turn);

    int speed_vector = (MISSLE_SILO_X - data->x > 0 ? 1 : -1);

    long long time_to_climb = pointsToUSec(MISSLE_SILO_Y - data->y, RCM_SPEED);
    time_to_turn = addTimespec(time_to_turn, uSecToTimespec(time_to_climb));

    double target_position_after_climb
        = data->x + (time_to_climb * data->speed * speed_vector);
    int direction
        = target_position_after_climb < MISSLE_SILO_X ? RCMC_LEFT : RCMC_RIGHT;

    putreg(RG_RCMN, data->rcm_id);
    putreg(RG_RCMC, RCMC_START);
    clock_nanosleep(CLOCK_REALTIME, TIMER_ABSTIME, &time_to_turn, NULL);

    putreg(RG_RCMN, data->rcm_id);
    putreg(RG_RCMC, direction);

    delete data;
    data = NULL;
    arg = NULL;
}

void* calcAndShootTarget(void* arg)
{
    target_data& data = *(target_data*)arg;

    int distance_between_locators = abs(data.loc2 - data.loc1);
    long long usec = diffTimeAsUSec(data.start_time, data.end_time);

    double target_speed = distance_between_locators / static_cast<double>(usec);

    double time_to_climb
        = pointsToUSec(MISSLE_SILO_Y - data.height, MISSLE_SPEED);
    double time_to_target = abs(MISSLE_SILO_X - data.loc2) / target_speed;

    long long fire_delay = time_to_target - time_to_climb;

    if (fire_delay < 0) {
        if (current_rcm < 11) {
            auto rcm_data = new rcm_target_data(
                current_rcm, data.loc2, data.height, target_speed);
            pthread_create(NULL, NULL, processRCM, (void*)rcm_data);
            current_rcm++;
        } else {
            cout << "RCM ran out\n" << flush;
        }
        return NULL;
    }

    timespec fire_time = data.end_time;
    fire_time = addTimespec(fire_time, uSecToTimespec(fire_delay));
    clock_nanosleep(CLOCK_REALTIME, TIMER_ABSTIME, &fire_time, NULL);

    putreg(RG_GUNS, GUNS_SHOOT);
}

void* locGroupHandler(void* arg)
{
    loc_group_data& data = *(loc_group_data*)arg;

    set<int> ufo_heights;
    timespec start;
    timespec end;
    while (true) {
        if (getreg(RG_LOCN) == data.loc1_id && getreg(RG_LOCW) == UFO_WIDTH) {
            if (ufo_heights.find(getreg(RG_LOCY)) == ufo_heights.end()) {
                clock_gettime(CLOCK_REALTIME, &start);
                ufo_heights.insert(getreg(RG_LOCY));
            }
        }
        if (getreg(RG_LOCN) == data.loc2_id && getreg(RG_LOCW) == UFO_WIDTH) {
            if (ufo_heights.find(getreg(RG_LOCY)) != ufo_heights.end()) {
                clock_gettime(CLOCK_REALTIME, &end);
                target_data target(
                    data.loc1_x, data.loc2_x, start, end, getreg(RG_LOCY));
                pthread_create(NULL, NULL, &calcAndShootTarget, (void*)&target);
                ufo_heights.erase(getreg(RG_LOCY));
            }
        }
        usleep(1);
    }
}

int main()
{
    StartGame(2);

    array<loc_group_data, 2> groups { loc_group_data(1, XL1, 2, XL2),
        loc_group_data(4, XL4, 3, XL3) };
    array<pthread_t, 2> threads;

    for (size_t i = 0; i < groups.size(); i++) {
        pthread_create(&threads[i], NULL, locGroupHandler, (void*)&groups[i]);
    }
    for (size_t i = 0; i < groups.size(); i++) {
        pthread_join(threads[i], NULL);
    }

    EndGame();
    return 0;
}