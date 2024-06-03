using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace AirportCodes {
    internal static class Program {
        static void Main() {
            string code;
            do {
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

                var result = new List<Dictionary<string, string>>();

                if (!string.IsNullOrWhiteSpace(code) && !"Airport".ToUpperInvariant().Contains(code.ToUpperInvariant()) && !"Intl".ToUpperInvariant().Contains(code.ToUpperInvariant())) {
                    result = GetCityCode(code.ToUpperInvariant());
                }

                stopwatch.Stop();

                if (result.Count > 0) {
                    foreach (var t in result) {
                        Console.WriteLine();

                        var tCode = t;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("IATA: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(tCode["IATA"]);
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("ICAO: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(tCode["ICAO"]);

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Airport: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(tCode["Airport"]);

                        Console.WriteLine();
                    }

                    var elapsed = stopwatch.ElapsedMilliseconds;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine();
                    Console.Write("Elapsed: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(elapsed);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("ms");
                    Console.ResetColor();
                } else {
                    Console.WriteLine();
                    Console.WriteLine("NOT FOUND");
                }

                Console.WriteLine();
            } while (code != null && !code.Equals("exit", StringComparison.OrdinalIgnoreCase));
        }

        private static List<Dictionary<string, string>> GetCityCode(string code) {
            var ds = new DataSet();
            ds.ReadXml(AppDomain.CurrentDomain.BaseDirectory + "\\airports.xml");
            ds.CaseSensitive = false;
            DataTable table = ds.Tables[0];
            DataView dvw = table.DefaultView;
            var result = new List<Dictionary<string, string>>();
            switch (code.Length) {
                case 3: {
                    dvw.RowFilter = "ThreeCode='" + code + "'";
                    result = GetCityCode(dvw);
                    break;
                }
                case 4: {
                    dvw.RowFilter = "FourCode='" + code + "'";
                    result = GetCityCode(dvw);
                    break;
                }
            }
            dvw.RowFilter = "AirportName LIKE '% " + code + " %' OR AirportName LIKE '" + code + " %' OR AirportName LIKE '% " + code + "'";
            result.AddRange(GetCityCode(dvw));
            return result;
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
}