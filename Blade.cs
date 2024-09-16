using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class Blade : MonoBehaviour
{
    [SerializeField] TrailRenderer _trail;
    [SerializeField] private bool _isCutting;
    [SerializeField] private Vector3 _mousePos, _previousBladePos;
    [SerializeField] private float _bladeActivationRange;
    [SerializeField] private GameObject _trailPrefab;
    private BoxCollider _collider;
    [SerializeField] private AudioClip _clipL, _clipR;

    public static float BladeAngleZ { get; private set; }
    Rigidbody _rb;
    private void Start()
    {
        _trail = GetComponentInChildren<TrailRenderer>();
        _collider = GetComponent<BoxCollider>();
    }
    private void MousePosition()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;
        transform.position = _mousePos;
    }
    private void ShowTrail()
    {
            _trail.emitting = _isCutting;
    }
    private void EnableCollider()
    {
        if (Time.timeScale<1) { return; }
        // Calculate the  distance per second
        float distance = Vector3.Distance(_previousBladePos, transform.position) / Time.deltaTime;
        Vector3 direction = (-transform.position + _previousBladePos).normalized;
        float zAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        if (distance > _bladeActivationRange && _isCutting && !Trail.FirstTouch && !GameManager.Instance.IsGameOver)
        {
            if (direction.x > 0 || direction.y > 0)
            {
                AudioManager.Instance.PlayBladeAudio(_clipR);
            }
            else if (direction.x < 0 || direction.y < 0)
            {
                AudioManager.Instance.PlayBladeAudio(_clipL);
            }

            //Debug.Log("distance "+distance + Trail.FirstTouch);
            _collider.enabled = true;
            BladeAngleZ = -zAngle;
        }
        else
        {
            _collider.enabled = false;
        }

        _previousBladePos = transform.position;
    }
    private IEnumerator TrailDisable()
    {
        yield return new WaitForSeconds(0.2f);
        _trail.enabled = false;
    }
    private IEnumerator TrailEnable()
    {
        yield return new WaitForSeconds(0.05f);
        _trail.enabled = true;
    }

    private void Update()
    {
        Profiler.BeginSample("Blade Update");
        if (Input.touchCount > 1)
        {
            return;
        }
        //TouchOperation();
        IsCutting();
        EnableCollider();
        Profiler.EndSample();

    }
    private void IsCutting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _previousBladePos = transform.position;
            _isCutting = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isCutting = false;
        }
        MousePosition();
#if UNITY_EDITOR
        _trail.enabled =true;
        ShowTrail();
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<DestroyMethod>(out DestroyMethod dm);
        if (dm != null)
        {
            dm.Destroy();
        }
    }
}
