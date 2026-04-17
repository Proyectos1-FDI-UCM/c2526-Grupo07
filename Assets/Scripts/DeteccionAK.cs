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
public class DeteccionAK : MonoBehaviour
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
    [SerializeField] private GameObject excl;
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
    private EnemyAK move;   //script de movimiento de enemigo
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
    void Start()
    {
        move = GetComponent<EnemyAK>();
        shoot = GetComponent<EnemyShoot>();
        excl.SetActive(false);
    }
    void Update()
    {
        float DirToPlayer = player.position.x - transform.position.x;   //dirección hacia el jugador

        float distanceX = Mathf.Abs(DirToPlayer);     //distancia entre jugador y enemigo
        float distanciaY = Mathf.Abs(player.position.y - transform.position.y);  //altura entre jugador y enemigo

        // Solo actúa si el jugador está en la dirección que mira
        //devolver la direccion correcta del jugador
        bool dirCambiada = (move.GetDirection() == 1 && DirToPlayer > 0) ||
                           (move.GetDirection() == -1 && DirToPlayer < 0);
        bool canSeePlayer = dirCambiada && distanciaY <= alturaMax;     //cuándo detecta el jugador

        //antes de ello desactivamos la acción
        move.SetChasing(false);
        move.SetShooting(false);
        shoot.SetCanShoot(false);
        if (distanceX < ShootDis && canSeePlayer)   //jugador dentro de la "caja" pequeña
        {
            move.SetShooting(true);
            shoot.SetCanShoot(true);

            excl.SetActive(true);
            time = forgetTime;  //reinicia el tiempo
        }
        else if (distanceX <= ChaseDis && distanceX > ShootDis && canSeePlayer)    //"caja" grande
        {
            move.SetChasing(true);
            excl.SetActive(true);
            
            time = forgetTime;  //reinicia el tiempo
        }
        else //fuera de la distancia
        {
            excl.SetActive(true);
            time -= Time.deltaTime;     //si pasa el tiempo más de 3 segundos, deja de perseguir al jugador
            if (time <= 0)
            {
                //excl.SetActive(false);
                move.SetShooting(false);
                move.SetChasing(false);
                shoot.SetCanShoot(false);
                excl.SetActive(false);
            }
        }
    }
    void OnDrawGizmos()    // Visualización de distancias
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(ShootDis * 2, ShootDis, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(ChaseDis * 2, ChaseDis, 0));
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

    #endregion

} // class DeteccionAK 
// namespace
