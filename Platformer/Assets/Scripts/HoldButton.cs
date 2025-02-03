using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image Img;
    [SerializeField] Sprite defaultSprite, holdSprite;
    [SerializeField] Color HoverColor;
    float current_hold_time;
    [SerializeField] float holdThreshold;
    [SerializeField] float holdMinTime;
    public bool buttonPressed;
    public UnityEvent onButtonClick;
    public UnityEvent onButtonHold;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
    }

    private void Update()
    {
        if (buttonPressed)
        {
            current_hold_time += Time.deltaTime;
            if (current_hold_time > holdThreshold)
            {
                Img.sprite = holdSprite;
                Img.fillAmount = Mathf.Clamp((current_hold_time-holdThreshold)/holdMinTime, 0f, 1f);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
        if (current_hold_time >= holdMinTime)
        {
            //GameManager.instance.ResetLevel();
            onButtonHold.Invoke();
        }
        else
        {
            //GameManager.instance.Respawn();
            onButtonClick.Invoke();
        }
        current_hold_time = 0;
        Img.sprite = defaultSprite;
        Img.fillAmount = 1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Img.color = HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Img.color = Color.white;
    }
}