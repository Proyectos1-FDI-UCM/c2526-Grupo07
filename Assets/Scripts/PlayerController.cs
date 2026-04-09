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
    [SerializeField] private GameObject Arma;
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
    private float lastTimeDashed = 2f;
    private float dashTime = 0.2f;
    private float now;
    private float CChuchillo;
    private Animator anim;
    private bool redFlash = false;
    float flashDuration = 0.1f;
    float now2;
    Color originalColor;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f, groundLayer);
        lastTimeDashed += Time.deltaTime;

        now = Time.time;

        if (anim != null)
        {
            if (!anim.GetBool("isAttacking"))
            {
                if (CChuchillo < CooldownChuchillo)
                {
                    CChuchillo += Time.deltaTime;
                }
            }
            else
            {
                CChuchillo = 0f;
            }
        }

        Chuchillo();
        DesCuchillo();

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
            if (!canDash) canDash = true;
        }
        else canDash = false;
        if (InputManager.Instance.DashWasPressedThisFrame())
        {
            if (lastTimeDashed >= cooldownDash)
            {
                Dash();
            }
            else Debug.Log("Refrescando");
        }
        if (redFlash)
        {
            now2 += Time.deltaTime;
            if (now2 > flashDuration)
            {
                spriteRenderer.color = originalColor;
                now2 = 0;
                redFlash = false;
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
        Vector2 dir1 = new Vector2(dir.x, dir.y);
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        if (dir.x < 0.2 && dir.x > 0) dir1.x = 1.1f;
        else if (dir.x > -0.2 && dir.x < 0) dir1.x = -1.1f;
        else if (dir.x < 0) dir1.x = -1f;
        else dir1.x = 1f;
        rb.AddForce(fuerzaEmpuje * dir1, ForceMode2D.Impulse);
        Knockback = true;
        KnockbackDuration = 1.5f;
    }
    public void RecibirDañoJugador(int cantidad)
    {
        GameManager.Instance.HealthPoints(cantidad);
    }
    public void RedFlash()
    {
        spriteRenderer.color = Color.red;
        redFlash = true;
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
        if (anim == null || !anim.GetBool("isAttacking"))
        {
            float horizontalInput = InputManager.Instance.MovementVector.x;

            // Actualizamos animación de caminar
            anim.SetFloat("speed", Mathf.Abs(horizontalInput));
            // Girar sprite según dirección
            if (horizontalInput != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(horizontalInput) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
            if (isDashing == false)
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
                    anim.SetBool("isDashing", isDashing);
                }
            }
        }
    }

    private void Chuchillo() //Si el boton se presiona y se puede se activa la hitbox del cuchillo 
    {
        if (InputManager.Instance.KnifeWasPressedThisFrame() && CChuchillo >= CooldownChuchillo && !anim.GetBool("isAttacking"))
        {
            //Activar hitbox PRIMERO
            if (HitboxCuchillo != null)
            {
                HitboxCuchillo.SetActive(true);
            }

            //Activar animación
            anim.SetBool("isAttacking", true);
            SetupChuchillo = 0f;

            //Orientación
            Vector3 Dir = Apuntado.MousePos();
            cuchillo.localPosition = new Vector3(Dir.x > 0 ? 0.8f : -1.3f, 0f, 0f);
        }
    }

    private void DesCuchillo() //Cuando el cuchillo esta activo lo desactiva cuando pase el tiempo establecido
    {
        if (anim != null && anim.GetBool("isAttacking"))
        {
            SetupChuchillo += Time.deltaTime;

            //Cuando pasa el tiempo del ataque
            if (SetupChuchillo >= 0.4f)
            {
                //Desactivar hitbox
                if (HitboxCuchillo != null)
                {
                    HitboxCuchillo.SetActive(false);
                }

                //Desactivar animación de ataque
                anim.SetBool("isAttacking", false);
                SetupChuchillo = 0f;
            }
        }
    }
    private void Dash()
    {
        float dir;
        if (canDash)
        {
            if (transform.localScale.x > 0) dir = 1f; //Dara +-1 si el jugador está mirando a la izquierda o derecha
            else dir = -1f;
            gameObject.layer = LayerMask.NameToLayer("JugadorDuringDash"); //Cambia la capa de colision
            dashStartTime = Time.time; //Momento en el que inicia el Dash
            isDashing = true;
            rb.linearVelocity = new Vector2(dashDistance * 10f * dir, 0f); //Ejerce fuerza al gameObject
            canDash = false;
            lastTimeDashed = 0f;
            anim.SetBool("isDashing", isDashing);
            Debug.Log("Dashed");
        }
        else Debug.Log("No pudo dashear");
    }
}
    #endregion
// class PlayerController 
// namespace
