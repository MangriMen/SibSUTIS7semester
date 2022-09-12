#include <stdio.h>
#include <stdbool.h>

int main() {
	int c = 0;
	
	while(true) {
		c = getchar();		
		
		if (c == 27) {
			break;
		}

		printf("%c code: %d", (char)c, c);
	}

	printf("\033[2J");
	printf("\033[1;1H");
	printf("\033[=7F");
	printf("\033[=0G");

	return 0;
}
