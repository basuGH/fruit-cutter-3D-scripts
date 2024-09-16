using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] private ParticleSystem _ps;
    private bool _isDrawing = false;
   static private bool _firstTouch;
    static public bool FirstTouch { get { return _firstTouch; } }

    void Update()
    {
        if (Time.timeScale < 1) {  return; }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // touch.position = touch.rawPosition;
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));

            Debug.Log(touch.pressure + "," + touchPosition + ", " + touch.tapCount + ", " + touch.radius);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _firstTouch = true;
                    _isDrawing = true;
                    GetComponent<TrailRenderer>().emitting = false;
                    Debug.Log(touch.pressure);
                    if (_ps != null)
                    {
                        if (!_ps.isEmitting)
                        {
                            _ps.Play();
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    _firstTouch = false;


                    if (_isDrawing)
                    {
                        transform.position = touchPosition;
                    }
                    if (GetComponent<TrailRenderer>().positionCount >= 10)
                    {
                        GetComponent<TrailRenderer>().emitting = true;
                    }
                    break;

                case TouchPhase.Ended:
                    if (_ps != null)
                    {
                            _ps.Stop();
                    }
                    GetComponent<TrailRenderer>().emitting = false;
                    _isDrawing = false;
                    break;
            }
        }
    }
}