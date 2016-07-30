using System;

namespace CatalogIds
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var border = new string('*', 75);

            if (args.Length < 3)
            {
                Console.WriteLine(border);
                Console.WriteLine("Usage");
                Console.WriteLine(border);
                Console.WriteLine("CatalogIds <input path> <output path> <file mask>");
                Console.WriteLine("Example:");
                Console.WriteLine(@"CatalogIds ""\\machine\folder\sub-folder"" ""c:\local-folder\sub-folder"" ""*.as?x,*.cshtml""");
            }

            var inputPath = args[0];
            var outputPath = args[1];
            var mask = args[2];
            Console.WriteLine(border);
            Console.WriteLine("Cataloguing Element IDs in {0} and its sub-folders for files that match {1}", inputPath, mask);
            Console.WriteLine(border);

            var parser = new ParserLib.Parser();
            parser.OutputIdCatalogByFolder(inputPath, outputPath, mask);

            Console.WriteLine("Cataloguing complete!");
            Console.WriteLine("Catalogue written to {0}\\{1} folder", outputPath, "id-catalogs.json");
        }
    }
}
