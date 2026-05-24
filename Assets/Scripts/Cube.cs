using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    [SerializeField] private Cube _cube;

    private Transform _transform;
    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private Color _defaulColor = Color.blue;
    private float _timeLifeMin = 2f;
    private float _timeLifeMax = 5f;
    private bool _haveDefaulColor = true;

    public event Action<Cube> LifeExpired;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer.material.color = _defaulColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_haveDefaulColor && collision.gameObject.TryGetComponent(out Platform _))
        {
            ChangeColor();
            StartCoroutine(ComputeTimeLife());
        }
    }

    public void ResetState()
    {
        _renderer.material.color = _defaulColor;
        _haveDefaulColor = true;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _transform.transform.rotation = Quaternion.identity;
    }

    private void ChangeColor()
    {
        _renderer.material.color = UnityEngine.Random.ColorHSV();
        _haveDefaulColor = false;
    }

    private IEnumerator ComputeTimeLife()
    {
        float timeLife = UnityEngine.Random.Range(_timeLifeMin, _timeLifeMax);

        yield return new WaitForSecondsRealtime(timeLife);

        LifeExpired?.Invoke(_cube);
    }
}