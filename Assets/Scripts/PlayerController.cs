//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Runtime.CompilerServices;
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

    //Sprites y animación
    [SerializeField] private FlashRedAux AnimationSprite;   //(BORRAR/ARREGLAR) Para hacer el flash rojo del jugador
    [SerializeField] private SpriteRenderer SpriteJugador; //Lo que se necesita de verdad (sustituye el de arriba)

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
    
    //Dash
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

    //Sprites y animación
    private Animator _anim;             //Animación del personaje
    private Color _originalColor;       //Color original del personaje
    private bool _redFlash = false;     //Poner al personaje en rojo si es true
    private float _flashDuration = 0.1f;//Duración del color rojo en el personaje
    private float _flashInitialTime;    //Tiempo inicio del color rojo
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
        SpriteJugador = GetComponent<SpriteRenderer>();

        _originalColor = SpriteJugador.color; //Guardar color original
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, Saltable);
        
        _lastTimeDashed += Time.deltaTime;
        _now += Time.deltaTime;

        if (_anim != null)
        {
            if (!_anim.GetBool("isAttacking"))
            {
                if (_cuchillo < CooldownChuchillo)
                {
                    _cuchillo += Time.deltaTime;
                }
            }
            else
            {
                _cuchillo = 0f;
            }
        }

        Cuchillo();
        DesCuchillo();

        if (_onKnockback != false)
        {
            if (_now - _knockbackFinish > _knockbackDuration)
            {
                _canMove = true;
                _knockbackFinish = Time.time;
            }
        }

        if (hit.collider != null)
        {
            if (!_canDash) _canDash = true;
        }
        else _canDash = false;
        if (InputManager.Instance.DashWasPressedThisFrame())
        {
            if (_lastTimeDashed >= CooldownDash)
            {
                Dash();
            }
            else Debug.Log("Refrescando");
        }
        if (_redFlash)
        {
            _flashInitialTime += Time.deltaTime;
            if (_flashInitialTime > _flashDuration)
            {
                SpriteJugador.color = _originalColor;
                _flashInitialTime = 0;
                _redFlash = false;
            }
        }
    }
    void FixedUpdate()
    {
        if (InputManager.Instance)
        {
            if (SpriteJugador != null && _canMove != false)
            {
                Salto();
                Moverse();
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
    public void Empuje(float fuerzaEmpuje, Vector2 dir)
    {
        Vector2 dir1 = new Vector2(dir.x, dir.y);
        _canMove = false;
        _rb.linearVelocity = Vector2.zero;
        if (dir.x < 0.2 && dir.x > 0) dir1.x = 1.1f;
        else if (dir.x > -0.2 && dir.x < 0) dir1.x = -1.1f;
        else if (dir.x < 0) dir1.x = -1f;
        else dir1.x = 1f;
        _rb.AddForce(fuerzaEmpuje * dir1, ForceMode2D.Impulse);
        _onKnockback = true;
        _knockbackDuration = 1.5f;
    }

    public void RedFlash()
    {
        AnimationSprite.RedFlash();
    }
    
    public void PlayerPause() 
    {
        Debug.Log("pausado");
        _canMove = false;
        _canDash = false;
        
    }
    public void PlayerContinue()
    {
        _canMove = true;
        _canDash = true;
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
        }
        if (hit.collider != null && InputManager.Instance.JumpIsPressed())
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, SaltoMax);
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
            // Girar sprite según dirección
            if (horizontalInput != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(horizontalInput) * Mathf.Abs(scale.x);
                transform.localScale = scale;
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
        }
    }

    //Si el boton se presiona y se puede se activa la hitbox del cuchillo 
    private void Cuchillo() 
    {
        if (InputManager.Instance.KnifeWasPressedThisFrame() && _cuchillo >= CooldownChuchillo && !_anim.GetBool("isAttacking"))
        {
            //Activar hitbox PRIMERO
            if (HitboxCuchillo != null)
            {
                HitboxCuchillo.SetActive(true);
            }

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
            if (transform.localScale.x > 0) dir = 1f; //Dara +-1 si el jugador está mirando a la izquierda o derecha
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
    private void Pause() // hace que el player este pausado
    {
        _rb.linearVelocity = Vector2.zero;
        _canDash = false;
    }
}
    #endregion
// class PlayerController 
// namespace
