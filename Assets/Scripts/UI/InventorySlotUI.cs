using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StorageEscape.UI
{
    /// <summary>
    /// Representa visualmente una ranura del inventario (icono, numero de slot y estado seleccionado).
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text slotNumberText;
        [SerializeField] private GameObject selectedStateObject;
        [SerializeField] private Color emptyIconColor = new Color(1f, 1f, 1f, 0.2f);
        [SerializeField] private Color filledIconColor = Color.white;

        private void Awake()
        {
            RefreshSelectionVisual(false);
        }

        /// <summary>
        /// Configura el contenido visual completo de la ranura.
        /// </summary>
        /// <param name="slotNumber">Numero de slot a mostrar en texto (por ejemplo 1..9).</param>
        /// <param name="icon">Icono del item en la ranura; null si esta vacia.</param>
        /// <param name="isSelected">Indica si la ranura esta actualmente seleccionada.</param>
        public void SetSlot(int slotNumber, Sprite icon, bool isSelected)
        {
            RefreshSlotNumber(slotNumber);
            RefreshIcon(icon);
            RefreshSelectionVisual(isSelected);
        }

        /// <summary>
        /// Actualiza unicamente si la ranura esta seleccionada.
        /// </summary>
        public void SetSelected(bool isSelected)
        {
            RefreshSelectionVisual(isSelected);
        }

        /// <summary>
        /// Limpia la ranura cuando el item se consume/elimina.
        /// Desactiva el icono.
        /// </summary>
        public void Clear()
        {
            RefreshIcon(null);
        }

        private void RefreshSlotNumber(int slotNumber)
        {
            if (slotNumberText == null)
            {
                return;
            }

            slotNumberText.text = slotNumber.ToString();
        }

        private void RefreshIcon(Sprite icon)
        {
            if (iconImage == null)
            {
                return;
            }

            iconImage.sprite = icon;
            iconImage.color = icon != null ? filledIconColor : emptyIconColor;
            iconImage.enabled = icon != null;
        }

        private void RefreshSelectionVisual(bool isSelected)
        {
            if (selectedStateObject == null)
            {
                return;
            }

            selectedStateObject.SetActive(isSelected);
        }
    }
}
