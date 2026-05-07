//---------------------------------------------------------
// Se enfoca en buscar al jugador y disparar balas contra él en su posición
// Zimin Chen
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------


using UnityEngine;
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

    //Jugador
    [SerializeField] private Transform Target; //La posición del jugador

    //Bala
    [SerializeField] private GameObject PrefabBullet;  //Prefab de bala
    [SerializeField] private float MaxBalasPorSeg = 3; //Máxima cantidad de balas que dispara por segundo
    [SerializeField] private Transform SalidaBala;     //Sitio por donde sala la bala
    [SerializeField] private AudioSource SonidoDisparo;//Sonido de disparo


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float _now;        //Registra el tiempo actual
    private Vector2 _offset;   //Calcula el vector hacia el jugador
    private float minInterval; //Intervalo minimo entre un disparo de bala y otra
    private bool _canShoot;    //Indica cuando se puede disparar

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
        //Se indica el intervalo minimo en función de las balas máximas por segundo
        minInterval = 1.0f / MaxBalasPorSeg;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_canShoot)
        {
            _now += Time.deltaTime;
            //Dispara bala si supera el intervalo
            if (_now > minInterval)
            {
                //Guardo en offset la direccion entre el objeto y el target
                _offset = Target.position - transform.position; 

                //Instancia una bala hacia el offset
                GameObject nuevaBala = Instantiate(PrefabBullet, SalidaBala.position, transform.rotation);
                BulletBehaviour balaDir = nuevaBala.GetComponent<BulletBehaviour>();
                balaDir.Dir(_offset);

                //Sonido
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

    /// <summary>
    /// Activa y desactiva el poder disparar.
    /// </summary>
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
    #endregion   
} // class EnemyShoot 
// namespace
