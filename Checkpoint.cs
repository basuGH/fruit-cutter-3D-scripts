using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject _targeMissIndicatorPrefab;

    private void OnTriggerEnter(Collider other)
    {
        Target target = other.GetComponent<Target>();

        if (other.CompareTag("Fruit") && !target.IsSpawned)
        {
            target.IsSpawned = true;
        }
        else if (other.CompareTag("Fruit") && target.IsSpawned && !ScoreManager.Instance.IsFruitsPowerUpActive)
        {
            GameManager.Instance.LifeDeduct();
            AudioManager.Instance.TargetMissAudio();
            Vector3 pos = new Vector3(other.transform.position.x, transform.position.y, 0);
            GameObject targetMisIndicator = Instantiate(_targeMissIndicatorPrefab, pos, Quaternion.identity);
            Destroy(targetMisIndicator, 1.5f);
        }
    }
}
