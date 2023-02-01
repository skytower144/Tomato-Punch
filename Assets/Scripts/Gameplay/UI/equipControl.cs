using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class equipControl : MonoBehaviour
{
    [SerializeField] private GameObject super_Parent, normal_Parent, normalCursor, normal_left_arrow, normal_right_arrow;
    private PlayerMovement player_movement;
    private bool enterNavigation; public bool enterEquipNavigation => enterNavigation;
    void Start()
    {
        player_movement = GameManager.gm_instance.player_movement;
    }
    void OnEnable()
    {
        enterNavigation = false;

        super_Parent.SetActive(false);
        normal_Parent.SetActive(true);

        normalCursor.SetActive(false);
        normal_left_arrow.SetActive(false);
        normal_right_arrow.SetActive(false);
    }

    void OnDisable()
    {
        OnEnable();
    }

    void Update()
    {
        if (!enterNavigation && player_movement.Press_Key("Interact"))
        {
            enterNavigation = true;
            normalCursor.SetActive(true);
        }
        else if (enterNavigation && !SlotNavigation.isBusy && player_movement.Press_Key("Cancel"))
        {
            enterNavigation = false;
            OnEnable();
        }
    }
}
