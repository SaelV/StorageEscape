using UnityEngine;

public class TVManager : MonoBehaviour
{
    public GameObject luzPantalla; // Un PointLight o Quad con emisión
    public GameObject textoPista;  // El Canvas o el objeto con el texto
    private bool estaEncendida = false;

    public void AlternarTV()
    {
        estaEncendida = !estaEncendida;
        luzPantalla.SetActive(estaEncendida);
        if(textoPista != null) textoPista.SetActive(estaEncendida);
        
        Debug.Log("TV Encendida: " + estaEncendida);
    }
}