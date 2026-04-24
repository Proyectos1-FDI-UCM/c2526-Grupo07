//---------------------------------------------------------
// Gestor independiente de consumibles (Granadas y Botiquines)
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Enumeración para identificar el tipo de consumible equipado.
/// </summary>
public enum TipoConsumible
{
    Ninguno,
    Granada,
    Botiquin
}

/// <summary>
/// Componente encargado de gestionar el cambio, uso y lanzamiento de consumibles.
/// Se comunica exclusivamente con GameManager para modificar cantidades y vida,
/// y con InputManager para leer las entradas del jugador.
/// Cada consumible tiene su propia lógica de uso independiente.
/// </summary>
public class ConsumibleHandler : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [Header("Referencias")]
    [SerializeField] private Transform PuntoLanzamiento;
    [SerializeField] private GameObject PrefabGranada;

    [Header("Configuración Granada")]
    [SerializeField] private float FuerzaLanzamiento = 12f;
    [SerializeField] private float AnguloLanzamiento = 40f;

    [Header("HUD Iconos")]
    [SerializeField] private GameObject IconoGranada;
    [SerializeField] private GameObject IconoBotiquin;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private TipoConsumible _consumibleEquipado = TipoConsumible.Ninguno;
    private GameManager _gameManager;

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
        _gameManager = GameManager.Instance;
        ActualizarHUD();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (InputManager.Instance.ChangeObjectWasPressedThisFrame())
            CambiarConsumible();

        if (InputManager.Instance.UseObjectWasPressedThisFrame())
            UsarConsumibleEquipado();
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
    /// Alterna entre granada y botiquín, saltando automáticamente aquellos que estén en cantidad 0.
    /// </summary>
    private void CambiarConsumible()
    {
        int granadas = _gameManager.CantidadGranadas();
        int botiquines = _gameManager.CantidadBotiquines();

        if (granadas <= 0 && botiquines <= 0)
        {
            _consumibleEquipado = TipoConsumible.Ninguno;
            ActualizarHUD();
            return;
        }

        // Ciclo inteligente: omite consumibles vacíos
        if (_consumibleEquipado == TipoConsumible.Ninguno || _consumibleEquipado == TipoConsumible.Botiquin)
            _consumibleEquipado = (granadas > 0) ? TipoConsumible.Granada : TipoConsumible.Botiquin;
        else
            _consumibleEquipado = (botiquines > 0) ? TipoConsumible.Botiquin : TipoConsumible.Granada;

        ActualizarHUD();
    }

    /// <summary>
    /// Ruta de uso que delega en la lógica independiente de cada consumible.
    /// </summary>
    private void UsarConsumibleEquipado()
    {
        switch (_consumibleEquipado)
        {
            case TipoConsumible.Granada:
                UsarGranada();
                break;
            case TipoConsumible.Botiquin:
                UsarBotiquin();
                break;
        }
    }

    /// <summary>
    /// Lógica independiente para usar y lanzar una granada.
    /// </summary>
    private void UsarGranada()
    {
        if (_gameManager.CantidadGranadas() <= 0) return;

        _gameManager.UsarGranadas();
        LanzarGranada();
        CambioAutomaticoSiVacio();
    }

    /// <summary>
    /// Lógica independiente para usar un botiquín.
    /// Solo se consume si la vida actual es inferior a la máxima.
    /// </summary>
    private void UsarBotiquin()
    {
        if (_gameManager.CantidadBotiquines() <= 0) return;

        // Verificación crítica: no gastar botiquín si ya tiene vida máxima
        if (_gameManager.GetVidaActual() >= _gameManager.GetVidaMaxima())
        {
            Debug.Log("[ConsumibleHandler] Vida ya al máximo. No se gasta el botiquín.");
            return;
        }

        _gameManager.UsarBotiquin();
        _gameManager.CurarVida(_gameManager.GetVidaMaxima());
        Debug.Log("[ConsumibleHandler] Botiquín usado. Vida restaurada al máximo.");
        CambioAutomaticoSiVacio();
    }

    /// <summary>
    /// Instancia y lanza la granada con trayectoria parabólica en 2D.
    /// </summary>
    private void LanzarGranada()
    {
        if (PrefabGranada == null || PuntoLanzamiento == null)
        {
            Debug.LogWarning("[ConsumibleHandler] Asigna PrefabGranada y PuntoLanzamiento en el Inspector.");
            return;
        }

        GameObject granadaObj = Instantiate(PrefabGranada, PuntoLanzamiento.position, Quaternion.identity);
        Rigidbody2D rb = granadaObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float dir = transform.localScale.x > 0 ? 1f : -1f;
            float anguloRad = AnguloLanzamiento * Mathf.Deg2Rad;
            Vector2 direccion = new Vector2(Mathf.Cos(anguloRad) * dir, Mathf.Sin(anguloRad));
            rb.AddForce(direccion * FuerzaLanzamiento, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Cambia automáticamente al otro consumible si el actual se ha agotado.
    /// </summary>
    private void CambioAutomaticoSiVacio()
    {
        if (_consumibleEquipado == TipoConsumible.Granada && _gameManager.CantidadGranadas() <= 0 && _gameManager.CantidadBotiquines() > 0)
            _consumibleEquipado = TipoConsumible.Botiquin;
        else if (_consumibleEquipado == TipoConsumible.Botiquin && _gameManager.CantidadBotiquines() <= 0 && _gameManager.CantidadGranadas() > 0)
            _consumibleEquipado = TipoConsumible.Granada;
        else if (_gameManager.CantidadGranadas() <= 0 && _gameManager.CantidadBotiquines() <= 0)
            _consumibleEquipado = TipoConsumible.Ninguno;

        ActualizarHUD();
    }

    /// <summary>
    /// Actualiza la visibilidad de los iconos en el HUD según el consumible equipado.
    /// </summary>
    private void ActualizarHUD()
    {
        if (IconoGranada != null) IconoGranada.SetActive(_consumibleEquipado == TipoConsumible.Granada);
        if (IconoBotiquin != null) IconoBotiquin.SetActive(_consumibleEquipado == TipoConsumible.Botiquin);
    }

    #endregion   

} // class ConsumibleHandler 
  // namespace