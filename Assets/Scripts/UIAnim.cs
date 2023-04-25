using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnim : MonoBehaviour
{
    private Vector2 defaultScale;
    private Vector2 currentScale;
    private int animStateControl = 0;
    private bool oneTimeControl = true;
    [SerializeField] float increaseMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UIZoomInOutAnim(this.gameObject));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator UIZoomInOutAnim(GameObject GO)
    {
        StartCoroutine(UIAnimTimer());
        defaultScale = GO.transform.localScale;
        currentScale = defaultScale;
        while (animStateControl < 3)
        {
            switch (animStateControl)
            {
                case 0: 
                    currentScale.x += (Time.deltaTime*increaseMultiplier);
                    currentScale.y += (Time.deltaTime*increaseMultiplier);
                    GO.transform.localScale = currentScale;
                    break;
                case 1:
                    //Just Wait
                    break;
                case 2:
                    currentScale.x -= (Time.deltaTime * increaseMultiplier);
                    currentScale.y -= (Time.deltaTime * increaseMultiplier);
                    GO.transform.localScale = currentScale;
                    break;
                default:
                    Debug.LogWarning("Kanka switch kodunu kontrol et.");
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
        GO.transform.localScale = defaultScale;
    }
    IEnumerator UIAnimTimer()
    {
        if (oneTimeControl)
        {
            oneTimeControl = false;
            yield return new WaitForSeconds(0.5f);
            animStateControl = 1;
            yield return new WaitForSeconds(0.2f);
            animStateControl = 2;
            yield return new WaitForSeconds(0.5f);
            animStateControl = 3;
            oneTimeControl = true;
        }
        
    }
}
