//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class MoveEnemigo : MonoBehaviour
{
    private Coroutine cambioDirCoroutine;       //corrutina para cambio de direccion en dif casos

    private bool movDer = true;    //direccion movimiento a la derecha para aplicar un movimiento automatico
    private Vector3 direccion;  //direccion de movimiento
    private float tiempoInicio; //tiempo iniciado para el movimiento
    private float restante;     //tiempo restante del movimiento para cambiar de direccion
    [SerializeField] private float duracion;    //duracion de tiempo en movimiento 
    [SerializeField] private float vel; //velocidad para el movimiento del enemigo

    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
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
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    #endregion

    void Start()
    {
        //inicio de una corrutina para cambiar la direcion del enemigo
        //cuando el movimiento llega hasta un limite de tiempo
        //o al colisionar con un objeto
        cambioDirCoroutine = StartCoroutine(CambioDireccion());

        //iniciar el tiempo
        tiempoInicio = Time.time;
        movDer=true;
    }
    void Update()
    {
        //movimiento horizontal automatico
        if (movDer) //si va en direccion derecha
        {
            transform.Translate(Vector3.right * vel * Time.deltaTime);      //aplicar movimiento derecha
        }
        else 
            transform.Translate(Vector3.left * vel * Time.deltaTime);      //movimiento izquierda
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //si se colisiona con un objeto(pared)
        //se cambia de direccion
        //y reinicia el tiempo de movimiento
        if (collision.gameObject.CompareTag("Pared"))
        {
            movDer = !movDer;
            //cuando se ha colisionado
            //se reinicia la corrutina
            if(cambioDirCoroutine != null)
            {
                StopCoroutine(cambioDirCoroutine);  //detener la corrutina
            }
            cambioDirCoroutine= StartCoroutine(CambioDireccion());  //volver a iniciar 
        }
    }

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

    IEnumerator CambioDireccion()
    {
        while (true)
        {
            yield return new WaitForSeconds(duracion);
            movDer = !movDer;     //invertir el movimiento del enemigo
            restante = duracion;        //reinicio de tiempo al cambiar de direcion
        }
    }

} // class MoveEnemigo 
// namespace
