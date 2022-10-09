#include <iostream>
#include <vector>
#include <array>
#include <vingraph.h>

using namespace std;

enum Color {
    Black = RGB(0, 0, 0),
    Blue = RGB(33, 150, 243),
    Yellow = RGB(255, 235, 59),
    White = RGB(255, 255, 255),
};

struct Storage {
    int x;
    int y;
    int z;
    Color color;
    const static int capacity = 10;

    Storage() = default;

    Storage(int x, int y, int z, Color color)
        : x(x)
        , y(y)
        , z(z)
        , color(color) {};
};

struct Boiler {
    int id;
};

struct Car {
    int x;
    int y;
    int shape;
    Color color;

    Car() = default;

    Car(int x, int y, int shape, Color color)
        : x(x)
        , y(y)
        , shape(shape)
        , color(color) {};
};

void* process_storage(Storage& storage)
{
}

void* process_boiler(Boiler& boiler)
{
}

void* process_car(Car& car)
{
    while (true) {
        Fill(car.shape, Color::Yellow);
        usleep(100000 * 10);
        Fill(car.shape, car.color);
        usleep(16000);
    }
}

int main()
{
    ConnectGraph();

    array<Car, 1> cars {
        Car(0, 0, -1, Color::White)
    };

    array<Boiler, 4> boilers;
    for (size_t i = 0; i < boilers.size(); i++) {
        boilers[i].id = i;
    }

    array<Storage, 1> storages {
        Storage(440, 440, 30, Color::Blue)
    };

    vector<pthread_t> threads(cars.size() + boilers.size() + storages.size());

    for (size_t i = 0; i < cars.size(); i++) {
        cars[i].x = 40 + 20 * i;
        cars[i].y = 105;
        cars[i].shape = Rect(cars[i].x, cars[i].y, 40, 40, 8, cars[i].color);
        pthread_create(&threads[i], NULL, (void* (*)(void*))process_car, &cars[i]);
    }

    for (size_t i = 0; i < boilers.size(); i++) {
        pthread_create(&threads[i], NULL, (void* (*)(void*))process_boiler, &boilers[i]);
    }

    for (size_t i = 0; i < storages.size(); i++) {
        pthread_create(&threads[i], NULL, (void* (*)(void*))process_storage, &storages[i]);
    }

    char input_char = 0;
    while (true) {
        input_char = InputChar();

        if (input_char == 27) {
            break;
        }
    }

    for (size_t i = 0; i < threads.size(); i++) {
        pthread_cancel(threads[i]);
    }

    CloseGraph();

    return 0;
}