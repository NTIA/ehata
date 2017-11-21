using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BVT
{
    public class TestDataGenerator
    {
        private const string TEST_ROOT_DIR = "Assets";
        private const string TEST_PFL_DIR = "pfls";
        private const string TEST_DATA_INPUTS = "test-inputs.csv";

        public static IEnumerable<object[]> UnitTestData()
        {
            string inputPath = Path.Combine(Directory.GetCurrentDirectory(), TEST_ROOT_DIR, TEST_DATA_INPUTS);
            string pflDir = Path.Combine(Directory.GetCurrentDirectory(), TEST_ROOT_DIR, TEST_PFL_DIR);

            var lines = File.ReadAllLines(inputPath);

            for (int i = 1; i < lines.Count(); i++)
            {
                var input = Parse(lines[i]);
                input.pfl = LoadPfl(Path.Combine(pflDir, input.pfl_csv));

                yield return new object[]
                    {
                        input
                    };
            }
        }

        private static TestInput Parse(string line)
        {
            var input = new TestInput();

            var parts = line.Split(',');

            input.ID = Convert.ToInt32(parts[0]);
            input.scenario_title = parts[1];
            input.f__mhz = Convert.ToSingle(parts[2]);
            input.h_b__meter = Convert.ToSingle(parts[3]);
            input.h_m__meter = Convert.ToSingle(parts[4]);
            input.enviro_code = Convert.ToInt32(parts[5]);
            input.pfl_csv = parts[6];
            input.tx_lat = Convert.ToSingle(parts[7]);
            input.tx_lon = Convert.ToSingle(parts[8]);
            input.rx_lat = Convert.ToSingle(parts[9]);
            input.rx_lon = Convert.ToSingle(parts[10]);
            input.d__km = Convert.ToSingle(parts[11]);
            input.expected_plb = Convert.ToSingle(parts[12]);



            return input;
        }

        private static List<float> LoadPfl(string path)
        {
            var data = File.ReadAllText(path).Split(',');

            var pfl = new List<float>();
            foreach (string d in data)
                pfl.Add(Convert.ToSingle(d));

            return pfl;
        }
    }
}
