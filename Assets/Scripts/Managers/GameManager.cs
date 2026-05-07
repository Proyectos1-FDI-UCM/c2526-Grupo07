//---------------------------------------------------------
// Contiene el componente GameManager
// Guillermo Jiménez Díaz, Pedro P. Gómez Martín
// Marco A. Gómez Martín
// Template-P1
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;


/// <summary>
/// Componente responsable de la gestión global del juego. Es un singleton
/// que orquesta el funcionamiento general de la aplicación,
/// sirviendo de comunicación entre las escenas.
///
/// El GameManager ha de sobrevivir entre escenas por lo que hace uso del
/// DontDestroyOnLoad. En caso de usarlo, cada escena debería tener su propio
/// GameManager para evitar problemas al usarlo. Además, se debería producir
/// un intercambio de información entre los GameManager de distintas escenas.
/// Generalmente, esta información debería estar en un LevelManager o similar.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----

    #region Atributos del Inspector (serialized fields)

    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    //Cantidad máxima de atributos
    [SerializeField] private int MaxGranadas;    //Cantidad máx de granadas
    [SerializeField] private int MaxBotiquin;    //Cantidad máx de botiquines
    [SerializeField] private int MaxVidaInicial; //Vida máxima del jugador

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----

    #region Atributos Privados (private fields)

    /// <summary>
    /// Instancia única de la clase (singleton).
    /// </summary>
    private static GameManager _instance;
    private float Scale;

    //Cantidad de los distintos objetos y la vida
    private int _vidaActual;     //Puntos de vida ACTUALES del personaje
    private int _cargador;       //Ver la situación del cargador
    private int _balasMax = 0;   //Ver las balas maximas de esa arma
    private int _granadas = 0;   //Cantidad de granadas actuales
    private int _botiquines = 0; //Cantidad de botiquines actuales
    private bool _usandoGranadas = false;   //Indica si usando las granadas
    private bool _usandoBotiquines = false; //Indica si usando los botiquines

    //Inventario armas
    private bool _AK47 = false; //Inventario interno para ver si tiene rifle o no

    //Invulnerabilidad
    private bool _invulnerable = false;         //True si el jugador es invulnerable a daños
    private float _invulnerableDuracion = 1.5f; //Tiempo que es invulnerable
    private float _invulnerableTiempoInicial;   //Tiempo inicial al ser invulnerable

    //Atributos auxiliares para controlar los objetos durante el cambio de escena
    private int _vidaActualAux;
    private int _cargadorAux;
    private int _balasMaxAux = 0;
    private int _granadasAux = 0;
    private int _botiquinesAux = 0;

    //Contador de diálogos vistos durante la partida
    private int _cinematicaSiguiente = 1;

    //Cheat
    private bool cheatMode;

    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----

    #region Métodos de MonoBehaviour

    /// <summary>
    /// Método llamado en un momento temprano de la inicialización.
    /// En el momento de la carga, si ya hay otra instancia creada,
    /// nos destruimos (al GameObject completo)
    /// </summary>

    protected void Awake()
    {
        if (_instance != null)
        {
            // No somos la primera instancia. Se supone que somos un
            // GameManager de una escena que acaba de cargarse, pero
            // ya había otro en DontDestroyOnLoad que se ha registrado
            // como la única instancia.
            // Si es necesario, transferimos la configuración que es
            // dependiente de este manager al que ya existe.
            // Esto permitirá al GameManager real mantener su estado interno
            // pero acceder a los elementos de la nueva escena
            // o bien olvidar los de la escena previa de la que venimos
            TransferManagerSetup();

            // Y ahora nos destruímos del todo. DestroyImmediate y no Destroy para evitar
            // que se inicialicen el resto de componentes del GameObject para luego ser
            // destruídos. Esto es importante dependiendo de si hay o no más managers
            // en el GameObject.
            DestroyImmediate(this.gameObject);
        }
        else
        {
            // Somos el primer GameManager.
            // Queremos sobrevivir a cambios de escena.
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        } // if-else somos instancia nueva o no. 
        _vidaActual = MaxVidaInicial;
        _usandoGranadas = true;
        cheatMode = false;
    }

    /// <summary>
    /// Método llamado cuando se destruye el componente.
    /// </summary>

    private void Start()
    {
        TransferManagerSetup();
    }

    /// <summary>
    /// Método llamado cuando se destruye el componente.
    /// </summary>
    protected void OnDestroy()
    {
        if (this == _instance)
        {
            // Éramos la instancia de verdad, no un clon.
            _instance = null;
        } // if somos la instancia principal
    }
    public void Update()
    {
        //Si es invulnerable, pasa un tiempo y deja de serlo
        if (_invulnerable)
        {
            _invulnerableTiempoInicial += Time.deltaTime; 

            if (_invulnerableTiempoInicial > _invulnerableDuracion)
            {
                _invulnerableTiempoInicial = 0;
                _invulnerable = false;
            }
        }
        //Cuando se cambia de objeto se intercambian entre las grandas y botiquines
        if (InputManager.Instance)
        {
            if (InputManager.Instance.ChangeObjectWasPressedThisFrame())
            {
                if (_usandoBotiquines)
                {
                    _usandoGranadas = true;
                    _usandoBotiquines = false;
                }
                else if (_usandoGranadas)
                {
                    _usandoBotiquines = true;
                    _usandoGranadas = false;
                }
            }
        }
        
    }
    #endregion

    // ---- MÉTODOS PÚBLICOS ----

    #region Métodos públicos

    /// <summary>
    /// Propiedad para acceder a la única instancia de la clase.
    /// </summary>
    public static GameManager Instance
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
    /// cierre para evitar usar el GameManager que podría haber sido
    /// destruído antes de tiempo.
    /// </summary>
    /// <returns>Cierto si hay instancia creada.</returns>



    /// <summary>
    /// Activa el cheat
    /// </summary>
    public bool GetCheatMode()
    {
        return cheatMode; 
    }
    /// <summary>
    /// 
    /// </summary>
    public void SetCheatFromMM(bool cheat)
    {
        cheatMode = cheat;
    }
    public static bool HasInstance()
    {
        return _instance != null;
    }

    /// <summary>
    /// Método que cambia la escena actual por la indicada en el parámetro.
    /// </summary>
    /// <param name="index">Índice de la escena (en el build settings)
    /// que se cargará.</param>
    public void ChangeScene(int index)
    {
        // Antes y después de la carga fuerza la recolección de basura, por eficiencia,
        // dado que se espera que la carga tarde un tiempo, y dado que tenemos al
        // usuario esperando podemos aprovechar para hacer limpieza y ahorrarnos algún
        // tirón en otro momento.
        // De Unity Configuration Tips: Memory, Audio, and Textures
        // https://software.intel.com/en-us/blogs/2015/02/05/fix-memory-audio-texture-issues-in-unity
        //
        // "Since Unity's Auto Garbage Collection is usually only called when the heap is full
        // or there is not a large enough freeblock, consider calling (System.GC..Collect) before
        // and after loading a level (or put it on a timer) or otherwise cleanup at transition times."
        //
        // En realidad... todo esto es algo antiguo por lo que lo mismo ya está resuelto)
        System.GC.Collect();
        GuardarDatos(index);
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        System.GC.Collect();
        Time.timeScale = 1;
    } // ChangeScene

    /// <summary>
    /// Método para restar la vida del personaje
    /// </summary>
    public void RestarVida(int Damage)
    {
        if (!cheatMode)
        {
            if (!_invulnerable)    //Solo recibe daño si no es invulnerable
            {
                _vidaActual -= Damage; // Restar la vida del jugador
                _invulnerable = true;
            }
            if (_vidaActual < 1)    // Si vida actual llega a 0, se llama a GameOver
            {
                _vidaActual = 0;   //Para que la vida no salga en negativo
                LevelManager.Instance.GameOver();
            }
        }
        TransferManagerSetup();
    }
    /// <summary>
    /// Método para curar la vida del personaje
    /// </summary>
    public void CurarVida(int vida)
    {
        _vidaActual += vida;
        if (_vidaActual > MaxVidaInicial) _vidaActual = MaxVidaInicial;
        TransferManagerSetup();
    }
    /// <summary>
    /// Método llamado por script "AimShoot" para las balas
    /// </summary>
    public void SetMunicion(int balasMax, int balasAct)
    {
        _cargador = balasAct;
        _balasMax = balasMax;
        TransferManagerSetup();
    }
    /// <summary>
    /// Método llamado cuando se usan granadas
    /// </summary>
    public void UsarGranadas()
    {
        if (_usandoGranadas && !cheatMode)
        {
             _granadas--;
            TransferManagerSetup();
        }
    }
    /// <summary>
    /// Método llamado cuando se guardan granadas
    /// </summary>
    public void GuardarGranadas()
    {
        _granadas++;
        TransferManagerSetup();
    }
    /// <summary>
    /// Método llamado cuando se usan botiquines
    /// </summary>
    public void UsarBotiquin()
    {
        if (_usandoBotiquines)
        {
            _botiquines--;
            TransferManagerSetup();
        }
    }
    /// <summary>
    /// Método llamado cuando se guardan botiquines
    /// </summary>
    public void GuardarBotiquines()
    {
        _botiquines++;
        TransferManagerSetup();
    }
    /// <summary>
    /// Método que devuelve true si el inventario de granadas esta lleno
    /// </summary>
    public bool GranadasFull()
    {
        if (_granadas == MaxGranadas)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Método que devuelve true si el inventario de botiquines esta lleno
    /// </summary>
    public bool BotiquinesFull()
    {
        if (_botiquines == MaxBotiquin)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Método que devuelve la cantidad de granadas actuales
    /// </summary>
    public int CantidadGranadas()
    {
        if (cheatMode) { _granadas = MaxGranadas;}
        return _granadas;
    }
    /// <summary>
    /// Método que devuelve la cantidad de botiquines actuales
    /// </summary>
    public int CantidadBotiquines()
    {
        return _botiquines;
    }
    /// <summary>
    /// Método que indica si se estan usando granadas
    /// </summary>
    public bool UsandoGranadas()
    {
        return _usandoGranadas;
    }
    /// <summary>
    /// Método que indica si se estan usando botiquines
    /// </summary>
    public bool UsandoBotiquines()
    {
        return _usandoBotiquines;
    }
    /// <summary>
    /// Método que guarda el ak47 al inventario
    /// </summary>
    public void RecogerAK47()
    {
        _AK47 = true;
    }
    /// <summary>
    /// Método que confirma que hay ak47
    /// </summary>
    public bool TieneAK47()
    {
        return _AK47;
    }
    /// <summary>
    /// Indica si el jugador esta invulnerable o no
    /// </summary>
    public bool Invulnerabilidad()
    {
        return _invulnerable;
    }
    /// <summary>
    /// Método para reiniciar la escena
    /// </summary>
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Metodo para poder acceder a la vida maxima desde otro script
    /// </summary>
    public int GetVidaMaxima()
    {
        return MaxVidaInicial;
    }
    /// <summary>
    /// Metodo para poder acceder a la vida actual desde otro script
    /// </summary>
    public int GetVidaActual()
    {
        return _vidaActual;
    }
    /// <summary>
    /// Metodo para mandar el estado del nivel al LevelManager
    /// </summary>
    public void TransferManagerSetup()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RecogerEstado(_cargador, _balasMax);
        }
    }
    /// <summary>
    /// Metodo para retomar los datos que habian al inicio del nivel
    /// </summary>
    public void RestableceDatos()
    {
        _cargador = _cargadorAux;
        _balasMax = _balasMaxAux;
        _granadas = _granadasAux;
        _botiquines = _botiquinesAux;
        _vidaActual = _vidaActualAux;
    }
    /// <summary>
    /// Metodo para guardar los datos del inicio del nivel
    /// </summary>
    public void GuardarDatos(int escena)
    {
        if (escena == 1)
        {
            _cinematicaSiguiente = 1;
            _vidaActual = MaxVidaInicial;
            _cargador = _balasMax;
            _granadas = 0;
            _botiquines = 0;
            _AK47 = false;
        }
        _cargadorAux = _cargador;
        _balasMaxAux = _balasMax;
        _granadasAux = _granadas;
        _botiquinesAux = _botiquines;
        _vidaActualAux = _vidaActual;
    }
    /// <summary>
    /// Cuando termina la cinemática lleva a la siguiente
    /// </summary>
    public int CinematicaActual()
    {
        return _cinematicaSiguiente;
    }
    /// <summary>
    /// Cuando se termina de ver una cinemática
    /// </summary>
    public void CinematicaVista()
    {
        _cinematicaSiguiente++;
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
    #endregion
} // class GameManager 
// namespace