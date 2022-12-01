#pragma once

const int W_WIDTH = 800;
const int W_HEIGHT = 600;

const double MISSLE_SILO_X = 400;
const double MISSLE_SILO_Y = 570;

const double MISSLE_SPEED = 100;
const double RCM_SPEED = 250;

const int USEC_IN_SEC = 1000000;

long long pointsToUSec(int points, double speed);
long long diffTimeAsUSec(const timespec& start, const timespec& end);
timespec normalizeTimespec(timespec ts);
timespec uSecToTimespec(long long useconds);
timespec addTimespec(const timespec& a, const timespec& b);

long long pointsToUSec(int points, double speed)
{
    return static_cast<long long>((points / speed) * USEC_IN_SEC);
}

long long diffTimeAsUSec(const timespec& start, const timespec& end)
{
    timespec diff = end;
    diff.tv_sec -= start.tv_sec;
    diff.tv_nsec -= start.tv_nsec;
    diff = normalizeTimespec(diff);
    return diff.tv_sec * USEC_IN_SEC + diff.tv_nsec / 1000;
}

timespec normalizeTimespec(timespec ts)
{
    while (ts.tv_nsec < 0) {
        ts.tv_nsec += 1e9;
        ts.tv_sec--;
    }
    while (ts.tv_nsec >= 1e9) {
        ts.tv_sec++;
        ts.tv_nsec -= 1e9;
    }
    return ts;
}

timespec uSecToTimespec(long long useconds)
{
    timespec ts;
    ts.tv_sec += useconds / USEC_IN_SEC;
    ts.tv_nsec += (useconds % USEC_IN_SEC) * 1000;
    ts = normalizeTimespec(ts);
    return ts;
}

timespec addTimespec(const timespec& a, const timespec& b)
{
    timespec ts = a;
    ts.tv_sec += b.tv_sec;
    ts.tv_nsec += b.tv_nsec;
    ts = normalizeTimespec(ts);
    return ts;
}