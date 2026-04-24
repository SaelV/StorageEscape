using UnityEngine;

public class JarronManager : MonoBehaviour
{
    public GameObject jarronEntero;
    public GameObject jarronRoto; // Opcional: trozos de cerámica
    public GameObject papelPista; // El objeto que el jugador podrá ver/recoger
    public AudioSource sonidoRuptura;

    public void RomperJarron()
    {
        if (jarronEntero.activeSelf)
        {
            jarronEntero.SetActive(false);
            if (jarronRoto != null) jarronRoto.SetActive(true);
            
            papelPista.SetActive(true); // Aparece el papel
            if (sonidoRuptura != null) sonidoRuptura.Play();
            
            Debug.Log("Jarrón roto, papel revelado.");
        }
    }
}