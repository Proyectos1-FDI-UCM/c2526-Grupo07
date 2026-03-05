//---------------------------------------------------------
// Detecta si los enemigos asignados para el rehén son destruidos, en ese caso, el rehén queda libre
// Carlos Alberto Ovando Barrios
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Permite al rehén detectar si los enemigos asignados en su lista siguen o han desaparecido,
/// si es es el caso, será libre.
/// </summary>
public class RehenesDetectEnemigos : MonoBehaviour
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
    [SerializeField]
    private SpriteRenderer Rehen;
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
    void Update()
    {
        //Si el rehén está libre, sale del Update
        if (_rehenLibre)
        {
            return;
        }
        //Axumimos que en cada frame no quedan enemigos
        bool QuedanEnemigos = false;
        //Buscamos que la lista de enemigos no está vacía
        for (int i = 0; i < Enemigos.Length; i++)
        {
            //Si encuentra algún enemigo en la lista, aún quedan enemigos y se termina el bucle
            if(Enemigos[i] != null)
            {
                QuedanEnemigos = true;
                break;
            }
        }
        //Si no quedan enemigos, el rehén está libre, y en el primer if, se saldrá del Update
        if (!QuedanEnemigos)
        {
            Debug.Log("Rehén Libre");
            _rehenLibre = true;
            Rehen.color = Color.green;
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

} // class RehenesDetectEnemigos 
// namespace
