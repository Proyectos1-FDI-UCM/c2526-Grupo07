//---------------------------------------------------------
// Script para la cámara que sigue el punto central entre el jugador y el mouse (horizontalmente)
// Responsable de la creación de este archivo: Izan Vázquez
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// la cámara que sigue el punto central entre el jugador y el mouse (horizontalmente), menos en las escenas con el boss
/// </summary>
public class FollowCamera : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField] private Transform target;
    [SerializeField] AimShoot Apuntado;
    [SerializeField] float TamañoCamara = 7f;
    [SerializeField] float Suavidad = 0.0025f;
    [SerializeField] float DistanciaMaxima = 3f;
    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints
    private float PosZ;
    private float PosY;
    private bool SalaDeBoss = false;
    private Transform LugarBoss;
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
        PosZ = -10;
        PosY = target.position.y;
        Camera.main.orthographicSize = TamañoCamara;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// Si no es una escena con el jefe, calcula la posición central entre el jugador (target) y el mouse (Apuntado)
    /// Y desplaza su posición
    /// En el otro caso, no se desplaza, se queda fijo en una posición donde se pueda ver el jefe y el jugador
    /// </summary>
    private void LateUpdate()
    {
        //si no esta en una escena con un boss
        if (!SalaDeBoss)
        {
            //calculo para determinar la posicion de la camara
            Vector3 Apunt = Apuntado.MousePos(); // posicion del mouse
            Vector3 direccionRaton = Apunt - target.position;
            Vector3 late = (Apunt - transform.position) / 2;
            Vector3 offset = Vector3.ClampMagnitude(direccionRaton / 2f, DistanciaMaxima); //Clampea la camara a una distancia del jugador
            Vector3 Objetivo = new Vector3(target.position.x + offset.x, PosY + 3.6f, PosZ);
            //Suavizar el movimiento de la cámara
            transform.position = Vector3.Lerp(transform.position, Objetivo, Suavidad);
        }
        // si esta en una escana con un boss (minijefe)
        else
        {
            //Vector3 ObjetivoBoss = LugarBoss.position;
            Vector3 Pos = transform.position;
            Pos.x = LugarBoss.position.x;
            transform.position = Vector3.Lerp(transform.position, Pos, 0.05f);
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
    /// deterina si es una sala con el boss (minijefe)
    /// </summary>
    public void SalaBoss(Transform ZonaDeBoss)
    {
        SalaDeBoss = true;
        LugarBoss = ZonaDeBoss;
    }
    /// <summary>
    /// deeactivar / transformar en false la SalaDeBoss (otro script llama a este metodo)
    /// </summary>
    public void UnableSalaBoss()
    {
        SalaDeBoss = false;
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    #endregion

} // class FollowCamera 
// namespace


