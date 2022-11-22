#include <iostream>
#include <unistd.h>
#include <vingraph.h>
#include "/root/labs/plates.h"
#include "utils.hpp"

using namespace std;

int main()
{
    StartGame(0);

    char input = 0;
    while (true) {
        input = InputChar();

        if (input == 27) {
            break;
        } else if (input == '0') {
            putreg(RG_RCMN, 0);
            putreg(RG_RCMC, RCMC_START);

            /* 200x200 */
            putreg(RG_RCMC, RCMC_UP);
            usleep(pointsToUSec(200, RCM_SPEED));

            putreg(RG_RCMC, RCMC_RIGHT);
            usleep(pointsToUSec(200, RCM_SPEED));

            putreg(RG_RCMC, RCMC_DOWN);
            usleep(pointsToUSec(200, RCM_SPEED));

            putreg(RG_RCMC, RCMC_LEFT);
            usleep(pointsToUSec(200, RCM_SPEED));

            /* 500x200 */
            putreg(RG_RCMC, RCMC_UP);
            usleep(pointsToUSec(500, RCM_SPEED));

            putreg(RG_RCMC, RCMC_RIGHT);
            usleep(pointsToUSec(200, RCM_SPEED));

            putreg(RG_RCMC, RCMC_DOWN);
            usleep(pointsToUSec(500, RCM_SPEED));

            putreg(RG_RCMC, RCMC_LEFT);
            usleep(pointsToUSec(200, RCM_SPEED));

            /* End */
            putreg(RG_RCMC, RCMC_UP);
            usleep(pointsToUSec(500, RCM_SPEED));
        }
    }

    EndGame();
    return 0;
}