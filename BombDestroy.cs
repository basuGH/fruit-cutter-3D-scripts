using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : DestroyMethod
{
    private Animator _camAnimator;
    private void Start()
    {
        _camAnimator = Camera.main.GetComponent<Animator>();
    }
    public override void Destroy()
    {
        if (!ScoreManager.Instance.IsBombDefuseActive)
        {
            _camAnimator.SetTrigger("CameraAnim");
            AudioManager.Instance.ExplosionAudio();
            GameManager.Instance.GameOver();
           // Debug.Log("Bomb");
            if (_vfx != null)
            {
                Instantiate(_vfx, transform.position, _vfx.transform.rotation);
            }
            
            gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
        }
    }
}
