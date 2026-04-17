//---------------------------------------------------------
// El fuego causará daño al jugador cuando entre y si lo sigue tocando (Parte del ataque del MiniBoss)
// Carlos Alberto Ovando Barrios
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class MiniBossMarcasFuego : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    private int Damage; //Daño que causa
    [SerializeField]
    private float DamageTime; //Tiempo en que te vuelve a hacer daño el fuego
    [SerializeField]
    private float TiempoDelFuego; //Tiempo que dura el fuego

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float tiempoFuego; //Tiempo que dura el fuego

    private float tiempoDamage = 0f; //Tiempo en que te vuelve a hacer daño el fuego
    
    private bool apagado = false; //Si no se apagó el fuego

    private PlayerController player = null; //El jugador
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
        tiempoFuego = TiempoDelFuego; //Se le asigna a tiempoFuego el tiempo que dura el fuego
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Si no se apagó el fuego, se irá reduciendo el tiempo que dura al fuego hasta que se apague
        if(!apagado)
        {
            //Reduce el tiempo del fuego
            tiempoFuego -= Time.deltaTime;
            //Cuando llega a cero, desaparece
            if (tiempoFuego <= 0)
            {
                apagado = true;
                Destroy(gameObject);
            }
        }
        //Si existe el jugador dentro del fuego, recibirá daño cada cierto tiempo
        if (player != null)
        {
            tiempoDamage += Time.deltaTime;//El tiempo en el que el fuego hace daño incrementa
            //Si este tiempo es mayor o igual al establecido, causa daño al jugador y se reinicia el tiempo
            if(tiempoDamage >= DamageTime)
            {
                GameManager.Instance.RestarVida(Damage); //Daña al jugador

                player.RedFlash();//Efecto de daño

                tiempoDamage = 0f;//Reinicia el tiempo
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

    //El jugador entra en el fuego, lo que le causa daño e inicia un contador que cada cierto tiempo le causará daño
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        PlayerController Player = collision.gameObject.GetComponent<PlayerController>(); //Busca al jugador cuando entra en el fuego
        //Si existe el jugador, le causa daño
        if(Player != null)
        {
            GameManager.Instance.RestarVida(Damage);//Daña al jugador

            Player.RedFlash();//Efecto de daño

            player = Player; //Existe el jugador en el fuego

            tiempoDamage = 0f; //Inicia el tiempo en el que el fuego hace daño en cero 0
            
        }
    }
    //Cuando sale del fuego, el jugador ya no recibirá daño
    private void OnTriggerExit2D(Collider2D collision)
    {

        PlayerController Player = collision.gameObject.GetComponent<PlayerController>(); //Busca si hay un jugador
        //Si había un jugador, ya no lo estará y dejará de estar en el fuego
        if (Player != null)
        {
            player = null; //Ya no está el jugador

        }
    }
    #endregion   

} // class MiniBossMarcasFuego 
// namespace
