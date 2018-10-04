#include "../include/ehata.h"
#include "math.h"

void PreprocessTerrainPath(double *pfl, double h_b__meter, double h_m__meter, InterValues *interValues)
{
    FindAverageGroundHeight(pfl, h_m__meter, h_b__meter, interValues);

    ComputeTerrainStatistics(pfl, interValues);

    MobileTerrainSlope(pfl, interValues);

    AnalyzeSeaPath(pfl, interValues);

    SingleHorizonTest(pfl, h_m__meter, h_b__meter, interValues);
}

/*
*   Description: Find the average ground height at each terminal and set the effective terminal heights
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*       h_m__meter : height of the mobile, in meters
*       h_b__meter : height of the base station, in meters
*   Outputs:
*       interValues->h_avg__meter : Average ground height of each terminal
*               above sea level, in meters
*                - h_avg__meter[0] = terminal at start of pfl
*                - h_avg__meter[1] = terminal at end of pfl
*       interValues->trace_code : Debug trace flag to document code
*               execution path for tracing and testing purposes
*/
void FindAverageGroundHeight(double *pfl, double h_m__meter, double h_b__meter, InterValues *interValues)
{
    int np = int(pfl[0]);
    double xi = pfl[1] * 0.001;      // step size of the profile points, in km
    double d__km = np * xi;          // path distance, in km

    int i_start, i_end;
    double sum = 0.0;

    if (d__km < 3.0)
    {
        interValues->h_avg__meter[0] = pfl[2];
        interValues->h_avg__meter[1] = pfl[np + 2];

        interValues->h_m_eff__meter = h_m__meter;
        interValues->h_b_eff__meter = h_b__meter;
    }
    else if (3.0 <= d__km && d__km <= 15.0)
    {
        i_start = 2 + int(3.0 / xi);
        i_end = np + 2;
        for (int i = i_start; i <= i_end; i++)
            sum = sum + pfl[i];
        interValues->h_avg__meter[0] = (d__km - 3.0) / 12.0 * (pfl[2] - sum / (i_end - i_start + 1)) ;

        i_start = 2;
        i_end = np + 2 - int(3.0 / xi);
        sum = 0.0;
        for (int i = i_start; i <= i_end; i++)
            sum = sum + pfl[i];
        interValues->h_avg__meter[1] = (d__km - 3.0) / 12.0 * (pfl[np + 2] - sum / (i_end - i_start + 1));

        interValues->h_m_eff__meter = h_m__meter + interValues->h_avg__meter[0];
        interValues->h_b_eff__meter = h_b__meter + interValues->h_avg__meter[1];
    }
    else // d__km > 15.0
    {
        i_start = 2 + int(3.0 / xi);
        i_end = 2 + int(15.0 / xi);
        for (int i = i_start; i <= i_end; i++)
            sum = sum + pfl[i];
        interValues->h_avg__meter[0] = pfl[2] - sum / (i_end - i_start + 1);

        i_start = np + 2 - int(15.0 / xi);
        i_end = np + 2 - int(3.0 / xi);
        sum = 0.0;
        for (int i = i_start; i <= i_end; i++)
            sum = sum + pfl[i];
        interValues->h_avg__meter[1] = pfl[np + 2] - sum / (i_end - i_start + 1);

        interValues->h_m_eff__meter = h_m__meter + interValues->h_avg__meter[0];
        interValues->h_b_eff__meter = h_b__meter + interValues->h_avg__meter[1];
    }
}

/*
*   Description: Compute the 10%, 50%, and 90% terrain height quantiles as well as the terrain
*                irregularity parameter, deltaH
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*   Outputs:
*       interValues->pfl10__meter : 10% terrain quantile
*       interValues->pfl50__meter : 50% terrain quantile
*       interValues->pfl90__meter : 90% terrain quantile
*       interValues->deltah__meter : terrain irregularity parameter
*       interValues->trace_code : debug trace flag to document code
*               execution path for tracing and testing purposes
*/
void ComputeTerrainStatistics(double *pfl, InterValues *interValues)
{
    int np = int(pfl[0]);
    double xi = pfl[1] * 0.001;      // step size of the profile points, in km
    double d__km = np * xi;          // path distance, in km

    int i_start, i_end;

    // "[deltah] may be found ... equal to the difference between 10% and 90% of the terrain
    // undulation height ... within a distance of 10km from the receiving point to the
    // transmitting point." Okumura, Sec 2.4 (1)(b)
    if (d__km < 10.0) // ... then use the whole path
    {
        i_start = 2;
        i_end = np + 2;
    }
    else // use 10 km adjacent to the mobile
    {
        i_start = 2;
        i_end = 2 + int(10.0 / xi);
    }

    // create a copy of the 10 km path at the mobile, or the whole path (if less than 10 km)
    double *pfl_segment = new double[i_end - i_start + 1];
    for (int i = i_start; i <= i_end; i++)
        pfl_segment[i - i_start] = pfl[i];

    int npts = i_end - i_start + 1;
    int i10 = 0.1 * npts - 1;
    int i50 = 0.5 * npts - 1;
    int i90 = 0.9 * npts - 1;
    interValues->pfl10__meter = FindQuantile(npts, pfl_segment, i10);
    interValues->pfl50__meter = FindQuantile(npts, pfl_segment, i50);
    interValues->pfl90__meter = FindQuantile(npts, pfl_segment, i90);
    interValues->deltah__meter = interValues->pfl10__meter - interValues->pfl90__meter;

    // "If the path is less than 10 km in distance, then the asymptotic value
    //  for the terrain irrgularity is computed" [TR-15-517]
    if (d__km < 10.0)
    {
        double factor = (1.0 - 0.8*exp(-0.2)) / (1.0 - 0.8*exp(-0.02 * d__km));
        interValues->pfl10__meter = interValues->pfl10__meter * factor;
        interValues->pfl50__meter = interValues->pfl50__meter * factor;
        interValues->pfl90__meter = interValues->pfl90__meter * factor;
        interValues->deltah__meter = interValues->deltah__meter * factor;
    }

    delete[] pfl_segment;
}

/*
*   Description: Find the slope of the terrain at the mobile
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*   Outputs:
*       interValues->slope_max : intermediate value
*       interValues->slope_min : intermediate value
*       interValues->theta_m__mrad : mobile terrain slope, in millirads
*       interValues->trace_code : debug trace flag to document code
*               execution path for tracing and testing purposes
*/
void MobileTerrainSlope(double *pfl, InterValues *interValues)
{
    int np = int(pfl[0]);           // number of points
    double xi = pfl[1];              // step size of the profile points, in meter
    double d__meter = np * xi;

    // find the mean slope of the terrain in the vicinity of the mobile station
    interValues->slope_max = -1.0e+31;
    interValues->slope_min = 1.0e+31;
    double slope_five = 0.0;
    double slope;

    double x1, x2;
    double *pfl_segment = new double[int(10000/xi) + 3];

    x1 = 0.0;
    x2 = 5000.0;
    while (d__meter >= x2 && x2 <= 10000.0)
    {
        int npts = x2 / xi;
        pfl_segment[0] = npts;
        pfl_segment[1] = xi;
        for (int i = 0; i < npts + 1; i++)
            pfl_segment[i + 2] = pfl[i + 2];

        double z1 = 0, z2 = 0;
        LeastSquares(pfl_segment, x1, x2, &z1, &z2);

        // flip the sign to match the Okumura et al.convention
        slope = -1000.0 * (z2 - z1) / (x2 - x1);
        interValues->slope_min = MIN(interValues->slope_min, slope);
        interValues->slope_max = MAX(interValues->slope_max, slope);
        if (x2 == 5000.0)
            slope_five = slope;
        x2 = x2 + 1000.0;
    }

    if (d__meter <= 5000.0 || interValues->slope_max * interValues->slope_min < 0.0)
    {
        interValues->theta_m__mrad = slope_five;
    }
    else
    {
        if (interValues->slope_max >= 0.0)
            interValues->theta_m__mrad = interValues->slope_max;
        else
            interValues->theta_m__mrad = interValues->slope_min;
    }

    delete[] pfl_segment;
}

/*
*   Description: Compute the sea details of the path
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*   Outputs:
*       interValues->beta : percentage of the path that is sea
*       interValues->iend_ov_sea : which end of the pfl is sea
*                1  : low end
*                0  : high end
*               -1  : equal amounts on both ends
*/
void AnalyzeSeaPath(double* pfl, InterValues *interValues)
{
    int np = int(pfl[0]);

    // determine the fraction of the path over sea and which end of the path is adjacent to the sea
    int index_midpoint = np / 2;

    int sea_cnt = 0;
    int low_cnt = 0;
    int high_cnt = 0;

    for (int i = 1; i <= np + 1; i++)
    {
        if (pfl[i + 1] == 0.0)
        {
            sea_cnt = sea_cnt + 1;
            if (i <= index_midpoint)
                low_cnt = low_cnt + 1;
            else
                high_cnt = high_cnt + 1;
        }
    }

    interValues->beta = double(sea_cnt) / double(np + 1);

    if (low_cnt > high_cnt)
        interValues->iend_ov_sea = 1;
    else if (high_cnt > low_cnt)
        interValues->iend_ov_sea = 0;
    else
        interValues->iend_ov_sea = -1;
}

/*
*   Description: Compute the average height of the terrain pfl
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*   Return:
*       [double] : average terrain height, in meters
*/
double AverageTerrainHeight(double *pfl)
{
    double h_gnd__meter = 0.0;
    int np = (int)pfl[0];

    for (int i = 1; i <= np + 1; i++)
        h_gnd__meter = h_gnd__meter + pfl[i + 1];
    h_gnd__meter = h_gnd__meter / (np + 1);

    return h_gnd__meter;
}

/*
*   Description: Determine the horizon details
*   Inputs:
*       pfl : Terrain profile line with:
*                - pfl[0] = number of terrain points + 1
*                - pfl[1] = step size, in meters
*                - pfl[i] = elevation above mean sea level, in meters
*       h_m__meter : height of the mobile, in meters
*       h_b__meter : height of the base station, in meters
*   Outputs:
*       interValues->d_hzn__meter : horizon distances, in meters
*                - d_hzn__meter[0] = mobile horizon distance, in meters
*                - d_hzn__meter[1] = base station horizon distance, in meters
*       interValues->single_horizon : horizon flag
*       interValues->hedge_tilda : correction factor
*       interValues->trace_code : debug trace flag to document code
*               execution path for tracing and testing purposes
*/
void SingleHorizonTest(double *pfl, double h_m__meter, double h_b__meter, InterValues *interValues)
{
    int np = int(pfl[0]);           // number of points
    double xi = pfl[1];              // step size of the profile points, in meter
    double d__meter = np * xi;

    double h_gnd__meter = AverageTerrainHeight(pfl);

    double en0 = 301.0f;
    double ens = 0;
    if (h_gnd__meter == 0)
        ens = en0;
    else
        ens = en0 * exp(-h_gnd__meter / 9460);
    double gma = 157e-9f;
    double gme = gma * (1 - 0.04665 * exp(ens / 179.3));

    FindHorizons(pfl, gme, d__meter, h_m__meter, h_b__meter, interValues->d_hzn__meter);

    double a = interValues->d_hzn__meter[0];
    double b = interValues->d_hzn__meter[1];
    double d_diff__meter = d__meter - interValues->d_hzn__meter[0] - interValues->d_hzn__meter[1];
    double q = MAX(d_diff__meter - 0.5*pfl[1], 0) - MAX(-d_diff__meter - 0.5*pfl[1], 0);
    if (q != 0.0)
    {
        interValues->single_horizon = false;
    }
    else
    {
        interValues->single_horizon = true;
        int iedge = interValues->d_hzn__meter[0] / pfl[1];

        double za, zb;
        za = h_b__meter + pfl[np + 2];
        zb = h_m__meter + pfl[2];
        interValues->hedge_tilda = pfl[iedge + 2] - (za*interValues->d_hzn__meter[1] + zb*interValues->d_hzn__meter[0]) / d__meter + 0.5*gme*interValues->d_hzn__meter[0] * interValues->d_hzn__meter[1];

        if (interValues->hedge_tilda < 0.0)
            interValues->hedge_tilda = 0.0;
    }
}
