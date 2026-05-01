//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
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
    [SerializeField] private Cinematic Cinema;
    [SerializeField] private GameObject DialogoCanvas;
    [SerializeField] private GameObject PanelNames;
    [SerializeField] private GameObject NamesEndPos;
    [SerializeField] private SpriteRenderer SpriteLeft;
    [SerializeField] private SpriteRenderer SpriteRight;
    [SerializeField] private TMPro.TextMeshProUGUI TextoNames;
    [SerializeField] private TMPro.TextMeshProUGUI TextoFrase;
    [SerializeField] private float Smothness = 0.5f;
    [SerializeField] private float TimeToDialogueStart = 1f;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private string[] _emisor;
    private string[] _frases;
    private string _primerEmisor;
    private bool _playerTouched = false;
    private bool _dialogosActivos = false;
    private bool _isLeft = true;
    private Vector3 _nameInitialPos;
    private Vector3 _leftInitialScale;
    private Vector3 _rightInitialScale;
    private Animator _player;
    private float _timePlayerWasTouched = 0f;
    private int _dialogosVistos = 0;
    #endregion

    // ---- MÉTODOS DE MONOBEHAVIOUR ----
    #region Métodos de MonoBehaviour

    // Por defecto están los típicos (Update y Start) pero:
    // - Hay que añadir todos los que sean necesarios
    // - Hay que borrar los que no se usen 

    private void Start()
    {
        ProcesaArchivo();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_playerTouched)
        {
            _timePlayerWasTouched += Time.deltaTime;

            if(_timePlayerWasTouched >= TimeToDialogueStart)
            {
                Destroy(GetComponent<Collider2D>());
                _player.enabled = false;
                Cinema.CinematicPause();
                DialogoCanvas.SetActive(true);
                _dialogosActivos = true;
                _nameInitialPos = PanelNames.transform.position;
                _leftInitialScale = SpriteLeft.transform.localScale;
                _rightInitialScale = SpriteRight.transform.localScale;
                _playerTouched = false;
                CalculaFrase();
            }
        }
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
    private void ResumeCinema()
    {
        _player.enabled = true;
        Cinema.CinematicResume();
        DialogoCanvas.SetActive(false);
        _dialogosActivos = false;
    }

    private void ProcesaArchivo()
    {
        TextAsset archivo = Resources.Load<TextAsset>("Dialogos/Dialogo" + GameManager.Instance.CinematicaActual());

        string texto = archivo.text;
        string[] lineas = texto.Split("\n");
        _emisor = new string[lineas.Length / 2];
        _frases = new string[lineas.Length / 2];
        for (int i = 0; i < lineas.Length; i += 2)
        {
            _emisor[i / 2] = lineas[i];
            _frases[i / 2] = lineas[i + 1];
        }
        _primerEmisor = _emisor[0];
        GameManager.Instance.CinematicaVista();
    }

    private void CalculaFrase()
    {
        if (_dialogosVistos < _emisor.Length)
        {
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
