using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);
var salesData = CalculateSalesTotal(salesFiles);

double totalSales = 0;
foreach (var value in salesData.Values)
{
    totalSales += value;
}

GenerateSalesReport(salesTotalDir, totalSales, salesData);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{totalSales}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName) =>
    Directory.EnumerateFiles(folderName, "*.json", SearchOption.AllDirectories);

Dictionary<string, double> CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    var salesData = new Dictionary<string, double>();

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        salesData[file] = data?.Total ?? 0;
    }

    return salesData;
}

void GenerateSalesReport(string directory, double totalSales, Dictionary<string, double> salesData)
{
    string reportFilePath = Path.Combine(directory, "sales_summary.txt");

    using (StreamWriter writer = new StreamWriter(reportFilePath))
    {
        writer.WriteLine("Sales Summary");
        writer.WriteLine("----------------------------");
        writer.WriteLine($"Total Sales: {totalSales.ToString("C", CultureInfo.CurrentCulture)}");
        writer.WriteLine("\nDetails:");

        foreach (var entry in salesData)
        {
            string fileName = Path.GetFileName(entry.Key);
            writer.WriteLine($"  {fileName}: {entry.Value.ToString("C", CultureInfo.CurrentCulture)}");
        }
    }
}

record SalesData(double Total);
