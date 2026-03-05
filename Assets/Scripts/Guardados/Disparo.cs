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
    private float Cadencia = 0f; //Balas por segundo
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

    private float _tiempoDisparo = 0f; //Tiempo que falta para poder disparar, controla la cadencia

    private int _balasActuales; //Balas disponibles en el cargador

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
        GameManager.Instance.Municion(balasActuales, Cargador);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        bool CargadorLleno=false;
        if (_balasActuales==Cargador)
        {
            CargadorLleno = true;
        }
        bool recargando = false;
        //1 El tiempo para poder volver a disparar se reduce con el delta time
        if (_tiempoDisparo > 0)
        {
            _tiempoDisparo -= Time.deltaTime;
        }
        //2 Comprueba si se recarga
        if (InputManager.Instance.ReloadWasPressedThisFrame() && !CargadorLleno)
        {
            recargando = true;
            Recargar();
            recargando = false;
        }
        //3 Comprueba si se dispara y si se puede disparar por el tiempo y por las balas disponibles
        if (InputManager.Instance.FireIsPressed() && _tiempoDisparo <= 0f && _balasActuales > 0 && !recargando)
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
        _balasActuales--;
        Debug.Log("Balas: " + _balasActuales);

        // Reiniciamos el tiempo de disparo según la cadencia
        _tiempoDisparo = 1f / Cadencia;
    }
    //Recargar==La munición disponible se vuelve la misma que la del cargador
    private void Recargar()
    {
        Debug.Log("Recargando");
        //Espera a que el tiempo de recarga termine para que se llenen las balas(Aún no terminado)

        _balasActuales = Cargador;
        Debug.Log("Balas: " + _balasActuales);
    }

    #endregion   

} // class Disparo 
// namespace
