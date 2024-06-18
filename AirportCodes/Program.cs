using AirportCodes.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;

namespace AirportCodes {
    internal abstract class Program {
        private static void Main() {
            string code;

            var airlines = LoadAirlines() as List<Airlines>;
            var airports = LoadAirports() as List<Airports>;

            do {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(">> ");
                code = Console.ReadLine();
                if (code == null || code.Equals("exit", StringComparison.OrdinalIgnoreCase) || code.Equals("quit", StringComparison.OrdinalIgnoreCase)) {
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
                        Console.Write(@"@");
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
                        Console.Write(@"@");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(t.Location);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(@", ");
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
                    Console.WriteLine("NO RESULT");
                }

                Console.WriteLine();
            } while (code != null && !code.Equals("exit", StringComparison.OrdinalIgnoreCase));
        }

        private static object LoadAirports() {
            using (var sReader = new StringReader(Resources.airports)) {
                using (var csvReader = new CsvReader(sReader, CultureInfo.InvariantCulture)) {
                    return csvReader.GetRecords<Airports>().ToList();
                }
            }
        }

        private static object LoadAirlines() {
            using (var sReader = new StringReader(Resources.airlines)) {
                using (var csvReader = new CsvReader(sReader, CultureInfo.InvariantCulture)) {
                    return csvReader.GetRecords<Airlines>().ToList();
                }
            }
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

        private static string CTrim(object inputs) {
            inputs = inputs ?? string.Empty;
            return inputs == DBNull.Value ? string.Empty : inputs.ToString().Trim();
        }

        private static List<Dictionary<string, string>> GetCityCode(DataView dvw) {
            var result = new List<Dictionary<string, string>>();
            if (dvw.Count != 0) {
                result.AddRange(from DataRowView record in dvw
                                select new Dictionary<string, string> {
                                    {
                                        "IATA", CTrim(record["ThreeCode"])
                                    }, {
                                        "ICAO", CTrim(record["FourCode"])
                                    }, {
                                        "Airport", CTrim(record["AirportName"])
                                    }
                                });
            }
            return result;
        }
    }

    internal class Airlines {
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public string Airline { get; set; }
        public string Callsign { get; set; }
        public string Country { get; set; }
    }

    internal class Airports {
        public string IATA { get; set; }
        public string ICAO { get; set; }
        public string Location { get; set; }
        public string Airport { get; set; }
        public string Country { get; set; }
    }
}