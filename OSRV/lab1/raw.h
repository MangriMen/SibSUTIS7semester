#include <stdlib.h>
#include <termios.h>

void setraw()
  {
  struct termios t;
  tcgetattr (0, &t);
  t.c_cc[VMIN] = 1;
  t.c_cc[VTIME] = 0;
  t.c_lflag &= ~( ECHO|ICANON|ISIG|ECHOE|ECHOK|ECHONL );
  t.c_oflag &= ~( OPOST );
  tcsetattr (0, TCSADRAIN, &t);
  }

void unsetraw()
  {
  struct termios t;
  tcgetattr (0, &t);
  t.c_lflag |= ( ECHO|ICANON|ISIG|ECHOE|ECHOK|ECHONL );
  t.c_oflag |= ( OPOST );
  tcsetattr (0, TCSADRAIN, &t);
  }
