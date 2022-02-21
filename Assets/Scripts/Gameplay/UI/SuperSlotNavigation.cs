using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperSlotNavigation : MonoBehaviour
{
    [SerializeField] private RectTransform pointer;
    [SerializeField] private List <RectTransform> slotGrid;
    [SerializeField] private Image pointerImage;
    [SerializeField] private Animator logoAnim;
    [SerializeField] private GameObject normal_Parent, super_Parent;
    private int slotNumber, prevSlot;
    //    8
    // 0 1 2 3
    void Start()
    {
        slotNumber = 0;

        pointerImage.enabled = true;
        pointer.position = slotGrid[0].position;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            if(slotNumber == 8){
                super_Parent.SetActive(false);
                normal_Parent.SetActive(true);
            }
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            if (slotNumber >=0 && slotNumber <= 2){
                slotNumber += 1;
                pointer.position = slotGrid[slotNumber].position;
            }
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            if (slotNumber >=1 && slotNumber <= 3){
                slotNumber -= 1;
                pointer.position = slotGrid[slotNumber].position;
            }
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if (slotNumber == 8){
                pointerImage.enabled = true;

                slotNumber = prevSlot;
                pointer.position = slotGrid[slotNumber].position;
                logoAnim.Play("superLogo_default");
            }
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            if (slotNumber >= 0 && slotNumber <= 3){
                pointerImage.enabled = false;

                prevSlot = slotNumber;
                slotNumber = 8;
                logoAnim.Play("superLogo_highlighted");
            }
        }
    }
    void OnDisable()
    {
        Start();
    }

}
