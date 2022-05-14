using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultCard : MonoBehaviour
{
    private int totalCounter_ct, totalParry_ct, totalSuper_ct;
    private float temp_ct;
    private float TEXTSPEED = 14f;
    [SerializeField] private TextMeshProUGUI totalCounter_txt, totalParry_txt, totalSuper_txt;
    private bool start_textChange_counter;
    private bool start_textChange_parry;
    private bool start_textChange_super;

    void Update()
    {
        if (start_textChange_counter && temp_ct <= (float)totalCounter_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            totalCounter_txt.text = temp_ct.ToString("F0");
            if(CheckText(temp_ct, (float)totalCounter_ct))
            {
                start_textChange_counter = false;
                totalCounter_txt.color = new Color32(248, 131, 50, 255);
            }
        }
        else if (start_textChange_parry && temp_ct <= (float)totalParry_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            totalParry_txt.text = temp_ct.ToString("F0");
            if(CheckText(temp_ct, (float)totalParry_ct))
            {
                start_textChange_parry = false;
                totalParry_txt.color = new Color32(248, 131, 50, 255);
            }
        }
        else if (start_textChange_super && temp_ct <= (float)totalSuper_ct)
        {
            temp_ct += Time.deltaTime * TEXTSPEED;
            totalSuper_txt.text = temp_ct.ToString("F0");
            if(CheckText(temp_ct, (float)totalSuper_ct))
            {
                start_textChange_super = false;
                totalSuper_txt.color = new Color32(248, 131, 50, 255);
            }
        }
    }
    public void ResultCard_Initialize(int counter_ct, int parry_ct, int super_ct)
    {
        totalCounter_ct = counter_ct;
        totalParry_ct = parry_ct;
        totalSuper_ct = super_ct;

        ResultCard_GetScore();
    }

    private void ResultCard_GetScore()
    {
        totalCounter_txt = transform.GetChild(1).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        totalParry_txt = transform.GetChild(2).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
        totalSuper_txt = transform.GetChild(3).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

        Invoke("TextChange_counter", 2.5f);
        Invoke("TextChange_parry", 3.8f);
        Invoke("TextChange_super", 5.1f);
    }

    private void TextChange_counter()
    {
        temp_ct = 0;
        start_textChange_counter = true;
    }
    private void TextChange_parry()
    {
        temp_ct = 0;
        start_textChange_parry = true;
    }

    private void TextChange_super()
    {
        temp_ct = 0;
        start_textChange_super = true;
    }

    private bool CheckText(float ct, float max_ct)
    {
        if (ct > max_ct)
        {
            return true;
        }
        return false;
    }
    
}
