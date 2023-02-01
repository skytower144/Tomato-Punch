using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperSlotNavigation : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private List <RectTransform> slotGrid;
    [SerializeField] private Image pointerImage;
    [SerializeField] private Animator logoAnim;
    [SerializeField] private GameObject normal_Parent, super_Parent, slotbox;
    private int pageNumber, slotNumber, prevSlot;
    [System.NonSerialized] public int s_invNumber;
    
    //    8
    // 0 1 2 3
    void OnEnable()
    {
        slotNumber = 0;
        pageNumber = 0;

        pointerImage.enabled = true;
        pointer.position = slotGrid[0].position;

        SlotNavigation.isBusy = false;
        slotbox.SetActive(false);
    }
    void OnDisable()
    {
        OnEnable();
    }
    void Update()
    {
        if(!SlotNavigation.isBusy)
        {
            if (playerMovement.Press_Key("Interact"))
            {
                if(slotNumber == 8){
                    super_Parent.SetActive(false);
                    normal_Parent.SetActive(true);
                }
                else {
                    s_invNumber = slotNumber + pageNumber*4;
                    if (s_invNumber < inventory.superEquip.Count){
                        slotbox.SetActive(true);
                        SlotNavigation.isBusy = true;
                    }
                }
            }
            else if (playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                gameManager.DetectHolding(UINavigate);
            }
            else if (gameManager.WasHolding)
            {
                gameManager.holdStartTime = float.MaxValue;
            }
        }
    }
    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();

        if(direction == "RIGHT")
        {
            if (slotNumber >=0 && slotNumber <= 2){
                slotNumber += 1;
                pointer.position = slotGrid[slotNumber].position;
            }
        }
        else if(direction == "LEFT")
        {
            if (slotNumber >=1 && slotNumber <= 3){
                slotNumber -= 1;
                pointer.position = slotGrid[slotNumber].position;
            }
        }
        else if(direction == "DOWN")
        {
            if (slotNumber == 8){
                pointerImage.enabled = true;

                slotNumber = prevSlot;
                pointer.position = slotGrid[slotNumber].position;
                logoAnim.Play("superLogo_default");
            }
        }
        else if(direction == "UP")
        {
            if (slotNumber >= 0 && slotNumber <= 3){
                pointerImage.enabled = false;

                prevSlot = slotNumber;
                slotNumber = 8;
                logoAnim.Play("superLogo_highlighted");
            }
        }
    }
}
