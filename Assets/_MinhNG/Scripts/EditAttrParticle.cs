using UnityEngine;

public class EditAttrParticle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _alpha;
    [SerializeField] private Color _color;
    [SerializeField] private string _id;
    private MaterialPropertyBlock _propertyAttr;

    private void Start()
    {
        _propertyAttr = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyAttr);
    }

    public void EditAlpha(float value)
    {
        _alpha = value;
        Color a = _propertyAttr.GetColor(_id);
        a.a = _alpha;
        _propertyAttr.SetColor(_id,a);
        _meshRenderer.SetPropertyBlock(_propertyAttr);
    }
    
    private void OnDrawGizmos()
    {
        _propertyAttr = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propertyAttr);
        _color.a = _alpha;
        _propertyAttr.SetColor(_id,_color);
        _meshRenderer.SetPropertyBlock(_propertyAttr);
    }
}
