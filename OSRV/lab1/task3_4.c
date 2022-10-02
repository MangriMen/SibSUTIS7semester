#include <stdio.h>
#include <stdlib.h>
#include <signal.h>
#include <unistd.h>
#include <stdbool.h>

void resume_terminal() {
    printf("\033[=39F");
	printf("\033[=32G");
	printf("\033[?25h");
	printf("\033[2J");
}

void sigint_handler(int sig) {
    resume_terminal();
    exit(1);
}

int main(int argc, char* argv[]) {
    if (argc < 4) {
        printf("Not enough parameters. program speed color dir_x dir_y\n");
        return -1;
    }

    int x = 1;
    int y = 1;

    int speed = atoi(argv[1]);
    int color = atoi(argv[2]);
    int dir_x = atoi(argv[3]);
    int dir_y = atoi(argv[4]);

    if (dir_x != 0) {
        dir_x = dir_x >= 0 ? 1 : -1;
    }
    if (dir_y != 0) {
        dir_y = dir_y >= 0 ? 1 : -1;
    }

    if (speed < 0) {
        printf("Speed must be greater than zero\n");
        return -1;
    }

    if (color < 0 && color > 7) {
        printf("Speed must be greater than zero\n");
        return -1;
    }

    signal(SIGINT, sigint_handler);

    printf("\033[?25l");
	printf("\033[=%dF", color);
	printf("\033[2J");
	fflush(stdout);

    while(true) {
        printf("\033[%d;%dH", x, y);
		printf("A");
		fflush(stdout);
		printf("\033[%d;%dH", x, y);
		printf(" ");
		usleep(speed);
		x += dir_x;
		y += dir_y;
    }

    resume_terminal();
    
    return 0;
}
