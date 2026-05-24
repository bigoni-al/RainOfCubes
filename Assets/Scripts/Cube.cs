using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    private Transform _transform;
    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private float _timeLifeMin = 2f;
    private float _timeLifeMax = 5f;
    private bool _haveDefaulColor = true;
    private Color _defaulColor = Color.blue;

    public bool HaveDefaulColor => _haveDefaulColor;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _renderer.material.color = _defaulColor;
    }

    public void ChangeColor() 
    {
        _renderer.material.color = Random.ColorHSV();
        _haveDefaulColor = false;
    }

    public void ResetState() 
    {
        _renderer.material.color = _defaulColor;
        _haveDefaulColor = true;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        _transform.transform.rotation = Quaternion.identity; 
    }

    public float GetLifeTime() 
    {
        return Random.Range(_timeLifeMin, _timeLifeMax);
    }
}
