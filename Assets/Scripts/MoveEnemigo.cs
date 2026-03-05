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
    [SerializeField] private Transform player;  //jugador para realizar las acciones
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private bool isChasing = false;  //controlar si está persiguiendo al jugador
    private bool isShooting = false;  //controlar si está disparando al jugador

    private int direction = 1;  //direccion del enemigo
    private float tiempoInicio = 0; //tiempo iniciado para el movimiento
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
        rb = GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
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
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //si se colisiona con un objeto(pared)
        //se cambia de direccion
        //y reinicia el tiempo de movimiento
        if (collision.gameObject.CompareTag("Pared"))
        {
            direction *= -1;
            CambioDireccion();
            Debug.Log("detecion pared");
            tiempoInicio = 0;
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
        isShooting = shooting;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Perseguir()
    {
        rb.linearVelocity = new Vector2(direction * vel, rb.linearVelocity.y);
        transform.localScale = new Vector3(Math.Sign(direction), 1, 1);
    }
    private void MovAuto()
    {
        tiempoInicio += Time.deltaTime; //tiempo en movimiento para cambiar de sentido
        rb.linearVelocity = new Vector2(direction * vel, rb.linearVelocity.y);
        if (tiempoInicio > duracion)
        {
            direction *= -1;    //cambio de direccion, invertir
            rb.linearVelocity = new Vector2(direction * vel, rb.linearVelocity.y);
            tiempoInicio = 0;   //vuelve a sincronizar el tiempo
        }
    }

    private void CambioDireccion()
    {
        transform.localScale = new Vector3(direction, 1, 1);
    }
    #endregion
} // class MoveEnemigo 
  // namespace