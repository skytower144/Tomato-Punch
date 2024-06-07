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
    private List<string> updatedRows = new List<string>();
    private List<string> outdatedRows = new List<string>();
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

        File.WriteAllLines(csvPath, updatedRows.ToArray(), new UTF8Encoding(true));
        PlayerPrefs.SetString("DialogueCSV-LastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        updatedRows = new List<string>();
        outdatedRows.Clear();
        sentenceInfo.Clear();
        csvRows.Clear();
    }
    private void CopyInkToCsv(Dictionary<string, int> existingDict)
    {
        // SPEAKER, ENGLISH, KOREAN ...
        updatedRows.Add(csvRows[0]);

        for (int i = 0; i < sentenceInfo.Count; i++) {
            string currentSentence = sentenceInfo[i].Item2;

            if (currentSentence == "\n") {
                updatedRows.Add(new string(',', UIControl.TotalLanguages));
            }
            else if (!existingDict.ContainsKey(currentSentence)) {
                currentSentence = currentSentence.Replace("\"", "\"\"");
                updatedRows.Add($"\"{sentenceInfo[i].Item1}\",\"{currentSentence}\"");
            }
            else {
                updatedRows.Add(csvRows[existingDict[currentSentence]]);
                existingDict.Remove(currentSentence);
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
            
            // if outdated bondary line
            else if (csvRows[i][0] == '=')
                csvRows.RemoveAt(i);
        }
        Dictionary<string, int> existingDict = new Dictionary<string, int>();

        for (int i = 1; i < csvRows.Count; i++) {
            string key = ReturnCells(CutSpeakerInfo(csvRows[i]), 1)[0];
            existingDict[key] = i;

            if (string.IsNullOrEmpty(key))
                Debug.LogError($"{key} => Empty key exsists in Dialogue_Table.csv");
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
        string line = new string('=', 20);
        string boundaryLine = line + " OUTDATED DIALOGUE " + line;
        updatedRows.Add(boundaryLine);

        // 0th row, last row's \n, boundary line
        int outdatedRowNum = sentenceInfo.Count + 3;

        foreach (KeyValuePair<string, int> kvp in existingDict) {
            Debug.LogWarning($"{outdatedRowNum}th row is outdated :\n{kvp.Key}");
            updatedRows.Add(csvRows[kvp.Value]);
            outdatedRowNum++;
        }
    }
}