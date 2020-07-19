using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Propagation
{
    /// <summary>
    /// The Extended Hata Urban Propagation Model
    /// </summary>
    public static partial class EHata
    {
        private const string EHata_x86_DLL_NAME = "ehata_x86.dll";
        private const string EHata_x64_DLL_NAME = "ehata_x64.dll";

        #region 32-Bit P/Invoke Definitions

        [DllImport(EHata_x86_DLL_NAME, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata")]
        private static extern void EHata_x86([MarshalAs(UnmanagedType.LPArray)]  double[] pfl, double f__mhz, double h_b__meter, 
            double h_m__meter, int enviro_code, double reliability, out double A__db);

        [DllImport(EHata_x86_DLL_NAME, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata_DBG")]
        private static extern void EHataEx_x86([MarshalAs(UnmanagedType.LPArray)] double[] pfl, double f__mhz, double h_b__meter, 
            double h_m__meter, int enviro_code, double reliability, out double A__db, ref IntermediateValues intervalues);

        #endregion

        #region 64-bit P/Invoke Definitions

        [DllImport(EHata_x64_DLL_NAME, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata")]
        private static extern void EHata_x64([MarshalAs(UnmanagedType.LPArray)] double[] pfl, double f__mhz, double h_b__meter, 
            double h_m__meter, int enviro_code, double reliability, out double A__db);

        [DllImport(EHata_x64_DLL_NAME, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata_DBG")]
        private static extern void EHataEx_x64([MarshalAs(UnmanagedType.LPArray)] double[] pfl, double f__mhz, double h_b__meter, 
            double h_m__meter, int enviro_code, double reliability, out double A__db, ref IntermediateValues intervalues);

        #endregion

        private delegate void EHata_Delegate(double[] pfl, double f__mhz, double h_b__meter, double h_m__meter, int enviro_code, double reliability, out double A__db);
        private delegate void EHataEx_Delegate(double[] pfl, double f__mhz, double h_b__meter, double h_m__meter, int enviro_code, double reliability, out double A__db, ref IntermediateValues intervalues);

        private static EHata_Delegate EHata_Invoke;
        private static EHataEx_Delegate EHataEx_Invoke;

        static EHata()
        {
            // set the binding to the correct native DLL architecture
            if (Environment.Is64BitProcess)
            {
                EHata_Invoke = EHata_x64;
                EHataEx_Invoke = EHataEx_x64;
            }
            else
            {
                EHata_Invoke = EHata_x86;
                EHataEx_Invoke = EHataEx_x86;
            }
        }

        /// <summary>
        /// The Extended Hata (eHata) Urban Propagation Model
        /// </summary>
        /// <param name="pfl">An ITM-formatted terrain profile, from the mobile to the base station</param>
        /// <param name="f__mhz">The frequency, in MHz</param>
        /// <param name="h_b__meter">The height of the base station, in meters</param>
        /// <param name="h_m__meter">The height of the mobile, in meters</param>
        /// <param name="enviro_code">The NLCD environment code</param>
        /// <param name="reliability">The percent not exceeded of the signal</param>
        /// <param name="A__db">Basic transmission loss, in dB</param>
        public static void Invoke(double[] pfl, double f__mhz, double h_b__meter, double h_m__meter, int enviro_code, double reliability, out double A__db)
        {
            EHata_Invoke(pfl, f__mhz, h_b__meter, h_m__meter, enviro_code, reliability, out A__db);
        }

        /// <summary>
        /// The Extended Hata (eHata) Urban Propagation Model
        /// </summary>
        /// <param name="pfl">An ITM-formatted terrain profile, from the mobile to the base station</param>
        /// <param name="f__mhz">The frequency, in MHz</param>
        /// <param name="h_b__meter">The height of the base station, in meters</param>
        /// <param name="h_m__meter">The height of the mobile, in meters</param>
        /// <param name="enviro_code">The NLCD environment code</param>
        /// <param name="reliability">The percent not exceeded of the signal</param>
        /// <param name="A__db">Basic transmission loss, in dB</param>
        /// <param name="interValues">A data structure containing intermediate values from the eHata calculations</param>
        public static void InvokeEx(double[] pfl, double f__mhz, double h_b__meter, double h_m__meter, int enviro_code, double reliability, out double A__db, out IntermediateValues interValues)
        {
            interValues = new IntermediateValues();

            EHataEx_Invoke(pfl, f__mhz, h_b__meter, h_m__meter, enviro_code, reliability, out A__db, ref interValues);
        }
    }
}
