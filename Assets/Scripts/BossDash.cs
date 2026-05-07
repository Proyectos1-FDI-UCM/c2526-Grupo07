//---------------------------------------------------------
// Es uno de los ataques del jefe, en la cual cada cierto tiempo hace dash de una esquina a otra
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

    [SerializeField] private Transform player;  // posicion del player
    [SerializeField] private AudioSource sonidoDash; // sonido que hace al ejecutar el dash
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
    private int dir = -1;
    private bool isDashing = false; // determina si el jugador esta dasheando o no (este es util para la animacion)

    private Animator _anim; //animacion de dash
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
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // hacer que el jefe mire al jugador
        Vector2 offset = player.transform.position - transform.position;
        if(offset.x < 0 && dir!= 1)
        {
            dir *= -1;
            CambioDireccion();
        }
        if (offset.x > 0 && dir != -1)
        {
            dir *= -1;
            CambioDireccion();
        }
        //ejecutar la ación
        Dash();
        //ejecutar la animación
        _anim.SetBool("IsDashing", isDashing);
        return;
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
    private void Dash()
    {
        float InitialG = rb.gravityScale;
        Timer += Time.deltaTime; // contar el tiempo
        isDashing = false;
        
        if (Timer > DashColdDown) //ColdDown
        {
            Timer2 += Time.deltaTime; // contar el tiempo
            rb.linearVelocity = Vector2.zero; // entidad no se moverá si no esta dasheando
            
            if (Timer2 > DashTime) //Duración del Dash
            {
                isDashing = true;

                sonidoDash.Play();

                rb.gravityScale = 0; // entidad no tendrá gravedad al ejecutar el dash

                //cambio de direccion, invertir
                dir *= -1;   
                //dashear
                rb.linearVelocity = new Vector2(dir * DashPower, rb.linearVelocity.y);
                //cambiar de sentido
                Vector2 scale = transform.localScale;
                scale.x = Mathf.Sign(dir) * Mathf.Abs(scale.x);
                transform.localScale = scale;

                //vuelve a sincronizar el tiempo
                Timer = 0; Timer2 = 0;   
            }
        }
    }
    //metodo que hace que la entidad cambie de sentido 
    private void CambioDireccion()
    {
        transform.localScale = new Vector3(dir*-1, 1, 1);
    }
    #endregion   

} // class BossDash 
// namespace
