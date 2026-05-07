//---------------------------------------------------------
// Script que convierte la camara y prepara una escena para combate de boss
// Zimin Chen
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class TriggerCameraBlock : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private FollowCamera Camara; //Recoge la camara para que esta se bloquee y dejé de seguir al jugador
    [SerializeField] private Transform ZonaBoss;  //Asigna una posición para que la camara permanezca ahi
    [SerializeField] private Transform Pared;     //Poner paredes en los laterales para que el jugador no se salga
    [SerializeField] private GameObject Boss;     //Quitar las paredes si el boss muere

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

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        Pared.gameObject.SetActive(false);
        Boss.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Cuando muere el boss la pared desaparece y la camara se desbloquea
        if (Boss == null)
        {
            Pared.gameObject.SetActive(false);
            Camara.UnableSalaBoss();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el jugador cruza por el trigger se activa la zona de boss
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            Camara.SalaBoss(ZonaBoss);
            Pared.gameObject.SetActive(true);
            Boss.gameObject.SetActive(true);
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

    #endregion

} // class TriggerCameraBlock 
// namespace
