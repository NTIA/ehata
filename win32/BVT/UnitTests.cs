using NTIA.Propagation.EHata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BVT
{
    [StructLayout(LayoutKind.Sequential)]
    struct InterValues
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

    public class UnitTests
    {
        [DllImport("ehata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata")]
        private static extern void EHATA(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, ref float plb);

        [DllImport("ehata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata_DBG")]
        private static extern void EHATA_DBG(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, ref float plb, ref InterValues intervalues);

        const int PRECISION = 2;

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void NativeTests(TestInput input)
        {
            float plb = 0;
            float[] pfl = input.pfl.ToArray();

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, ref plb);

            Assert.Equal(input.expected_plb, plb, PRECISION);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void NativeTestsDBG(TestInput input)
        {
            float plb_dbg = 0;
            InterValues intervalues = new InterValues();

            float[] pfl = input.pfl.ToArray();
            
            EHATA_DBG(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, ref plb_dbg, ref intervalues);

            Assert.Equal(input.expected_plb, plb_dbg, PRECISION);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void ManagedTests(TestInput input)
        {
            float plb;

            EHata.Invoke(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, out plb);

            Assert.Equal(input.expected_plb, plb, PRECISION);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void ManagedTestsDBG(TestInput input)
        {
            float plb_dbg;
            NTIA.Propagation.EHata.InterValues intervalues;
            
            EHata.Invoke(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, out plb_dbg, out intervalues);

            Assert.Equal(input.expected_plb, plb_dbg, PRECISION);
        }
    }
}
