using Predictions;

var cts = new CancellationTokenSource();
var nrOfSims = 1000000;

Console.CancelKeyPress += (s, e) =>
{
    cts.Cancel(true);
    e.Cancel = true;
};

var database = await Data.ReadRankings(cts.Token);
var groups = await Data.ReadGroups(cts.Token);
var random = new GausianRandom();
var points = database.ToDictionary(x => x.Key, x => 0.0);
foreach (var group in groups)
{
    var pairs = GetPairs(group);
    foreach (var pair in pairs)
    {
        var countryA = database[pair.CountryA];
        var countryB = database[pair.CountryB];
        var matchPointsA = 0.0;
        var matchPointsB = 0.0;
        for (var i = 0; i < nrOfSims; i++)
        {
            var strengthA = random.Next(countryA);
            var strengthB = random.Next(countryB);

            var normalizedA = strengthA / (strengthA + strengthB);
            var normalizedB = strengthB / (strengthA + strengthB);

            if (Math.Abs(normalizedB - normalizedA) < 0.05)
            {
                matchPointsA += 1;
                matchPointsB += 1;
            }
            else if (normalizedA > normalizedB)
            {
                matchPointsA += 3;
            }
            else
            {
                matchPointsB += 3;
            }
        }
        points[pair.CountryA] += matchPointsA / nrOfSims;
        points[pair.CountryB] += matchPointsB / nrOfSims;

    }
    Console.WriteLine("Group:");
    var sortedGroup = group.Select(x => (x, points[x])).OrderByDescending(x => x.Item2);
    foreach (var country in sortedGroup)
        Console.WriteLine($"{country.Item1}: {Math.Round(country.Item2, 6)}");
}

if (cts.Token.IsCancellationRequested)
    return;
Console.WriteLine("Finished");
Console.ReadLine();

static IEnumerable<(string CountryA, string CountryB)> GetPairs(IEnumerable<string> countries)
{
    var count = countries.Count();
    var sorted = countries.OrderBy(x => x).ToArray();
    Console.WriteLine(string.Join(", ", sorted));
    for (var i = 0; i < count - 1; i++)
        for (var j = i + 1; j < count; j++)
            yield return (sorted[i], sorted[j]);
}