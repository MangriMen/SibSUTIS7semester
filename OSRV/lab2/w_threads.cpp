#include <iostream>
#include <vector>
#include <vingraph.h>
#include <random>
#include <cstring>
#include <unistd.h>
#include <sys/ioctl.h>
#include <sys/mman.h>
#include "/root/raw.h"

using namespace std;

const int FRAME_TIME = 16000;

struct figure_t
{
    int id;
    float dx;
    float dy;
};

struct main_figure_t
{
    int id;
    char *control;
};

void *processFigure(void *figure)
{
    figure_t *fg = (figure_t *)figure;
    int figure_id = fg->id;
    int dx = fg->dx;
    int dy = fg->dy;
    tPoint window;
    tPoint f_pos;
    tPoint f_size;
    while (true)
    {
        window = GetDim(0);
        f_pos = GetPos(figure_id);
        f_size = GetDim(figure_id);

        if (f_pos.x <= 0 || f_pos.x + f_size.x >= window.x)
        {
            dx = -dx;
        }

        if (f_pos.y <= 0 || f_pos.y + f_size.y >= window.y)
        {
            dy = -dy;
        }

        Move(figure_id, dx, dy);

        usleep(FRAME_TIME);
    }
}

float getPathPoint(float phi, int a, int b)
{
    return 2 * a * a * cos(2 * phi) + b;
}

void convertToDecartCoords(float rho, float phi, float &x, float &y)
{
    x = rho * cos(phi);
    y = rho * sin(phi);
}

void *processSomething(void *figure)
{
    main_figure_t *fg = (main_figure_t *)figure;
    int figure_id = fg->id;
    char *control = fg->control;
    tPoint window;
    tPoint f_pos;
    tPoint f_size;

    float x = 0;
    float y = 0;

    int a = 10;
    int b = 100;

    bool isReverse = false;

    float phi = 0;
    float rho = 0;
    while (true)
    {
        window = GetDim(0);
        f_pos = GetPos(figure_id);
        f_size = GetDim(figure_id);

        if (*control != '\0')
        {
            if (*control == 'T')
            {
                isReverse = true;
            }
            else if (*control == 'R')
            {
                isReverse = false;
            }
            else if (*control == 'Q')
            {
                b -= 10;
            }
            else if (*control == 'S')
            {
                b += 10;
            }
            *control = '\0';
        }

        float offset_x = window.x / 2;
        float offset_y = window.y / 2;

        convertToDecartCoords(getPathPoint(phi, a, b), phi, x, y);
        MoveTo(offset_x + x - (f_size.x / 2), offset_y + y - (f_size.y / 2), figure_id);

        phi += 0.05 * (isReverse ? -1 : 1);

        usleep(FRAME_TIME);
    }
}

int main()
{
    ConnectGraph();

    tPoint window = GetDim(0);

    tPoint p1[] = {{140, 100}, {200, 40}, {200, 100}, {155, 55}};
    tPoint p2[] = {{280, 100}, {310, 40}, {340, 100}};

    vector<int> figures{
        Pixel(20, 40),
        Line(70, 50, 130, 90),
        Polyline(p1, 4),
        Rect(210, 40, 60, 60),
        Polygon(p2, 3),
        Ellipse(350, 40, 60, 60),
        Grid(560, 40, 60, 60, 3, 2)};

    pthread_t t_id;
    for (size_t i = 0; i < figures.size(); i++)
    {
        figure_t args;
        args.id = figures[i];
        args.dx = 1 + i;
        args.dy = 1 + i * (i % 2 == 0 ? -1 : 1);
        pthread_create(&t_id, 0, processFigure, &args);
    }

    int picture = Picture(41, 41);
    int square = Rect(7, 7, 26, 26, 0, RGB(255, 255, 0), picture);
    int circle = Ellipse(1, 1, 40, 40, RGB(255, 0, 0), picture);
    SetLineWidth(circle, 2);
    Show(picture);

    char c = '\0';
    main_figure_t args;
    args.id = picture;
    args.control = &c;
    pthread_create(&t_id, 0, processSomething, &args);

    while (true)
    {
        c = InputChar();
    }

    CloseGraph();
    return 0;
}