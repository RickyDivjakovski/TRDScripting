using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace TRDScriptLib
{
    public class TRDScriptProcessor
    {
        private string workingDirectory = "";

        public bool Process(string inputScript)
        {
            bool success = true;

            if (File.Exists(Path.GetDirectoryName(inputScript) + "\\server.cfg")) workingDirectory = Path.GetDirectoryName(inputScript);
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                if (!File.Exists(Path.GetDirectoryName(Path.GetDirectoryName(inputScript)) + "sever.cfg"))
                {
                    workingDirectory = Path.GetDirectoryName(Path.GetDirectoryName(inputScript));
                }
            }

            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                foreach (string file in Directory.GetFiles(Path.GetDirectoryName(inputScript), "server.cfg", SearchOption.AllDirectories))
                {
                    workingDirectory = Path.GetDirectoryName(file);
                }
            }

            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                Console.WriteLine("Error: Could not determine project root.");
                Console.WriteLine("       Please make sure your TRDScript is in the project root or in a folder in the root directory.");
            }
            else
            {
                string script = "";

                try
                {
                    StreamReader reader = new StreamReader(inputScript);
                    script = reader.ReadToEnd().Replace("\r\n", "\n");
                    reader.Close();
                    reader.Dispose();

                    foreach (string line in script.Split('\n'))
                    {
                        if (success == true)
                        {
                            string formattedLine = line.Replace("^,", "@|").Replace("^(", "@[").Replace("^)", "@]");
                            string[] args = formattedLine.Replace("/", "\\").Split('(').Last().Split(')').First().Replace(", ", ",").Split(',');
                            if (line.StartsWith("ModifyFile")) if (args.Length == 5) success = ModifyFile(FixArg(args[0]), FixArg(args[1]), FixArg(args[2]), FixArg(args[3]), FixArg(args[4]));
                            if (line.StartsWith("ImportSql")) if (args.Length == 1) success = ImportSQL(FixArg(args[0]));
                            if (line.StartsWith("ExtractZip")) if (args.Length == 2) success = ExtractZip(FixArg(args[0]), FixArg(args[1]));
                            if (line.StartsWith("MoveFile")) if (args.Length == 2) success = MoveFile(FixArg(args[0]), FixArg(args[1]));
                            if (line.StartsWith("MoveFolder")) if (args.Length == 2) success = MoveFolder(FixArg(args[0]), FixArg(args[1]));
                            if (line.StartsWith("CreateFile")) if (args.Length == 1) success = CreateFile(FixArg(args[0]));
                            if (line.StartsWith("CreateFolder")) if (args.Length == 1) success = CreateFolder(FixArg(args[0]));
                        }
                    }
                }
                catch
                {
                    success = false;
                }
                return success;
            }
            return false;
        }

        private string FixArg(string arg)
        {
            return arg.Replace("@|", ",").Replace("@[", "(").Replace("@]", ")");
        }

        private bool CreateFile(string file)
        {
            if (!File.Exists(workingDirectory + "\\" + file))
            {
                if (Directory.Exists(Path.GetDirectoryName(workingDirectory + "\\" + file)))
                {
                    File.Create(workingDirectory + "\\" + file).Dispose();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private bool CreateFolder(string directory)
        {
            if (!Directory.Exists(workingDirectory + "\\" + directory))
            {
                if (Directory.Exists(Path.GetDirectoryName(workingDirectory + "\\" + directory)))
                {
                    Directory.CreateDirectory(workingDirectory + "\\" + directory);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private bool MoveFolder(string inputFolder, string outputFolder)
        {
            if (workingDirectory + "\\" + inputFolder != workingDirectory + "\\" + outputFolder)
            {
                if (Directory.Exists(workingDirectory + "\\" + inputFolder))
                {
                    if (Directory.Exists(workingDirectory + "\\" + outputFolder))
                    {
                        try
                        {
                            Directory.Move(workingDirectory + "\\" + inputFolder, workingDirectory + "\\" + outputFolder + "\\" + Path.GetFileName(workingDirectory + "\\" + inputFolder));
                            return true;
                        }
                        catch (Exception e) { Console.WriteLine(e); return false; }
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        private bool MoveFile(string inputFile, string outputFile)
        {
            if (workingDirectory + "\\" + inputFile != workingDirectory + "\\" + outputFile)
            {
                if (File.Exists(workingDirectory + "\\" + inputFile))
                {
                    if (!File.Exists(workingDirectory + "\\" + outputFile))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(workingDirectory + "\\" + outputFile)))
                        {
                            try
                            {
                                File.Move(workingDirectory + "\\" + inputFile, workingDirectory + "\\" + outputFile);
                                return true;
                            }
                            catch { return false; }
                        }
                        else return false;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        private bool ExtractZip(string zipFile, string outputPath)
        {
            if (!Directory.Exists(workingDirectory + "\\" + outputPath))
            {
                if (Directory.Exists(Path.GetDirectoryName(workingDirectory + "\\" + outputPath))) Directory.CreateDirectory(workingDirectory + "\\" + outputPath);
            }

            if (Directory.Exists(workingDirectory + "\\" + outputPath))
            {
                try
                {
                    ZipFile.ExtractToDirectory(workingDirectory + "\\" + zipFile, workingDirectory + "\\" + outputPath);
                    return true;
                }
                catch { return false; }
            }
            else return false;
        }

        private bool ImportSQL(string sqlScript)
        {
            bool success = true;
            string serverConfig = "";

            if (File.Exists(workingDirectory + "\\server.cfg")) serverConfig = workingDirectory + "\\server.cfg";

            StreamReader reader = new StreamReader(serverConfig);
            string cfg = reader.ReadToEnd().Replace("\r\n", "\n");
            reader.Close();
            reader.Dispose();

            string database = "";
            foreach (string line in cfg.Split('\n')) if (line.Contains("connection_string")) database = line.Split('?').First().Split('/').Last();

            if (!string.IsNullOrWhiteSpace(serverConfig) && !string.IsNullOrWhiteSpace(database))
            {
                MySqlConnection connection = new MySqlConnection("Server=localhost;database=" + database.ToLower() + ";uid=root;pwd=;");
                connection.Open();

                try
                {
                    string script = File.ReadAllText(workingDirectory + "\\" + sqlScript);

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { success = false; }
                connection.Close();
            }

            return success;
        }

        private bool ModifyFile(string file, string action, string searchMethod, string find, string replace)
        {
            bool success = true;

            try
            {
                string filePath = workingDirectory + "\\" + file;
                if (File.Exists(filePath) && !string.IsNullOrWhiteSpace(action) && !string.IsNullOrWhiteSpace(searchMethod) && !string.IsNullOrWhiteSpace(find) && !string.IsNullOrWhiteSpace(replace))
                {
                    StreamReader reader = new StreamReader(filePath);
                    string data = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();

                    string eol = "\n";
                    if (data.Contains("\r\n")) eol = "\r\n";

                    data = data.Replace("\r\n", "\n");

                    string outputdata = "";

                    if (action == "ReplaceLine")
                    {
                        if (searchMethod == "LineStarts")
                        {
                            foreach (string line in data.Split('\n'))
                            {
                                if (line.StartsWith(find)) outputdata = outputdata + replace + eol;
                                else outputdata = outputdata + line + eol;
                            }
                        }
                        if (searchMethod == "LineEnds")
                        {
                            foreach (string line in data.Split('\n'))
                            {
                                if (line.EndsWith(find)) outputdata = outputdata + replace + eol;
                                else outputdata = outputdata + line + eol;
                            }
                        }
                        if (searchMethod == "Contains")
                        {
                            foreach (string line in data.Split('\n'))
                            {
                                if (line.Contains(find)) outputdata = outputdata + replace + eol;
                                else outputdata = outputdata + line + eol;
                            }
                        }
                    }
                    if (action == "ReplaceText")
                    {
                        if (searchMethod == "Contains")
                        {
                            outputdata = data.Replace(find, replace);
                        }
                    }
                    if (action == "ConvertEOL")
                    {
                        if (!string.IsNullOrWhiteSpace(searchMethod))
                        {
                            if (find.ToLower() == "windows" && replace.ToLower() == "unix") ;
                            {
                                outputdata = data.Replace("\r\n", "\n");
                            }
                            if (find.ToLower() == "unix" && replace.ToLower() == "windows") ;
                            {
                                outputdata = data.Replace("\r\n", "\n");
                                outputdata = outputdata.Replace("\n", "\r\n");
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(outputdata))
                    {
                        StreamWriter writer = new StreamWriter(filePath);
                        writer.Write(outputdata);
                        writer.Close();
                        writer.Dispose();
                    }
                }
            } catch { success = false; }
            return success;
        }
    }
}
