using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace AirCodes {
    internal abstract class Program {
        private static void Main() {
            string code;

            var airlines = LoadAirlines();
            var airports = LoadAirports();

            do {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">> ");
                code = Console.ReadLine() ?? string.Empty;
                if (code.Equals("exit", StringComparison.OrdinalIgnoreCase) || code.Equals("quit", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                if (code.Equals("clear", StringComparison.OrdinalIgnoreCase) || code.Equals("cls", StringComparison.OrdinalIgnoreCase)) {
                    Console.Clear();
                    continue;
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var resultAirlines = new List<Airlines>();
                var resultAirports = new List<Airports>();

                if (!string.IsNullOrWhiteSpace(code) && !"Airport".ToUpperInvariant().Contains(code.ToUpperInvariant()) && !"Intl".ToUpperInvariant().Contains(code.ToUpperInvariant())) {
                    resultAirlines = GetAirlines(airlines, code);
                    resultAirports = GetAirports(airports, code);
                }

                stopwatch.Stop();

                if (resultAirlines.Count > 0) {
                    Console.WriteLine();
                    Console.WriteLine(resultAirlines.Count == 1 ? "1 Airline: " : resultAirlines.Count + " Airlines: ");

                    foreach (Airlines t in resultAirlines) {
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Airline);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("IATA: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.IATA);
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("ICAO: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.ICAO);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Callsign: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Callsign);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Country);

                        Console.WriteLine();
                    }
                }

                if (resultAirports.Count > 0) {
                    Console.WriteLine();
                    Console.WriteLine(resultAirports.Count == 1 ? "1 Airport: " : resultAirports.Count + " Airports: ");

                    foreach (Airports t in resultAirports) {
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Airport);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("IATA: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.IATA);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("ICAO: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.ICAO);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Location);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(", ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Country);

                        Console.WriteLine();
                    }
                }

                if (resultAirlines.Count > 0 || resultAirports.Count > 0) {
                    var elapsed = stopwatch.ElapsedMilliseconds;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine();
                    Console.Write("Elapsed: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(elapsed);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("ms");
                    Console.ResetColor();
                }

                if (resultAirlines.Count == 0 && resultAirports.Count == 0) {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine();
                    Console.WriteLine("NO RESULT");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
            } while (!code.Equals("exit", StringComparison.OrdinalIgnoreCase));
        }

        private static List<Airports> LoadAirports() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Resources.airports.csv";
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null) {
                using var reader = new StreamReader(stream);
                using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                var records = csvReader.GetRecords<Airports>().ToList();
                return records;
            }
            return new List<Airports>();
        }

        private static List<Airlines> LoadAirlines() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Resources.airlines.csv";
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null) {
                using var reader = new StreamReader(stream);
                using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                var records = csvReader.GetRecords<Airlines>().ToList();
                return records;
            }
            return new List<Airlines>();
        }

        private static List<Airlines> GetAirlines(List<Airlines> airlines, string code) {
            var result = new List<Airlines>();
            foreach (Airlines airline in airlines) {
                var pattern = @"\b" + code + @"\b";
                MatchCollection matches = Regex.Matches(airline.Airline, pattern, RegexOptions.IgnoreCase);
                if (matches.Count > 0) {
                    result.Add(airline);
                }
                MatchCollection matchesCountry = Regex.Matches(airline.Country, pattern, RegexOptions.IgnoreCase);
                if (matchesCountry.Count > 0) {
                    result.Add(airline);
                }
                if (airline.IATA.Equals(code, StringComparison.OrdinalIgnoreCase) || airline.ICAO.Equals(code, StringComparison.OrdinalIgnoreCase) || airline.Callsign.Equals(code, StringComparison.OrdinalIgnoreCase)) {
                    result.Add(airline);
                }
            }
            return result.Distinct().ToList();
        }

        private static List<Airports> GetAirports(List<Airports> airports, string code) {
            var result = new List<Airports>();
            foreach (Airports airport in airports) {
                var pattern = @"\b" + code + @"\b";
                MatchCollection matchesAirport = Regex.Matches(airport.Airport, pattern, RegexOptions.IgnoreCase);
                if (matchesAirport.Count > 0) {
                    result.Add(airport);
                }
                MatchCollection matchesLocation = Regex.Matches(airport.Location, pattern, RegexOptions.IgnoreCase);
                if (matchesLocation.Count > 0) {
                    result.Add(airport);
                }
                MatchCollection matchesCountry = Regex.Matches(airport.Country, pattern, RegexOptions.IgnoreCase);
                if (matchesCountry.Count > 0) {
                    result.Add(airport);
                }
                if (airport.IATA.Equals(code, StringComparison.OrdinalIgnoreCase) || airport.ICAO.Equals(code, StringComparison.OrdinalIgnoreCase)) {
                    result.Add(airport);
                }
            }
            return result.Distinct().ToList();
        }
    }

    internal class Airlines {
        public string IATA { get; }
        public string ICAO { get; }
        public string Airline { get; }
        public string Callsign { get; }
        public string Country { get; }
    }

    internal class Airports {
        public string IATA { get; }
        public string ICAO { get; }
        public string Location { get; }
        public string Airport { get; }
        public string Country { get; }
    }
}