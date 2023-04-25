using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GiftObj : MonoBehaviour
{
    public GameObject gift;
    public GameObject display;
    public TextMeshProUGUI amountText;
    private int amount;
    private int type;//0=energy,1=gold,2=micro
    private bool isUnlocked = false;
    public GiftObj(GameObject gift, GameObject display, TextMeshProUGUI amountText, int amount, int type)
    {
        this.gift = gift;
        this.display = display;
        this.amountText = amountText;
        this.amount = amount;
        this.type = type;
    }
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
    public int Type
    {
        get { return type; }
        set { type = value; }
    }
    public bool IsUnlocked
    {
        get { return isUnlocked; }
        set { isUnlocked = value; }
    }
}

public class GiftManager : MonoBehaviour
{
    #region energy gold and micro assigments
    private int IncreaseAmount = 0;
    private bool isThatBlock = false;
    public TextMeshProUGUI energyText, goldText, bagText, increaseText, decreaseText;
    public TextMeshProUGUI[] microAmountTexts;
    public GameObject[] microGiftsDisplay;

    public GameObject[] microGifts;
    public GameObject goldGift;
    public GameObject energyGift;

    public GameObject uIEnergyImage, uIGoldImage, uIBagImage;

    public GiftObj[] microGiftO = new GiftObj[8];
    public GiftObj energyO, goldO;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < microGiftO.Length; i++)
        {
            microGifts[i] = Instantiate(microGifts[i], transform.position, Quaternion.identity);
            microGifts[i].SetActive(false);

            microGiftO[i] = new GiftObj(microGifts[i], uIBagImage, microAmountTexts[i], 0,2);
        }
        energyGift = Instantiate(energyGift, transform.position, Quaternion.identity);
        energyGift.SetActive(false);
        goldGift = Instantiate(goldGift, transform.position, Quaternion.identity);
        goldGift.SetActive(false);

        energyO = new GiftObj(energyGift, uIEnergyImage, energyText, 7,0);
        goldO = new GiftObj(goldGift, uIGoldImage, goldText, 0,1);
        UpdateAllTexts();

    }
    public void GiveEarthGift(Transform blockT)
    {
        isThatBlock = false;
        //mikroorganizmalar verilecek.
        int randomNum = Random.RandomRange(0, 100);
        if(randomNum < 40)
        {//micro ver.
            if(randomNum < 15)
            {//%15
                if(randomNum < 4)
                {//%4
                    GiveGift(microGiftO[3], blockT);
                }
                else
                {//%11
                    GiveGift(microGiftO[1], blockT);
                }
            }
            else
            {
                if(randomNum < 33)
                {//%7
                    GiveGift(microGiftO[2], blockT);
                }
                else
                {//%18
                    GiveGift(microGiftO[0], blockT);
                }

            }
        }
        GiveGift(goldO, blockT);
    }
    public void GiveBlockGift(Transform blockT)
    {
        isThatBlock = true;
        int randomNum = Random.RandomRange(0, 100);
        if (randomNum < 40)
        {//micro ver.
            if (randomNum < 15)
            {//%15
                if (randomNum < 4)
                {//%4
                    GiveGift(microGiftO[7], blockT);
                }
                else
                {//%11
                    GiveGift(microGiftO[5], blockT);
                }
            }
            else
            {
                if (randomNum < 33)
                {//%7
                    GiveGift(microGiftO[6], blockT);
                }
                else
                {//%18
                    GiveGift(microGiftO[4], blockT);
                }

            }
        }
        GiveGift(goldO, blockT);
    }

    [System.Obsolete]
    IEnumerator MoveGiftO(GiftObj giftGO)//0=energy,1=gold,2=micro
    {
        giftGO.gift.SetActive(true);
        bool finishThisFunction = false;
        float distance = 0;
        int moveSpeed = Random.RandomRange(11, 15);
        Vector3 targetGOLocation = Camera.main.ScreenToWorldPoint(giftGO.display.transform.position);
        while (!finishThisFunction)
        {
            distance = Vector2.Distance(targetGOLocation, giftGO.gift.transform.position);
            if (distance < 0.2f)
            {
                finishThisFunction = true;
            }
            else
            {
                giftGO.gift.transform.position = Vector3.MoveTowards(giftGO.gift.transform.position, targetGOLocation, moveSpeed * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(FindObjectOfType<UIAnim>().UIZoomInOutAnim(giftGO.display));
        if (giftGO.Type == 0)
        {
            IncreaseAmount = 1;
            giftGO.Amount += IncreaseAmount;
            giftGO.amountText.text = giftGO.Amount.ToString();
        }
        else if (giftGO.Type == 1)
        {
            if (!isThatBlock)
                IncreaseAmount = Random.RandomRange(45, 60);
            else
                IncreaseAmount = Random.RandomRange(90, 120);
            giftGO.Amount += IncreaseAmount;
            giftGO.amountText.text = giftGO.Amount.ToString();
        }
        else
        {
            IncreaseAmount = 1;
            giftGO.Amount += IncreaseAmount;
            int totalMicroNum = 0;
            for (int i = 0; i < microGiftO.Length; i++)
            {
                totalMicroNum += microGiftO[i].Amount;
            }
            UpdateText(giftGO, totalMicroNum);
        }
        giftGO.gift.SetActive(false);
        StartCoroutine(IncreaseText(IncreaseAmount));
        //Destroy(giftGO.gift.gameObject);
    }
    private void GiveGift(GiftObj giftO, Transform startT)
    {
        giftO.gift.transform.position = startT.position;
        StartCoroutine(MoveGiftO(giftO));
    }

    public void UpdateMicroDisplays()
    {
        for (int i = 0; i < microGiftO.Length; i++)
        {
            if(microGiftO[i].Amount > 0)
            {
                if (microGiftO[i].IsUnlocked)
                {
                    microGiftsDisplay[(i * 2)].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                    Debug.Log("i : " + i + " microGiftsDisplay[(i*2)].GetComponent<Image>().color: " + microGiftsDisplay[(i * 2)].GetComponent<Image>().color);
                    microGiftsDisplay[(i * 2) + 1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    microGiftsDisplay[(i * 2)].GetComponent<Image>().color = new Color32(65, 65, 65, 255);
                    Debug.Log("i : " + i + " microGiftsDisplay[(i*2)].GetComponent<Image>().color: " + microGiftsDisplay[(i * 2)].GetComponent<Image>().color);
                    microGiftsDisplay[(i * 2) + 1].GetComponent<Image>().color = new Color32(65, 65, 65, 255);
                }
            }
        }
    }

    #region message
    public IEnumerator IncreaseText(int increase)
    {
        increaseText.text = "+" + increase.ToString();
        yield return new WaitForSeconds(1);
        increaseText.text = " ";
    }
    public IEnumerator DecreaseText(int decrease)
    {
        decreaseText.text = "-" + decrease.ToString();
        yield return new WaitForSeconds(1);
        decreaseText.text = " ";
    }
    public void UpdateText(GiftObj giftO, TextMeshProUGUI text)
    {
        giftO.amountText.text = text.ToString();
    }
    public void UpdateText(GiftObj giftO, int text)
    {
        giftO.amountText.text = text.ToString();
    }
    public void UpdateAllTexts()
    {
        UpdateText(energyO, energyO.Amount);
        UpdateText(goldO, goldO.Amount);
        int bagAmount = 0;
        for (int i = 0; i < microGiftO.Length; i++)
        {
            microAmountTexts[i].text = microGiftO[i].Amount.ToString();
            bagAmount += microGiftO[i].Amount;
        }
        bagText.text = bagAmount.ToString();
        Debug.Log("energyO.Amount: "+energyO.Amount);
        Debug.Log("goldO.Amount: " + goldO.Amount);
    }
#endregion
}
