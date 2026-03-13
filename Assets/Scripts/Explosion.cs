//---------------------------------------------------------
// Permite a un objeto(Granada) explotar después de un tiempo y causar daño a todos los enemigos dentro de su radio de explosión
// Carlos Alberto Ovando Barrios
// Clear the Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using System;
using UnityEngine;
// Añadir aquí el resto de directivas using


/// <summary>
/// Permite a un objeto explotar después de un tiempo asignable
/// Al explotar, se destruye y causa un daño asignable dento de un radio asignable
/// Realiza el daño a todos los objetos con "EnemyHealth", siempre y cuando no estén detrás e algún objeto con el tag "Pared"
/// </summary>
public class Explosion : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    [SerializeField]
    private float TiempoGranada = 2f; //El tiempo en segundos que tarda en explotar la granada
    [SerializeField]
    private float RadioGranada = 2f; //El radio de explosión de la granada
    [SerializeField]
    private int Damage = 2; //El daño que causa


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float tiempo; //El tiempo que se reducirá para que explote la granada

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
        tiempo = TiempoGranada; //El tiempo que se reducirá, es el mismo que el tiempo en que explota la granada, para no reducir directamente el tiempo de la granada
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Reduce el tiempo de explosión
        tiempo -= Time.deltaTime;
        //Cuando llega a cero explota
        if(tiempo <= 0)
        {
            Explotar();
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

    //Explotar==Crea una lista con los objetos dentro del radio de la granada, si estos tiene EnemyHealth, les causará el daño de la granada,
    //solo si el RayCast no detecta algún objeto con el tag "Pared" antes que al enemigo
    private void Explotar()
    {
        //Crea un lista con los objetos que estén en el radio al explotar
        Collider2D[] Enemigos = Physics2D.OverlapCircleAll(transform.position, RadioGranada);
        //Recorre la lista
        for(int i = 0; i < Enemigos.Length; i++)
        {
            EnemyHealth VidaEnemigo = Enemigos[i].GetComponent<EnemyHealth>(); //Guarda el componente EnemyHealth del objeto en VidaEnemigo, pero si no lo tiene, estará vacío(null)

            if(VidaEnemigo != null ) //Revisa si no está vacío, s decir, que tiene EnemyHealth
            {
                Vector2 vectorEnemigo = Enemigos[i].transform.position - transform.position; //Vector entre la granada y el enemigo

                float longitudVector = vectorEnemigo.magnitude; //Longitud del vector

                RaycastHit2D[] Objetos = Physics2D.RaycastAll(transform.position, vectorEnemigo, longitudVector); //Crea una lista con todos los objetos que estén entre la granada y el enemigo

                bool Pared = false; //Si hay una pared o no

                for (int j = 0; j < Objetos.Length; j++) //Recorre la lista de los objetos
                {
                    if (Objetos[j].collider.CompareTag("Pared")) //Detecta si uno de esos objetos tiene el tag "Pared"
                    {
                        Pared = true; //Hay una pared

                        break; //Sale del bucle for
                    }
                }

                if (!Pared) //Si no hay pared causa daño al enemigo
                {
                    VidaEnemigo.EnemyHealthPoint(Damage);
                }
            }
        }
        Destroy(gameObject); //Se destruye la granada

    }



    //Dibuja el radio de la granada en el inspector
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RadioGranada);
    }
    #endregion   

} // class Explosion 
// namespace
