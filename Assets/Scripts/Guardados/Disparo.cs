//---------------------------------------------------------
// Permite disparar balas, según el tamaño del cargador(asignable) y también la recarga de esas balas
// Carlos Alberto Ovando Barrios
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Permite que el jugador cree las balas, siguiendo los intervalos de la cadencia.
/// Tiene una posición asignable de donde saldrán las balas.
/// La cantidad de balas es asignable desde el editor y esa cantidad se podrá reiniciar cuando se recargue.
/// El tiempo que tarde en recargar, también será ajustable.
/// </summary>
public class Disparo : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    private GameObject Bala; //Objeto Bala que se crea al Dispara
    [SerializeField]
    private Transform SalidaBala; //Posición donde saldrá la bala
    [SerializeField]
    private float Cadencia = 1f; //Balas por segundo
    [SerializeField]
    private int Cargador = 10; //Número de balas que se pueden disparar
    [SerializeField]
    private float TiempoRecarga = 0f; //Tiempo que el jugador tarda en recargar

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float tiempoDisparo = 0f; //Tiempo que falta para poder disparar, controla la cadencia

    private int balasActuales; //Balas disponibles en el cargador

    private bool recargando = false; //No está recargando, por ahora

    private float tiempoRecarga = 0f; //Tiempo que el jugador tarda en recargar, recibe el valor del TiempoRecarga, pero este se modifica

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
        balasActuales = Cargador; //Iniciamos con el cargador lleno, las balas disponibles son todas las del cargador
        Debug.Log("Balas: " + balasActuales);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Comprueba si está recargando, si es así, reduce el tiempo de recarga
        if (recargando)
        {
            tiempoRecarga -= Time.deltaTime;

            // Cuando el tiempo de recarga termina, se llena el cargador 
            if (tiempoRecarga <= 0f)
            {
                TerminarRecarga(); //Llena el cargador y "recargando" se vuelve falso
            }
            //Salimos del Update mientras se recarga, para que el jugador no dispare, y como "recargando" se vuelve falso al terminar, la proxima vez podrá disparar
            return;
        }
        //Si no se recarga:

        //1 El tiempo para poder volver a disparar se reduce con el delta time
        if (tiempoDisparo > 0)
        {
            tiempoDisparo -= Time.deltaTime;
        }
        
        bool cargadorLleno = (balasActuales == Cargador); //El cargador está lleno(true) si las balas actuales son las mismas que las del cargador

        //2 Si el cargador no está lleno e intentamos recargar, empieza la recarga
        if (InputManager.Instance.ReloadWasPressedThisFrame() && !cargadorLleno)
        {
            EmpezarRecarga(); //Vuelve true a recargando y asigna el tiempo de recarga a "tiempoRecarga"

            return; //Sale del Update para que no dispare
        }
        //3 Comprueba si se dispara y si se puede disparar por el tiempo y por las balas disponibles
        if (InputManager.Instance.FireIsPressed() && tiempoDisparo <= 0f && balasActuales > 0 )
        {
            Disparar();
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

    //Disparar==Crea una bala, resta la bala del cargador y reinicia el tiempo de disparo
    private void Disparar()
    {
        //Crea la bala en su posición de salida
        Instantiate(Bala, SalidaBala.position, SalidaBala.rotation);

        // Restamos una bala al cargador
        balasActuales--;
        Debug.Log("Balas: " + balasActuales);

        // Reiniciamos el tiempo de disparo según la cadencia
        tiempoDisparo = 1f / Cadencia;
    }
    //EmpezarRecarga==Vuelve true a recargando y asigna el tiempo de recarga a "tiempoRecarga"
    private void EmpezarRecarga()
    {
        recargando = true;
        tiempoRecarga = TiempoRecarga;
        Debug.Log("Recargando");
    }
    //TerminarRecarga==Vuelve false a recargando y las balas actuales se llenan
    private void TerminarRecarga()
    {
        recargando = false;
        balasActuales = Cargador;
        Debug.Log("Balas: " + balasActuales);
    }
    #endregion   

} // class Disparo 
// namespace
