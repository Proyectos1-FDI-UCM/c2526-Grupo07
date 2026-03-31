//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Drawing;
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
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float SaltoMax; //Ajustar la altura máxima a la que puede saltar
    [SerializeField]
    private float Velocity; //Velocidad para correr
    [SerializeField]
    private Transform Pies;  //Un empty en los pies para la detección del suelo al saltar
    [SerializeField]
    private float cooldownDash = 3f;
    [SerializeField]
    private float dashDistance = 5f;
    [SerializeField]
    private GameObject HitboxCuchillo;
    [SerializeField] AimShoot Apuntado;
    [SerializeField] private Transform cuchillo;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float CooldownChuchillo = 3f;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private Rigidbody2D rb; //Declaro rb del gameObject para manipular su velocidad al saltar
    private bool tocandoPared = false; //Dejar de moverse horizontalmente a esa dirección si toca pared
    private bool canMove = true; //Ver si se puede mover o no
    private bool canDash = true; //Ver si se puede Dashear o no
    private bool isDashing = false; //Ver si está el dash activo
    private float SetupChuchillo = 0f;
    private bool Knockback = false;
    private float dashStartTime;
    private float KnockbackDuration;
    private float KnockbackFinish;
    private float lastTimeDashed = 0;
    private float dashTime = 0.2f;
    private float now;
    private float CChuchillo;
    private Animator anim;
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
        rb = GetComponent<Rigidbody2D>();
        CChuchillo = 5f;
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, groundLayer);
        lastTimeDashed += Time.deltaTime;

        now = Time.time;

        if (CChuchillo > CooldownChuchillo)
        {
            Chuchillo();
        }
        else
        {
            CChuchillo = CChuchillo + Time.deltaTime;
        }
        if (HitboxCuchillo.activeSelf) //Comprueba si la hitbox del cuchillo esta activada
        {
            //Solo si el cuchillo esta activado empieza a contar los 0.5s
            DesCuchillo();
        }
    
        if (Knockback != false)
        {
            if (now - KnockbackFinish > KnockbackDuration)
            {
                canMove = true;
                KnockbackFinish = Time.time;
            }
        }

        if (hit.collider != null)
        {
            if (canDash != true) canDash = true;
        }
        if (InputManager.Instance.DashWasPressedThisFrame())
        {
            if (lastTimeDashed >= cooldownDash)
            {
                Dash();
            }
        }
    }
    void FixedUpdate()
    {
        if (InputManager.Instance)
        {
            if (spriteRenderer != null && canMove != false)
            {
                Salto();
                Moverse();
                /*if (tocandoPared == false)
                {
                    Vector2 movement = new Vector2(InputManager.Instance.MovementVector.x, 0f) * Velocity * Time.deltaTime;
                    transform.Translate(movement);
                }*/
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
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerzaEmpuje * dir.normalized, ForceMode2D.Impulse);
        Knockback = true;
        KnockbackDuration = 1.5f;
    }

    public void RecibirDañoJugador(int cantidad)
    {
        GameManager.Instance.HealthPoints(cantidad);
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Salto()
    {
        //El raycast guarda la info en "hit"
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, groundLayer);
        //Saltar cuando se detecta suelo y el boton de saltar esta pulsado o mantenido
        if (hit.collider != null && InputManager.Instance.JumpWasPressedThisFrame())
        {
            //Manipulo la velocidad lineal del gameObject en el eje Y según SaltoMax
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, SaltoMax);
        }
        if (hit.collider != null && InputManager.Instance.JumpIsPressed())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, SaltoMax);
        }
    }
    private void Moverse()
    {
        if(isDashing == false)
        {
            //Manipulo la velocidad lineal del gameObject en el eje X según lo que recibo del InputManager * Velocidad
            rb.linearVelocity = new Vector2(InputManager.Instance.MovementVector.x * Velocity, rb.linearVelocity.y);
        }
        else
        {
            if (Time.time - dashStartTime > dashTime)
            {
                gameObject.layer = LayerMask.NameToLayer("Jugador");
                isDashing = false;
            }
        }
    }

    private void Chuchillo() //Si el boton se presiona y se puede se activa la hitbox del cuchillo 
    {
        if (InputManager.Instance.KnifeWasPressedThisFrame())
        {
            anim.SetBool("isAttacking", true); //Se activa la animacion del ataque del cuchillo

            Vector3 Dir = Apuntado.MousePos();
            if (Dir.x > 0)
            {
                cuchillo.localPosition = new Vector3(0.8f, 0f, 0f);
            }
            else
            {
                cuchillo.localPosition = new Vector3(-1.3f, 0f, 0f);
            }

            SetupChuchillo = 0f;
            HitboxCuchillo.SetActive(true);
            CChuchillo = 0;
        }
    }
    private void DesCuchillo() //Cuando el cuchillo esta activo lo desactiva cuando pase el tiempo establecido
    {
        if (HitboxCuchillo)
        {
            SetupChuchillo = SetupChuchillo + Time.deltaTime;
        }
        if (SetupChuchillo >= 0.5f)
        {
            HitboxCuchillo.SetActive(false);
            anim.SetBool("isAttacking", false); //Se desactiva la animacion del ataque del cuchillo
            SetupChuchillo = 0f;
        }
    }
    private void Dash()
    {
        float dir;
        if (canDash)
        {
            if (Apuntado.AimDir().x >= 0) dir = 1f; //Dara +-1 si el jugador está mirando a la izquierda o derecha
            else dir = -1f;
            gameObject.layer = LayerMask.NameToLayer("JugadorDuringDash"); //Cambia la capa de colision
            dashStartTime = Time.time; //Momento en el que inicia el Dash
            isDashing = true; 
            rb.linearVelocity = new Vector2(dashDistance * 10f * dir, 0f); //Ejerce fuerza al gameObject
            canDash = false;
            lastTimeDashed = 0f;
        }
    }
    #endregion

} // class PlayerController 
// namespace
