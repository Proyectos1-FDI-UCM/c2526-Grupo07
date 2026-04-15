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
public class DeteccionEnemyAK : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float ChaseDis;    //distancia para perseguir
    [SerializeField] private float ShootDis;    //distancia para disparo
    [SerializeField] private Transform player;
    [SerializeField] private int alturaMax;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float perTime = 0;
    private float forgetTime = 3;   //tiempo que deja de perseguir al jugador
    private EnemyAK move;
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
        move= GetComponent<EnemyAK>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        float dirPlayer = player.position.x - transform.position.x;  //dirección que persigue hacia el jugador
        //calculamos la distancia entre jugador y enemigo
        float disX = Mathf.Abs(dirPlayer);
        float disY = Mathf.Abs(player.position.y - transform.position.y);
        bool dirCorrecta = false;

        if (move.GetDirection() == 1 && dirPlayer > 0) dirCorrecta = true;  //si mira a la derecha
        if (move.GetDirection() == -1 && dirPlayer < 0) dirCorrecta = true;  //si mira a la izquierda

        if (disX < ShootDis && dirCorrecta == true && disY <= alturaMax)   //jugador dentro de la "caja" pequeña
        {
            move.SetShooting(true);
            move.SetChasing(false);
            perTime = forgetTime;
        }
        else if (disX <= ChaseDis && disX > ShootDis && dirCorrecta == true && disY <= alturaMax)
        {
            move.SetChasing(true);
            move.SetShooting(false);
            perTime = forgetTime;
        }

        else     //fuera de la distancia
        {
            perTime -= Time.deltaTime;     //si pasa el tiempo más de 3 segundos, deja de perseguir al jugador
            if (perTime <= 0)
            {
                move.SetChasing(false);
                move.SetShooting(false);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(ShootDis*2, ShootDis, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(ChaseDis*2, ChaseDis, 0));
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

} // class DeteccionEnemyAK 
// namespace
