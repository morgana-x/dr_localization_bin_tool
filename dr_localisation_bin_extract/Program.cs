using dr_localisation_bin_extract;

public partial class Program
{ 

    static void Execute(string filePath)
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine($"Extracting {filePath}...");

            LocalizationBin bin = new LocalizationBin(filePath);
            bin.ExtractToFolder(filePath + "_extracted");

            Console.WriteLine("Extracted!");
            return;
        }
        if (Directory.Exists(filePath))
        {
            Console.WriteLine($"Repacking {filePath}...");

            LocalizationBin.RepackFolder(filePath);

            Console.WriteLine("Repacked!");
            return;
        }
        Console.WriteLine($"Could not find file or folder at {filePath}");
    }
    public static void Main(string[] args)
    {

        if (args.Length > 0)
        {
            Execute(args[0]);
            return;
        }
        while (true)
        {
            Console.WriteLine("Drag and drop the dr_localization.bin file to extract\nor the .txt file to repack");
            Execute(Console.ReadLine().Replace("\"", ""));
        }
    }
}
