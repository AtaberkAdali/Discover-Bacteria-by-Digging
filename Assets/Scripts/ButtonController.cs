using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MonoBehaviour
{
    public GameObject digGameObject;
    public Sprite[] digObjeSprites; //kazma, kürek, buldozer.
    public GameObject buldozerLockedImage, finishTheGameTextGO, buyBuldozerPanelGO;
    public GameObject[] factoryPanel;

    public GameObject infoCard, infoImage;
    public TextMeshProUGUI infoText;

    private bool isBuldozerUnlocked = true;

    private DigGameObjeScript digObj;
    private GiftManager buttonGiftManager;

    private void Start()
    {
        digObj = FindObjectOfType<DigGameObjeScript>();
        buttonGiftManager = FindObjectOfType<GiftManager>();
    }

    public void ChosePickaxe()
    {
        digGameObject.GetComponent<SpriteRenderer>().sprite = digObjeSprites[0];
        digObj.whichToolStateControl = 0;
    }
    public void ChoseShovle()
    {
        digGameObject.GetComponent<SpriteRenderer>().sprite = digObjeSprites[1];
        digObj.whichToolStateControl = 1;
    }
    public void ChoseBuldozer()
    {
        if (!isBuldozerUnlocked)
        {
            digGameObject.GetComponent<SpriteRenderer>().sprite = digObjeSprites[2];
            digObj.whichToolStateControl = 2;
        }
        else
        {
            StartCoroutine(digObj.DisplayMessage("Buldozer kilitli.", 2));
        }
    }

    public void CloseFactoryPanel()
    {
        factoryPanel[0].SetActive(false);
        factoryPanel[1].SetActive(false);
        digObj.isPanelOpen = false;
    }
    public void OpenFactoryPanel()
    {
        factoryPanel[0].SetActive(true);
        factoryPanel[1].SetActive(true);
        digObj.isPanelOpen = true;
        buttonGiftManager.UpdateMicroDisplays();
        buttonGiftManager.UpdateAllTexts();
    }
    public void GiveEnergy()
    {
        Debug.Log("give enerji0");
        if(buttonGiftManager.goldO.Amount > 45)
        {
            buttonGiftManager.goldO.Amount -= 45;
            buttonGiftManager.energyO.Amount += 1;
            buttonGiftManager.UpdateAllTexts();
            StartCoroutine(buttonGiftManager.DecreaseText(45));
            StartCoroutine(buttonGiftManager.IncreaseText(1));
        }
        else
        {
            StartCoroutine(digObj.DisplayMessage("Yeterli para yok.", 3));
        }
    }
    public void BuyBuldozer()
    {
        if (FindObjectOfType<GiftManager>().goldO.Amount > 500) {
            bool hasAllResearcheBeenDone = false;
            int micro = 0;
            for (int i = 0; i < buttonGiftManager.microGiftO.Length; i++)
            {
                if (buttonGiftManager.microGiftO[i].IsUnlocked)
                {
                    micro++;
                }
            }
            if (buttonGiftManager.microGiftO.Length <= micro)
            {
                hasAllResearcheBeenDone = true;
            }


            if (hasAllResearcheBeenDone)
            {
                //Burada digger'ý açacaðýz.
                FindObjectOfType<GiftManager>().goldO.Amount -= 500;
                FindObjectOfType<GiftManager>().UpdateAllTexts();
                StartCoroutine(buttonGiftManager.DecreaseText(500));
                StartCoroutine(buttonGiftManager.IncreaseText(1));
                buyBuldozerPanelGO.SetActive(false);
                finishTheGameTextGO.SetActive(true);
                buldozerLockedImage.SetActive(false);
                isBuldozerUnlocked = false;
            }
            else
            {
                StartCoroutine(digObj.DisplayMessage("Bütün araþtýrmalar yapýlmamýþ.", 2));
            }
        }
        else
        {
            StartCoroutine(digObj.DisplayMessage("Yeterli para yok.", 2));
        }
    }
    public void CloseInfoCard()
    {
        infoCard.SetActive(false);
    }

    public void OpenMicro1()
    {
        OpenCard(0, "Basit Bakteri: Bu buluþ biyoloji alanýnda ilerlemeni saðladý ama zaten dünyaca bilinen bir þeydi daha keþfetmen gereken bir çok þey var.");
    }
    public void OpenMicro2()
    {
        OpenCard(1, "Basit Virüs: Bu buluþ biyoloji alanýnda ilerlemeni saðladý ama sadece bilimini geliþtirdi litaretüre direkt bir faydasý mevcut deðil.");
    }
    public void OpenMicro3()
    {
        OpenCard(2, "Komplex Bakteri: Bu keþif kimya ve biyoloji alanýnda ilerlemeni saðladý ve bu keþiften esinlenen birisi buldozer diþlisini icat etti!");
    }
    public void OpenMicro4()
    {
        OpenCard(3, "Komplex Virüs: Bu keþif kimya ve biyoloji alanýnda çok büyük ilerlemelere yol açtý. Bu keþiften esinlenen birisi buldozer motorunu icat etti!!");
    }
    public void OpenMicro5()
    {
        OpenCard(4, "Basit Taþ: Bu buluþ jeofizik alanýnda ilerlemeni saðladý ama zaten dünyaca bilinen bir þeydi daha keþfetmen gereken bir çok þey var.");
    }
    public void OpenMicro6()
    {
        OpenCard(5, "Basit Metal: Bu buluþ jeofizik alanýnda ilerlemeni saðladý ama sadece bilimini geliþtirdi litaretüre direkt bir faydasý mevcut deðil.");
    }
    public void OpenMicro7()
    {
        OpenCard(6, "Komplex Taþ: Bu keþif fizik ve jeofizik alanýnda ilerlemeni saðladý ve bu keþif sayesinde dünyada buldozer ihtiyacý ortaya çýktý!");
    }
    public void OpenMicro8()
    {
        OpenCard(7, "Komplex Metal: Bu keþif fizik ve jeofizik alanýnda çok büyük ilerlemelere yol açtý. Bu keþif sayesinde buldozerlerin bilinen bütün parçalarýnýn üretimi baþladý!!");
    }

    private void OpenCard(int microNum, string text)
    {
        if(buttonGiftManager.microGiftO[microNum].Amount > 0)
        {
            infoCard.SetActive(true);
            infoImage.GetComponent<Image>().sprite = buttonGiftManager.microGiftO[microNum].gift.GetComponent<SpriteRenderer>().sprite;
            if (!buttonGiftManager.microGiftO[microNum].IsUnlocked)
            {
                infoText.text = text.ToString() + "\nKÝLÝT AÇILDI!";
                buttonGiftManager.microGiftO[microNum].IsUnlocked = true;
                buttonGiftManager.UpdateMicroDisplays();
            }
            else
            {
                infoText.text = text.ToString();
            }
        }
        else
        {
            StartCoroutine(digObj.DisplayMessage("Henüz keþfetmedin.", 2));
        }
    }
}
