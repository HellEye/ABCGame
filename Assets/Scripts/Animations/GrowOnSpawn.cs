using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class GrowOnSpawn : MonoBehaviour {
    [SerializeField] float time = 0.5f;

    void Start() =>
        LMotion.Create(Vector3.zero, Vector3.one, time)
            .WithEase(Ease.InCubic)
            .BindToLocalScale(transform);
}