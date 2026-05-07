//---------------------------------------------------------
// Manager que solo existe en el menu, esto solo da datos del cheat y los da al GM (gameManager).
// Responsable de la creación de este archivo: Xinying Xu
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using TMPro;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Manager que solo existe en el menu, esto solo da datos del cheat para que los reciva el GM.
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
    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    public void Start()
    {
        // dal el valor que tiene el gameManager
        cheatMode = GameManager.Instance.GetCheatMode(); 
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// Ejecuta el método updateGUI
    /// </summary>
    public void Update()
    {
        // cambiar la letra del boton
        UpdateGUI();
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    /// <summary>
    /// cambia el valor del cheatMode
    /// este método se usará para el botón
    /// Además se llamará al gameManager para que tenga el booleano CheatMode
    /// </summary>
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
    /// <summary>
    /// Cambia la letra del botón
    /// Si cheatMode es true la letra cambia a "Cheat on"
    /// Y lo contrarion, si cheatMode es false en el botón pondrá "Cheat off"
    /// </summary>
    private void UpdateGUI()
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
