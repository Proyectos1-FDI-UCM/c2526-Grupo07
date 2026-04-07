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
    private float Timer2;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool Dashing = false;
    private int dir = 1;
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
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {   
        Dash();
    }
    private void FixedUpdate()
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
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log("this is player");
            GameManager.Instance.HealthPoints(DashDamage);
        }
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Dash()
    {
        float InitialG = rb.gravityScale;
        Timer += Time.deltaTime; 
        if (Timer > DashColdDown) //ColdDown
        {
            Timer2 += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            if (Timer2 > DashTime) //Duración del Dash
            { 
                Dashing = true;
                rb.gravityScale = 0;
                dir *= -1;    //cambio de direccion, invertir
                rb.linearVelocity = new Vector2(dir * DashPower, rb.linearVelocity.y);
                Timer = 0;
                Timer2 = 0;   //vuelve a sincronizar el tiempo
            }
        }
        //else { Dashing = false; }
        //if (Dashing) { col.isTrigger = true; }
        //else { col.isTrigger = false; }
            rb.gravityScale = InitialG;
    }
    #endregion   

} // class BossDash 
// namespace
