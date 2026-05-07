//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ---- 
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    //Movimiento
    [SerializeField] private float Velocidad = 7f; //Velocidad para correr
    [SerializeField] private float SaltoMax = 12f; //Ajustar la altura máxima a la que puede salta
    [SerializeField] private LayerMask Saltable;   //Capas donde el jugador puede saltar
    [SerializeField] private Transform Pies;       //Un empty en los pies para la detección del suelo al saltar

    //Dash
    [SerializeField] private float CooldownDash = 3f;  //Enfriamientodel Dash
    [SerializeField] private float DistanciaDash = 5f; //Distancia a la que puede avanzar el Dash

    //Cuchillo
    [SerializeField] private float CooldownChuchillo = 3f; //Enfriamiento del uso del cuchillo
    [SerializeField] private GameObject HitboxCuchillo;    //Area donde se puede hacer daño con el cuchillo
    [SerializeField] private SpriteRenderer SpriteJugador; //Sprite del jugador
    [SerializeField] private SpriteRenderer SpriteCuchillo;//Sprite del cuchillo
    [SerializeField] private TextMeshProUGUI TextCuchillo; //Couldown de cuchillo

    //Sonido
    [SerializeField] private AudioSource[] soundMove;
    [SerializeField] private AudioSource soundDash;
    [SerializeField] private AudioSource soundJump;
    [SerializeField] private AudioSource soundDead;
    [SerializeField] private AudioSource soundPop;
    [SerializeField] private AudioSource soundCuchillo;
    [SerializeField] private AudioSource soundDamage;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints   

    //Tiempo
    private float _now;                 //Tiempo del juego

    //RigidBody y movimiento
    private Rigidbody2D _rb;            //Declaro rb del gameObject para manipular su velocidad al saltar
    private bool _canMove = true;       //Ver si se puede mover o no
    private bool _onFloor = true;       //Ver si está en el suelo
    private int _sonidoActual = 0;      //Sonido de correr pendiente
    private int _sonidoAnterior = 2;

    //Dash
    private bool _lookingRight = true;  //Ver a qué dirección Dashear
    private bool _canDash = true;       //Ver si se puede Dashear o no
    private bool _isDashing = false;    //Ver si está el dash activo
    private float _dashStartTime;       //Tiempo cuando empieza el Dash
    private float _lastTimeDashed = 2f; //Última vez que ha dasheado
    private float _dashTime = 0.2f;     //Cuanto dura el Dash

    //Empuje
    private bool _onKnockback = false;  //True si el jugador esta siendo empujado
    private float _knockbackDuration;   //Duración del empuje (jugador no puede moverse durante esta duración)
    private float _knockbackFinish;     //Termina el empuje

    //Cuchillo
    private float _coolDownCuchillo;    //Enfriamiento del uso del cuchillo
    private float _cuchillo = 5f;
    private Color _originalKnifeColor;  //Color de inicio del cuchillo

    //Sprites y animación
    private Animator _anim;             //Animación del personaje
    private Color _originalColor;       //Color original del personaje
    private bool _redFlash = false;     //Poner al personaje en rojo si es true
    private float _flashDuration = 0.1f;//Duración del color rojo en el personaje

    private float _recibeDaño;          //Tiempo inicial cuando ha recibido daño

    private Color _transparency;            //Transparencia del jugador
    private float _parpadeoDuracion = 1.5f; //Parpadeo cuando es invulnerable
    private float _intervaloParpadeo = 0.2f;//El intervalo entre un parpadeo y otro
    private bool _parpadeando = false;      //Estado que indica si está parpadeando o no
    private float _tiempoInicioParpadeo;    //Tiempo para parpadear

    #endregion

    private int _consumibleActual = 0; //En que consumible se esta
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
        SpriteJugador = GetComponent<SpriteRenderer>();
        _transparency.a = 0.1f;
        _originalColor = SpriteJugador.color; //Guardar color original
        _originalKnifeColor = SpriteCuchillo.color; //Guardar color original
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!LevelManager.Instance.IsPaused())
        {
            //Guardar tiempos
            _lastTimeDashed += Time.deltaTime;
            _now += Time.deltaTime;

            //Activar animación de cuchillo
            if (_anim != null)
            {
                if (!_anim.GetBool("isAttacking"))
                {
                    if (_cuchillo < CooldownChuchillo)
                    {
                        _cuchillo += Time.deltaTime;
                        TextCuchillo.text = "" + Mathf.Round(CooldownChuchillo - _cuchillo + 0.5f);
                    }
                }
                else
                {
                    _cuchillo = 0f;
                }
            }

            Cuchillo();
            DesCuchillo();

            //Si el jugador es empujado, tarda un rato antes de poder moverse
            if (_onKnockback != false)
            {

                if (_now - _knockbackFinish > _knockbackDuration)
                {
                    _canMove = true;
                    _knockbackFinish = Time.time;
                }
            }

            //Si el jugador está en el aire no puede dashear
            RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, Saltable);
            if (hit.collider != null)
            {
                if (!_canDash) _canDash = true;
            }
            else _canDash = false;

            //Ejecuta el dash si ha pasado del cooldown
            if (InputManager.Instance.DashWasPressedThisFrame())
            {
                if (_lastTimeDashed >= CooldownDash)
                {
                    soundDash.Play();
                    Dash();
                }
                else Debug.Log("Refrescando");
            }

            //Si ha recibido daño, ejecuta un efecto visual de flash rojo
            if (_redFlash || _parpadeando)
            {
                _recibeDaño += Time.deltaTime;
                if (_recibeDaño > _flashDuration && _redFlash)
                {
                    SpriteJugador.color = _originalColor;
                    _recibeDaño = 0;
                    _redFlash = false;
                }
                if (_recibeDaño < _parpadeoDuracion && _parpadeando)
                {
                    _tiempoInicioParpadeo += Time.deltaTime;

                    if (_tiempoInicioParpadeo > _intervaloParpadeo)
                    {
                        CambioParpadeo();
                        _tiempoInicioParpadeo = 0;
                    }
                }
                else
                {
                    SpriteJugador.color = _originalColor;
                    _recibeDaño = 0;
                    _parpadeando = false;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (!LevelManager.Instance.IsPaused())
        {
            //Movimiento del jugador 
            if (InputManager.Instance)
            {
                if (SpriteJugador != null && _canMove != false)
                {
                    Salto();
                    Moverse();
                }
            }
        }
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----
    #region Métodos públicos
    // Documentar cada método que aparece aquí con 
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    // Ejemplo: GetPlayerController

    //Cuando el jugador choca con objetos que lo empujan
    public void Empuje(float fuerzaEmpuje, Vector2 dir)
    {
        Vector2 dir1 = new Vector2(dir.x, dir.y);
        _rb.linearVelocity = Vector2.zero;
        if (dir.x < 0.2 && dir.x > 0) dir1.x = 1.2f;
        else if (dir.x > -0.2 && dir.x < 0) dir1.x = -1.2f;
        else if (dir.x < 0) dir1.x = -1f;
        else dir1.x = 1f;
        dir.y = 1f;
        _rb.AddForce(fuerzaEmpuje * dir1, ForceMode2D.Impulse);
        _onKnockback = true;
        _knockbackDuration = 1.5f;
    }

    //Activar efecto visual de flash rojo al recibir daño
    public void RedFlash()
    {
        SpriteJugador.color = Color.red;
        _redFlash = true;
        _parpadeando = true;
        soundDamage.Play();
    }

    //Reproducir sonido al recoger objeto
    public void PlayPop()
    {
        soundPop.Play();
    }
   
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    //Método para saltar
    private void Salto()
    {
        //El raycast guarda la info en "hit"
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, Saltable);

        //Saltar cuando se detecta suelo y el boton de saltar esta pulsado o mantenido
        if (hit.collider != null && InputManager.Instance.JumpWasPressedThisFrame())
        {
            //Manipulo la velocidad lineal del gameObject en el eje Y según SaltoMax
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, SaltoMax);
            soundJump.Play();

        }
        if (hit.collider != null && InputManager.Instance.JumpIsPressed())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, SaltoMax);
            soundJump.Play();

        }
    }
    //Método para moverse horizontalmente y dash
    private void Moverse()
    {
        if (_anim == null || !_anim.GetBool("isAttacking"))
        {
            float horizontalInput = InputManager.Instance.MovementVector.x;

            // Actualizamos animación de caminar
            _anim.SetFloat("speed", Mathf.Abs(horizontalInput));
            // Girar el gameObject entero según dirección
            
            //VERSION ANTIGUA
            //if (horizontalInput != 0)
            //{
            //    Vector2 scale = transform.localScale;
            //    scale.x = Mathf.Sign(horizontalInput) * Mathf.Abs(scale.x);
            //    transform.localScale = scale;
            //}
            //NUEVA VERSION GIRANDO SOLO EL SPRITE
            if (SpriteJugador != null && horizontalInput != 0)
            {
                SpriteJugador.flipX = horizontalInput < 0;
                _lookingRight = horizontalInput > 0;

            }
            if (_isDashing == false)
            {
                //Manipulo la velocidad lineal del gameObject en el eje X según lo que recibo del InputManager * Velocidad
                _rb.linearVelocity = new Vector2(InputManager.Instance.MovementVector.x * Velocidad, _rb.linearVelocity.y);
            }
            else
            {
                if (Time.time - _dashStartTime > _dashTime)
                {
                    gameObject.layer = LayerMask.NameToLayer("Jugador");
                    _isDashing = false;
                    _anim.SetBool("isDashing", _isDashing);
                }
            }
            if (SpriteJugador != null && horizontalInput != 0 && _isDashing == false)
            {
                if (!soundMove[(_sonidoAnterior)].isPlaying)
                {
                    soundMove[_sonidoActual].Play();
                    _sonidoAnterior = _sonidoActual;
                    if (_sonidoActual < 2) _sonidoActual++;
                    else _sonidoActual = 0;
                }
            }
        }
    }

    //Si el boton se presiona y se puede se activa la hitbox del cuchillo 
    private void Cuchillo() 
    {
        if (_cuchillo >= CooldownChuchillo && SpriteCuchillo.color != _originalKnifeColor)
        {
            TextCuchillo.text = "Ready";
            SpriteCuchillo.color = _originalKnifeColor;
        }

        if (InputManager.Instance.KnifeWasPressedThisFrame() && _cuchillo >= CooldownChuchillo && !_anim.GetBool("isAttacking"))
        {
            //Activar hitbox PRIMERO
            if (HitboxCuchillo != null)
            {
                HitboxCuchillo.SetActive(true);
            }
            SpriteCuchillo.color = new Color32(255, 80, 80, 255);
            soundCuchillo.Play();
            //Activar animación
            _anim.SetBool("isAttacking", true);
            _coolDownCuchillo = 0f;
        }
    }
    //Cuando el cuchillo esta activo lo desactiva cuando pase el tiempo establecido
    private void DesCuchillo() 
    {
        if (_anim != null && _anim.GetBool("isAttacking"))
        {
            _coolDownCuchillo += Time.deltaTime;

            //Cuando pasa el tiempo del ataque
            if (_coolDownCuchillo >= 0.4f)
            {
                //Desactivar hitbox
                if (HitboxCuchillo != null)
                {
                    HitboxCuchillo.SetActive(false);
                }

                //Desactivar animación de ataque
                _anim.SetBool("isAttacking", false);
                _coolDownCuchillo = 0f;
            }
        }
    }
    //Método para ejecutar el dash
    private void Dash()
    {
        float dir;
        if (_canDash)
        {
            soundDash.Play();
            if (_lookingRight) dir = 1f; //Dara +-1 si el jugador está mirando a la izquierda o derecha
            else dir = -1f;
            gameObject.layer = LayerMask.NameToLayer("JugadorDuringDash"); //Cambia la capa de colision
            _dashStartTime = Time.time; //Momento en el que inicia el Dash
            _isDashing = true;
            _rb.linearVelocity = new Vector2(DistanciaDash * 10f * dir, 0f); //Ejerce fuerza al gameObject
            _canDash = false;
            _lastTimeDashed = 0f;
            _anim.SetBool("isDashing", _isDashing);
            Debug.Log("Dashed");
        }
        else Debug.Log("No pudo dashear");
    }

    // Usa el consumible equipado usando GameManager
    private void CambioParpadeo()
    {
        if (SpriteJugador.color == _transparency)
        {
            SpriteJugador.color = _originalColor;
        }
        else
        {
            SpriteJugador.color = _transparency;
        }
    }
}
    #endregion
// class PlayerController 
// namespace
