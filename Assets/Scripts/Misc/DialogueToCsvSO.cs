using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using Ink.Runtime;

[CreateAssetMenu(fileName = "DialogueToCsvSO", menuName = "DialogueToCsvSO")]
public class DialogueToCsvSO : ScriptableObject
{
    private List<(string, string)> sentenceInfo = new List<(string, string)>();
    private List<string> csvRows = new List<string>();
    private List<string> outdatedKeys = new List<string>();
    HashSet<string> ignoreSet = new HashSet<string>() {
        "Yes", "No",
        "/cut", "/ignore", "<br>", "<i>", "</i>",
        "ExitShop", "ConfirmPurchase",
    };
    public void ExecuteUpdate()
    {
        string dialogueRootPath = Application.dataPath + "/Resources/Dialogue/eng";
        string csvPath = Application.dataPath + "/Resources/TranslationTable/Dialogue_Table.csv";

        ExtractTextFromFiles(dialogueRootPath);
        Dictionary<string, int> existingDict = ReturnKeysFromCsv(csvPath);

        CopyInkToCsv(existingDict);
        NotifyOutdatedKeys(existingDict);

        File.WriteAllLines(csvPath, csvRows.ToArray(), new UTF8Encoding(true));
        PlayerPrefs.SetString("DialogueCSV-LastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        sentenceInfo.Clear();
        csvRows.Clear();
    }
    private void CopyInkToCsv(Dictionary<string, int> existingDict)
    {
        for (int i = 0; i < sentenceInfo.Count; i++) {
            if (sentenceInfo[i].Item2 == "\n") {
                csvRows.Insert(i + 1, new string(',', UIControl.TotalLanguages));
            }
            else if (!existingDict.ContainsKey(sentenceInfo[i].Item2)) {
                string currentSentence = sentenceInfo[i].Item2;
                currentSentence = currentSentence.Replace("\"", "\"\"");
                csvRows.Insert(i + 1, $"\"{sentenceInfo[i].Item1}\",\"{currentSentence}\"");
            }
        }
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
        csvRows = new List<string>(File.ReadAllLines(path, new UTF8Encoding(true)));
        
        for (int i = csvRows.Count - 1; i > 0; i--) {
            if (csvRows[i].Split(',')[0] == "")
                csvRows.RemoveAt(i);
        }
        Dictionary<string, int> existingDict = new Dictionary<string, int>();

        for (int i = 1; i < csvRows.Count; i++) {
            string key = ReturnCells(CutSpeakerInfo(csvRows[i]), 1)[0];
            if (key != "")
                existingDict[key] = i;
        }
        return existingDict;
    }
    public static string CutSpeakerInfo(string row)
    {
        int cutIndex = 0;
        for (int i = 0; i < row.Length; i++) {
            if (row[i] == ',') {
                cutIndex = i + 1;
                break;
            }
        }
        return row.Substring(cutIndex);
    }
    public static List<string> ReturnCells(string row, int count = -1)
    {
        List<string> cells = new List<string>();
        int cellCount = 0;
        string currentCell = "";
        bool withinQuotes = false;

        for (int i = 0; i < row.Length; i++) {
            char c = row[i];

            if (c == '"') {
                if (withinQuotes && i + 1 < row.Length && row[i + 1] == '"') {
                    currentCell += '"';
                    i++;
                }
                else
                    withinQuotes = !withinQuotes;
            }
            else if (c == ',' && !withinQuotes) {
                cells.Add(currentCell);
                cellCount++;
                currentCell = "";

                if (count != -1 && cellCount >= count)
                    return cells;
            }
            else {
                if (c != '\n' && c != '\r')
                    currentCell += c;
            }
        }
        cells.Add(currentCell);
        return cells;
    }
    private void NotifyOutdatedKeys(Dictionary<string, int> existingDict)
    {
        foreach (KeyValuePair<string, int> kvp in existingDict) {
            if (!sentenceInfo.Any(x => x.Item2 == kvp.Key)) {
                Debug.LogWarning($"{kvp.Value + 1}th row is Outdated:\n{kvp.Key}");
            }
        }
    }
}