using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVT
{
    public class TestInput
    {
        public int ID { get; set; }

        // eHata model inputs
        public List<float> pfl { get; set; } = new List<float>();
        public float f__mhz { get; set; }
        public float h_b__meter { get; set; }
        public float h_m__meter { get; set; }
        public int enviro_code { get; set; }

        // expected eHata result
        public float expected_plb { get; set; }

        // path endpoint locations
        public float tx_lat { get; set; }
        public float tx_lon { get; set; }
        public float rx_lat { get; set; }
        public float rx_lon { get; set; }

        // additional info
        public string scenario_title { get; set; }
        public string pfl_csv { get; set; }
        public float d__km { get; set; }
    }
}
