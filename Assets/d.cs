
using UnityEngine;
using TMPro;

public class TMPTexto : MonoBehaviour
{
    [SerializeField] private TMP_Text texto;

    public void CambiarTexto(string nuevoTexto)
    {
        texto.text = nuevoTexto;
    }

    public void LimpiarTexto()
    {
    }
}

