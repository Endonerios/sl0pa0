using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
[RequireComponent (typeof (Image))]

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    Image Img;
    [SerializeField] Sprite defaultSprite, holdSprite;
    [SerializeField] Color HoverColor;
    [SerializeField] KeyCode PressKey;
    [SerializeField] float current_hold_time;
    [SerializeField] float holdThreshold;
    [SerializeField] float holdMinTime;
    [SerializeField] float holdStartTime;
    [SerializeField] float lastFrameTime;
    public bool buttonPressed;
    public UnityEvent onButtonClick;
    public UnityEvent onButtonHold;

    private void Start()
    {
        Img = GetComponent<Image> ();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        holdStartTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (Input.GetKeyDown(PressKey))
        {
            buttonPressed = true;
            holdStartTime = Time.realtimeSinceStartup;
        }
        else if (Input.GetKeyUp(PressKey))
        {
            HoldCheck();
        }
        if (buttonPressed)
        {
            current_hold_time += (Time.realtimeSinceStartup - holdStartTime) - current_hold_time;
            if (current_hold_time > holdThreshold)
            {
                Img.sprite = holdSprite;
                Img.fillAmount = Mathf.Clamp((current_hold_time-holdThreshold)/holdMinTime, 0f, 1f);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HoldCheck();
    }

    public void HoldCheck()
    {
        buttonPressed = false;
        if ((Time.realtimeSinceStartup - holdStartTime) <= holdMinTime)
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