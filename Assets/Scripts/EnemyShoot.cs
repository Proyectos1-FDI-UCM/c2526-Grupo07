//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class EnemyShoot : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    private Transform Target; //La posición del jugador
    [SerializeField]
    private GameObject PrefabBullet;
    [SerializeField]
    private float MaxBalasPorSeg = 3;
    [SerializeField]
    private float HoraDisparo;
    [SerializeField]
    private Transform SalidaBala;
    [SerializeField]
    private AudioSource SonidoDisparo;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float minInterval;
    private bool _canShoot;
    private float _now;
    Vector2 offset; //Calcula el vector entre la posición del jugador y la del enemigo

    private MoveEnemigo move;

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
        minInterval = 1.0f / MaxBalasPorSeg;
        move = GetComponent<MoveEnemigo>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_canShoot)
        {
            _now += Time.deltaTime;
            if (_now > minInterval)
            {
                //Guardo en offset la direccion entre el objeto y el target
                offset = Target.position - transform.position;
                GameObject nuevaBala = Instantiate(PrefabBullet, SalidaBala.position, transform.rotation);
                BulletBehaviour balaDir = nuevaBala.GetComponent<BulletBehaviour>();
                balaDir.Dir(offset);
                SonidoDisparo.Play();
                _now = 0;
            }
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

    public void SetCanShoot(bool canShoot)
    {
        _canShoot = canShoot; 
    }
    #endregion
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void CambioDir()
    {
        transform.position = new Vector2(-transform.position.x,transform.position.y);
    }
    #endregion   
} // class EnemyShoot 
// namespace
