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
public class AtaqueJefe : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private Transform player;      //lanzar objeto al jugador
    [SerializeField] private Transform puntoAtaque;     //punto donde lanza el objeto
    [SerializeField] private GameObject granadaPrefab;  //objeto que lanza

    [SerializeField] private float fuerzaVertical;  //altura max que llega el objeto
    [SerializeField] private float vel;     //velocidad de lanzamiento

    [SerializeField] private float rangoAtaque; //distancia para atacar
    [SerializeField] private float cooldown;    //tiempo de enfriamiento

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float time=0; //tiempo para el siguiente ataque
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
        
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //calcular la distancia entre el jefe y el jugador
        float distancia = Vector2.Distance(transform.position, player.position);

        //si el jugador está lejos lanza granada
        if (Time.time >= time)
        {
            if(distancia >= rangoAtaque)
            {
                LanzarGranada();
                time = Time.time + cooldown;    //añadir un tiempo de enfriamiento para el siguiente ataque
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

    #endregion
    
    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void LanzarGranada()
    {
        if (granadaPrefab == null || puntoAtaque == null || player == null) return;

        //añadir un origen y posicion del punto de ataque para granada
        GameObject granada = Instantiate(granadaPrefab, puntoAtaque.position, Quaternion.identity);

        Rigidbody2D rb= granada.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            //arco de movimiento
            Vector2 direccion = (player.position - puntoAtaque.position).normalized;    //vector horizontal hacia el jugador
            float fuerzaX = direccion.x * vel;    //aplicar fuerza x
            float fuerzaY = fuerzaVertical;     //fuerza vertical fija 
            rb.linearVelocity= new Vector2(fuerzaX, fuerzaY);

            Explosion explosion = granada.GetComponent<Explosion>();    //aplicar direccion de granada tirada
            explosion.SetDireccion(direccion);
        }
    }
    #endregion   

} // class DisparoJefe 
// namespace
