using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotNavigation : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private Inventory inventory;
    [SerializeField] private List <RectTransform> slotGrid;
    [SerializeField] private List <GameObject> slotPage;
    public int Last_pageNumber;
    [SerializeField] private Image pointerImage, logoImage;
    [SerializeField] private Sprite defaultLogo, highlightedLogo;
    [SerializeField] private GameObject normal_Parent, super_Parent, leftArrow, rightArrow, slotbox;
    private int pageNumber, slotNumber, prevSlot;
    [System.NonSerialized] public int invNumber;
    [System.NonSerialized] static public bool isBusy; // when selecting a slot

    //     8 .. logo
    // 0  1  2  3  
    // 4  5  6  7
    void OnEnable()
    {
        slotNumber = 0;
        pageNumber = 0;
        ResetPage();

        logoImage.sprite = defaultLogo;
        pointerImage.enabled = true;
        pointer.position = slotGrid[0].position;

        isBusy = false;
        slotbox.SetActive(false);
    }
    void OnDisable()
    {
        OnEnable();
    }
    void Update()
    {
        if (!isBusy)
        {
            if(playerMovement.Press_Key("Interact"))
            {
                if(slotNumber == 8) {
                    super_Parent.SetActive(true);
                    normal_Parent.SetActive(false);
                }
                else {
                    invNumber = slotNumber + pageNumber*8;
                    if (invNumber < inventory.normalEquip.Count) {
                        slotbox.SetActive(true);
                        isBusy = true;
                    }
                }
            }
            else if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
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
            if ((slotNumber >= 0 && slotNumber <=2) || (slotNumber >=4 && slotNumber <= 6)) {
                slotNumber += 1;
            }
            else if ((pageNumber < Last_pageNumber) && (slotNumber == 3 || slotNumber == 7)) {
                FlipPage(1);
                slotNumber -= 3;
            }
        }
        else if(direction == "LEFT")
        {
            if ((slotNumber >= 1 && slotNumber <= 3) || (slotNumber >= 5 && slotNumber <= 7)) {
                slotNumber -= 1;
            }
            else if ((pageNumber > 0) && (slotNumber == 0 || slotNumber == 4)) {
                FlipPage(-1);
                slotNumber += 3;
            }
        }
        else if(direction == "DOWN")
        {
            if (slotNumber == 8) {
                pointerImage.enabled = true;
                slotNumber = prevSlot;
                logoImage.sprite = defaultLogo;
            }
            else if (slotNumber >= 0 && slotNumber <= 3) {
                slotNumber += 4;
            }
        }
        else if(direction == "UP")
        {
            if (slotNumber >=0 && slotNumber <= 3) {
                prevSlot = slotNumber;
                pointerImage.enabled = false;
                slotNumber = 8;
                logoImage.sprite = highlightedLogo;
            }
            else if (slotNumber >= 4 && slotNumber <= 7) {
                slotNumber -= 4;
            }
        }
        pointer.position = slotGrid[Mathf.Clamp(slotNumber, 0, 7)].position;
    }
    private void FlipPage(int direction)
    {
        pageNumber += direction;
        slotPage[pageNumber].SetActive(true);
        slotPage[pageNumber - direction].SetActive(false);

        ArrowControl();
    }

    private void ResetPage()
    {
        slotPage[0].SetActive(true);
        for( int i=1; i<=Last_pageNumber; i++)
        {
            if(slotPage[i].activeSelf){
                slotPage[i].SetActive(false);
            }
        }
        ArrowControl();
    }

    private void ArrowControl()
    {
        if (pageNumber == 0){
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
        else if (pageNumber >0 && pageNumber < Last_pageNumber){
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }
        else if (pageNumber == Last_pageNumber){
            leftArrow.SetActive(true);
            rightArrow.SetActive(false);
        }
    }
}
