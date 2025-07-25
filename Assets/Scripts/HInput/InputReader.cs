using UnityEngine;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }

    public Vector2 currentTouchPos;

    public enum InputState { Idle, Touching, Dragging, Pinching, Touching_2 }

    private InputState preState=InputState.Idle;
    public InputState CurrentState { get; private set; }

    public Vector2 DragDelta { get; private set; }

    public bool TapDetected { get; private set; }
    public bool SwipeDetected { get; private set; }
    public Vector2 SwipeDelta { get; private set; }

    public bool PinchDetected { get; private set; }
    public float PinchDelta { get; private set; }

    [SerializeField] private float dragThreshold = 4f;
    [SerializeField] private float dragStopThreshold = 0.05f;
    [SerializeField] private float swipeThreshold = 20f;
    [SerializeField] private float swipeTimeLimit = 0.5f;
    [SerializeField] private float tapTimeLimit = 0.5f;
    [SerializeField] private float pinchThreshold = 3f;
    [SerializeField] private float pinchStopThreshold = 0.1f;


    private Vector2 startTouchPos;
    private Vector2 previousTouchPos;
    private float pinchStartDistance;
    
    private float dragStopTimer = 0f;
    private float pinchStopTimer = 0f;
    private float touchStartTime = 0f;
    private float preTouchingTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        ResetFlags();
        HandleTouchInput();
        DebugInputState();
    }

    void ResetFlags()
    {
        TapDetected = false;
        SwipeDetected = false;
        PinchDetected = false;
        DragDelta = Vector2.zero;
        SwipeDelta = Vector2.zero;
        
       
    }

    void HandleTouchInput()
    {
        int touchCount = Input.touchCount;

        if (touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            currentTouchPos=touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    previousTouchPos = touch.position;
                    preTouchingTime = Time.time;
                    touchStartTime = Time.time;
                    CurrentState = InputState.Touching;
                    break;

                case TouchPhase.Moved:
                    
                    DragDelta = touch.position-previousTouchPos;

                    if (DragDelta.magnitude > dragThreshold)
                    {
                        CurrentState = InputState.Dragging;
                        dragStopTimer = 0f;
                    }
                    else if (CurrentState == InputState.Dragging)
                    {
                        dragStopTimer += Time.deltaTime;
                        if (dragStopTimer >= dragStopThreshold)
                        {
                            CurrentState = InputState.Touching;
                            startTouchPos = touch.position;
                            preTouchingTime = Time.time;

                        }
                    }

                    previousTouchPos = touch.position;
                    break;

                case TouchPhase.Stationary:
                    if (CurrentState == InputState.Dragging)
                    {
                        dragStopTimer += Time.deltaTime;
                        if (dragStopTimer >= dragStopThreshold)
                        {
                            CurrentState = InputState.Touching;
                            startTouchPos = touch.position;
                            previousTouchPos = touch.position;
                            preTouchingTime = Time.time;

                        }
                    }
                    else if(CurrentState == InputState.Touching)
                    {
                        preTouchingTime = Time.time;
                    }

                    break;

                case TouchPhase.Ended:
                    CurrentState = InputState.Idle;
                    Vector2 delta = touch.position - startTouchPos;
                    float tapDuration = Time.time - touchStartTime;
                    float swipeDuration = Time.time - preTouchingTime;


                    if (delta.magnitude < swipeThreshold)
                    {
                        if (tapDuration < tapTimeLimit)
                        TapDetected = true;
                    }
                    else if (swipeDuration < swipeTimeLimit)
                    {
                        SwipeDetected = true;
                        SwipeDelta = delta;
                    }

                    dragStopTimer = 0f;
                    break;

                case TouchPhase.Canceled:
                    CurrentState = InputState.Idle;
                    break;
            }
        }
        /*else if (touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 pos0 = t0.position;
            Vector2 pos1 = t1.position;

            currentTouchPos = (pos0 + pos1) * 0.5f;


            if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(t0.position, t1.position);
                float delta = currentDistance - pinchStartDistance;

                if (Mathf.Abs(delta) > pinchThreshold)
                {
                    pinchStopTimer = 0f;
                    PinchDetected = true;
                    PinchDelta = delta;
                    pinchStartDistance = currentDistance;
                    CurrentState = InputState.Pinching;
                }
                else if (CurrentState == InputState.Pinching)
                {
                    pinchStopTimer += Time.deltaTime;
                    if (pinchStopTimer >= pinchStopThreshold)
                        CurrentState = InputState.Touching_2;
                }

            }
            else if(t0.phase == TouchPhase.Stationary && t1.phase == TouchPhase.Stationary)
            {
                if (CurrentState == InputState.Pinching)
                {
                    pinchStopTimer += Time.deltaTime;
                    if (pinchStopTimer >= pinchStopThreshold)
                        CurrentState = InputState.Touching_2;
                }
            }
            else if (t0.phase == TouchPhase.Began && t1.phase == TouchPhase.Began)
            {
                pinchStartDistance = Vector2.Distance(t0.position, t1.position);
                CurrentState = InputState.Touching_2;
            }
            else if(t0.phase == TouchPhase.Ended || t1.phase == TouchPhase.Ended)
            {
                pinchStopTimer = 0f;
            }
            else if(t0.phase == TouchPhase.Canceled && t1.phase == TouchPhase.Canceled)
            {
                CurrentState = InputState.Idle;
            }    
        }*/
    }

    void DebugInputState()
    {
        //if (preState != CurrentState)
        //{
        //    preState = CurrentState;
        //    Debug.Log(CurrentState.ToString());
        //}


        if (TapDetected) Debug.Log("Tapped");
        if (SwipeDetected) Debug.Log("Swipe: " + SwipeDelta);
        if (PinchDetected) Debug.Log("Pinch delta: " + PinchDelta);

    }
}
