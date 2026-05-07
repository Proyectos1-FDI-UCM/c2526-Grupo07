//---------------------------------------------------------
// Este script se ha creado para la entidad puerta, en la cual si el jugador colisiona con él llama al LM para saltar el panel de victoria
// Responsable de la creación de este archivo: Xinying Xu
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
///  Este script se ha creado para la entidad puerta, en la cual si el jugador colisiona con él llama al LM para saltar el panel de victoria
/// </summary>
public class Door : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
   
    [SerializeField] public AudioSource DoorFX; // sonido de la puerta

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    #endregion

    /// <summary>
    ///  Puerta es un trigger, se ejecuta cuando colisiona con una entidad con tag "player"
    ///  Ejecuta el sonido de la puerta 
    ///  // llama al gameManager para que aparezca el panel de victoria
    /// </summary>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Tag Typing
        {
            DoorFX.Play(); // ejecutar sonido
            LevelManager.Instance.Victoria(); // llamada a LevelManager para que salte el panel de victoria
        }
    }
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   

} // class Door 
// namespace
