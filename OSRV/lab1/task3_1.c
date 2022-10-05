#include <stdio.h>
#include <sys/ioctl.h>
#include <unistd.h>

int get_screen_size(int* rows, int* cols) {
	struct winsize w;
	if (!ioctl(STDOUT_FILENO, TIOCGWINSZ, &w)) {
		*rows = w.ws_row;
		*cols = w.ws_col;
		return 0;
	}
	return -1;
}


int main() {
	int rows;
	int cols;

	get_screen_size(&rows, &cols);

	printf("\033[2J");
	printf("\033[%d;%dH", rows/2, cols/2 - 3);
	printf("HELLO\n");
	return 0;
}
