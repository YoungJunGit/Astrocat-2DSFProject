using System.Collections.Generic;
using UnityEngine;

namespace ARGameDev
{
    [System.Serializable]
    public class DiceSides
    {
        [HideInInspector] public string name;
        public Vector3 rotation;
        public MeshRenderer meshRenderer;
    }

    public class Dice : MonoBehaviour
    {
        [SerializeField] private int selectedSide = 1;
        [SerializeField] private List<DiceSides> diceSides;
        [SerializeField] private Material selectedNumberMaterial, numberMaterial;
        [SerializeField] private List<MeshRenderer> notActiveNumbers;
        private void OnValidate() => RefreshDiceVisuals();

        private void RefreshDiceVisuals()
        {
            ResetInactiveNumbers();
            UpdateDiceSides();
        }

        private void ResetInactiveNumbers()
        {
            if (notActiveNumbers == null || notActiveNumbers.Count == 0) return;
            foreach (var number in notActiveNumbers)
            {
                if (number != null) number.material = numberMaterial;
            }
        }

        private void UpdateDiceSides()
        {
            if (diceSides == null || diceSides.Count == 0) return;
            for (int i = 0; i < diceSides.Count; i++)
            {
                var side = diceSides[i];
                side.name = (i + 1).ToString();

                bool isSelected = (i == selectedSide - 1);
                Material materialToApply = isSelected ? selectedNumberMaterial : numberMaterial;

                if (side.meshRenderer != null) side.meshRenderer.material = materialToApply;

                if (isSelected) transform.rotation = Quaternion.Euler(side.rotation);
            }
        }
    }
}