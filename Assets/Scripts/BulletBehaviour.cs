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
public class BulletBehaviour : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float DestroyTime = 3f; // Tiempo antes de destruirse una bala
    [SerializeField] private float speed = 10f;      // Velocidad de la bala
    [SerializeField] private int Damage;             //Daño de la bala
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float _createBulletMoment; //Cuanto tiempo lleva la bala x creada
    private Rigidbody2D _rb; //RigidBody para manipular la velocidad de la bala
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
        _createBulletMoment = Time.time;

        /*
        if (aimVector != null)
        {
            Vector3 dir = aimVector.AimDir(); // Usa tu método existente
            Vector2 dir2D = new Vector2(dir.x, dir.y).normalized;
            rb.linearVelocity = dir2D * speed;

            float length = Mathf.Sqrt(dir.x * dir.x + dir.y * dir.y);

            // Evitar división por cero si el cursor esta encima del jugador, porque la magnitud es 0
            if (length > 0.01f)
            {
                // Escalar para que la velocidad sea siempre sea la misma
                float factor = speed / length;
                Vector2 velocity = new Vector2(dir.x * factor, dir.y * factor);
                rb.linearVelocity = velocity;
            }
        }*/
    }
    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision != null)
        {
            Destruccion(colision);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Time.time - _createBulletMoment > DestroyTime) //Destruccion si la bala pasa un tiempo determinado, si el Time.time - el momento creacion bala es mayor que su vida
        {
            Destroy(gameObject);
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

    //Método llamado por disparo y apuntado para determinar la dirección de la bala
    public void Dir(Vector2 dir)
    {
        _rb = GetComponent<Rigidbody2D>();
        transform.right = dir;
        _rb.linearVelocity = (dir.normalized * speed);
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    
    //Método llamado cuando hay colision, si choca con Player le resta vida y después destruye la bala
    private void Destruccion(Collider2D colision)
    {
        PlayerController player = colision.gameObject.GetComponent<PlayerController>();
        EnemyHealth enemy = colision.gameObject.GetComponent<EnemyHealth>();
        if (player != null)
        {
            //jugador no se puede disparar a si mismo
            if (gameObject.layer == LayerMask.NameToLayer("bala") && player.gameObject.layer == LayerMask.NameToLayer("Jugador"))
            { return; }
            else
            {
                if (!GameManager.Instance.Invulnerabilidad())
                {
                    //Llama al GameManager para bajar vida
                    GameManager.Instance.RestarVida(Damage);
                    player.RedFlash();
                }
            }
        }
        else if (enemy != null)
        {
            enemy.EnemyHealthPoint(Damage);
            enemy.RedFlash();
        }
        Destroy(gameObject);

    }
    #endregion

} // class BulletBehaviour 
// namespace
