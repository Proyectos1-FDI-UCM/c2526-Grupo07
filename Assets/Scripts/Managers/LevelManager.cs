//---------------------------------------------------------
// Gestor de escena. Podemos crear uno diferente con un
// nombre significativo para cada escena, si es necesario
// Guillermo Jiménez Díaz, Pedro Pablo Gómez Martín
// Template-P1
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] private TextMeshProUGUI TextAmmo; //Cantidad de balas

    [SerializeField] private GameObject SpriteGranada;     //Sprite de la granada
    [SerializeField] private TextMeshProUGUI TextGranadas; //Número de granadas

    [SerializeField] private GameObject SpriteBotiquin;      //Sprite del botiquín
    [SerializeField] private TextMeshProUGUI TextBotiquines; //Número de botiquines

    [SerializeField] private Slider BarraDeVida;             //Sprite de barra de vida
    [SerializeField] private GameObject SpriteCorazonDañado; //Sprite del corazon dañado

    [SerializeField] private GameObject GameOverPanel; //Menú game over
    [SerializeField] private GameObject PanelVictoria; //Menú victoria

    [SerializeField] private int TiempoLimite;         //Tiempo limite para una de las estrellas
    [SerializeField] public TextMeshProUGUI TimerText; //Texto para ver el tiempo

    [SerializeField] private GameObject[] SpriteEstrellas; //Sprite para las estrellas logradas

    [SerializeField] private bool IsFinalLevel; //True si es el nivel final
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    
    private static LevelManager _instance;

    private int _vidaActual; //Vida actual del jugador
    private int _vidaMax;    //Vida máxima del jugador

    private int _cargador;     //Situación del cargador
    private int _maxBalas = 0; //Balas maximas de esa arma

    private int _granadas = 0;   //Cantidad de granadas
    private int _botiquines = 0; //Cantidad de botiquines

    private bool _juegoTerminado= false; //True si el juego termina

    private float timeSec;   //Tiempo en segundos
    private float timeMin;   //Tiempo en minutos
    private float timeTotal; //Tiempo total en segundos

    private int numRehenes; //Número de rehenes en el mapa
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
            //Desactivar paneles
            GameOverPanel.SetActive(false);
            PanelVictoria.SetActive(false);
        }
    }

    void Start()
    {
        _vidaMax = GameManager.Instance.GetVidaMaxima();
        numRehenes = GameObject.FindGameObjectsWithTag("Rehen").Length;
        IniciarBarraVida(_vidaMax, _vidaActual);

        UpdateGUI();
        GameManager.Instance.TransferManagerSetup();
    }
    private void Update()
    {
        timeSec += Time.deltaTime;
        timeTotal += Time.deltaTime;
        UpdateGUI();
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----

    #region Métodos públicos

    /// <summary>
    /// Propiedad para acceder a la única instancia de la clase.
    /// </summary>
    /// 
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
    
    public void RecogerEstado(int Cargador, int BalasMax)
    {
        _cargador = Cargador;
        _maxBalas = BalasMax;
        _vidaActual = GameManager.Instance.GetVidaActual();
        _granadas = GameManager.Instance.CantidadGranadas();
        _botiquines = GameManager.Instance.CantidadBotiquines();
        UpdateGUI();
    }
    public void RehenSalvado()
    {
        numRehenes -= 1;
    }
    public void GameOver()
    {
        _juegoTerminado = true; 
        //Time.timeScale = 0;     //detener el tiempo
        GameOverPanel.SetActive(true);
    }
    public void Victoria()
    {
        _juegoTerminado = true;
        if (IsFinalLevel)
        {
            SpriteEstrellas[2].SetActive(true);
        }
        else if (numRehenes == 0)
        {
            SpriteEstrellas[2].SetActive(true);
        }
        if (numRehenes < 2)
        {
            SpriteEstrellas[1].SetActive(true);
        }
        if (timeTotal < TiempoLimite)
        {
            SpriteEstrellas[0].SetActive(true);
        }
        //Time.timeScale = 0;     //detener el tiempo
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
        ActualizarBarraVida(_vidaActual);
        ActualizarCoraon();
        if (_vidaActual  < 0)
        {
            GameOverPanel.SetActive(true);
        }
        TextAmmo.text = _cargador + "/" + _maxBalas;
        TextBotiquines.text = "x" + _botiquines;
        TextGranadas.text = "x" + _granadas;
        if (timeSec > 60)
        {
            timeSec = 0;
            timeMin++;
        }
        if (timeSec < 10 && timeMin < 10)
        {
            TimerText.text = "0" + timeMin + ":0" + Mathf.Floor(timeSec).ToString();
        }
        else if (timeSec > 10 && timeMin < 10)
        {
            TimerText.text = "0" + timeMin + ":" + Mathf.Floor(timeSec).ToString();
        }
        else
        {
            TimerText.text = +timeMin + ":0" + Mathf.Floor(timeSec).ToString();
        }
    }
    private void IniciarBarraVida(int VidaMax, int VidaActual)
    {
        BarraDeVida.maxValue = VidaMax;
        BarraDeVida.value = VidaActual;
    }
    private void ActualizarBarraVida(int VidaActual)
    {
        BarraDeVida.value = VidaActual;
    }

    private void ActualizarCoraon()
    {
        if (_vidaActual <= _vidaMax / 4)
        {
            SpriteCorazonDañado.SetActive(true);
        }
        else
        {
            SpriteCorazonDañado.SetActive(false);
        }
    }
    #endregion
} // class LevelManager 
// namespace