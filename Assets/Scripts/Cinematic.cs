//---------------------------------------------------------
// Gestión de la cinemática permitiendo pausarla y reanudarla desde fuera. Cambia de escena al terminar la cinemática.
// Cristopher Jeremy Villacís Galindo
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Playables;
// Añadir aquí el resto de directivas using


/// <summary>
/// Inicia una cinemática asignada al iniciar la escena.
/// Una vez terminada la cinemática se cambiará a la escena cuyo índice que establece desde el inspector.
/// La clase tiene métodos que permiten pausar y reanudar la escena desde fuera.
/// </summary>
public class Cinematic : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private int _nextScene;    //Índice de la escena a la que se irá tras terminar la cinemática
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private PlayableDirector _cinematic; //Variable que adoptará el componente que tiene la TimeLine
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
        _cinematic = GetComponent<PlayableDirector>();   //_cinematic adopta el componente PlayableDirector
        _cinematic.Play();                               //Inicia la cinemática
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        //Cambia la escena una vez termina la cinemática
        if (_cinematic.time >= _cinematic.duration)
        {
            GameManager.Instance.ChangeScene(_nextScene);
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

    /// <summary>
    /// Método que pausa la cinemática.
    /// </summary>
    public void CinematicPause()
    {
        _cinematic.Pause();
    }

    /// <summary>
    /// Método que reanuda la cinemática.
    /// </summary>
    public void CinematicResume()
    {
        _cinematic.Resume();
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class Cinematic 
// namespace
