//---------------------------------------------------------
// Gestor de escena. Podemos crear uno diferente con un
// nombre significativo para cada escena, si es necesario
// Guillermo Jiménez Díaz, Pedro Pablo Gómez Martín
// Template-P1
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Componente que se encarga de la gestión de un nivel concreto.
/// Este componente es un singleton, para que sea accesible para todos
/// los objetos de la escena, pero no tiene el comportamiento de
/// DontDestroyOnLoad, ya que solo vive en una escena.
///
/// Contiene toda la información propia de la escena y puede comunicarse
/// con el GameManager para transferir información importante para
/// la gestión global del juego (información que ha de pasar entre
/// escenas)
/// </summary>
public class LevelManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----

    #region Atributos del Inspector (serialized fields)

    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField]
    private TMPro.TextMeshProUGUI Health; //Texto del canvas
    [SerializeField]
    private TMPro.TextMeshProUGUI Ammo; //Ver cantidad de balas
    [SerializeField]
    private GameObject spriteGranada; //Enseñar la granada
    [SerializeField]
    private TMPro.TextMeshProUGUI TextGranadas; //Enseñar el número de granadas
    [SerializeField]
    private GameObject Botiquin; //Enseñar el botiquín
    [SerializeField]
    private TMPro.TextMeshProUGUI TextBotiquines; //Enseñar el número de botiquines
    [SerializeField]
    private GameObject Menu;
    [SerializeField]
    private GameObject Puerta;
    [SerializeField] private GameObject CorazonDañado;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject PanelVictoria;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    private static LevelManager _instance;
    private int vidaActual;
    private int MaxGranadas;
    private int MaxBotiquin;

    private float Scale;
    private int vidaMax; //Vida máxima del jugador
    private int numGranadas;
    private int Cargador; //Ver la situación del cargador
    private int BalasMax = 0; //Ver las balas maximas de esa arma
    private int granadas = 0;
    private int botiquines = 0;
    public bool juegoTerminado= false;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    protected void Awake()
    {
        if (_instance == null)
        {
            // Somos la primera y única instancia
            _instance = this;
            Init();
            gameOverPanel.SetActive(false);
            PanelVictoria.SetActive(false);
        }
    }

    void Start()
    {
        UpdateGUI();
        Menu.SetActive(false);

        //desactivar los paneles

        vidaMax = GameManager.Instance.GetVidaMaxima();
        GameManager.Instance.TransferManagerSetup();
        
    }
    //private void Update()
    //{
    //    UpdateGUI();
    //}
    #endregion

    // ---- MÉTODOS PÚBLICOS ----

    #region Métodos públicos

    /// <summary>
    /// Propiedad para acceder a la única instancia de la clase.
    /// </summary>
    public static LevelManager Instance
    {
        get
        {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }

    /// <summary>
    /// Devuelve cierto si la instancia del singleton está creada y
    /// falso en otro caso.
    /// Lo normal es que esté creada, pero puede ser útil durante el
    /// cierre para evitar usar el LevelManager que podría haber sido
    /// destruído antes de tiempo.
    /// </summary>
    /// <returns>Cierto si hay instancia creada.</returns>
    public static bool HasInstance()
    {
        return _instance != null;
    }

    /// <summary>
    /// Devuelve cierto si la instancia del singleton está creada y
    /// falso en otro caso.
    /// Lo normal es que esté creada, pero puede ser útil durante el
    /// cierre para evitar usar el LevelManager que podría haber sido
    /// destruído antes de tiempo.
    /// </summary>
    /// <returns>Cierto si hay instancia creada.</returns>
    public void RecogerEstado(int CargadorG, int BalasMaxG)
    {
        Cargador = CargadorG;
        BalasMax = BalasMaxG;
        vidaActual = GameManager.Instance.GetVidaActual();
        granadas = GameManager.Instance.CantidadGranadas();
        botiquines = GameManager.Instance.CantidadBotiquines();
        UpdateGUI();
    }

    public void GameOver()
    {
        juegoTerminado = true; 
        Time.timeScale = 0;     //detener el tiempo
        gameOverPanel.SetActive(true);
    }
    public void Victoria()
    {
        juegoTerminado = true;
        Time.timeScale = 0;     //detener el tiempo
        PanelVictoria.SetActive(true);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MenuInicial()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----

    #region Métodos Privados

    /// <summary>
    /// Dispara la inicialización.
    /// </summary>
    private void Init()
    {
        // De momento no hay nada que inicializar
    }

    private void UpdateGUI()
    {

        if (vidaActual > 0)
        {
            Health.text = "Vida: " + vidaActual;
        }
        else
        {
            Health.text = "Vida: 0";
            //Menu.SetActive(true);
            gameOverPanel.SetActive(true);
        }
        Ammo.text = Cargador + "/" + BalasMax;
        TextBotiquines.text = "x" + botiquines;
        TextGranadas.text = "x" + granadas;
        if(vidaMax <= vidaActual / 4)
        {
            CorazonDañado.SetActive(true);
        }
        else
        {
            CorazonDañado.SetActive(false);
        }
    }
    #endregion
} // class LevelManager 
// namespace