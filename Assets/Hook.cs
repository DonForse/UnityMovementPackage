using UnityEngine;

public class Hook :MonoBehaviour
{
    private Material mr;

    private void Awake()
    {
        mr = this.GetComponent<MeshRenderer>().material;
    }

    public void ToggleFocus(bool focused)
    {
        if (focused)
            mr.color = Color.blue;
        else
            mr.color = Color.red;
    }
}