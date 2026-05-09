using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();

var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);
var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

//var salesJson = File.ReadAllText($"stores{Path.DirectorySeparatorChar}201{Path.DirectorySeparatorChar}sales.json");

GenerateSalesReport(salesTotal, salesFiles, currentDirectory);

IEnumerable<string> FindFiles(string folderName)
{
  List<string> salesFiles = new List<string>();
  var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

  foreach (var file in foundFiles)
  {
    var extension = Path.GetExtension(file);

    if (extension == ".json" && Path.GetFileName(file) == "sales.json")
    {
      salesFiles.Add(file);
    }
  }
  return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
  double salesTotal = 0;

  foreach (var file in salesFiles)
  {
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    salesTotal += data?.Total ?? 0;
  }
  return salesTotal;
}


static void GenerateSalesReport(double salesTotal, IEnumerable<string> salesFiles, string currentDirectory)
{
  var salesReportDir = Path.Combine(currentDirectory, "salesReport");
  Directory.CreateDirectory(salesReportDir);

  StringBuilder report = new StringBuilder("Sales Summary \n----------------------");
  report.AppendLine();
  report.AppendFormat("Total Sales: {0:C}", salesTotal);
  report.AppendLine();
  report.AppendLine();
  report.Append("Details:");
  report.AppendLine();
  
  foreach (var file in salesFiles)
  {
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

    report.AppendFormat("{0}: {1:C}", Path.GetFileName(Path.GetDirectoryName(file) + " " + Path.GetFileName(file)), data?.Total ?? 0);
    report.AppendLine();
  }
  File.WriteAllText(Path.Combine(salesReportDir, "report.txt"), report.ToString());
}
record SalesData(double Total);

