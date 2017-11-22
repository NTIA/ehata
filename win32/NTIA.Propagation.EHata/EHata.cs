using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NTIA.Propagation.EHata
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InterValues
    {
        public float d_bp__km;
        public float att_1km;
        public float att_100km;

        public float h_b_eff__meter;
        public float h_m_eff__meter;

        // Terrain Stats
        public float pfl10__meter;
        public float pfl50__meter;
        public float pfl90__meter;
        public float deltah__meter;

        // Path Geometry
        public float d__km;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] d_hzn__meter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] h_avg__meter;
        public float theta_m__mrad;
        public float beta;
        public int iend_ov_sea;
        public float hedge_tilda;
        public bool single_horizon;

        // Misc
        public float slope_max;
        public float slope_min;

        public int trace_code;

    }

    public static class EHata
    {
        [DllImport("ehata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata")]
        private static extern void EHATA(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, ref float plb);

        [DllImport("ehata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata_DBG")]
        private static extern void EHATA_DBG(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, ref float plb, ref InterValues intervalues);

        /// <summary>
        /// The Extended Hata (eHata) Urban Propagation Model
        /// </summary>
        /// <param name="pfl">An ITM-formatted terrain profile, from the mobile to the base station</param>
        /// <param name="f__mhz">The frequency, in MHz</param>
        /// <param name="h_b__meter">The height of the base station, in meters</param>
        /// <param name="h_m__meter">The height of the mobile, in meters</param>
        /// <param name="enviro_code">The NLCD environment code</param>
        /// <param name="plb">The path loss, in dB</param>
        public static void Invoke(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, out float plb)
        {
            plb = 0;

            EHATA(pfl, f__mhz, h_b__meter, h_m__meter, enviro_code, ref plb);
        }

        /// <summary>
        /// The Extended Hata (eHata) Urban Propagation Model
        /// </summary>
        /// <param name="pfl">An ITM-formatted terrain profile, from the mobile to the base station</param>
        /// <param name="f__mhz">The frequency, in MHz</param>
        /// <param name="h_b__meter">The height of the base station, in meters</param>
        /// <param name="h_m__meter">The height of the mobile, in meters</param>
        /// <param name="enviro_code">The NLCD environment code</param>
        /// <param name="plb">The path loss, in dB</param>
        /// <param name="interValues">A data structure containing intermediate values from the eHata calculations</param>
        public static void Invoke(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, out float plb, out InterValues interValues)
        {
            plb = 0;
            interValues = new InterValues();

            EHATA_DBG(pfl, f__mhz, h_b__meter, h_m__meter, enviro_code, ref plb, ref interValues);
        }
    }
}
