//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class BossDash : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float DashPower = 20f; // fuerza o velocidad del dash
    [SerializeField] private float DashTime = 0.8f; // duracion del dash
    [SerializeField] private float DashColdDown = 3f; // duracion entre cada dash 
    [SerializeField] private int DashDamage = 10;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float Timer; // tiempo que empieza desde 0
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool CanDash = true;
    private int dir = -1;
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
        Timer = 0;
        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<BoxCollider2D>();
        StartCoroutine(Dash());
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
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        GameManager GM = GetComponent<GameManager>();
        if (player != null) 
        {
            Debug.Log("this is player");
            GM.HealthPoints(DashDamage);
        }
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private IEnumerator Dash()
    {
        while(CanDash)
        {
            float originalGScale = rb.gravityScale;
            yield return new WaitForSeconds(DashColdDown); // esperar por unos segundos
            dir *= -1;
            Timer = 0;
            while(Timer<DashTime)
            {   
                rb.gravityScale = 0;
                rb.linearVelocity = new Vector2(transform.localScale.x * DashPower * dir, 0f);
                col.isTrigger = true;
                Timer += Time.deltaTime;
                yield return null;
            }
            rb.gravityScale = originalGScale;
            col.isTrigger = false;
        }
    }
    #endregion   

} // class BossDash 
// namespace
