using UnityEngine;

public class EditAttrParticle : MonoBehaviour
{
    [SerializeField] private Material _matEdit;
    [SerializeField] private float _alpha;
    [SerializeField] private string _id;
    
    public float Alpha
    {
        get => _alpha;
    }

    public void EditAlpha(float value)
    {
        if (value > 1)
        {
            _alpha = 1;
        }
        else
        {
            _alpha = value;
        }

        Color ab = _matEdit.GetColor(_id);
        ab.a = _alpha;
        _matEdit.SetColor(_id,ab);
    }
    
    private void OnDrawGizmos()
    {
        EditAlpha(_alpha);
    }
}
