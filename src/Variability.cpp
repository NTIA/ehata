#include "../include/ehata.h"
#include "math.h"

double Variability(double plb_med__db, double f__mhz, int enviro_code, double reliability)
{
    double sigma;
    double plb__db;

    if (enviro_code == 23 || enviro_code == 24)
    {
        sigma = Sigma_u(f__mhz);
    }
    else
    {
        sigma = Sigma_r(f__mhz);
    }

    plb__db = plb_med__db - ierf(reliability) * sigma;

    return plb__db;
}

/*
*   Description: Standard deviation for location variability for urban environments
*   Inputs:
*       f__mhz : frequency, in MHz
*   Return:
*       [double] : sigma_u
*/
double Sigma_u(double f__mhz)
{
    // Given the location variability standard deviations from Okumura et al,
    //      - sigma_u = 7.1 @ 1500 MHz
    //      - sigma_u = 7.5 @ 2000 MHz
    //      - sigma_u = 8.1 @ 3000 MHz
    // Solve the simultaneous equations for coefficients,
    double alpha_u = 4.0976291;
    double beta_u = -1.2255656;
    double gamma_u = 0.68350345;

    return alpha_u + beta_u * log10(f__mhz) + gamma_u * pow(log10(f__mhz), 2);
}

/*
*   Description: Standard deviation for location variability for suburban and rurual environments
*   Inputs:
*       f__mhz : frequency, in MHz
*   Return:
*       [double] : sigma_r
*/
double Sigma_r(double f__mhz)
{
    return Sigma_u(f__mhz) + 2;
}

double ierf(double q)
{
    // this approximate inverse error function code is borrowed from the Irregular Terrain Model (ITM)

    double x, t, v;
    double c0 = 2.515516698;
    double c1 = 0.802853;
    double c2 = 0.010328;
    double d1 = 1.432788;
    double d2 = 0.189269;
    double d3 = 0.001308;

    x = 0.5 - q;
    t = MAX(0.5 - fabs(x), 0.000001);
    t = sqrt(-2.0 * log(t));
    v = t - ((c2 * t + c1) * t + c0) / (((d3 * t + d2) * t + d1) * t + 1.0);
    if (x < 0.0) v = -v;

    return v;
}