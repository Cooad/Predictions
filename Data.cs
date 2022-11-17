using System.Globalization;
using System.Text.Json;

namespace Predictions
{
    internal static class Data
    {
        public static ValueTask<Dictionary<string, Country>> ReadRankings(CancellationToken cancellationToken)
        {
            var fileList = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "rankings"));
            var dates = new List<string>(fileList.Length);
            var database = new Dictionary<string, Country>();
            var minimumPoints = double.MaxValue;

            foreach (var fileName in fileList)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var date = fileName.Split("\\").Last().Replace(".csv", "");
                dates.Add(date);
                using var file = File.OpenRead(fileName);
                using var reader = new StreamReader(file);
                var line = reader.ReadLine();
                while (line != null)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    var splited = line.Split(',');
                    var countryName = splited[0];
                    var points = double.Parse(splited[1], CultureInfo.InvariantCulture);
                    if (minimumPoints > points)
                        minimumPoints = points;
                    if (!database.ContainsKey(countryName))
                        database.Add(countryName, new Country(countryName));
                    database[countryName].AddPoints(date, points);
                    line = reader.ReadLine();
                }
            }
            minimumPoints -= 50;

            foreach (var country in database.Values) { 
                foreach (var date in dates) { 
                    if (!country.ContainsPoints(date))
                        country.AddPoints(date, minimumPoints);
                }
                country.CalculateStrength();
            }

            return ValueTask.FromResult(database);
        }

        public static ValueTask<IEnumerable<IEnumerable<string>>> ReadGroups(CancellationToken cancellationToken)
        {
            using var groupsFile = File.OpenRead(Path.Combine(Environment.CurrentDirectory, "groups.json"));
            return JsonSerializer.DeserializeAsync<IEnumerable<IEnumerable<string>>>(groupsFile, cancellationToken: cancellationToken);
        }
    }
}
