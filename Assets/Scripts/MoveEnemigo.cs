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

    [SerializeField] private float duracion;       //duracion de tiempo en movimiento 
    [SerializeField] private float duracionQuieto; // tiempo en que va a estar quieto

    [SerializeField] private float vel;        //velocidad para el movimiento del enemigo
    [SerializeField] private Transform player; //jugador para realizar las acciones

    [SerializeField] private AudioSource soundMove;
    [SerializeField] private Transform salidaBala;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private bool _isChasing = false;  //controlar si está persiguiendo al jugador
    private bool _isShooting = false; //controlar si está disparando al jugador
    private int _direction = 1;       //direccion del enemigo
    private float _tiempoInicio = 0;  //tiempo iniciado para el movimiento
    private float _tiempoQuieto = 0;  //tiempo inicial en que va a estar quieto
    private Rigidbody2D _rb;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
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
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Vector2 offset = player.transform.position - transform.position;
        if (_isShooting)
        {
            if (soundMove != null)
            {
                soundMove.Pause();
            }
            //cuando dispara se deja de mover
            _isChasing = false;

             if (offset.x > 0 && _direction != 1)
             {
                 _direction *= -1;
                 CambioDireccion();
             }
             if (offset.x < 0 && _direction != -1)
             {
                 _direction *= -1;
                 CambioDireccion();
            }
            return;
        }
        else if (_isChasing)
        {
            if (soundMove != null)
            {
                soundMove.Play();
            }
            else
            {
                Debug.Log("No hay sonido (soundMove) en MoveEnemigo");
            }
            if (offset.x > 0 && _direction != 1)
            {
                _direction *= -1;
                CambioDireccion();
            }
            else if (offset.x < 0 && _direction != -1)
            {
                _direction *= -1;
                CambioDireccion();
            }
            _tiempoInicio = 0;
        }
    }
    void FixedUpdate()
    {
        if (_anim != null)
        {
           float speed = Mathf.Abs(_rb.linearVelocity.x); //Valor absoluto de la velocidad en el eje x
           _anim.SetFloat("enemySpeed", speed); //Para la transicion
        }
        if (_isShooting)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
        else if (_isChasing)
        {
            Perseguir();
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
             _direction *= -1;
             CambioDireccion();
             _tiempoInicio = 0;
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
        return _direction;   //devolver la direccion del enemigo para detectar con el jugador
    }

    //devolver el valor booleano si está realizando esta acción
    public void SetChasing(bool chasing)
    {
        _isChasing = chasing;
    }
    public void SetShooting(bool shooting)
    {
        _isShooting = shooting;
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
         _rb.linearVelocity = new Vector2(_direction * vel, _rb.linearVelocity.y);
    }
    private void MovAuto()
    {
        _tiempoInicio += Time.deltaTime; //tiempo en movimiento para cambiar de sentido
        _rb.linearVelocity = new Vector2(_direction * vel, _rb.linearVelocity.y);
        if (_tiempoInicio > duracion)
        {
            _tiempoQuieto += Time.deltaTime;
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            if (_tiempoQuieto > duracionQuieto)
            {
                _direction *= -1;    //cambio de direccion, invertir
                CambioDireccion();
                _rb.linearVelocity = new Vector2(_direction * vel, _rb.linearVelocity.y);
                _tiempoQuieto = 0;
                _tiempoInicio = 0;   //vuelve a sincronizar el tiempo
                if (soundMove != null)
                {
                    soundMove.Play();
                }
                else
                {
                    Debug.Log("No hay sonido (soundMove) en MoveEnemigo");
                }
            }
         }
    }

    private void CambioDireccion()
    {
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (_direction == 1)
        {
            _spriteRenderer.flipX = false;
            salidaBala.transform.position = new Vector2(transform.position.x + 0.7f, transform.position.y);
        }
        if (_direction == -1)
        {
            _spriteRenderer.flipX = true;
            salidaBala.transform.position = new Vector2(transform.position.x - 0.8f, transform.position.y);
        }
    }
    #endregion
}
 // class MoveEnemigo 
  // namespace