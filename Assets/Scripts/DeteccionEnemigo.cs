//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEditor.Analytics;
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
    [SerializeField] private float alturaMax; //altura max para detectar al jugador
    [SerializeField] private GameObject excl;       //objeto que avisa la deteccion
    [SerializeField] private LayerMask Interferencias;      //el objeto que separa para no detectar

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
    private float time = 0;     //tiempo que está persiguiendo
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
        excl.SetActive(false);  //desactivar el objeto
    }
    void Update()
    {
        float DirToPlayer = player.position.x - transform.position.x;   //calcular la dirección donde está el jugador

        float distanceX = Mathf.Abs(DirToPlayer);     //valor absoluto de la distancia entre jugador y enemigo
        float distanciaY = Mathf.Abs(player.position.y - transform.position.y);  //altura entre jugador y enemigo
        //Vector2 offset = player.position - transform.position;
        // Solo actúa si el jugador está en la dirección que mira
        //devolver la direccion correcta del jugador
        bool dirCambiada = (move.GetDirection() == 1 && DirToPlayer > 0) ||
                           (move.GetDirection() == -1 && DirToPlayer < 0);
        bool canSeePlayer = dirCambiada && distanciaY <= alturaMax;     //cuándo detecta el jugador

        //antes de ello desactivamos la acción
        move.SetChasing(false);
        move.SetShooting(false);
        shoot.SetCanShoot(false);
        if (distanceX < ShootDis && !HayPared())   //jugador dentro de la "caja" pequeña
        {
            //emieza a atacar al jugador
            //deja de mover
            move.SetShooting(true);
            shoot.SetCanShoot(true);

            excl.SetActive(true);   //se activa la señal de aviso, jugador detectada
            time = forgetTime;  //se reinicia el tiempo en movimiento
        }
        else if (distanceX < ChaseDis && distanceX > ShootDis && canSeePlayer)    //jugador en "caja" grande
        {
            move.SetChasing(true);              //empieza a perseguir al jugador

            excl.SetActive(true);       //se activa la señal de aviso, jugador detectada
            time = forgetTime;      //cada vez se reinicia el tiempo en movimiento
        }
        else //fuera de la distancia, no ataca ni persigue, mueve automáticamente
        {
            excl.SetActive(true);
            time -= Time.deltaTime;     //si pasa el tiempo más de 3 segundos, deja de perseguir al jugador
            if (time <= 0)
            {
                move.SetShooting(false);    //desactivamos las acciones de deteccion
                move.SetChasing(false);
                shoot.SetCanShoot(false);
                excl.SetActive(false);
            }
        }
    }
    void OnDrawGizmos()    // Visualización de distancias
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(ShootDis * 2, ShootDis, 0));    //rojo en caja pequeña, desde la posicion central del enemigo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(ChaseDis * 2, ChaseDis, 0));    //amarillo en caja grande, tmb desde la posicion central del enemigo
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

    private bool HayPared()     //detectar si hay una pared que separa entre enemigo y jugador
    {
        Vector2 direccion = (player.position - transform.position);     //calculamos la direccion del jugador
        float distancia = Vector2.Distance(transform.position, player.position);    //calcular distancia entre jugador y enemigo
        bool hayInterferencia = false;      //hacemos un raycast con los datos anteriores para averiguar si hay una "pared"
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, distancia, Interferencias);
        if (hit.collider != null)
        {
            hayInterferencia = true;    //si se colisiona con un objeto que no sea jugador, es una pared
        }

        return hayInterferencia;
    }
    #endregion
}// class DeteccionEnemigo 
// namespace

