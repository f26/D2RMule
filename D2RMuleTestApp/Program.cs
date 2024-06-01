using D2RMuleLib;
using System.IO;

namespace D2RMuleTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // A console app used for testing things without having to use the GUI.  Feel free to change any
            // or all things in here, this has no bearing on the main D2RMule GUI.

            string folderPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + @"\D2RMule\_\saves\";
            ParseAllCharacter(folderPath);
        }

        static void ParseAllCharacter(string folderPath)
        {
            string targetExtension = ".d2s";
            string[] files = Directory.GetFiles(folderPath, $"*{targetExtension}");

            // Iterate over the file names
            int counter = 0;
            foreach (string file in files)
            {
                Console.WriteLine("##############################################################");
                Console.WriteLine("Opening " + Path.GetFileName(file));
                D2SFile d2SFile = new D2SFile(file);
                counter++;
            }

            Console.WriteLine("Processed " + counter.ToString() + " files");
        }
    }
}
