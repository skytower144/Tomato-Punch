using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDB
{
    private static Dictionary<string, List<string>> dialogueTable, uiTextTable, fontTable;

    public static void Initialize()
    {
        dialogueTable = CreateTable("TranslationTable/Dialogue_Table", true);
        uiTextTable = CreateTable("TranslationTable/UI_Table");
        fontTable = CreateTable("TranslationTable/Font_Table");
    }
    private static Dictionary<string, List<string>> CreateTable(string path, bool isDialogueTable = false)
    {
        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
        TextAsset table = Resources.Load<TextAsset>(path);

        string[] rows = table.text.Split('\n');

        for (int i = 1; i < rows.Length; i++) {
            List<string> cells = DialogueToCsvSO.ReturnCells(rows[i]);
            string key;

            if (isDialogueTable && cells.Count > 1)
                key = cells[1];
            else
                key = cells[0];
            key = key.Trim();

            if (key == "")
                continue;
            
            cells.RemoveAt(0);
            dict[key] = cells;
            
            int emptyCellCount = UIControl.TotalLanguages - cells.Count;
            for (int j = 0; j < emptyCellCount; j++) {
                dict[key].Add("");
            }
        }
        return dict;
    }
    public static string Translate(string key, TranslationType tableType)
    {
        Dictionary<string, List<string>> table;
        switch (tableType) {
            case TranslationType.DIALOGUE:
                table = dialogueTable;
                break;
            
            case TranslationType.UI:
                table = uiTextTable;
                break;
            
            case TranslationType.FONT:
                table = fontTable;
                break;
            
            default:
                table = new Dictionary<string, List<string>>();
                break;
        }
        if (string.IsNullOrEmpty(key))
            return key;
        
        if (!table.ContainsKey(key)) {
            Debug.LogError($"{key} => not found in TextDB");
            return key;
        }
        if (table[key][(int)UIControl.currentLang] == "") {
            return table[key][0];
        }
        return table[key][(int)UIControl.currentLang];
    }
}
