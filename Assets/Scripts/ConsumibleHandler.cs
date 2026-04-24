using UnityEngine;

public enum TipoConsumible
{
    Ninguno,
    Granada,
    Botiquin
}

public class ConsumibleHandler : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    [SerializeField] private Transform PuntoLanzamiento;
    [SerializeField] private GameObject PrefabGranada;
    [SerializeField] private float FuerzaLanzamiento = 12f;
    [SerializeField] private float AnguloLanzamiento = 40f;
    [SerializeField] private GameObject IconoGranada;
    [SerializeField] private GameObject IconoBotiquin;

    // ---- ATRIBUTOS PRIVADOS ----
    private TipoConsumible _consumibleEquipado = TipoConsumible.Ninguno;
    private GameManager _gameManager;
    private int _ultimaCantidadGranadas = -1;   //Para detectar cambios en el inventario
    private int _ultimaCantidadBotiquines = -1; 

    void Start()
    {
        _gameManager = GameManager.Instance;
        SincronizarConInventario(); //Auto-equipa al iniciar si ya tienes objetos
    }

    void Update()
    {
        //Detecta si el inventario cambió (por Recolect o GameManager) y refresca automáticamente
        if (_gameManager.CantidadGranadas() != _ultimaCantidadGranadas ||
            _gameManager.CantidadBotiquines() != _ultimaCantidadBotiquines)
        {
            SincronizarConInventario();
        }

        if (InputManager.Instance.ChangeObjectWasPressedThisFrame())
        {
            CambiarConsumible();
        }

        if (InputManager.Instance.UseObjectWasPressedThisFrame())
        {
            UsarConsumibleEquipado();
        }
    }

    // ---- MÉTODOS PÚBLICOS ----
    /// <summary>
    /// Llama a este método desde Recolect o GameManager cuando se recoge un objeto.
    /// </summary>
    public void SincronizarConInventario()
    {
        _ultimaCantidadGranadas = _gameManager.CantidadGranadas();
        _ultimaCantidadBotiquines = _gameManager.CantidadBotiquines();

        //Si no tienes nada, equipa Ninguno
        if (_ultimaCantidadGranadas <= 0 && _ultimaCantidadBotiquines <= 0)
        {
            if (_consumibleEquipado != TipoConsumible.Ninguno)
            {
                _consumibleEquipado = TipoConsumible.Ninguno;
                ActualizarHUD();
            }
            return;
        }

        //Si estás en Ninguno, auto-equipa el primer objeto disponible
        if (_consumibleEquipado == TipoConsumible.Ninguno)
        {
            _consumibleEquipado = (_ultimaCantidadGranadas > 0) ? TipoConsumible.Granada : TipoConsumible.Botiquin;
            ActualizarHUD();
        }
    }

    // ---- MÉTODOS PRIVADOS ----
    private void CambiarConsumible()
    {
        int granadas = _gameManager.CantidadGranadas();
        int botiquines = _gameManager.CantidadBotiquines();

        //Lógica de toggle limpia y predecible
        if (_consumibleEquipado == TipoConsumible.Granada)
        {
            _consumibleEquipado = (botiquines > 0) ? TipoConsumible.Botiquin : TipoConsumible.Granada;
        }
        else if (_consumibleEquipado == TipoConsumible.Botiquin)
        {
            _consumibleEquipado = (granadas > 0) ? TipoConsumible.Granada : TipoConsumible.Botiquin;
        }
        else //Estado Ninguno
        {
            _consumibleEquipado = (granadas > 0) ? TipoConsumible.Granada : TipoConsumible.Botiquin;
        }

        Debug.Log($"[ConsumibleHandler] Equipado: {_consumibleEquipado}");
        ActualizarHUD();
    }

    private void UsarConsumibleEquipado()
    {
        if (_consumibleEquipado == TipoConsumible.Granada) UsarGranada();
        else if (_consumibleEquipado == TipoConsumible.Botiquin) UsarBotiquin();
        else Debug.Log("[ConsumibleHandler] No hay consumible equipado.");
    }

    private void UsarGranada()
    {
        if (_gameManager.CantidadGranadas() <= 0) return;
        _gameManager.UsarGranadas();
        LanzarGranada();
        CambioAutomaticoSiVacio();
    }

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

    private void LanzarGranada()
    {
        if (PrefabGranada == null || PuntoLanzamiento == null) return;

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

    private void CambioAutomaticoSiVacio()
    {
        int granadas = _gameManager.CantidadGranadas();
        int botiquines = _gameManager.CantidadBotiquines();

        if (_consumibleEquipado == TipoConsumible.Granada && granadas <= 0)
        {
            _consumibleEquipado = (botiquines > 0) ? TipoConsumible.Botiquin : TipoConsumible.Ninguno;
        }
        else if (_consumibleEquipado == TipoConsumible.Botiquin && botiquines <= 0)
        {
            _consumibleEquipado = (granadas > 0) ? TipoConsumible.Granada : TipoConsumible.Ninguno;
        }

        ActualizarHUD();
    }

    private void ActualizarHUD()
    {
        if (IconoGranada != null) IconoGranada.SetActive(_consumibleEquipado == TipoConsumible.Granada);
        if (IconoBotiquin != null) IconoBotiquin.SetActive(_consumibleEquipado == TipoConsumible.Botiquin);
    }
}