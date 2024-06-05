using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueToCsvSO", menuName = "DialogueToCsvSO")]
public class DialogueToCsvSO : ScriptableObject
{
    private List<(string, string)> sentenceInfo = new List<(string, string)>();
    private List<string> csvRows = new List<string>();
    private string lastUpdated;
    public string LastUpdated => lastUpdated;

    HashSet<string> ignoreSet = new HashSet<string>() {
        "Yes", "No",
        "/cut", "/ignore", "<br>", "<i>", "</i>",
        "ExitShop", "ConfirmPurchase",
    };
    public void ExecuteUpdate()
    {
        string dialogueRootPath = Application.dataPath + "/Resources/Dialogue/eng";
        string csvPath = Application.dataPath + "/Resources/Dialogue/DialogueSheet.csv";

        ExtractTextFromFiles(dialogueRootPath);
        Dictionary<string, int> existingDict = ReturnKeysFromCsv(csvPath);
        EraseOutdatedKeys(existingDict);

        for (int i = 0; i < sentenceInfo.Count; i++) {
            if (sentenceInfo[i].Item2 == "\n")
                csvRows.Insert(i + 1, "");
            
            else if (!existingDict.ContainsKey(sentenceInfo[i].Item2)) {
                string currentSentence = sentenceInfo[i].Item2;
                currentSentence = currentSentence.Replace("\"", "\"\"");
                csvRows.Insert(i + 1, $"\"{sentenceInfo[i].Item1}\",\"{currentSentence}\"");
            }
        }
        File.WriteAllLines(csvPath, csvRows.ToArray());
        lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        sentenceInfo.Clear();
        csvRows.Clear();
    }
    private void ExtractTextFromFiles(string path)
    {
        sentenceInfo.Clear();
        sentenceInfo.Add(("_", "Yes"));
        sentenceInfo.Add(("_", "No"));

        if (Directory.Exists(path))
        {
            IEnumerable<string> jsonFiles = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (string filePath in jsonFiles)
            {
                try {
                    string fileContent = File.ReadAllText(filePath);
                    FilterPureSentences(fileContent);
                }
                catch (Exception e) {
                    Debug.LogError($"Error reading file {filePath}: {e.Message}");
                }
            }
        }
        else
            Debug.LogError("Root directory not found.");
    }
    private void FilterPureSentences(string rawText)
    {
        rawText = rawText.Replace("\",\"ev\",{\"VAR?\":\"br\"},\"out\",\"/ev\",\"^", "<br>");
        string[] blocks = rawText.Split(new string[] {"\"\\n\""}, StringSplitOptions.None);
        string pattern = "\"\\^.*?\",";
        string portraitPattern = "portrait:.*?\"";
        string previousSpeaker = "";

        foreach (string block in blocks) {
            MatchCollection matches = Regex.Matches(block, pattern);

            foreach (Match match in matches)
            {
                string sentence = match.Value;
                sentence = sentence.Substring(2, sentence.Length - 4);

                if (!ignoreSet.Contains(sentence)) {
                    sentence = sentence.Replace("\\\"", "\"");
                    string speaker = "_";

                    if (block.Contains("portrait:")) {
                        Match tag = Regex.Match(block, portraitPattern);
                        speaker = tag.Value.Split(':')[1];
                        speaker = speaker.Remove(speaker.Length - 1);

                        if (speaker == "_")
                            speaker = "NPC";
                    }
                    if (sentence.Contains("*"))
                        speaker = "Narration";
                    else if (previousSpeaker == "Narration" && speaker == "_")
                        speaker = "NPC";

                    if (previousSpeaker != "" && previousSpeaker == speaker)
                        speaker = "_";
                    else
                        previousSpeaker = speaker;

                    sentenceInfo.Add((speaker, sentence));
                }
            }
        }
        sentenceInfo.Add(("\n", "\n"));
    }
    private Dictionary<string, int> ReturnKeysFromCsv(string path)
    {
        csvRows = new List<string>(File.ReadAllLines(path));
        
        for (int i = csvRows.Count - 1; i > 0; i--) {
            if (csvRows[i] == "")
                csvRows.RemoveAt(i);
        }
        Dictionary<string, int> existingDict = new Dictionary<string, int>();
        for (int i = 1; i < csvRows.Count; i++) {
            string key = ReturnFirstCell(CutSpeakerInfo(csvRows[i]));
            existingDict[key] = i;
        }
        return existingDict;
    }
    private string CutSpeakerInfo(string row)
    {
        if (row == ",")
            return row;
        
        int cutIndex = 0;
        for (int i = 0; i < row.Length; i++) {
            if (row[i] == ',') {
                cutIndex = i + 1;
                break;
            }
        }
        return row.Substring(cutIndex);
    }
    private string ReturnFirstCell(string row)
    {
        string key = "";
        bool withinQuotes = false;

        for (int i = 0; i < row.Length; i++) {
            char c = row[i];

            if (c == '"') {
                if (withinQuotes && i + 1 < row.Length && row[i + 1] == '"') {
                    key += '"';
                    i++;
                }
                else
                    withinQuotes = !withinQuotes;
            }
            else if (c == ',' && !withinQuotes)
                break;
            else
                key += c;
        }
        return key;
    }
    private void EraseOutdatedKeys(Dictionary<string, int> existingDict)
    {
        foreach (KeyValuePair<string, int> kvp in existingDict) {
            if (!sentenceInfo.Any(x => x.Item2 == kvp.Key)) {
                csvRows.RemoveAt(kvp.Value);
            }
        }
    }
}