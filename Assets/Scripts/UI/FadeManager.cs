using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    [SerializeField] List<MaskableGraphic> components = new List<MaskableGraphic>();
    private void Start()
    {
        foreach (var component in GetComponentsInChildren<MaskableGraphic>())
        {
            components.Add(component);
        }

        foreach (var component in components)
        {
            
        }
    }
}
