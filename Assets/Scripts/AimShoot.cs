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
public class AimShoot : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    public GameObject Bala; //Objeto Bala que se crea al Dispara
    [SerializeField]
    public Transform SalidaBala; //Posición donde saldrá la bala
    [SerializeField]
    public float Cadencia = 0f; //Balas por segundo
    [SerializeField]
    public int Cargador = 10; //Número de balas que se pueden disparar
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    Vector3 direction, lastMousePos, mousePosition;
    private float tiempoDisparo = 0f; //Tiempo que falta para poder disparar, controla la cadencia
    private int balasActuales; //Balas disponibles en el cargador
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
        direction = transform.position;
        mousePosition = InputManager.Instance.GetAimMouseValue();

        balasActuales = Cargador; //Iniciamos con el cargador lleno, las balas disponibles son todas las del cargador
        Debug.Log("Balas: " + balasActuales);
        GameManager.Instance.Municion(balasActuales, Cargador);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        mousePosition = InputManager.Instance.GetAimMouseValue();
        if (InputManager.Instance.AimControllerIsPressed())
        {
            Vector3 rStickdir = InputManager.Instance.GetAimControllerValue();
            direction = rStickdir;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (mousePosition != lastMousePos)
        {
            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            direction = cursorWorldPosition - transform.position;
            lastMousePos = mousePosition;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        //1 El tiempo para poder volver a disparar se reduce con el delta time
        if (tiempoDisparo > 0)
        {
            tiempoDisparo -= Time.deltaTime;
        }
        //2 Comprueba si se recarga
        if (InputManager.Instance.ReloadWasPressedThisFrame())
        {
            Recargar();
        }
        //3 Comprueba si se dispara y si se puede disparar por el tiempo y por las balas disponibles
        if (InputManager.Instance.FireIsPressed() && tiempoDisparo <= 0f && balasActuales > 0)
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
    public Vector3 AimDir()
    {
        return direction;
    }
    public Vector3 MousePos()
    {
        return mousePosition;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)
    private void Disparar()
    {
        //Crea la bala en su posición de salida
        //Instantiate(Bala, SalidaBala.position, SalidaBala.rotation);
        GameObject nuevaBala = Instantiate(Bala, SalidaBala.position, SalidaBala.rotation);
        BulletBehaviour balaDir = nuevaBala.GetComponent<BulletBehaviour>();
        balaDir.Dir(direction);

        // Restamos una bala al cargador
        balasActuales--;
        Debug.Log("Balas: " + balasActuales);

        // Reiniciamos el tiempo de disparo según la cadencia
        tiempoDisparo = 1f / Cadencia;
    }
    //Recargar==La munición disponible es la misma que la del cargador
    private void Recargar()
    {
        balasActuales = Cargador;
        Debug.Log("Balas: " + balasActuales);
    }
    #endregion

} // class AimShoot 
// namespace
