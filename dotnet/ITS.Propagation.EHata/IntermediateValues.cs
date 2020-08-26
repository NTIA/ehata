using System.Runtime.InteropServices;

namespace ITS.Propagation
{
    /// <summary>
    /// The Extended Hata Urban Propagation Model
    /// </summary>
    public static partial class EHata
    {
        /// <summary>
        /// Intermediate values from EHata
        /// </summary>
        public class IntermediateValues
        {
            /// <summary>
            /// Breakpoint distance, in km
            /// </summary>
            public double d_bp__km;

            /// <summary>
            /// Attenuation at 1 km
            /// </summary>
            public double att_1km;

            /// <summary>
            /// Attenuation at 100 km
            /// </summary>
            public double att_100km;

            /// <summary>
            /// Effective height of the base station, in meters
            /// </summary>
            public double h_b_eff__meter;

            /// <summary>
            /// Effective height of the mobile, in meters
            /// </summary>
            public double h_m_eff__meter;

            /// <summary>
            /// 10% terrain quantile
            /// </summary>
            public double pfl10__meter;

            /// <summary>
            /// 50% terrain quantile
            /// </summary>
            public double pfl50__meter;

            /// <summary>
            /// 90% terrain quantile
            /// </summary>
            public double pfl90__meter;

            /// <summary>
            /// Terrain irregularity parameter, "Delta H"
            /// </summary>
            public double deltah__meter;

            /// <summary>
            /// Path distance, in km
            /// </summary>
            public double d__km;

            /// <summary>
            /// Terminal horizon distances, in meters
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public double[] d_hzn__meter;

            /// <summary>
            /// Average ground height of each terminal (MSL), in meters
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public double[] h_avg__meter;

            /// <summary>
            /// Mobile terrain slope, in mrad
            /// </summary>
            public double theta_m__mrad;

            /// <summary>
            /// Percentage of the path that is sea
            /// </summary>
            public double beta;

            /// <summary>
            /// Flag identifying which end of path is over sea
            /// </summary>
            public int iend_ov_sea;

            /// <summary>
            /// Intermediate correction factor
            /// </summary>
            public double hedge_tilda;

            /// <summary>
            /// Horizon flag
            /// </summary>
            public bool single_horizon;

            /// <summary>
            /// Intermediate value in computing theta_m__mrad
            /// </summary>
            public double slope_max;

            /// <summary>
            /// Intermediate value in computing theta_m__mrad
            /// </summary>
            public double slope_min;
        }
    }
}
