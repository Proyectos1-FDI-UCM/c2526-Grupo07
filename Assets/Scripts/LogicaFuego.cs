//---------------------------------------------------------
// Permite al prefab fuego hacer daño al jugador, le da la velocidad y todas las caracteristcas 
// Izan Vázquez Sánchez
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Animations;
// Añadir aquí el resto de directivas using


/// <summary>
/// Permite al fuego hacer un daño ajustable al jugador, moverse hacia el con una velocidad ajustable y eliminarse cuando cumpla las condiciones necesarias
/// </summary>
public class LogicaFuego : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private float Velocidad; // la velocidad a la que se movera el fuego
    [SerializeField] private int Daño; // el daño que hara el fuego
    [SerializeField] private float TiempoEnDestruirse; // el tiempo que tarda en destruirse
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float TiempoConVida; //Una variable que almacena el tiempo que ha permanecido activo
    private Vector3 posicionJugador; //Un vector con la posicion actual del jugador
    private Vector2 dir2D;
    Rigidbody2D rb;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Start()
    {
        dir2D = new Vector2(posicionJugador.x, posicionJugador.y).normalized; // Le da la velocidad en la direccion del jugador
        TiempoConVida = Time.time; // Inicializa a 0 el tiempo con vida
    }
    void Update()
    {
        if (Time.time - TiempoConVida > TiempoEnDestruirse)//Actualiza el tiempo con vida y lo destruye si pasa el tiempo maximo con vida lo destruye
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)// Cuando detecta al jugador llama al game manager y le hace daño
        {
            //Llama al GameManager para bajar vida
            GameManager.Instance.RestarVida(Daño);
            player.RedFlash();
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
    /// Le da al rigidbody una direccion y una fuerza hacia dir
    /// </summary>
    public void Dir(Vector2 dir)
    {
        rb = GetComponent<Rigidbody2D>();
        transform.right = dir;
        rb.linearVelocity = dir.normalized * Velocidad;
    }
    
    public void ModifyDestroyTime(float newTime)
    {
        TiempoEnDestruirse = newTime;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class LogicaFuego 
  // namespace