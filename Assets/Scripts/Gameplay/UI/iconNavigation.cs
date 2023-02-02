using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class iconNavigation : MonoBehaviour
{
    [System.Serializable]
    private class CharacterCard
    {
        public GameObject cardObj;
        public List <Sprite> SelectedIconSprite;
        public List <Sprite> defaultIconSprite;
        public List <Image> buttonImage;
        public List <InventoryButtonScript> texts;
        public List <GameObject> uiBundle;
    }

    [System.Serializable]
    private class InventoryBookMark
    {
        public GameObject shade;
        public GameObject bookmark;
        public RectTransform bookmarkPos;
    }

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private StatusNavigation statusNavigation;
    [SerializeField] private equipControl equipcontrol;
    [SerializeField] private List<InventoryBookMark> bookmarkList;
    [SerializeField] private List<CharacterCard> cardList;
    private int cardNumber, maxCardNumber, iconNumber, maxIconNumber;
    void Start()
    {
        maxCardNumber = cardList.Count - 1;
    }
    void OnEnable()
    {
        cardNumber = 0;
        buttonHighlight(0);
        SetPage(0);
    }
    void OnDisable()
    {
        buttonNormalize(iconNumber);
        cardList[cardNumber].uiBundle[iconNumber].SetActive(false);
        cardList[cardNumber].cardObj.SetActive(false);
        SetBookMark(cardNumber, 0);
    }
    void Update() 
    {
        if (!equipcontrol.enterEquipNavigation && !statusNavigation.navigating_status)
        {
            if(playerMovement.InputDetection(playerMovement.ReturnMoveVector()))
            {
                GameManager.gm_instance.DetectHolding(UINavigate);
            }
            else if (GameManager.gm_instance.WasHolding)
            {
                GameManager.gm_instance.holdStartTime = float.MaxValue;
            }
        }

        else if(playerMovement.Press_Key("Status"))
        {
            playerMovement.HitStatus();
        }
    }

    private void UINavigate()
    {
        string direction = playerMovement.Press_Direction();
        if(direction == InputDir.RIGHT)
        {
            FlipPage(1);
        }
        else if(direction == InputDir.LEFT)
        {
            FlipPage(-1);
        }
        else if (direction == InputDir.UP)
        {
            FlipCard(-1);
        }
        else if (direction == InputDir.DOWN)
        {
            FlipCard(1);
        }
    }
    private void SetPage(int card_number)
    {
        cardNumber = card_number;
        iconNumber = 0;
        maxIconNumber = cardList[card_number].uiBundle.Count - 1;

        cardList[card_number].uiBundle[0].SetActive(true);
        cardList[card_number].cardObj.SetActive(true);
    }
    private void FlipCard(int direction)
    {
        int prevCardNumber = cardNumber;

        cardNumber += direction;
        cardNumber = Mathf.Clamp(cardNumber, 0, maxCardNumber);

        if (prevCardNumber == cardNumber)
            return;

        buttonNormalize(iconNumber, prevCardNumber);
        cardList[prevCardNumber].uiBundle[iconNumber].SetActive(false);
        cardList[prevCardNumber].cardObj.SetActive(false);

        buttonHighlight(0);
        SetPage(cardNumber);

        SetBookMark(prevCardNumber, cardNumber);
    }
    private void FlipPage(int direction)
    {
        buttonNormalize(iconNumber);
        cardList[cardNumber].uiBundle[iconNumber].SetActive(false);

        iconNumber += direction;
        iconNumber = Mathf.Clamp(iconNumber, 0, maxIconNumber);

        cardList[cardNumber].uiBundle[iconNumber].SetActive(true);
        buttonHighlight(iconNumber);
    }

    private void SetBookMark(int prev_num, int curr_num)
    {
        Vector3 bookmarkPos = bookmarkList[prev_num].bookmarkPos.localPosition;
        bookmarkList[prev_num].shade.SetActive(true);
        bookmarkList[prev_num].bookmarkPos.localPosition = new Vector2(bookmarkPos.x + 42, bookmarkPos.y);

        bookmarkPos = bookmarkList[curr_num].bookmarkPos.localPosition;
        bookmarkList[curr_num].shade.SetActive(false);
        bookmarkList[curr_num].bookmarkPos.localPosition = new Vector2(bookmarkPos.x - 42, bookmarkPos.y);
    }

    void buttonNormalize(int number, int card_number = -1)
    {
        if (card_number == -1)
            card_number = cardNumber;
            
        cardList[card_number].buttonImage[number].sprite = cardList[card_number].defaultIconSprite[number];     
        cardList[card_number].texts[number].buttonNormalizeText();
    }
    void buttonHighlight(int number)
    {
        cardList[cardNumber].buttonImage[number].sprite = cardList[cardNumber].SelectedIconSprite[number];
        cardList[cardNumber].texts[number].buttonHighlightText();
    }
}
