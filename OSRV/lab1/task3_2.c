#include <stdio.h>
#include <stdbool.h>
#include "raw.h"

int main() {
	int c = 0;
	
	setraw();

	while(true) {
		c = getchar();		
		
		if (c == 3) {
			break;
		}

		printf("%c code: %d\n\r", (char)c, c);
	}
	
	unsetraw();	

	printf("\033[2J");
	printf("\033[1;1H");
	printf("\033[=7F");
	printf("\033[=0G");

	return 0;
}
