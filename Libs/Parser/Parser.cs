using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Parser;

// TODO: need ignores list
// TODO: need to handle multiple masks

namespace ParserLib
{
    public class Parser
    {
        public List<string> GetIds(string html)
        {
            var idList = new List<string>();
            var matches = Regex.Matches(html, @"\Wid(\W+)?\=(\W+)?(\w+)\W", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                idList.Add(match.Groups[match.Groups.Count - 1].Value);
                Console.WriteLine(match.Groups[match.Groups.Count - 1].Value);
            }
            return idList;
        }

        private string _outputFileName;
        private string[] _masks;

        public void OutputIdCatalogByFolder(string folderPath, string masks)
        {
            _outputFileName = Path.Combine(folderPath, "id-catalog.json");
            _masks = masks.Split(",".ToCharArray());
            ProcessFolder(folderPath);
        }

        private void ProcessFolder(string folderPath)
        {
            var list = new List<ListModel>();
            var files = _masks.SelectMany(mask => Directory.GetFiles(folderPath, mask)).ToArray();
            foreach (var file in files)
            {
                Console.WriteLine("****" + file + "****");
                var fileLines = File.ReadAllLines(file);
                var idList = new List<string>();
                foreach (var line in fileLines)
                {
                    idList.AddRange(GetIds(line));
                }
                idList.ForEach(Console.WriteLine);
                if (idList.Any())
                {
                    list.Add(new ListModel {FileName = file, IdList = idList});
                }
            }

            if (list.Any())
            {
                GetPreviousFiles(list);
                var json = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(_outputFileName, json);
            }

            var directories = Directory.GetDirectories(folderPath);
            foreach (var directory in directories)
                ProcessFolder(directory);
        }

        private void GetPreviousFiles(List<ListModel> list)
        {
            List<ListModel> previousFiles = null;
            if (File.Exists(_outputFileName))
            {
                var previousJson = File.ReadAllText(_outputFileName);
                previousFiles = JsonConvert.DeserializeObject<List<ListModel>>(previousJson);
            }

            if (previousFiles != null)
                list.InsertRange(0, previousFiles);
        }
    }
}
