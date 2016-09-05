using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelGen
{
    class Program
    {
        const int TYPE_BLOCK = 1;
        const int TYPE_PICKUP_TOKEN = 2;

        static void Main(string[] args)
        {
            var searchDir = args[0];
            var outputFilename = args[1];
            var json = new JArray();
            foreach (var imageFilename in Directory.GetFiles(searchDir, "*.png"))
            {
                json.Add(GenerateMap(new Bitmap(Bitmap.FromFile(imageFilename))));
            }

            var jsonContent = JsonConvert.SerializeObject(json);
            File.WriteAllText(outputFilename, jsonContent);
        }

        public static JObject GenerateMap(Bitmap image)
        {
            var map = new JObject();
            var items = new JArray();
            int length = image.Width;
            int height = image.Height;
            var bits = new List<int>(length * height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    var pixel = image.GetPixel(x, y);
                    var color = $"{pixel.R.ToString("X2")}{pixel.G.ToString("X2")}{pixel.B.ToString("X2")}";
                    switch (color)
                    {
                        case "FFFF00":
                            {
                                map.Add(new JProperty("s",new JArray(x, y)));
                                break;
                            }
                        case "FF0000":
                            {
                                items.Add(new JArray(TYPE_PICKUP_TOKEN, x, y));
                                break;
                            }
                        case "FFFFFF":
                            {
                                items.Add(new JArray(TYPE_BLOCK, x, y));
                                break;
                            }
                    }
                }
            }

            map.Add(new JProperty("i", items));
            return map;
        }
    }
}
