//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    private int Vida;
    [SerializeField]
    private Transform BarraVida;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private int VidaInitial;
    private float Scale; // la escala de la barra de vida inicial
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
        VidaInitial = Vida;
        Scale = transform.localScale.x;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con ///<summary>
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController
    public void EnemyHealthPoint(int Damage)
    {
        // si la bala colisiona con el enemigo llama a este metodo
        Vida -= Damage; // restar vida
<<<<<<< HEAD
        Debug.Log("Vida: " + Vida);
        BarraVida.localScale = new Vector2((BarraVida.localScale.x - (Scale * Damage / VidaInitial)), BarraVida.localScale.x); // cambiar la escala de la barra de vida
=======
        BarraVida.localScale = new Vector2((BarraVida.localScale.x - (Scale * Damage / VidaInitial)), BarraVida.localScale.y); // cambiar la escala de la barra de vida
>>>>>>> 74fd23239414cec58feb56ce39c09910fbbc0ad3
        BarraVida.position = new Vector2(BarraVida.position.x - ((Scale * Damage / VidaInitial) / 2f), BarraVida.position.y); // moverlo hacia la izquierda
        if (Vida < 1) { Destroy(gameObject); } // destruye el enemigo a la que esta asignado la barra de vida
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion   
    
} // class EnemyHealth 
// namespace
