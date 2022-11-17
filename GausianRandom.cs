namespace Predictions
{
    internal class GausianRandom
    {
        private readonly Random _random;

        public GausianRandom()
        {
            _random = new Random();
        }

        public double Next(double mean, double stdDev)
        {
            double u1 = 1.0 - _random.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }

        public double Next(Country country) => Next(country.Strength, country.StandardDeviation);
    }
}
