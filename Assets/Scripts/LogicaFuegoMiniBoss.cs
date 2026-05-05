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
public class LogicaFuegoMiniBoss : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] 
    private float Velocidad;
    [SerializeField] 
    private int Daño;
    [SerializeField] 
    private float TiempoEnDestruirse;
    [SerializeField] 
    private GameObject MarcasFuego;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float TiempoConVida;
    private Vector3 posicionJugador;
    private Vector2 dir2D;
    Rigidbody2D rb;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Start()
    {
        dir2D = new Vector2(posicionJugador.x, posicionJugador.y).normalized;
        TiempoConVida = Time.time;
    }
    void Update()
    {
        if (Time.time - TiempoConVida > TiempoEnDestruirse)
        {
            Destroy(gameObject);
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
    public void Dir(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        transform.right = dir;
        rb.linearVelocity = dir.normalized * Velocidad;
    }

    public void ModifyDestroyTime(float newTime)
    {
        TiempoEnDestruirse = newTime;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            //Llama al GameManager para bajar vida
            GameManager.Instance.RestarVida(Daño);
            player.RedFlash();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Suelo"))
        {
            if (MarcasFuego != null)
            {
                Instantiate(MarcasFuego, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    #endregion

} // class LogicaFuegoMiniBoss 
// namespace
