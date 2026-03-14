//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System.Drawing;
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
    private GameObject HitboxCuchillo;
    [SerializeField] AimShoot Apuntado;
    [SerializeField] private Transform cuchillo;
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
    private float SetupChuchillo = 0f;
    private bool Knockback = false;
    private float KnockbackDuration;
    private float CChuchillo;
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
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (CChuchillo > CooldownChuchillo)
        {
            Chuchillo();
        }
        else
        {
            CChuchillo = CChuchillo + Time.deltaTime;
        }
        DesCuchillo();

        if (Knockback != false)
        {
            if (Time.time > KnockbackDuration)
            {
                canMove = true;
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

                //El raycast guarda la info en "hit"
                RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f);
                //Saltar cuando se detecta suelo y el boton de saltar esta pulsado o mantenido

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
    public void Empuje(float fuerzaEmpuje, Vector2 dir)
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerzaEmpuje * dir, ForceMode2D.Impulse);
        Knockback = true;
        KnockbackDuration = Time.time * 1.25f;
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
        RaycastHit2D hit = Physics2D.Raycast(Pies.position, Vector2.down, 0.1f);
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
        //Manipulo la velocidad lineal del gameObject en el eje X según lo que recibo del InputManager * Velocidad
        rb.linearVelocity = new Vector2(InputManager.Instance.MovementVector.x * Velocity, rb.linearVelocity.y);
    }

    private void Chuchillo() //Si el boton se presiona y se puede se activa la hitbox del cuchillo 
    {
        if (InputManager.Instance.KnifeWasPressedThisFrame())
        {
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        tocandoPared = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        tocandoPared = false;
    }
    #endregion

} // class PlayerController 
// namespace
