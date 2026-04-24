using UnityEngine;
using StorageEscape.Interaction; // Importante para que reconozca la interfaz

public class InterruptorPantalla : MonoBehaviour, IInteractable
{
    public GameObject pantalla; // Arrastra aquí el objeto de la pantalla
    public GameObject CuboNegroJarro;
    // Implementación de la propiedad de la interfaz
    public string InteractionPrompt => "Encender TV";

    // Implementación del método CanInteract
    public bool CanInteract(GameObject interactor)
    {
        // Puedes poner condiciones aquí (ej: si el jugador está muy lejos)
        return true; 
    }

    // Implementación del método Interact exigido por la interfaz
    public void Interact(GameObject interactor)
    {
        AlternarEstado();
    }

    // Tu lógica original para alternar el estado
    public void AlternarEstado()
    {
        if (pantalla != null)
        {
            pantalla.SetActive(!pantalla.activeSelf);
            CuboNegroJarro.SetActive(!CuboNegroJarro.activeSelf);
            Debug.Log("Estado de pantalla: " + (pantalla.activeSelf ? "Encendida" : "Apagada"));
        }
    }

    public void SoloEncender()
    {
        if (pantalla != null) pantalla.SetActive(true);
    }
}
