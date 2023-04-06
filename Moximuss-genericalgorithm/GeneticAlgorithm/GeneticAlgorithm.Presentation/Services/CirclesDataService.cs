using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm.CirclesChromosome;

namespace GeneticAlgorithm.Presentation.Services
{
    public class CirclesDataService
    {
        public static IEnumerable<Circle> ReadFromFile(string name, string path = null)
        {
            if (path == null)
                path = System.IO.Directory.GetCurrentDirectory();

            return null;
        }

        public static List<Circle> GetRandomCircles(int count, double minRad, double maxRad)
        {
            var rnd = new Random();
            var result = new List<Circle>();
            for (int i = 0; i < count; i++)
            {
                var circle = new Circle();
             //   circle.Radius = rnd.Next((int)minRad, (int)maxRad);
                circle.Radius = rnd.Next(50, 70);
                result.Add(circle);
            }
            return result;
        }
    }
}
