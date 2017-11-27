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
        private static extern void EHATA(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, float reliability, ref float plb);

        [DllImport("ehata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "ExtendedHata_DBG")]
        private static extern void EHATA_DBG(float[] pfl, float f__mhz, float h_b__meter, float h_m__meter, int enviro_code, float reliability, ref float plb, ref InterValues intervalues);

        const int PRECISION = 2;

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void NativeTests(TestInput input)
        {
            float plb_med__db = 0;
            float plb_pts__db = 0;      // positive two sigmas loss
            float plb_nts__db = 0;      // negative two sigmas loss

            float median = 0.5f;
            float pts = 0.977f;         // positive two sigmas
            float nts = 0.032f;         // nagative two sigmas

            float[] pfl = input.pfl.ToArray();

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, median, ref plb_med__db);

            Assert.Equal(input.expected_plb, plb_med__db, PRECISION);

            // These following two tests are simply basic sanity checks - not validation against any numerical results

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, pts, ref plb_pts__db);

            Assert.True(plb_pts__db > plb_med__db);

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, nts, ref plb_nts__db);

            Assert.True(plb_nts__db < plb_med__db);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void NativeTestsDBG(TestInput input)
        {
            float plb_med__db = 0;
            float plb_pts__db = 0;      // positive two sigmas loss
            float plb_nts__db = 0;      // negative two sigmas loss

            float median = 0.5f;
            float pts = 0.977f;         // positive two sigmas
            float nts = 0.032f;         // nagative two sigmas

            InterValues intervalues = new InterValues();

            float[] pfl = input.pfl.ToArray();

            EHATA_DBG(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, median, ref plb_med__db, ref intervalues);

            Assert.Equal(input.expected_plb, plb_med__db, PRECISION);

            // These following two tests are simply basic sanity checks - not validation against any numerical results

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, pts, ref plb_pts__db);

            Assert.True(plb_pts__db > plb_med__db);

            EHATA(pfl, input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, nts, ref plb_nts__db);

            Assert.True(plb_nts__db < plb_med__db);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void ManagedTests(TestInput input)
        {
            float plb_med__db = 0;
            float plb_pts__db = 0;      // positive two sigmas loss
            float plb_nts__db = 0;      // negative two sigmas loss

            float median = 0.5f;
            float pts = 0.977f;         // positive two sigmas
            float nts = 0.032f;         // nagative two sigmas

            EHata.Invoke(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, median, out plb_med__db);

            Assert.Equal(input.expected_plb, plb_med__db, PRECISION);

            // These following two tests are simply basic sanity checks - not validation against any numerical results

            EHATA(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, pts, ref plb_pts__db);

            Assert.True(plb_pts__db > plb_med__db);

            EHATA(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, nts, ref plb_nts__db);

            Assert.True(plb_nts__db < plb_med__db);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.UnitTestData), MemberType = typeof(TestDataGenerator))]
        public void ManagedTestsDBG(TestInput input)
        {
            float plb_med__db = 0;
            float plb_pts__db = 0;      // positive two sigmas loss
            float plb_nts__db = 0;      // negative two sigmas loss

            float median = 0.5f;
            float pts = 0.977f;         // positive two sigmas
            float nts = 0.032f;         // nagative two sigmas

            NTIA.Propagation.EHata.InterValues intervalues;

            EHata.Invoke(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, median, out plb_med__db, out intervalues);

            Assert.Equal(input.expected_plb, plb_med__db, PRECISION);

            // These following two tests are simply basic sanity checks - not validation against any numerical results

            EHATA(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, pts, ref plb_pts__db);

            Assert.True(plb_pts__db > plb_med__db);

            EHATA(input.pfl.ToArray(), input.f__mhz, input.h_b__meter, input.h_m__meter, input.enviro_code, nts, ref plb_nts__db);

            Assert.True(plb_nts__db < plb_med__db);
        }
    }
}
