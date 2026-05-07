//---------------------------------------------------------
// Ataques del minijefe
// Cristopher Jeremy Villacís Galindo
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Gestiona los dos patrones de ataque que tiene el Minijefe
/// </summary>
public class MiniJefe : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private GameObject _fuego;                 //Prefab que se va a instanciar
    [SerializeField] private float _timeBtwActions = 5f;        //Tiempo de enfriamiento entre acciones
    [SerializeField] private float _runSpeed = 0.5f;            //Suavizado para el Vector.Lerp
    [SerializeField] private float _restShootingTime = 0.2f;    //Tiempo entre disparos
    [SerializeField] private Transform _player;                 //Posición del jugador
    [SerializeField] private Vector2 _medioArena;               //Posición central de la zona de combate
    [SerializeField] private AudioSource FuegoSFX;              //Sonido lanzallamas
    [SerializeField] private AudioSource RunSFX;                //Sonido al correr
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float _lastTimeAction = 0;          //Tiempo transcurrido tras la última acción
    private float _startTimeAtack = 0;          //Tiempo transcurrido tras el inicio del ataque
    private float _shootingTime = 0;            //Tiempo transcurrido tras haber empezado a disparar
    private float _intervaloDisparos = 3f / 5f; //Tiempo que dura cada disparo
    private float _durationAtack = 4f;          //Duración total del ataque
    private float _now = 0;                     //Tiempo actual
    private bool _canAtack = false;             //Booleano que marca si puede o no atacar
    private bool _isAtacking = false;           //Booleano que determina si se está atacando
    private Rigidbody2D _rb;                    //Variable que adoptará el componente Rigidbody2D
    private Animator _animator;                 //Será el componente Animator
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
        _rb = GetComponent<Rigidbody2D>();      //Asigna el componente Rigidbody2D a _rb
        _animator = GetComponent<Animator>();   //Asigna el componente Animator a _animator
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        _now += Time.deltaTime;
        _lastTimeAction += Time.deltaTime;
        _canAtack = CondicionAtaque();

        //Si no está atacando entra y comprueba la condición de ataque y de tiempo
        if (!_isAtacking)
        {
            if (_lastTimeAction >= _timeBtwActions && _canAtack)
            {
                _isAtacking = true;
                _startTimeAtack = 0;
                Debug.Log("Inicia el ataque");
            }
            if (_now > _intervaloDisparos)
            {
                DisparoLejos();
                _now = 0;
            }
        }
        else
        {
            _startTimeAtack += Time.deltaTime;
            if (_startTimeAtack < 1f) transform.position = Vector3.Lerp(transform.position, _medioArena, _runSpeed);
            if (_startTimeAtack >= 1f && _startTimeAtack < _durationAtack)
            {
                if (!_animator.GetBool("Ataque"))
                {
                    _animator.SetBool("Ataque", true); //El parámetro "Ataque" se vuelve true
                }
                _shootingTime += Time.deltaTime;
                //Ataca disparando proyectiles a 5 direcciones en lo que resta de ataque
                if (_shootingTime < _intervaloDisparos - _restShootingTime)
                {
                    Lanzallamas(new Vector2(-2, 1));
                }
                else if (_shootingTime >= _intervaloDisparos && _shootingTime < 2f * _intervaloDisparos - _restShootingTime)
                {
                    Lanzallamas(new Vector2(-1, 2));
                }
                else if (_shootingTime >= 2f * _intervaloDisparos && _shootingTime < 3f * _intervaloDisparos - _restShootingTime)
                {
                    Lanzallamas(new Vector2(0, 1));
                }
                else if (_shootingTime >= 3f * _intervaloDisparos && _shootingTime < 4 * _intervaloDisparos - _restShootingTime)
                {
                    Lanzallamas(new Vector2(1, 2));
                }
                else if (_shootingTime >= 4f *_intervaloDisparos && _shootingTime < 5f * _intervaloDisparos - _restShootingTime)
                {
                    Lanzallamas(new Vector2(2, 1));
                }
            }
            //Una vez terminado el ataque se aleja y se reinicia todo
            if (_startTimeAtack >= _durationAtack)
            {
                _animator.SetBool("Ataque", false); //El parámetro "Ataque" se vuelve false
                Alejarse();
                _shootingTime = 0;
                _isAtacking = false;
                _lastTimeAction = 0;
                Debug.Log("Ha terminado el ataque");
            }
        }

        Animaciones(); //Actualiza las animaciones

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

    /// <summary>
    /// Devuelve true si la condición de ataque se cumple
    /// </summary>
    /// <returns></returns>
    private bool CondicionAtaque()
    {
        bool _ataca = false;
        if (Mathf.Abs(transform.position.x - _player.transform.position.x) >= 8f)
        {
            _ataca = true;
        }
        return _ataca;
    }

    /// <summary>
    /// Impulsa al mini jefe a la dirección contraria a la que se encuentre el jugador
    /// </summary>
    private void Alejarse()
    {
        int dir = 1;
        if (transform.position.x - _player.transform.position.x < 0) dir = -1;
        _rb.AddForceX(8f * dir, ForceMode2D.Impulse);
    } 

    /// <summary>
    /// Dispara un proyectil en la dirección dada
    /// y se modifican ciertos parámetros
    /// </summary>
    /// <param name="dir"></param>
    private void Lanzallamas(Vector2 dir)
    {
        GameObject _proyectil = Instantiate(_fuego, transform.position, transform.localRotation);
        LogicaFuegoMiniBoss _fuegoLogic = _proyectil.GetComponent<LogicaFuegoMiniBoss>();
        Rigidbody2D _rbFuego = _proyectil.GetComponent<Rigidbody2D>();
        _rbFuego.gravityScale = 1.5f;
        _fuegoLogic.Dir(dir);
        _fuegoLogic.ModifyDestroyTime(3f);
    }

    /// <summary>
    /// Dispara un proyectil con dirección al jugador
    /// y se modifican ciertos parámetros
    /// </summary>
    private void DisparoLejos()
    {
        Vector3 offset = _player.transform.position - transform.position;
        GameObject _proyectil = Instantiate(_fuego, transform.position, transform.localRotation);
        LogicaFuegoMiniBoss _fuegoLogic = _proyectil.GetComponent<LogicaFuegoMiniBoss>();
        Rigidbody2D _rbFuego = _proyectil.GetComponent<Rigidbody2D>();
        _rbFuego.gravityScale = 0f;
        _fuegoLogic.Dir(offset);
        _fuegoLogic.ModifyDestroyTime(5f);
    }

    /// <summary>
    /// Método para actualizar las animaciones del Mini Jefe
    /// </summary>
    private void Animaciones()
    {
        bool miraDerecha = (_player.position.x - transform.position.x) > 0; //El jefe mira hacia el jugador
        _animator.SetBool("MiraDerecha", miraDerecha);

        bool enMovimiento = false; //Si el mini jefe se está moviendo

        bool mueveDerecha = false; //Si se mueve hacia la derecha

        //Comprobar si se mueve al alejarse
        if (Mathf.Abs(_rb.linearVelocity.x) > 0.1f)
        {
            enMovimiento = true;
            mueveDerecha = _rb.linearVelocity.x > 0;
        }
        //Comprueba si se mueve al atacar
        else if (_isAtacking && _startTimeAtack < 1f)
        {
            //Comprueba que no llegó al centro
            if (Mathf.Abs(transform.position.x - _medioArena.x) > 0.1f)
            {
                enMovimiento = true;
                mueveDerecha = (_medioArena.x - transform.position.x) > 0;
            }
        }
        //Animaciones para moverse y hacia donde se mueve
        _animator.SetBool("EnMovimiento", enMovimiento);
        _animator.SetBool("MueveDerecha", mueveDerecha);
        
    }

    private void SonidoFuego()
    {
        if(FuegoSFX != null)
        {
            FuegoSFX.Play();
        }
    }

    /// <summary>
    /// Sonido del mini jefe al correr
    /// </summary>
    private void SonidoRun()
    {
        if (RunSFX != null)
        {
            RunSFX.Play();
        }
    }

    /// <summary>
    /// Sonido del mini jefe al dejar de correr
    /// </summary>
    private void StopRun()
    {
        if (RunSFX != null)
        {
            RunSFX.Stop();
        }
    }

    #endregion
}
// class MiniJefe 
// namespace
