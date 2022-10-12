#include <iostream>
#include <vector>
#include <array>
#include <random>
#include <unistd.h>
#include <pthread.h>
#include <sys/neutrino.h>
#include <vingraph.h>

using namespace std;

class Channel {
public:
    static int boiler_channel;
    static int storage_channel;
};

int Channel::boiler_channel = 0;
int Channel::storage_channel = 0;

class Connection {
public:
    static int boiler_connection;
    static int storage_connection;
};

int Connection::boiler_connection = 0;
int Connection::storage_connection = 0;

enum Color {
    Black = RGB(0, 0, 0),
    Blue = RGB(33, 150, 243),
    Cyan = RGB(59, 255, 235),
    Yellow = RGB(255, 235, 59),
    White = RGB(255, 255, 255),
};

struct Storage {
    const static int blocks = 10;

    int x;
    int y;
    int z;
    Color color;

    Storage() = default;

    Storage(int x, int y, int z, Color color)
        : x(x)
        , y(y)
        , z(z)
        , color(color) {};
};

struct Boiler {
    const static int blocks = 10;

    int x;
    int y;
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
    static int block_width = 60;
    static int block_height = 20;

    static int block_spacing = 4;

    array<int, Storage::blocks> blocks;
    for (size_t i = 0; i < storage.blocks; i++) {
        blocks[i]
            = Rect(storage.x, storage.y + (block_height + block_spacing) * i,
                block_width, block_height, 6, Color::Cyan);
    }

    int block_count = 0;
    while (true) {
        block_count = rand() % (storage.blocks - 1) + 1;
        for (size_t i = 0; i < block_count; i++) {
            Fill(blocks[i], Color::Cyan);
            usleep(100000);
        }

        int car_message = MsgReceive(Channel::storage_channel, 0, 0, 0);

        for (int64_t i = block_count - 1; i >= 0; i--) {
            usleep(100000);
            Fill(blocks[i], Color::Black);
        }

        MsgReply(car_message, 0, &block_count, sizeof(block_count));
    }
}

void* process_boiler(Boiler& boiler)
{
    static int block_width = 60;
    static int block_height = 20;

    static int block_spacing = 4;

    array<int, Boiler::blocks> blocks;
    for (size_t i = 0; i < boiler.blocks; i++) {
        blocks[i]
            = Rect(boiler.x, boiler.y + (block_height + block_spacing) * i,
                block_width, block_height, 6, Color::Yellow);
    }

    int block_count = 0;

    while (true) {
        MsgSend(Connection::boiler_connection, &boiler, sizeof(boiler),
            &block_count, sizeof(block_count));
        for (size_t i = 0; i < block_count; i++) {
            Fill(blocks[i], Color::Yellow);
            usleep(100000);
        }

        for (int64_t i = block_count - 1; i >= 0; i--) {
            Fill(blocks[i], Color::Black);
            if (i == 2) {
                MsgSendPulse(Connection::boiler_connection, 10, 0, 0);
            }
            usleep(2000000);
        }
    }
}

void* process_car(Car& car)
{
    Boiler current_storage;
    Boiler current_boiler;
    int fill_counter = 0;

    tPoint carPos = GetPos(car.shape);
    tPoint carSize = GetDim(car.shape);

    int block_count = 0;

    while (true) {
        int boiler_message = MsgReceive(Channel::boiler_channel,
            &current_boiler, sizeof(current_boiler), 0);

        MsgSend(Connection::storage_connection, 0, 0, &block_count,
            sizeof(block_count));
        Fill(car.shape, Color::Yellow);

        carPos = GetPos(car.shape);
        for (size_t i = carPos.x; i < current_boiler.x + carSize.x / 2; i++) {
            MoveTo(i, carPos.y, car.shape);
            usleep(1000);
        }

        MsgReply(boiler_message, 0, &block_count, sizeof(block_count));

        usleep(100000 * block_count);
        Fill(car.shape, Color::Black);

        for (int64_t i = current_boiler.x; i >= carPos.x; i--) {
            MoveTo(i, carPos.y, car.shape);
            usleep(1000);
        }
    }
}

int main()
{
    srand(time(NULL));
    
    ConnectGraph();

    Channel::boiler_channel = ChannelCreate(0);
    Channel::storage_channel = ChannelCreate(0);

    Connection::boiler_connection
        = ConnectAttach(0, 0, Channel::boiler_channel, 0, 0);
    Connection::storage_connection
        = ConnectAttach(0, 0, Channel::storage_channel, 0, 0);

    Line(20, 90, 80 * 6 - 20, 90, Color::White);

    array<Car, 2> cars { Car(0, 0, -1, Color::White),
        Car(0, 0, -1, Color::White) };

    array<Boiler, 4> boilers;
    for (size_t i = 0; i < boilers.size(); i++) {
        boilers[i].x = 160 + i * (60 + 20);
        boilers[i].y = 100;
    }

    array<Storage, 1> storages { Storage(20, 100, 30, Color::Blue) };

    vector<pthread_t> threads(cars.size() + boilers.size() + storages.size());

    for (size_t i = 0; i < cars.size(); i++) {
        cars[i].x = 33;
        cars[i].y = 50;
        cars[i].shape = Rect(cars[i].x, cars[i].y, 32, 32, 6, cars[i].color);
        pthread_create(
            &threads[i], NULL, (void* (*)(void*))process_car, &cars[i]);
    }

    for (size_t i = 0; i < boilers.size(); i++) {
        pthread_create(&threads[i + cars.size()], NULL,
            (void* (*)(void*))process_boiler, &boilers[i]);
    }

    for (size_t i = 0; i < storages.size(); i++) {
        pthread_create(&threads[i + cars.size() + boilers.size()], NULL,
            (void* (*)(void*))process_storage, &storages[i]);
    }

    SetColor(Color::White);

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