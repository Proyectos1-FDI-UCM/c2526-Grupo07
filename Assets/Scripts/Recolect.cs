//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


// Los enums se declaran FUERA de la clase para que Unity los serialice
// correctamente y el inspector los reconozca sin ambigüedad.
public enum TipoObjeto
{
    None,
    Granada,
    Botiquin
}

public enum TipoArma
{
    None,
    AK47,
    Lanzallamas
}

/// <summary>
/// Componente que gestiona la recogida de objetos del escenario.
/// Al entrar en contacto con el jugador, añade al inventario el objeto
/// configurado (consumible o arma) si hay hueco disponible y se destruye.
/// </summary>
public class Recolect : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    /// <summary>Tipo de consumible que representa este pickup (None si es un arma).</summary>
    [SerializeField] private TipoObjeto Objeto;

    /// <summary>Tipo de arma que representa este pickup (None si es un consumible).</summary>
    [SerializeField] private TipoArma Arma;

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // (Los enums han sido movidos fuera de la clase — ver arriba)
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

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    /// <summary>

    /// Se ejecuta cuando otro collider 2D entra en el trigger de este objeto.
    /// Comprueba si es el jugador y, si tiene hueco en el inventario,
    /// añade el objeto correspondiente y destruye el pickup.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && other != null)
        {
            Debug.Log($"[Recolect] Objeto={Objeto} (valor int={(int)Objeto}), Arma={Arma}");
            if (Objeto != TipoObjeto.None)
            {
                if (Objeto == TipoObjeto.Granada && GameManager.Instance.GranadasFull() == false)
                {
                    GameManager.Instance.GuardarGranadas();
                    Destroy(gameObject);
                }
                else if (Objeto == TipoObjeto.Botiquin && GameManager.Instance.BotiquinesFull() == false)
                {
                    GameManager.Instance.GuardarBotiquines();
                    Destroy(gameObject);
                }
            }
            else if (Arma != TipoArma.None)
            {
                if (Arma == TipoArma.AK47 && !GameManager.Instance.TieneAK47())
                {
                    GameManager.Instance.RecogerAK47();
                    Destroy(gameObject);
                }
            }
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

    #endregion

} // class Recolect 
  // namespace