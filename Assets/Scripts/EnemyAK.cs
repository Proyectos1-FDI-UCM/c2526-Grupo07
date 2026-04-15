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
public class EnemyAK : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float time;    //tiempo para cambiar de sentido
    [SerializeField] private float speed;   
    [SerializeField] private Transform player;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float duracion=0;     //tiempo que dura en movimiento
    private int direction = 1;  //direccion derecha
    private Rigidbody2D rb;

    private bool isChasing = false; //controlar si está persiguiendo
    private bool isShooting= false; //controlar si está disparando
    //script disaro enemigoAK
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
        rb= GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Vector2 offset = player.transform.position - transform.position;
        if (isShooting)
        {
            //cuando dispara se deja de mover
            rb.linearVelocity = Vector2.zero;
            //ver posición del jugador y cambiar a esa dirección
            if (offset.x > 0 && direction != 1)
            {
                direction *= -1;
            }
            if (offset.x < 0 && direction != -1)
            {
                direction *= -1;
            }
            return;
        }
        if (isChasing)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            if (offset.x > 0 && direction != 1)
            {
                direction *= -1;
            }
            if (offset.x < 0 && direction != -1)
            {
                direction *= -1;
            }
            duracion = 0;
        }
        else MovAuto();        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //si se colisiona con la pared cambia de direccion
        //vuelve a sincronizar el tiempo
        if (collision.gameObject.CompareTag("Pared"))
        {
            direction *= -1;
            duracion = 0;
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
        return direction;   //devolver la direccion del enemigo para detectar con el jugador
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
    private void MovAuto()
    {
        //el enemigo mueve en un tiempo determinado
        //si pasa el tiempo cambia de dirección
        //y se vuelve a sincronizar el tiempo
        duracion+= Time.deltaTime;
        rb.linearVelocity= new Vector2(direction * speed, rb.linearVelocity.y);
        if(duracion > time)
        {
            direction *= -1;
            duracion=0;
        }
    }
    #endregion   

} // class EnemyAK 
// namespace
