//---------------------------------------------------------
// Breve descripción del contenido del archivo
// Responsable de la creación de este archivo
// Nombre del juego
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Antes de cada class, descripción de qué es y para qué sirve,
/// usando todas las líneas que sean necesarias.
/// </summary>
public class RehenEsposa : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints

    [SerializeField]
    private GameObject[] Enemigos; //Lista de enemigos que hay que eliminar para que el rehén esté libre

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private bool _rehenLibre = false; //Asumimos que el rehén no está libre 

    private Animator _animator; //Será el componente Animator

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
        _animator = GetComponent<Animator>(); //Asigna el componente Animator a _animator
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Si el rehén está libre, sale del Update
        if (_rehenLibre)
        {
            return;
        }
        //Asumimos que en cada frame no quedan enemigos
        bool QuedanEnemigos = false;
        //Buscamos que la lista de enemigos no está vacía
        int i = 0;
        while (QuedanEnemigos == false && i < Enemigos.Length)
        {
            //Si encuentra algún enemigo en la lista, aún quedan enemigos y se termina el bucle
            if (Enemigos[i] != null)
            {
                QuedanEnemigos = true;
                break;
            }
            i++;
        }
        //Si no quedan enemigos, el rehén está libre, y en el primer if, se saldrá del Update
        if (!QuedanEnemigos)
        {
            LiberarRehen();
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

    private void LiberarRehen()
    {
        Debug.Log("Rehén Libre");
        _rehenLibre = true;
        _animator.SetBool("Liberada", true); //El parámetro "Liberada" se vuelve true
    }

    #endregion

} // class RehenEsposa 
// namespace
