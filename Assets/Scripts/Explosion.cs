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

    [SerializeField] private Vector2 VelIn;

    [SerializeField] private float TiempoGranada = 2f; //El tiempo en segundos que tarda en explotar la granada

    [SerializeField] private float RadioGranada = 2f; //El radio de explosión de la granada

    [SerializeField] private int Damage; //El daño que causa

    [SerializeField] private FollowCamera Camara;

    [SerializeField] private float TiempoAnimacionExplosion = 0.15f; //Tiempo de la animación de la explosión

    [SerializeField] private GameObject Particulas;


    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    private float _tiempo; //El tiempo que se reducirá para que explote la granada
    private Rigidbody2D _rb;
    private float _direction;
    private Animator _animator; //Será el componente Animator
    private bool _destruida = false; //Comprueba si explotó
    private int _damageReduction; 

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
        _rb = GetComponent<Rigidbody2D>();
        VelIn.x *= _direction;
        _rb.linearVelocity = VelIn;
        _tiempo = TiempoGranada; //El tiempo que se reducirá, es el mismo que el tiempo en que explota la granada, para no reducir directamente el tiempo de la granada
        _animator = GetComponent<Animator>(); // Recoge el Animator de la granada
        _damageReduction = (int)(Damage * 0.5f);

        //Comprueba si la granada fue lanzada para iniciar la animación
        if (_animator != null)
        {
            _animator.SetBool("Lanzada", true);
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Comprueba si explotó para salir del Update
        if (_destruida)
        {
            return;
        }
        //Reduce el tiempo de explosión
        _tiempo -= Time.deltaTime;
        //Cuando llega a cero explota
        if(_tiempo <= 0)
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
    public void SetDireccion(Vector3 dir)
    {
        if (dir.x < 0) _direction = -1f;
        else _direction = 1f;
    }
    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    //Explotar==Crea una lista con los objetos dentro del radio de la granada, si estos tiene EnemyHealth o PlayerController, les causará el daño de la granada,
    //solo si el RayCast no detecta algún objeto con el tag "Pared" antes que al enemigo
    private void Explotar()
    {
        _destruida = true; //Explotó
        //Crea un lista con los objetos que estén en el radio al explotar
        Collider2D[] Enemigos = Physics2D.OverlapCircleAll(transform.position, RadioGranada);
        //Recorre la lista
        for(int i = 0; i < Enemigos.Length; i++)
        {
            EnemyHealth VidaEnemigo = Enemigos[i].GetComponent<EnemyHealth>(); //Guarda el componente EnemyHealth para comprobar si hay enemigos

            PlayerController Player = Enemigos [i].GetComponent<PlayerController>(); //Guaerda el componente Player controller para comprobar si es el jugador

            if (VidaEnemigo != null || Player != null) //Comprueba que haya enemigos o el jugador
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

                if (!Pared) //Si no hay pared causa daño
                {
                    if(VidaEnemigo != null)
                    {
                        VidaEnemigo.EnemyHealthPoint(Damage);
                        VidaEnemigo.RedFlash();
                    }
                    else if(Player != null && !GameManager.Instance.Invulnerabilidad())
                    {
                        GameManager.Instance.RestarVida(_damageReduction);
                        Player.RedFlash();
                    }
                }
            }
        }

        _rb.linearVelocity = Vector2.zero; //Quita la velocidad de la granada para que la animación de explosión no se mueva
        _rb.simulated = false; //Quita las físicas de la granada
        if (Particulas != null)
        {
            // Crea las partículas de la explosión
            Instantiate(Particulas, transform.position, Quaternion.identity);
        }
        //Inicia la animación de explosión
        if (_animator != null)
        {
            _animator.SetTrigger("Explosion");

        }

        Destroy(gameObject, TiempoAnimacionExplosion); //Se destruye la granada

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
