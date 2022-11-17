namespace Predictions
{
    internal class Country
    {
        public string Name { get; set; }
        private Dictionary<string, double> Points { get; set; }
        public double Strength { get; private set; }
        public double StandardDeviation { get; private set; }

        public Country(string countryName)
        {
            Name = countryName;
            Points = new();
        }

        public void AddPoints(string when, double points)
        {
            Points.Add(when, points);
        }

        public double GetPoints(string when)
        {
            return Points[when];
        }

        public bool ContainsPoints(string when)
        {
            return Points.ContainsKey(when);
        }

        public void CalculateStrength()
        {
            Strength = Points.Values.Sum() / Points.Count;
            StandardDeviation = Points.Values.Select(x => Strength - x).Select(x => x * x).Sum();
            StandardDeviation /= Points.Count;
            StandardDeviation = Math.Sqrt(StandardDeviation);
        }
    }
}
