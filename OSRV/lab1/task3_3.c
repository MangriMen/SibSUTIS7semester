#include <stdio.h>

int main()
{
	int x = 1;
	int y = 1;

	printf("\033[?25l");
	printf("\033[2J");

	while (x != 10)
	{
		printf("\033[%d;%dH", x, y);
		printf("A");
		fflush(stdout);
		printf("\033[%d;%dH", x, y);
		printf(" ");
		sleep(1);
		x++;
		y++;
	}

	printf("\033[?25h");
	printf("\033[2J");

	return 0;
}