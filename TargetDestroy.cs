using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestroy : DestroyMethod
{
    [SerializeField] private GameObject _juiceEffect;
    private ScoreManager _scoreManager;
    private void Start()
    {
        _scoreManager = FindObjectOfType<ScoreManager>();
    }
    public override void Destroy()
    {
        AudioManager.Instance.TargetSlicedAudio();
        _scoreManager.ScoreUpdate(1);
        _scoreManager.ComboActivation(transform.position);


        if (_targetSlicedPrefab != null)
        {
           GameObject targetSlice =  Instantiate(_targetSlicedPrefab, transform.position, Quaternion.Euler(0, 0, Blade.BladeAngleZ));
            targetSlice.transform.localScale = Vector3.one * GameManager.Instance.TargetScale;
        }
        if (_vfx != null)
        {
            GameObject vfx = Instantiate(_vfx, transform.position + new Vector3(0, 0, 2), _vfx.transform.rotation);
            ParticleSystem PS = vfx.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule pMain;
            pMain=PS.main;
            pMain.startRotation = new ParticleSystem.MinMaxCurve(-Blade.BladeAngleZ * Mathf.Deg2Rad);
        }

        Instantiate(_juiceEffect, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }
}
