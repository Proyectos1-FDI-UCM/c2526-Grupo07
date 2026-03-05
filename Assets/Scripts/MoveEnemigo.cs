//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------


// Añadir aquí el resto de directivas using
/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
using System;
using System.Collections;
using UnityEngine;

public class MoveEnemigo : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float duracion;    //duracion de tiempo en movimiento 
    [SerializeField] private float vel; //velocidad para el movimiento del enemigo
<<<<<<< HEAD
    [SerializeField] private Transform player;  //jugador para realizar las acciones
=======
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
<<<<<<< HEAD
    private bool isChasing=false;  //controlar si está persiguiendo al jugador
    private bool isShooting = false;  //controlar si está disparando al jugador

    private int direction=1;  //direccion del enemigo
    private float tiempoInicio=0; //tiempo iniciado para el movimiento
=======
    private bool movDer = true;    //direccion movimiento a la derecha para aplicar un movimiento automatico
    private bool movIzq = true;
    private bool canMove = true;  //controlar si se puede mover
    private Vector3 direccion = Vector3.zero;  //direccion de persecucion-
    private float tiempoInicio; //tiempo iniciado para el movimiento
    private float restante;     //tiempo restante del movimiento para cambiar de direccion
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    private Rigidbody2D rb;
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
<<<<<<< HEAD
        rb = GetComponent<Rigidbody2D>();
=======
        //inicio de una corrutina para cambiar la direcion del enemigo
        //cuando el movimiento llega hasta un limite de tiempo
        //o al colisionar con un objeto
        rb = GetComponent<Rigidbody2D>();

        //iniciar el tiempo
        tiempoInicio = Time.time;
        movDer = true;
        movIzq = false;
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
<<<<<<< HEAD
        if (isShooting)
        {   
            //cuando dispara se deja de mover
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (isChasing)
        {
            Perseguir();
            tiempoInicio = 0;
        }
        else MovAuto();
=======
        if (canMove && direccion != Vector3.zero)
        {
            transform.Translate(direccion * vel * Time.deltaTime);
        }

        else
        {
            if (movDer) //si va en direccion derecha
            {
                transform.Translate(Vector3.right * vel * Time.deltaTime);      //aplicar movimiento derecha
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movIzq)
            {
                transform.Translate(Vector3.left * vel * Time.deltaTime);      //movimiento izquierda
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        if (Time.time - tiempoInicio >= duracion)
        {
            CambioDireccion();
            tiempoInicio = Time.time;
        }
    }
    private void FixedUpdate()
    {

>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //si se colisiona con un objeto(pared)
        //se cambia de direccion
        //y reinicia el tiempo de movimiento
        if (collision.gameObject.CompareTag("Pared"))
        {
<<<<<<< HEAD
            direction *= -1; //invertir dirección
            CambioDireccion();
            Debug.Log("detecion pared");
            tiempoInicio = 0;
=======
            CambioDireccion();
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
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
<<<<<<< HEAD
    public int GetDirection()   
    {
        return direction;   //devolvel la direccion del enemigo para detectar con el jugador
    }

    //devolver el valor booleano si está realizando esta acción
    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
    }
    public void SetShooting(bool shooting)
    {
        isShooting= shooting;
=======
    public void StopMovement()
    {
        canMove = false;
    }

    ///<summary>Reanudar el movimiento normal del enemigo</summary>
    public void ResumeMovement()
    {
        canMove = true;
        direccion = Vector3.zero; // volver a patrullar automático
    }

    ///<summary>Mover al enemigo hacia una posición (persecución)</summary>
    public void FollowPlayer(Vector3 targetPosition)
    {
        canMove = true;
        float dirX = targetPosition.x - transform.position.x;
        movDer = dirX > 0;  // actualizar dirección
        direccion = new Vector3(Mathf.Sign(dirX), 0, 0); // movimiento hacia el jugador
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
<<<<<<< HEAD
    private void Perseguir()
    {
        rb.linearVelocity = new Vector2(direction * vel, rb.linearVelocity.y);

        // ajustar sprite según direction
        transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
    }
    private void MovAuto()
    {
        tiempoInicio += Time.deltaTime; //tiempo en movimiento para cambiar de sentido
        rb.linearVelocity=new Vector2(direction*vel, rb.linearVelocity.y);
        if (tiempoInicio > duracion)
        {
            direction *= -1;    //cambio de direccion, invertir
            rb.linearVelocity = new Vector2(direction * vel, rb.linearVelocity.y);
            tiempoInicio = 0;   //vuelve a sincronizar el tiempo
        }
    }
    
    private void CambioDireccion()
    {
        transform.localScale = new Vector3(direction,1,1);
    }
=======

    private void CambioDireccion()
    {
        movDer = !movDer;
        movIzq = !movIzq;
        tiempoInicio = Time.time;       //reinicio de tiempo al cambiar de direcion
    }
>>>>>>> 7f2690ac5666ae18bcaa6cca285ccdbec1583bbd
    #endregion
} // class MoveEnemigo 
  // namespace