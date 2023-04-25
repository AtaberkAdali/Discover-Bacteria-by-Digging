using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DigGameObjeScript : MonoBehaviour
{
    public bool isClickDigGameobject = false;
    public bool isDigging = false;

    #region touchControlAssigmnets
    //for move
    private Touch touch;
    private Vector3 touchPos;

    //for cameraMove
    public Camera mainCam;
    private Vector3 dragOrigin;
    private Vector3 touchDiffrence;

    //for Zoom
    private Touch touch2;
    private Vector3 touchPos2;
    private float touchDistanceDiff, firstDistance, amountOfIncrease, newSize, maxSize, minSize;
    private float mapMinX, mapMinY, mapMaxX, mapMaxY, camHeight, camWidth;
    public SpriteRenderer backgroundRenderer;
    private bool oneTime = true;

    public bool isPanelOpen = false;
    #endregion

    public Sprite[] blockView; // %100saðlam, %50saðlam, %25saðlam
    public GameObject crushParticleEffect; //block crush

    public int whichToolStateControl = 0; // 0=kürek 1=kazma 2=buldozer

    private Vector3 myDiggerPos;

    public TextMeshProUGUI messageText;
    private bool isThereAnyMassege = false;

    private GiftManager giftManager;

    public GameObject finishPanel;
    public GameObject finishTextGO;



    // Start is called before the first frame update
    void Start()
    {
        minSize = 3;
        maxSize = 8;

        mapMinX = backgroundRenderer.transform.position.x - backgroundRenderer.bounds.size.x/2;
        mapMaxX = backgroundRenderer.transform.position.x + backgroundRenderer.bounds.size.x/2;

        mapMinY = backgroundRenderer.transform.position.y - backgroundRenderer.bounds.size.y/2;
        mapMaxY = backgroundRenderer.transform.position.y + backgroundRenderer.bounds.size.y/2;

        myDiggerPos = transform.position;
        messageText.text = " ";

        giftManager = FindObjectOfType<GiftManager>();

    }

    // Update is called once per frame
    void Update()
    {
        TouchControl();
    }
    private void OnMouseEnter()
    {
        isClickDigGameobject = true;
    }
    private void OnMouseExit()
    {
        isClickDigGameobject = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //dokunduðu objenin ve kendinin pozisyonunu biliyoruz.
        //Debug.Log("trigger : " + collision.name);
        TriggerControl(collision.gameObject);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerControl(collision.gameObject);
    }

    private void TriggerControl(GameObject collisionGO)
    {
        if (collisionGO.CompareTag("block") && !isDigging)
        {
            if(whichToolStateControl == 1 && giftManager.energyO.Amount > 1)
            {
                StartCoroutine(UpdateBlockView(collisionGO, true));
            }
            else
            {
                SetDiggerOffset(collisionGO.transform, true);
            }
        }
        if (collisionGO.CompareTag("toprak")&& !isDigging)
        {
            if (whichToolStateControl == 0 && giftManager.energyO.Amount > 0)
            {
                StartCoroutine(UpdateBlockView(collisionGO, false));
            }
            else
            {
                SetDiggerOffset(collisionGO.transform, true);
            }
        }
        if(collisionGO.CompareTag("BuldozerBlock") && whichToolStateControl == 2 && !isDigging)
        {
            isDigging = true;
            Debug.Log("çalýþtý.");
            Vector3 randoVec = new Vector3(0, 0, 1);
            // Finish The Game
            for (int i = 0; i < 10; i++)
            {
                randoVec.x = Random.RandomRange(-5, 5);
                randoVec.y = Random.RandomRange(-5, 5);
                GameObject myParticle = Instantiate(crushParticleEffect, randoVec, Quaternion.identity);
                myParticle.GetComponent<ParticleSystem>().startColor = collisionGO.GetComponent<SpriteRenderer>().color;
            }
            finishPanel.SetActive(true);
            StartCoroutine(GameFinishScreenI());
        }
    }
    private void SetDiggerOffset(Transform collisionGOTransform, bool giveAMessage)
    {
        if (collisionGOTransform.position.x > transform.position.x)
        {
            //blok sagda
            myDiggerPos = transform.position;
            myDiggerPos.x -= 0.05f;
            myDiggerPos.y += 0.1f;
            transform.position = myDiggerPos;
        }
        else
        {
            //blok solda
            myDiggerPos = transform.position;
            myDiggerPos.x += 0.05f;
            myDiggerPos.y += 0.1f;
            transform.position = myDiggerPos;
        }
        if (!isThereAnyMassege && giveAMessage) // bu fonksiyonu mesajlý ve mesajsýz olarak kullanmak için.
        {
            StartCoroutine(DisplayMessage("Kazamazsýn.", 0.6f));
        }
    }


    private void TouchControl()
    {
        if (!isPanelOpen)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                touchPos.z = 0;
                if (Input.touchCount == 1)
                {
                    if (isClickDigGameobject)
                    {
                        if (!isDigging)
                        {
                            transform.position = touchPos;
                        }

                    }
                    else
                    {
                        //haritayý hareket ettir.
                        CameraMove();
                    }
                }
                else if (Input.touchCount == 2)
                {
                    //burada haritayý büyütüp küçült.
                    CameraZoom();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.collider.gameObject.CompareTag("factory"))
                {
                    Debug.Log("fabrikayý aç.");
                    FindObjectOfType<ButtonController>().OpenFactoryPanel();
                }
                if (hit.collider != null)
                {
                    //Debug.Log("hit.collider.name:  "+ hit.collider.name);
                }

                dragOrigin = touchPos;
                oneTime = true;
            }
        }
    }
    private void CameraMove()
    {
        touchDiffrence = dragOrigin - touchPos;
        mainCam.transform.position += touchDiffrence;
        mainCam.transform.position = ClampCamera(mainCam.transform.position);
    }
    private void CameraZoom()
    {
        touch2 = Input.GetTouch(1);
        touchPos2 = Camera.main.ScreenToWorldPoint(touch2.position);
        touchPos2.z = 0;

        if (oneTime)
            firstDistance = Vector3.Distance(touchPos, touchPos2);
        
        touchDistanceDiff = Vector2.Distance(touchPos, touchPos2);
        amountOfIncrease = touchDistanceDiff - firstDistance;

        newSize = mainCam.orthographicSize - amountOfIncrease;
        mainCam.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);

        firstDistance = touchDistanceDiff;
        mainCam.transform.position = ClampCamera(mainCam.transform.position);
        oneTime = false;
    }

    private Vector3 ClampCamera(Vector3 targetPos)
    {
        camHeight = mainCam.orthographicSize;
        camWidth = mainCam.orthographicSize * mainCam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(newX, newY, targetPos.z);
    }


    private IEnumerator UpdateBlockView(GameObject block, bool isThatBlock)
    {
        isDigging = true;
        block.GetComponent<SpriteRenderer>().sprite = blockView[1];
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1);

        if (!isThatBlock)
        {
            giftManager.energyO.Amount--;//miktarý azalttýk.
            StartCoroutine(giftManager.DecreaseText(1));//azaltma miktarýný yazdýk
            giftManager.UpdateText(giftManager.energyO, giftManager.energyO.Amount);//total enerji yazýsýný güncelledik.
        }
        else
        {
            giftManager.energyO.Amount -= 2;//miktarý azalttýk.
            StartCoroutine(giftManager.DecreaseText(2));//azaltma miktarýný yazdýk
            giftManager.UpdateText(giftManager.energyO, giftManager.energyO.Amount);//total enerji yazýsýný güncelledik.
        }

        yield return new WaitForSeconds(0.3f);

        block.GetComponent<SpriteRenderer>().sprite = blockView[2];
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1);

        yield return new WaitForSeconds(0.3f);

        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GameObject myParticle = Instantiate(crushParticleEffect, block.transform.position, Quaternion.identity);
        myParticle.GetComponent<ParticleSystem>().startColor = block.GetComponent<SpriteRenderer>().color;

        if (!isThatBlock)
        {
            giftManager.GiveEarthGift(block.transform);
        }
        else
        {
            giftManager.GiveBlockGift(block.transform);
        }

        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - 12, block.transform.position.z);
        block.GetComponent<SpriteRenderer>().sprite = blockView[0];
        SetDiggerOffset(block.transform, false);

        isDigging = false;
        
        yield return new WaitForSeconds(0.6f);
        Destroy(myParticle);
    }

    IEnumerator GameFinishScreenI()
    {
        finishPanel.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        float i = 1;
        while (i < 255)
        {
            finishPanel.GetComponent<Image>().color = new Color(0, 0, 0, (i/255));
            Debug.Log(finishPanel.GetComponent<Image>().color);
            i++;
            yield return new WaitForFixedUpdate();
        }
        finishTextGO.SetActive(true);
    }


    #region IncreaseDecreaseNums
    public void IncreaseDecreaseNumbers(int objectToChange, int increaseAmount, TextMeshProUGUI text)
    {
        objectToChange += increaseAmount;
        ChangeString(text, objectToChange);
    }
    public void IncreaseDecreaseNumbers(int objectToChange, int increaseAmount)
    {
        objectToChange += increaseAmount;
    }
    public void IncreaseDecreaseNumbers(float objectToChange, float increaseAmount, TextMeshProUGUI text)
    {
        objectToChange += increaseAmount;
        ChangeString(text, objectToChange);
    }
    public void IncreaseDecreaseNumbers(float objectToChange, float increaseAmount)
    {
        objectToChange += increaseAmount;
    }
    public void IncreaseDecreaseNumbers(float objectToChange, int increaseAmount, TextMeshProUGUI text)
    {
        objectToChange += increaseAmount;
        ChangeString(text, objectToChange);
    }
    public void IncreaseDecreaseNumbers(float objectToChange, int increaseAmount)
    {
        objectToChange += increaseAmount;
    }
    #endregion

    #region messages
    public IEnumerator DisplayMessage(string message, float displayTime)
    {
        isThereAnyMassege = true;
        messageText.text = message.ToString();
        yield return new WaitForSeconds(displayTime);
        messageText.text = " ";
        isThereAnyMassege = false;
    }
    public void ChangeString(TextMeshProUGUI targetText, string myString)
    {
        targetText.text = myString.ToString();
    }
    public void ChangeString(TextMeshProUGUI targetText, int myString)
    {
        targetText.text = myString.ToString();
    }
    public void ChangeString(TextMeshProUGUI targetText, float myString)
    {
        targetText.text = myString.ToString();
    }
    #endregion
}
