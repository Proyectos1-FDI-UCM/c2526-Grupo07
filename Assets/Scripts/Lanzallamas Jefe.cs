//---------------------------------------------------------
// Permite al jefe disparar el prefab fuego
// Izan Vázquez Sánchez
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using static UnityEngine.GraphicsBuffer;
// Añadir aquí el resto de directivas using


/// <summary>
/// Permite a un enemigo(Jefe) disparar con el lanzallamas
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class LanzallamasJefe : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField] private Transform SalidaFuego; // Un objeto por el que se spawnea y se lanza el fuego
    [SerializeField] private GameObject Fuego; //El prefab que generara cada vez que dispare
    [SerializeField] private Transform PosJugador; //El Jugador al que se le dispararan los fuegos
    [SerializeField] private float Cadencia; //La velocidad a la que podra disparar el jefe
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    Vector2 offset; //Un offset para 
    private float TiempoEntreBalas; //El tiempo que tardara en volver a disparar(Variable)
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    /// <summary>
    /// Cuando se inicializa el tiempo entre balas se inicializa a 0
    /// </summary>
    void Start()
    {
        TiempoEntreBalas = 0f;
    }

    /// <summary>
    /// Se actualiza el tiempo entre balas y se comprueba si se puede llamar al metodo UsarLanzallams
    /// </summary>
    void Update()
    {
        TiempoEntreBalas += Time.deltaTime;
        if (TiempoEntreBalas >= Cadencia)
        {
            UsarLanzallamas();
            TiempoEntreBalas = 0f;
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

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    /// <summary>
    /// Crea y le da la direccion al prefab fuego
    /// </summary>
    public void UsarLanzallamas()
    {
            offset = PosJugador.position - transform.position;
            GameObject Fueguito = Instantiate(Fuego, SalidaFuego.position, SalidaFuego.rotation);
            LogicaFuego DireccionFuego = Fueguito.GetComponent<LogicaFuego>();
            DireccionFuego.Dir(offset);
    }

    #endregion   

} // class LanzallamasJefe 
  // namespace