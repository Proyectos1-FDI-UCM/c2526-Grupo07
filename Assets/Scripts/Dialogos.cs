//---------------------------------------------------------
// Gestor de Diálogos. Procesa archivos y muestra diálogos en pantalla tras la activación de un trigger.
// Cristopher Jeremy Villacís Galindo
// Clear The building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Al iniciar la escena se procesa el archivo que se va a representar.
/// Se guardan las lineas con su respectiva, emoción, emisor y frase.
/// Una vez activado el trigger se activará el canvas asociado y se
/// asignarán nombres y frases según el archivo anteriormente procesado.
/// Al iniciar, si viene de una cinemática, la pausará, si no es el caso,
/// entonces pausará el juego, y al terminar reanudará lo que corresponda.
/// </summary>
public class Dialogos : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private Cinematic Cinema;                  //Componente Cinematic de un GameObject
    [SerializeField] private GameObject DialogoCanvas;          //Canvas que contiene toda la estructura de los diálogos
    [SerializeField] private GameObject PanelNames;             //Panel sobre el que estará el texto del emisor correspondiente
    [SerializeField] private GameObject NamesEndPos;            //GameObject cuyo transform se usará como posición objetivo
    [SerializeField] private SpriteRenderer SpriteLeft;         //Componente SpriteRenderer del Sprite que se situa a la izquierda
    [SerializeField] private SpriteRenderer SpriteRight;        //Componente SpriteRenderer del Sprite que se situa a la derecha
    [SerializeField] private TMPro.TextMeshProUGUI TextoNames;  //Texto que mostrará al emisor correspondiente
    [SerializeField] private TMPro.TextMeshProUGUI TextoFrase;  //Texto que mostrará la frase correspondiente
    [SerializeField] private float Smothness = 0.5f;            //Valor que controla la suavidad de cambio de estado de los GameObject
    [SerializeField] private float TimeToDialogueStart = 1f;    //Tiempo que tarda en iniciar el diálogo trás activar el trigger
    [SerializeField] private AudioSource ImpactoSound;          //Audio que sonará al requerír la emoción Impacto
    [SerializeField] private AudioSource GraciosoSound;         //Audio que sonará al requerír la emoción Gracioso
    [SerializeField] private AudioSource Beep;                  //Audio que sonará al avanzar en cada frase
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private string[] _emociones;                //Array que guarda las emociones posibles
    private string[] _emisor;                   //Array que guarda el orden de emisores
    private string[] _frases;                   //Array que guarda el orden de frases
    private string[] _sonido;                   //Array que guarda el orden de emociones
    private string _primerEmisor;               //String que guarda el nombre del primer emisor
    private bool _playerTouched = false;        //Booleano que marca si el jugador ha tocado el trigger
    private bool _dialogosActivos = false;      //Booleano que marca si se está mostrando los diálogos
    private bool _isLeft = true;                //Booleano que determina si el emisor activo está a la izquierda o no
    private Vector3 _nameInitialPos;            //Guarda la posición inicial de PanelNames
    private Vector3 _leftInitialScale;          //Guarda la escala inicial de SpriteLeft
    private Vector3 _rightInitialScale;         //Guarda la escala inicial de SpriteRight
    private Animator _player;                   //Componente animator del Player
    private float _timePlayerWasTouched = 0f;   //Tiempo que ha pasado tras haberse activado el trigger
    private int _dialogosVistos = 0;            //Número de diálogos mostrados del archivo procesado
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
    private void Start()
    {
        ProcesaArchivo();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Una vez activado el trigger se espera el tiempo establecido antes de inicializar el canvas
        if (_playerTouched)
        {
            _timePlayerWasTouched += Time.deltaTime;
            if(_timePlayerWasTouched >= TimeToDialogueStart)
            {
                Destroy(GetComponent<Collider2D>());                    //Destruye el collider para que no se pueda volver a activar por accidente
                _player.enabled = false;                                //Desactiva el componente Animator del player para evitar cierto bug
                //Pausa la cinemática si existe, sino pausa el juego
                if (Cinema != null) Cinema.CinematicPause(); 
                else LevelManager.Instance.PauseTimeOnly();
                //Inicialización de los diálogos
                DialogoCanvas.SetActive(true);
                _dialogosActivos = true;
                _nameInitialPos = PanelNames.transform.position;
                _leftInitialScale = SpriteLeft.transform.localScale;
                _rightInitialScale = SpriteRight.transform.localScale;
                _playerTouched = false;
                CalculaFrase();
            }
        }
        //Una vez ya inicializado los diálogos se controla el input y la representación gráfica correspondiente
        if (_dialogosActivos)
        {
            if (_isLeft)
            {
                PanelNames.transform.position = Vector3.Lerp(PanelNames.transform.position, _nameInitialPos, Smothness);
                SpriteLeft.transform.localScale = Vector3.Lerp(SpriteLeft.transform.localScale, _leftInitialScale, Smothness);
                SpriteRight.transform.localScale = Vector3.Lerp(SpriteRight.transform.localScale, _rightInitialScale - new Vector3(30f, 30f, 30f), Smothness);
                SpriteLeft.color = Color.Lerp(SpriteLeft.color, new Color32(255, 255, 255, 255), Smothness);
                SpriteRight.color = Color.Lerp(SpriteRight.color, new Color32(255, 255, 255, 70), Smothness);
            }
            else
            {
                PanelNames.transform.position = Vector3.Lerp(PanelNames.transform.position, NamesEndPos.transform.position, Smothness);
                SpriteLeft.transform.localScale = Vector3.Lerp(SpriteLeft.transform.localScale, _leftInitialScale - new Vector3(30f, 30f, 30f), Smothness);
                SpriteRight.transform.localScale = Vector3.Lerp(SpriteRight.transform.localScale, _rightInitialScale, Smothness);
                SpriteLeft.color = Color.Lerp(SpriteLeft.color, new Color32(255, 255, 255, 70), Smothness);
                SpriteRight.color = Color.Lerp(SpriteRight.color, new Color32(255, 255, 255, 255), Smothness);
            }
            if (InputManager.Instance.JumpWasPressedThisFrame()) CalculaFrase();
        }
    }

    /// <summary>
    /// Asigna el componente Animator a _player 
    /// y pone _playerTouched a true iniciando así el contador
    /// para la inicialización de los diálogos
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player = collision.gameObject.GetComponent<Animator>();
        _playerTouched = true;
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
    /// Llama a Cinema para reanudar la cinemática
    /// Si no hay Cinema reanuda el juego
    /// Ademas reactiva el animator y desactiva el canvas de diálogos
    /// </summary>
    private void ResumeCinema()
    {
        _player.enabled = true;
        if (Cinema != null) Cinema.CinematicResume();
        else LevelManager.Instance.Continue();
        DialogoCanvas.SetActive(false);
        _dialogosActivos = false;
    }

    /// <summary>
    /// Procesa el archivo de Diálogos correspondiente
    /// y guarda su contenido de manera organizada
    /// para su posterior uso
    /// </summary>
    private void ProcesaArchivo()
    {
        //Guarda las posibles emociones
        TextAsset archivoEmociones = Resources.Load<TextAsset>($"Dialogos/Emociones");
        string textoEmociones = archivoEmociones.text;
        _emociones = textoEmociones.Split('\n');

        //Determina el archivo a procesar y destrulle el GameObject antes de procesar si ya no hay más
        int _indiceDialogo = GameManager.Instance.CinematicaActual();
        if (_indiceDialogo > 3) Destroy(this.gameObject);
        TextAsset archivo = Resources.Load<TextAsset>($"Dialogos/Dialogo{_indiceDialogo}");

        //Se guardan las lineas del documento de texto en un Array
        string texto = archivo.text;
        string[] lineas = texto.Split("\n");

        //Se organizan en sus Arrays correspondientes
        _emisor = new string[lineas.Length / 3];
        _frases = new string[lineas.Length / 3];
        _sonido = new string[lineas.Length / 3];
        for (int i = 0; i < lineas.Length; i += 3)
        {
            _sonido[i / 3] = lineas[i];
            _emisor[i / 3] = lineas[i + 1];
            _frases[i / 3] = lineas[i + 2];
        }
        _primerEmisor = _emisor[0];             //Se determina el primer emisor
        GameManager.Instance.CinematicaVista(); //Se aumenta la cantidad de diálogos vistos en el GameManager
    }

    /// <summary>
    /// Respresenta el emisor, la frase y el sonido correspondiente
    /// y marca si el emisor está a la izquierda o no
    /// Si se llega al final de los diálogos, estos se terminan
    /// y se reanuda la cinemática o el juego
    /// </summary>
    private void CalculaFrase()
    {
        if (_dialogosVistos < _emisor.Length)
        {
            if (Beep != null) Beep.Play();
            if (_sonido[_dialogosVistos] == _emociones[1]) ImpactoSound.Play();
            else if (_sonido[_dialogosVistos] == _emociones[2]) GraciosoSound.Play();
            TextoNames.text = _emisor[_dialogosVistos];
            TextoFrase.text = _frases[_dialogosVistos];
            if (_emisor[_dialogosVistos] != _primerEmisor) _isLeft = true;
            else _isLeft = false;
            _dialogosVistos++;
        }
        else
        {
            _dialogosActivos = false;
            ResumeCinema();
        }
    }
    #endregion

} // class Dialogos 
// namespace
