using UnityEngine;

public class CuadroManager : MonoBehaviour
{
    public GameObject cajaFuerte;
    public Animator cuadroAnimator; // Si usas animación
    private bool movido = false;

    public void MoverCuadro()
    {
        if (!movido)
        {
            // Opción A: Si usas Animación de Unity
            if (cuadroAnimator != null) cuadroAnimator.SetTrigger("Mover");
            
            // Opción B: Si solo quieres que desaparezca o se mueva por código
            // transform.Translate(Vector3.right * 0.5f); 
            
            cajaFuerte.SetActive(true);
            movido = true;
            Debug.Log("Cuadro movido, caja fuerte a la vista.");
        }
    }
}