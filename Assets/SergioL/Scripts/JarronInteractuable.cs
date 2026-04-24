using UnityEngine;
using StorageEscape.Interaction;

public class JarronInteractuable : MonoBehaviour, IInteractable
{
    public GameObject objetoJarro;
    public GameObject notaOculta;
    public AudioSource sonidoRuptura;

    public string InteractionPrompt => "Romper jarrón";

    public bool CanInteract(GameObject interactor)
    {
        return objetoJarro != null && objetoJarro.activeSelf;
    }

    public void Interact(GameObject interactor)
    {
        Romper();
    }

    public void Romper()
    {
        if (objetoJarro != null)
        {
            objetoJarro.SetActive(false);
        }

        if (notaOculta != null)
        {
            notaOculta.SetActive(true);
        }

        if (sonidoRuptura != null)
        {
            sonidoRuptura.Play();
        }

        Debug.Log("Jarrón roto.");
    }
}