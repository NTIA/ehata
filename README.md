# The Extended Hata (eHata) Urban Propagation Model #

This code repository contains a C language reference version of the eHata 
urban propagation model.  The model was developed by NTIA and used in NTIA 
Technical Report [TR-15-517](https://www.its.bldrdoc.gov/publications/2805.aspx), 
"3.5 GHz Exclusion Zone Analyses and Methodology," June 2015 
[TR-15-517] for the 3.5 GHz exclusion zone analysis.

## Inputs ##

| Variable      | Type     | Units | Description |
|---------------|----------|-------|-------------|
| `pfl`         | double[] |       | A terrain profile line, from the mobile to the base station.  EHata uses the ITM-method of formatting terrain information, in that: <ul><li>`pfl[0]` : Number of elevation points + 1</li><li>`pfl[1]` : Resolution, in meters</li><li>`pfl[i]` : Elevation above sea level, in meters</li></ul> |
| `f__mhz`      | double   | MHz   | Frequency   |
| `h_m__meter`  | double   | meter | The height of the mobile |
| `h_b__meter`  | double   | meter | The height of the base station |
| `enviro_code` | int      |       | The NLCD environment code |
| `reliability` | double   |       | The quantile percent not exceeded of the signal.  Limits: 0 < `reliability` < 1  |

## Outputs ##

| Variable      | Type   | Units | Description |
|---------------|--------|-------|-------------|
| `plb`         | double | dB    | Path loss   |
| `intervalues` | struct |       | [Optional] A data structure containing intermediate values from the eHata calculations |

## Intermediate Values ##

When calling the ExtendedHata_DBG() function, the function will populate the 
**InterValues** data structure with intermediate values from the eHata 
calculations.  Those values are as follows:

| Variable         | Type      | Units       | Description |
|------------------|-----------|-------------|-------------|
| `d_bp__km`       | double    | km          | The breakpoint distance |
| `att_1km`        | double    | dB          | Attenuation at 1 km |
| `att_100km`      | double    | dB          | Attenuation at 100 km |
| `h_b_eff__meter` | double    | meter       | Effective height of the base station |
| `h_m_eff__meter` | double    | meter       | Effective height of the mobile |
| `pfl10__meter`   | double    | meter       | 10% terrain quantile |
| `pfl50__meter`   | double    | meter       | 50% terrain quantile |
| `pfl90__meter`   | double    | meter       | 90% terrain quantile |
| `deltah__meter`  | double    | meter       | Terrain irregularity parameter |
| `d__km`          | double    | km          | Path distance |
| `d_hzn__meter`   | double[2] | meter       | Horizon distances |
| `h_avg__meter`   | double[2] | meter       | Average heights |
| `theta_m__mrad`  | double    | milliradian | Slope of the terrain at the at the mobile |
| `beta`           | double    |             | Percentage of path that is sea |
| `iend_ov_sea`    | int       |             | Flag specifying which end is over the sea |
| `hedge_tilda`    | double    | meter       | Horizon correction factor |
| `single_horizon` | bool      |             | Flag for specifying number of horizons |
| `slope_max`      | double    | milliradian | Intermediate value when calculating the mobile terrain slope |
| `slope_min`      | double    | milliradian | Intermediate value when calculating the mobile terrain slope |

## Notes on Code Style ##

* In general, variables follow the naming convention in which a single underscore
denotes a subscript (pseudo-LaTeX format), where a double underscore is followed
by the units, i.e. h_m__meter.
* Variables are named to match their corresponding mathematical variables 
in the underlying technical references, i.e., gamma_1.
* Most values are calculated and stored in the **InterValues** data structure
that is passed between function calls.  In general, only the correction factor
functions return their result as a value.

## References ##

* [Okumura, 1968] Okumura, Y., Ohmori, E., Kawano, T., Fukuda, K.  "Field Strength 
and Its Variability in VHF and UHF Land-Mobile Radio Service", 
_Review of the Electrical Communication Laboratory_, Vol. 16, Num 9-10. 
Sept-Oct 1968. pp. 825-873.
* [Hata, 1980] Hata, M. "Empirical Formula for Propagation Loss in Land Mobile 
Radio Services", _IEEE Transactions on Vehicular Technology_, Vol VT-29, Num 3.  
Aug 1980.  pp 317-325.  DOI: 10.1109/T-VT.1980.23859
* [[TR-15-517](https://www.its.bldrdoc.gov/publications/2805.aspx)] Drocella, 
E., Richards, J., Sole, R., Najmy, F., Lundy, A., McKenna, P. "3.5 
GHz Exclusion Zone Analyses and Methodology", _NTIA Report 15-517_, June 2015.

## Configure and Build ##

This project was developed and built using Microsoft Visual Studio
2015, using the Visual Studio 2015 (v140) C compiler.  By default, the
project file is configured to build with Runtime Library set to 
Multi-threaded (/MT), thus removing the requirement that the target machine
have the matching version of the Microsoft C Redistributable installed.

# Contact #
For questions, contact Paul McKenna, (303) 497-3474, pmckenna@ntia.gov
