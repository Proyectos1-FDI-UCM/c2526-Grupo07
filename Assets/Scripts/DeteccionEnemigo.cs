//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEditor;
using UnityEditor.Analytics;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class DeteccionEnemigo : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private Transform player;  //objeto que persigue y dispara (jugador)
    [SerializeField] private float ChaseDis;    //distancia para perseguir
    [SerializeField] private float ShootDis;  //distancia para disparo

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float forgetTime = 3;   //tiempo para dejar de perseguir
    private float time = 0;
    private MoveEnemigo move;   //script de movimiento de enemigo
    private EnemyShoot shoot;   //script de disparo de enemigo
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
        move = GetComponent<MoveEnemigo>();
        shoot = GetComponent<EnemyShoot>();
    }
    void Update()
    {
        float DirToPlayer = player.position.x - transform.position.x;   //dirección hacia el jugador
        float distance = Vector2.Distance(transform.position, player.position);     //distancia entre jugador y enemigo
        bool dirCambiada = false;

        // Solo actúa si el jugador está en la dirección que mira
        if (move.GetDirection()==1 && DirToPlayer>0)    //cuando mira a la derecha
        {
            dirCambiada= true;
        }
        if (move.GetDirection() == -1 && DirToPlayer < 0)   //cuando mira a la izquierda
        {
            dirCambiada = true;
        }
        //se detecta si el jugador está en la direccion del enemigo
        if (distance < ShootDis && dirCambiada==true)   //jugador dentro de la "caja" pequeña
        {
            move.SetShooting(true);
            move.SetChasing(false);
            if (shoot != null)
            {
                shoot.enabled = true;   //activar disparo al jugador
            }
            time = forgetTime;  //reinicia el tiempo
        }
        else if (distance <= ChaseDis && distance > ShootDis && dirCambiada == true)    //"caja" grande
        {
            move.SetChasing(true);
            move.SetShooting(false);
            if (shoot != null)
            {
                shoot.enabled = false;   //desactivar disparo al jugador
            }
            time = forgetTime;  //reinicia el tiempo
        }
        else //fuera de la distancia
        {
            time -= Time.deltaTime;     //si pasa el tiempo más de 3 segundos, deja de perseguir al jugador
            if (time <= 0)
            {
                move.SetShooting(false);
                move.SetChasing(false);
                if (shoot != null)
                {
                    shoot.enabled = false;   //desactivar disparo al jugador
                }
            }
        }
    } 
    void OnDrawGizmos()    // Visualización de distancias
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(ShootDis, ShootDis, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(ChaseDis,ChaseDis,0));
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
}// class DeteccionEnemigo 
// namespace

