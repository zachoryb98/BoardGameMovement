using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomizationManager : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject Bodies;
    private int bodyIndex = 0;
    private int bodyCount = 0;
    private List<GameObject> bodiesList = new List<GameObject>();
    public TextMeshProUGUI bodyText;

    [Header("Eyes")]
    public GameObject Eyes;
    private int eyesIndex = 0;
    private int eyesCount = 0;
    private List<GameObject> eyesList = new List<GameObject>();
    public TextMeshProUGUI eyesText;

    [Header("Head Parts")]
    public GameObject HeadParts;
    private int headPartsIndex = 0;
    private int headPartsCount = 0;
    private List<GameObject> headPartsList = new List<GameObject>();
    public TextMeshProUGUI headPartsText;

    [Header("Mouths")]
    public GameObject Mouths;
    private int mouthIndex = 0;
    private int mouthCount = 0;
    private List<GameObject> mouthList = new List<GameObject>();
    public TextMeshProUGUI mouthText;

    public void Start()
    {
        //Body set up
        bodyIndex = 0;
        bodiesList = GetAllChildrenInObject(Bodies);
        bodyCount = bodiesList.Count;

        //Eyes Set up
        eyesIndex = 0;
        eyesList = GetAllChildrenInObject(Eyes);
        eyesCount = eyesList.Count;

        //Head Parts Set up
        headPartsIndex = 0;
        headPartsList = GetAllChildrenInObject(HeadParts);
        headPartsCount = headPartsList.Count;

        //Mouth Set up
        mouthIndex = 0;
        mouthList = GetAllChildrenInObject(Mouths);
        mouthCount = mouthList.Count;
    }

    #region Right Buttons
    public void btnBodyRightPressed()
    {        
        //Disable Body
        DisableBody();

        //Increment
        bodyIndex++;

        //If at the end of the list reset
        if (bodyIndex >= bodyCount)
        {
            bodyIndex = 0;
        }

        //Enable new body
        EnableBody();
    }

    public void btnEyesRightPressed()
    {
        DisableEyes();
        
        eyesIndex++;
        
        if (eyesIndex >= eyesCount)
        {
            eyesIndex = 0;
        }
        
        EnableEyes();
    }

    public void btnHeadPartsRightPressed()
    {
        DisableHeadParts();

        headPartsIndex++;

        if (headPartsIndex >= headPartsCount)
        {
            headPartsIndex = 0;
        }

        EnableHeadParts();
    }

    public void btnMouthRightPressed()
    {
        DisableMouth();

        mouthIndex++;

        if (mouthIndex >= mouthCount)
        {
            mouthIndex = 0;
        }

        EnableMouth();
    }
    #endregion

    #region Left Buttons
    public void btnBodyLeftPressed()
    {
        //Disable Body
        DisableBody();

        //Increment
        bodyIndex--;

        //If at the end of the list reset
        if (bodyIndex <= 0)
        {
            bodyIndex = bodyCount - 1;
        }

        //Enable new body
        EnableBody();
    }

    public void btnEyesLeftPressed()
    {
        DisableEyes();
        
        eyesIndex--;

        if (eyesIndex <= 0)
        {
            eyesIndex = eyesCount - 1;
        }
        
        EnableEyes();
    }

    public void btnHeadPartsLeftPressed()
    {
        DisableHeadParts();

        headPartsIndex--;

        if (headPartsIndex <= 0)
        {
            headPartsIndex = headPartsCount - 1;
        }

        EnableHeadParts();
    }

    public void btnMouthLeftPressed()
    {
        DisableMouth();

        mouthIndex--;

        if (mouthIndex <= 0)
        {
            mouthIndex = mouthCount - 1;
        }

        EnableMouth();
    }

    #endregion

    #region Disable Events
    private void DisableEyes()
    {
        eyesList[eyesIndex].SetActive(false);
    }

    private void DisableBody()
    {
        bodiesList[bodyIndex].SetActive(false);
    }

    private void DisableHeadParts()
    {
        headPartsList[headPartsIndex].SetActive(false);
    }

    private void DisableMouth()
    {
        mouthList[mouthIndex].SetActive(false);
    } 

    #endregion

    #region Enable Events
    private void EnableBody()
    {
        bodiesList[bodyIndex].SetActive(true);
        bodyText.text = bodiesList[bodyIndex].name;
    }

    private void EnableEyes()
    {
        eyesList[eyesIndex].SetActive(true);
        eyesText.text = eyesList[eyesIndex].name;
    }

    private void EnableHeadParts()
    {
        headPartsList[headPartsIndex].SetActive(true);
        headPartsText.text = headPartsList[headPartsIndex].name;
    }

    private void EnableMouth()
    {
        mouthList[mouthIndex].SetActive(true);
        mouthText.text = mouthList[mouthIndex].name;
    }
    #endregion

    public List<GameObject> GetAllChildrenInObject(GameObject parentObject)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parentObject.transform)
        {
            children.Add(child.gameObject);
        }            

        return children;
    }
}
