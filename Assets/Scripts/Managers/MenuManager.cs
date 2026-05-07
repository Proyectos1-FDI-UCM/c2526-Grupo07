//---------------------------------------------------------
// Manager que solo existe en el menu, esto solo da datos del cheat y los da al GM.
// Responsable de la creación de este archivo: Xinying Xu
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using TMPro;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class MenuManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private TextMeshProUGUI textcheat; // texto para decir si esta activado el cheat 
    #endregion


    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private bool cheatMode; // booleano que determina y usa el cheat
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 
    public void Start()
    {
        // dal el valor que tiene el gameManager
        cheatMode = GameManager.Instance.GetCheatMode(); 
    }
    public void Update()
    {
        // cambiar la letra del boton
        updateGUI();
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController
   
    // cambia el valor del cheatMode
    // este metodo se usará para el boton
    public void SwitchCheatMode()
    {
        cheatMode = !cheatMode; 
        //llamada al gameManager para dar el valor del CheatMode
        GameManager.Instance.SetCheatFromMM(cheatMode);
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void updateGUI()
    {
        if (cheatMode == true)
        {
            textcheat.text = "Cheat on"; // si esta activado
        }
        else { textcheat.text = "Cheat off"; } // si no esta activado
    }

    #endregion

} // class MenuManager 
// namespace
