#include <termios.h>
#include <unistd.h>
#include <stdio.h>
#include <string.h>
#include <stdbool.h>

struct termios tsaved;

int rk_mytermsave() {
  if (tcgetattr(STDOUT_FILENO, &tsaved)) {
    return -1;
  }

  return 0;
}

int rk_mytermrestore() {
  if (tcsetattr(STDOUT_FILENO, TCSANOW, &tsaved)) {
    return -1;
  }

  return 0;
}

int rk_mytermregime(int regime, int vtime, int vmin, int echo, int sigint) {
  struct termios tnew;

  tnew.c_lflag &= ~(regime | echo | sigint);
  tnew.c_cc[VTIME] = vtime;
  tnew.c_cc[VMIN] = vmin;

  if (tcsetattr(STDOUT_FILENO, TCSANOW, &tnew)) {
    return -1;
  }

  return 0;
}

int main() {
  rk_mytermsave();
  rk_mytermregime(ICANON, 0, 1, ECHO, 0);

  int c = 0;
  while(true) {
    if ((char)c == '\033') {
      break;
    }

    c = getchar();
    
    printf("%c - %d", (char)c, c);
    printf("\r\n");
  }

  rk_mytermrestore();

  return 0;
}

