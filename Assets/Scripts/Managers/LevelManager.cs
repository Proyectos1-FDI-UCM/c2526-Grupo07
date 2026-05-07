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
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngineInternal;

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
    [SerializeField] private GameObject BotonGameOver; //El botón a seleccionar
    [SerializeField] private GameObject PanelVictoria; //Menú victoria
    [SerializeField] private GameObject BotonVictoria; //El botón a seleccionar
    [SerializeField] private GameObject PanelPausa; //Menú victoria
    [SerializeField] private GameObject BotonPausa; //El botón a seleccionar

    [SerializeField] private int TiempoLimite;         //Tiempo limite para una de las estrellas
    [SerializeField] private TextMeshProUGUI TimerText; //Texto para ver el tiempo

    [SerializeField] private GameObject[] SpriteEstrellas; //Sprite para las estrellas logradas
    [SerializeField] private GameObject[] Rehenes;         //Determinar el número de rehenes

    [SerializeField] private bool IsFinalLevel; //True si es el nivel final
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    
    private static LevelManager _instance;
    
    //Vida
    private int _vidaActual; //Vida actual del jugador
    private int _vidaMax;    //Vida máxima del jugador

    //Balas y munición
    private int _cargador;     //Situación del cargador
    private int _maxBalas = 0; //Balas maximas de esa arma

    //Objetos
    private int _granadas = 0;   //Cantidad de granadas
    private int _botiquines = 0; //Cantidad de botiquines

    //Tiempo
    private float timeSec;   //Tiempo en segundos
    private float timeMin;   //Tiempo en minutos
    private float timeTotal; //Tiempo total en segundos
    private bool _isRunning; // determina si esta parado o no (usado para la pausa)

    //Rehenes
    private int numRehenes; //Número de rehenes en el mapa

    private bool _juegoTerminado = false; //True si el juego termina
    private bool _juegoPausado = false;   //True si está en pausa

    // pause
    private float time; //Tiempo
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
            // Desactivar paneles
            GameOverPanel.SetActive(false);
            PanelVictoria.SetActive(false);
            PanelPausa.SetActive(false);
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before 
    /// any of the Update methods are called the first time.
    /// </summary>
    void Start()
    {
        _isRunning = true;
        _vidaMax = GameManager.Instance.GetVidaMaxima();
        numRehenes = Rehenes.Length;
        IniciarBarraVida(_vidaMax, _vidaActual);

        UpdateGUI();
        GameManager.Instance.TransferManagerSetup();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (InputManager.Instance.PauseWasPressedThisFrame())
        {
            if (!_juegoTerminado && !_juegoPausado)
            {
                Pause();
            }
            else if (!_juegoTerminado && _juegoPausado)
            {
                Continue();
            }
        }
        if (GameManager.Instance.UsandoBotiquines())
        {
            SpriteGranada.SetActive(false);
            SpriteBotiquin.SetActive(true);
        }
        if (GameManager.Instance.UsandoGranadas())
        {
            SpriteBotiquin.SetActive(false);
            SpriteGranada.SetActive(true);
        }
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


    /// <summary>
    /// Recoge el estado actual del juego y lo muestra en pantalla
    /// </summary>
    /// <param name="Cargador"></param>
    /// <param name="BalasMax"></param>
    public void RecogerEstado(int Cargador, int BalasMax)
    {
        _cargador = Cargador;
        _maxBalas = BalasMax;
        _vidaActual = GameManager.Instance.GetVidaActual();
        _granadas = GameManager.Instance.CantidadGranadas();
        _botiquines = GameManager.Instance.CantidadBotiquines();
        UpdateGUI();
    }

    /// <summary>
    /// Resta un rehen a la cantidad total una vez se salva
    /// </summary>
    public void RehenSalvado()
    {
        numRehenes -= 1;
    }

    /// <summary>
    /// Activa el panel de GameOver y lo relacionado con el fin de partida
    /// </summary>
    public void GameOver()
    {
        Time.timeScale = 0f;
        _juegoTerminado = true; 
        //Time.timeScale = 0;     //detener el tiempo
        GameOverPanel.SetActive(true);
        if (BotonGameOver != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(BotonGameOver);
        }
    }

    /// <summary>
    /// Activa el panel de Victoria y muestra las estrellas conseguidas
    /// </summary>
    public void Victoria()
    {
        Time.timeScale = 0;     //detener el tiempo
        PanelVictoria.SetActive(true);
        _juegoTerminado = true;
        if (IsFinalLevel)
        {
            SpriteEstrellas[2].SetActive(true);
            SpriteEstrellas[1].SetActive(true);

            if (timeTotal < TiempoLimite)
            {
                SpriteEstrellas[0].SetActive(true);
            }
        }
        else
        {
            if (numRehenes == 0)
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
        }

        //Time.timeScale = 0;     //detener el tiempo
        if(BotonVictoria != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(BotonVictoria);
        }
    }

    /// <summary>
    /// Reinicia la escena actual
    /// </summary>
    public void Reiniciar()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestableceDatos();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Cambia la escena a la del menú principal
    /// </summary>
    public void MenuInicial()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestableceDatos();
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// metodo que hace pausar la escena
    /// usarlo cuando demos al esc, cuando aparece el panel de victoria o de derrota
    /// </summary>
    public void Pause() 
    {
        _juegoPausado = true;
        PanelPausa.SetActive(true);
        if (BotonPausa != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(BotonPausa);
        }
        time = Time.timeScale;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// metodo que hace continuar el juego despues de la pausa
    /// usarlo para el boton de continue
    /// </summary>
    public void Continue()
    {
        _juegoPausado = false;
        PanelPausa.SetActive(false);
        Time.timeScale = time;
    }

    /// <summary>
    /// Método que pausa el tiempo
    /// sin activar el canvas
    /// </summary>
    public void PauseTimeOnly()
    {
        _juegoPausado = true;
        time = Time.timeScale;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Devuelve true si el juego está pausado o si se ha terminado
    /// </summary>
    /// <returns></returns>
    public bool IsPaused()
    {
        return _juegoPausado || _juegoTerminado;
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
        if (_isRunning)
        {
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