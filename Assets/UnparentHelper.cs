using UnityEngine;

public class UnparentHelper : MonoBehaviour
{
    public void Unparent()
        => gameObject.transform.SetParent(null);
}
