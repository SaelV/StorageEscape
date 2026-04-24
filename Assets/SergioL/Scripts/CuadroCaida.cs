using UnityEngine;
using System.Collections;
using StorageEscape.Interaction;

public class CuadroCaida : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private bool yaSeCayo = false;

    public string InteractionPrompt => "Tirar cuadro";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public bool CanInteract(GameObject interactor)
    {
        return !yaSeCayo;
    }

    public void Interact(GameObject interactor)
    {
        ActivarCaida();
    }

    public void ActivarCaida()
    {
        if (rb != null && !yaSeCayo)
        {
            yaSeCayo = true;
            rb.isKinematic = false; // Activa la gravedad
            
            StartCoroutine(DesaparecerDespuesDeTiempo(5f));
            
            Debug.Log("El cuadro está cayendo...");
        }
    }

    private IEnumerator DesaparecerDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        gameObject.SetActive(false);
        Debug.Log("El cuadro ha desaparecido después de 5 segundos.");
    }
}