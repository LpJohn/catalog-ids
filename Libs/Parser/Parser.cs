using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Parser;

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
            }
            return idList;
        }

        private string _outputFileName;
        private string[] _masks;

        public void OutputIdCatalogByFolder(string inputFolderPath, string outputFolderPath, string masks)
        {
            var ignoreFile = ConfigurationManager.AppSettings["IgnoreFile"];
            _outputFileName = Path.Combine(outputFolderPath, "id-catalog.json");
            _masks = masks.Split(",".ToCharArray());
            var ignoredFiles = File.ReadAllLines(ignoreFile);
            ProcessFolder(inputFolderPath, ignoredFiles);
        }

        private void ProcessFolder(string inputFolderPath, string[] ignoredFiles)
        {
            var list = new List<ListModel>();
            var files = _masks.SelectMany(mask => Directory.GetFiles(inputFolderPath, mask)).ToArray();
            foreach (var file in files)
            {
                if (ignoredFiles.Any(f => file.EndsWith(f)))
                    continue;
                var fileLines = File.ReadAllLines(file);
                var idList = new List<string>();
                foreach (var line in fileLines)
                {
                    idList.AddRange(GetIds(line));
                }
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

            var directories = Directory.GetDirectories(inputFolderPath);
            foreach (var directory in directories)
                ProcessFolder(directory, ignoredFiles);
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
