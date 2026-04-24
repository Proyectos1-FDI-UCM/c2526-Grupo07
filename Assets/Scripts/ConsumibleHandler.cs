//---------------------------------------------------------
// Gestiona el cambio y uso de consumibles del jugador (Granadas y Botiquines).
// Responsable de la creación de este archivo
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;

/// <summary>
/// Enumeración que representa el tipo de consumible que el jugador tiene equipado.
/// Ninguno indica que no hay ningún consumible seleccionado.
/// </summary>
public enum TipoConsumible
{
    Ninguno,
    Granada,
    Botiquin
}

/// <summary>
/// Gestiona el sistema de consumibles del jugador: granadas y botiquines.
/// Permite al jugador cambiar entre consumibles, usarlos y lanzarlos.
/// Las granadas se lanzan con trayectoria parabólica y explotan tras un tiempo,
/// causando daño en área. Los botiquines restauran toda la vida del jugador.
/// Se comunica con GameManager para consultar y modificar cantidades y vida.
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

    [SerializeField] private Transform PuntoLanzamiento;  // Punto desde el que se lanza la granada
    [SerializeField] private GameObject PrefabGranada;    // Prefab de la granada a instanciar

    [SerializeField] private float FuerzaLanzamiento = 12f;  // Fuerza con la que se lanza la granada
    [SerializeField] private float AnguloLanzamiento = 40f;  // Ángulo de lanzamiento en grados

    [SerializeField] private GameObject IconoGranada;    // Icono de la granada en el HUD
    [SerializeField] private GameObject IconoBotiquin;   // Icono del botiquín en el HUD

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private TipoConsumible _consumibleEquipado = TipoConsumible.Ninguno; // Consumible actualmente equipado
    private GameManager _gameManager;                                     // Referencia al GameManager singleton

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
        {
            CambiarConsumible();
        }

        if (InputManager.Instance.UseObjectWasPressedThisFrame())
        {
            UsarConsumibleEquipado();
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

    /// <summary>
    /// Alterna el consumible equipado entre Granada y Botiquín,
    /// omitiendo automáticamente aquellos cuya cantidad sea cero.
    /// Si no hay ninguno disponible, establece el equipado como Ninguno.
    /// </summary>
    private void CambiarConsumible()
    {
        int cantidadGranadas = _gameManager.CantidadGranadas();
        int cantidadBotiquines = _gameManager.CantidadBotiquines();

        if (cantidadGranadas <= 0 && cantidadBotiquines <= 0)
        {
            _consumibleEquipado = TipoConsumible.Ninguno;
            Debug.Log("[ConsumibleHandler] No hay consumibles disponibles.");
            ActualizarHUD();
            return;
        }

        if (_consumibleEquipado == TipoConsumible.Ninguno || _consumibleEquipado == TipoConsumible.Botiquin)
        {
            if (cantidadGranadas > 0)
            {
                _consumibleEquipado = TipoConsumible.Granada;
            }
            else
            {
                _consumibleEquipado = TipoConsumible.Botiquin;
            }
        }
        else
        {
            if (cantidadBotiquines > 0)
            {
                _consumibleEquipado = TipoConsumible.Botiquin;
            }
            else
            {
                _consumibleEquipado = TipoConsumible.Granada;
            }
        }

        Debug.Log("[ConsumibleHandler] Consumible equipado: " + _consumibleEquipado);
        ActualizarHUD();
    }

    /// <summary>
    /// Redirige la lógica de uso al consumible actualmente equipado.
    /// </summary>
    private void UsarConsumibleEquipado()
    {
        if (_consumibleEquipado == TipoConsumible.Granada)
        {
            UsarGranada();
        }
        else if (_consumibleEquipado == TipoConsumible.Botiquin)
        {
            UsarBotiquin();
        }
        else
        {
            Debug.Log("[ConsumibleHandler] No hay consumible equipado para usar.");
        }
    }

    /// <summary>
    /// Consume una granada del inventario y la lanza con trayectoria parabólica.
    /// Si no quedan granadas, no hace nada.
    /// Tras el uso, cambia automáticamente al otro consumible si este se agotó.
    /// </summary>
    private void UsarGranada()
    {
        if (_gameManager.CantidadGranadas() <= 0)
        {
            Debug.Log("[ConsumibleHandler] No quedan granadas.");
            return;
        }

        _gameManager.UsarGranadas();
        LanzarGranada();
        Debug.Log("[ConsumibleHandler] Granada lanzada. Granadas restantes: " + _gameManager.CantidadGranadas());
        CambioAutomaticoSiVacio();
    }

    /// <summary>
    /// Consume un botiquín del inventario y restaura toda la vida del jugador.
    /// Solo se gasta si la vida actual es inferior a la máxima.
    /// Si no quedan botiquines, no hace nada.
    /// Tras el uso, cambia automáticamente al otro consumible si este se agotó.
    /// </summary>
    private void UsarBotiquin()
    {
        if (_gameManager.CantidadBotiquines() <= 0)
        {
            Debug.Log("[ConsumibleHandler] No quedan botiquines.");
            return;
        }

        if (_gameManager.GetVidaActual() >= _gameManager.GetVidaMaxima())
        {
            Debug.Log("[ConsumibleHandler] La vida ya está al máximo. No se gasta el botiquín.");
            return;
        }

        _gameManager.UsarBotiquin();
        _gameManager.CurarVida(_gameManager.GetVidaMaxima());
        Debug.Log("[ConsumibleHandler] Botiquín usado. Vida restaurada al máximo.");
        CambioAutomaticoSiVacio();
    }

    /// <summary>
    /// Instancia el prefab de la granada en el punto de lanzamiento
    /// y le aplica una fuerza en dirección parabólica según el ángulo y la dirección del jugador.
    /// </summary>
    private void LanzarGranada()
    {
        if (PrefabGranada == null || PuntoLanzamiento == null)
        {
            Debug.Log("[ConsumibleHandler] Falta asignar PrefabGranada o PuntoLanzamiento en el Inspector.");
            return;
        }

        GameObject granadaObj = Instantiate(PrefabGranada, PuntoLanzamiento.position, Quaternion.identity);
        Rigidbody2D rb = granadaObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float direccionHorizontal = 1f;
            if (transform.localScale.x < 0)
            {
                direccionHorizontal = -1f;
            }

            float anguloRad = AnguloLanzamiento * Mathf.Deg2Rad;
            float componenteX = Mathf.Cos(anguloRad) * direccionHorizontal;
            float componenteY = Mathf.Sin(anguloRad);
            Vector2 direccionLanzamiento = new Vector2(componenteX, componenteY);

            rb.AddForce(direccionLanzamiento * FuerzaLanzamiento, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Si el consumible actualmente equipado se ha agotado,
    /// cambia automáticamente al otro si está disponible,
    /// o establece Ninguno si ambos están vacíos.
    /// </summary>
    private void CambioAutomaticoSiVacio()
    {
        if (_consumibleEquipado == TipoConsumible.Granada && _gameManager.CantidadGranadas() <= 0)
        {
            if (_gameManager.CantidadBotiquines() > 0)
            {
                _consumibleEquipado = TipoConsumible.Botiquin;
                Debug.Log("[ConsumibleHandler] Sin granadas. Cambio automático a Botiquín.");
            }
            else
            {
                _consumibleEquipado = TipoConsumible.Ninguno;
                Debug.Log("[ConsumibleHandler] Sin granadas ni botiquines. Consumible: Ninguno.");
            }
        }
        else if (_consumibleEquipado == TipoConsumible.Botiquin && _gameManager.CantidadBotiquines() <= 0)
        {
            if (_gameManager.CantidadGranadas() > 0)
            {
                _consumibleEquipado = TipoConsumible.Granada;
                Debug.Log("[ConsumibleHandler] Sin botiquines. Cambio automático a Granada.");
            }
            else
            {
                _consumibleEquipado = TipoConsumible.Ninguno;
                Debug.Log("[ConsumibleHandler] Sin granadas ni botiquines. Consumible: Ninguno.");
            }
        }

        ActualizarHUD();
    }

    /// <summary>
    /// Actualiza la visibilidad de los iconos del HUD
    /// según el consumible que el jugador tiene equipado en ese momento.
    /// </summary>
    private void ActualizarHUD()
    {
        if (IconoGranada != null)
        {
            IconoGranada.SetActive(_consumibleEquipado == TipoConsumible.Granada);
        }

        if (IconoBotiquin != null)
        {
            IconoBotiquin.SetActive(_consumibleEquipado == TipoConsumible.Botiquin);
        }
    }

    #endregion

} // class ConsumibleHandler 
  // namespace