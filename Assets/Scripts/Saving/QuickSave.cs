using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSave : MonoBehaviour
{
    [SerializeField] private bool isQuickSaved = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isQuickSaved && (collision.tag == "Player"))
        {
            isQuickSaved = true;
            Debug.Log("Proceeding quick save...");
            GameManager.gm_instance.save_load_menu.ProceedSave(3);
        }
    }
}
