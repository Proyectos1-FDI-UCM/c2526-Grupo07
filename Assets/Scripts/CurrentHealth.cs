//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class CurrentHealth : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private int health;    //cantidad de vida que cura
    [SerializeField] private GameObject ParticulasCura;     //particula cuando cura de vida
    [SerializeField] private GameObject botiquin;       //objeto para la curacion de vida
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private int PotionCount;    //cantidad de botiquin

    //cheat
    private bool cheatMode;
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
    void Start()
    {
        cheatMode = GameManager.Instance.GetCheatMode();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(!LevelManager.Instance.IsPaused())   //cuando está jugando y utilizas el botiquin de cura vida
        {
            if (InputManager.Instance.UseObjectWasPressedThisFrame() && !cheatMode)
            {
                UsePotion();
            }
            PotionCount = GameManager.Instance.CantidadBotiquines();    //al recoger suma la cantidad de botiquin
        }
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    /// <summary>
    /// UsePotion is called to current health
    /// </summary>
    private void UsePotion()
    {
        //jugador utiliza el botiquín
        //desaparece el botiquin utilizado
        if (PotionCount > 0 && GameManager.Instance.UsandoBotiquines())
        {
            PotionCount--;  //se quita la cantidad de botiquin utilizado
            GameManager.Instance.CurarVida(health); //llama al metodo que cura una cantidad de vida determinada
            GameManager.Instance.UsarBotiquin();    //llama al método que controla el uso de botiquin
            if (ParticulasCura != null)     //aparece las partículas de curación
            {
                Instantiate(ParticulasCura, transform.position, Quaternion.identity, transform);
            }
        }
        else Debug.Log("ERROR: no hay consumible para utilizar");
    }
    #endregion   

}// class CurrentHealth 
// namespace
