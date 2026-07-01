using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

[DefaultExecutionOrder(5)]
public class GrowOnSpawn : MonoBehaviour
{
    [SerializeField] float time = 0.5f;

    void Start() =>
        LMotion.Create(Vector3.zero, transform.localScale, time)
            .WithEase(Ease.InCubic)
            .BindToLocalScale(transform);
}