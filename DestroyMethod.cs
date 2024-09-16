using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DestroyMethod : MonoBehaviour
{
    [SerializeField] protected GameObject _targetSlicedPrefab, _vfx;
    public abstract void Destroy();
}
