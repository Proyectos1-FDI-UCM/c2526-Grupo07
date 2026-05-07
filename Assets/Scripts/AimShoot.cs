//---------------------------------------------------------
// Este script dispara, lanza granadas, cambia de arma y recarga automaticamente, se usa a la entidad jugador.
// Responsable de la creación de este archivo: Cristopher Jeremy 
// Clear The Building
// Proyectos 1 - Curso 2025-26
//---------------------------------------------------------

using UnityEngine;
using UnityEngine.Rendering;
// Añadir aquí el resto de directivas using


/// <summary>
/// dispara, lanza granadas, cambia de arma y recarga automaticamente, se usa a la entidad jugador.
/// Además rota la posición de la pistola segun donde mire el mouse
/// Y ejecuta las animaciones para la pistola y el rifle si cambia de arma
/// </summary>
public class AimShoot : MonoBehaviour
{
    // ---- ATRIBUTOS DEL INSPECTOR ----
    #region Atributos del Inspector (serialized fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // públicos y de inspector se nombren en formato PascalCase
    // (palabras con primera letra mayúscula, incluida la primera letra)
    // Ejemplo: MaxHealthPoints
    
    //Granadas
    [SerializeField] private GameObject Granada; //Objeto Granada que se crea al usar objeto
    [SerializeField] private int NumGranadas;    //Numero de granadas que se tiene
    [SerializeField] private AudioSource AudioGranada; // audio de explosion de granada

    //Balas
    [SerializeField] private GameObject BalaPistola; //Objeto Bala de pistola que se crea al Dispara
    [SerializeField] private GameObject BalaRifle;   //Objeto Bala de pistola que se crea al Dispara
    [SerializeField] private Transform SalidaBala;   //Posición donde saldrá la bala
    [SerializeField] private float Cadencia = 1f;    //Balas por segundo
    [SerializeField] private AudioSource PistolaSFX; //Sonido del disparo pistola
    [SerializeField] private AudioSource RecargaPistolaSFX; //Sonido recarga pistola
    [SerializeField] private AudioSource RifleSFX; //Sonido del disparo rifle
    [SerializeField] private AudioSource RecargaRifleSFX; //Sonido recarga pistola

    //Sprites armas
    [SerializeField] private GameObject SpritePistola; //Sprite para la pistola
    [SerializeField] private GameObject SpriteRifle;   //Sprite para el rifle

    //Recarga
    [SerializeField] private int Cargador = 10;                 //Número de balas que se pueden disparar
    [SerializeField] private float TiempoRecargaPistola = 0.1f; //Tiempo que tarda la pistola en recargar
    [SerializeField] private float TiempoRecargaRifle = 1.5f;   //Tiempo que tarda el rifle en recargar
    [SerializeField] private GameObject SpriteRecarga;          //Sprite del símbolo de recarga

    #endregion

    // ---- ATRIBUTOS PRIVADOS ----
    #region Atributos Privados (private fields)
    // Documentar cada atributo que aparece aquí.
    // El convenio de nombres de Unity recomienda que los atributos
    // privados se nombren en formato _camelCase (comienza con _, 
    // primera palabra en minúsculas y el resto con la 
    // primera letra en mayúsculas)
    // Ejemplo: _maxHealthPoints

    //Direcciones de apuntado
    Vector3 _direction, _lastMousePos, _mousePosition; //Direcciones para disparar
    private GameObject Bala; //Bala actual que sale
    private SpriteRenderer _spriteRenderer; // sprite del la pistola

    //Disparo
    private float _tiempoDisparo = 0f; //Tiempo que falta para poder disparar, controla la cadencia
    private int _balasActuales;        //Balas actuales
    private float _tiempoRecarga = 0f;  //Tiempo que el jugador tarda en recargar

    //Pistola
    private int _balasActualesPistola;   //Balas disponibles en el cargador pistola
    private const float _cadenciaPistola = 2f;   //Intervalo de disparo de la pistola
    private const int _cargadorPistola = 10;     //Cargador de la pistola

    //Rifle
    private int _balasActualesRifle;     //Balas disponibles en el cargador rifle
    private const float _cadenciaRifle = 10f;    //Intervalo de disparo de el rifle
    private const int _cargadorRifle = 30;       //Cargador del rifle

    //Cambio de arma
    private string _armaActual; //Arma actual que se usa

    //Recarga
    private bool _recargando = false;  //No está recargando, por ahora
    private float _recibirTiempoRecarga = 0f; //Tiempo que el jugador tarda en recargar, recibe el valor del TiempoRecarga, pero este se modifica
    #endregion
    //Granada
    private int _cantidadGranada;
    private bool _usandoGranada;

    // Animacion
    private Animator _anim;

    //Cheat
    private bool cheatMode = false;
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
        cheatMode = GameManager.Instance.GetCheatMode();
        _anim = GetComponent<Animator>();
        SpriteRifle.SetActive(false);
        SpriteRecarga.SetActive(false);
        _direction = transform.position;
        _mousePosition = InputManager.Instance.GetAimMouseValue();
        _balasActuales = Cargador; //Iniciamos con el cargador lleno, las balas disponibles son todas las del cargador
        _balasActualesPistola = 10;
        _balasActualesRifle = 30;
        _tiempoRecarga = TiempoRecargaPistola;
        Bala = BalaPistola;
        _armaActual = "Pistola";
        Debug.Log("Balas: " + _balasActuales);
        GameManager.Instance.SetMunicion(Cargador, _balasActuales);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// Modifica la rotacion de la entidad para apuntar donde debe
    /// ejecuta el metodo recarga si presiona el boton correspondiente
    /// Cooldown de pistola: no se podrá lanzar continuamente la pistola
    /// Ejecuta método disparar si presiona el boton correspondiente
    /// ejecuta lanza granada si presiona el boton correspondiente: crea la granada, lo lanza según la posicion del cursor y ejecuta el metodo de explosión
    /// </summary>
    void Update()
    {
        //animaciones se incializa a false
        _anim.SetBool("RifleShoot", false);
        _anim.SetBool("GunShoot", false);

        //Si el juego esta pausado no puede disparar
        if (!LevelManager.Instance.IsPaused())
        {
            //si el jugador usa mando
            _mousePosition = InputManager.Instance.GetAimMouseValue();
            if (InputManager.Instance.AimControllerIsPressed())
            {
                Vector3 rStickdir = InputManager.Instance.GetAimControllerValue();
                _direction = rStickdir;

                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                //booleano que determina si el raton esta mirando hacia la derecha o la izquierda
                bool mouseRight;
                //comprueba en que sentido mira (lo hace mediante angulos)
                if (angle > -90 && angle <= 90) { mouseRight = true; }
                else { mouseRight = false; }
                // dependiendo de la posicion del raton cambia de un sentido a otro
                if (mouseRight) // derecha 
                {
                    if (transform.localScale.y < 0)
                    { transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y); }
                }
                else if (!mouseRight) // izquierda
                {
                    if (transform.localScale.y > 0)
                    { transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y); }
                }
            }
            else if (_mousePosition != _lastMousePos) // si el jugador usa raton
            {
                Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(_mousePosition);

                _direction = cursorWorldPosition - transform.position;
                _lastMousePos = _mousePosition;

                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                bool mouseRight;
                //comprueba en que sentido mira (lo hace mediante angulos)
                if (angle > -90 && angle <= 90) { mouseRight = true; }
                else { mouseRight = false; }
                // dependiendo de la posicion del raton cambia de un sentido a otro
                if (mouseRight) // derecha
                {
                    if (transform.localScale.y < 0)
                    { transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y); }
                }
                else if (!mouseRight) // izquierda
                {
                    if (transform.localScale.y > 0)
                    { transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y); }
                }
            }
            //Comprueba si está recargando, si es así, reduce el tiempo de recarga
            if (_recargando)
            {
                _recibirTiempoRecarga -= Time.deltaTime;

                // Cuando el tiempo de recarga termina, se llena el cargador 
                if (_recibirTiempoRecarga <= 0f)
                {
                    TerminarRecarga(); //Llena el cargador y "recargando" se vuelve falso
                }
                //Salimos del Update mientras se recarga, para que el jugador no dispare, y como "recargando" se vuelve falso al terminar, la proxima vez podrá disparar
                return;
            }
            //Si no se recarga:

            //1 El tiempo para poder volver a disparar se reduce con el delta time
            if (_tiempoDisparo > 0)
            {
                _tiempoDisparo -= Time.deltaTime;
            }

            bool cargadorLleno = (_balasActuales == Cargador); //El cargador está lleno(true) si las balas actuales son las mismas que las del cargador

            //2 Si el cargador no está lleno e intentamos recargar, empieza la recarga
            if (InputManager.Instance.ReloadWasPressedThisFrame() && !cargadorLleno)
            {
                EmpezarRecarga(); //Vuelve true a recargando y asigna el tiempo de recarga a "tiempoRecarga"
                return; //Sale del Update para que no dispare
            }
            //3 Comprueba si se dispara y si se puede disparar por el tiempo y por las balas disponibles
            if (InputManager.Instance.FireIsPressed() && _tiempoDisparo <= 0f && _balasActuales > 0)
            {
                Disparar();
            }
            //4 Comprueba si se dispara y si no puede disparar por no tener balas disponibles
            if (InputManager.Instance.FireIsPressed() && _tiempoDisparo <= 0f && _balasActuales <= 0)
            {
                EmpezarRecarga(); //Vuelve true a recargando y asigna el tiempo de recarga a "tiempoRecarga"
                return; //Sale del Update para que no dispare
            }

            // usando granada
            _cantidadGranada = GameManager.Instance.CantidadGranadas(); // dar la cantidad de granada
            _usandoGranada = GameManager.Instance.UsandoGranadas(); // restar la cantidad de granada

            //usar la granada
            if (InputManager.Instance.UseObjectWasPressedThisFrame())
            {
                if (InputManager.Instance.UseObjectWasPressedThisFrame())
                {
                    if (_cantidadGranada > 0 && _usandoGranada)
                    {
                        GameManager.Instance.UsarGranadas();
                        GameObject newGranada = Instantiate(Granada, transform.position, transform.rotation);
                        Explosion bomba = newGranada.GetComponent<Explosion>();
                        bomba.SetDireccion(_direction, AudioGranada);
                    }
                }
            }
            // cambiar de arma
            if (InputManager.Instance.ChangeWeaponWasPressedThisFrame())
            {
                CambioDeArma();
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

    /// <summary>
    /// dar a la camara la posicion del raton
    /// </summary>
    public Vector3 MousePos() // dar a la camara la posicion del raton
    {
        return Camera.main.ScreenToWorldPoint(_mousePosition);
    }

    /// <summary>
    /// Método para cambiar de arma(lineal)
    /// </summary>
    public void CambioDeArma()
    {
        if (_armaActual == "Pistola")
        {
            _balasActualesPistola = _balasActuales;
            SetRifle();
        }
        else if (_armaActual == "Rifle")
        {
            _balasActualesRifle = _balasActuales;
            SetPistola();
        }
    }

    #endregion

    // ---- MÉTODOS PRIVADOS ----
    #region Métodos Privados
    // Documentar cada método que aparece aquí
    // El convenio de nombres de Unity recomienda que estos métodos
    // se nombren en formato PascalCase (palabras con primera letra
    // mayúscula, incluida la primera letra)

    /// <summary>
    /// Disparar==Crea una bala, resta la bala del cargador y reinicia el tiempo de disparo
    /// ejecuta la animacion correspondiente
    /// </summary>
    private void Disparar()
    {
        //Crea la bala en su posición de salida
        //Instantiate(Bala, SalidaBala.position, SalidaBala.rotation);
        if (_armaActual == "Pistola")
        {
            PistolaSFX.Play(); // ejecutar sonido
            _anim.SetBool("GunShoot", true); // ejecutar animacion
        }
        else if(_armaActual == "Rifle")
        {
            RifleSFX.Play(); // ejecutar sonido
            _anim.SetBool("RifleShoot", true); // ejecutar animacion
        }
        // crear bala
        GameObject nuevaBala = Instantiate(Bala, SalidaBala.position, SalidaBala.rotation); 
        BulletBehaviour balaDir = nuevaBala.GetComponent<BulletBehaviour>();
        balaDir.Dir(_direction);

        // Restamos una bala al cargador
        if (!cheatMode)
        { _balasActuales--; }
        GameManager.Instance.SetMunicion(Cargador, _balasActuales);
        Debug.Log("Balas: " + _balasActuales);

        // Reiniciamos el tiempo de disparo según la cadencia
        _tiempoDisparo = 1f / Cadencia;

    }
    /// <summary>
    /// EmpezarRecarga==Vuelve true a recargando y asigna el tiempo de recarga a "tiempoRecarga"
    /// </summary>
    private void EmpezarRecarga()
    {
        _recargando = true;
        _recibirTiempoRecarga = _tiempoRecarga;
        Debug.Log("Recargando");
        if (_armaActual == "Pistola")
        {
            RecargaPistolaSFX.Play();
        }
        else if (_armaActual == "Rifle")
        {
            RecargaRifleSFX.Play();
        }

        SpriteRecarga.SetActive(true);
    }
    /// <summary>
    /// TerminarRecarga==Vuelve false a recargando y las balas actuales se llenan
    /// </summary>
    private void TerminarRecarga()
    {
            _recargando = false;
            _balasActuales = Cargador;
            GameManager.Instance.SetMunicion(Cargador, _balasActuales);
            Debug.Log("Balas: " + _balasActuales);
            SpriteRecarga.SetActive(false);
    }
    /// <summary>
    /// Método llamado si se cambia a la pistola
    /// cambia la animación (desactivar la animación del rifle)
    /// Cambiar las recargas a las que tenia en esa arma
    /// modificar el hud para que aparezca el sprite de pistola
    /// </summary>
    private void SetPistola()
    {
        _anim.SetBool("Rifle", false); // desactivar la animacion del rifle para que se vea la animacion de pistola
        _balasActuales = _balasActualesPistola;
        Cargador = _cargadorPistola;
        Cadencia = _cadenciaPistola;
        _tiempoRecarga = TiempoRecargaPistola;
        Bala = BalaPistola;
        _armaActual = "Pistola";
        Debug.Log("Cambiado a pistola");
        GameManager.Instance.SetMunicion(Cargador, _balasActuales);
        // cambiar el arma en el hud
        SpritePistola.SetActive(true);
        SpriteRifle.SetActive(false);
    }
    /// <summary>
    /// Método llamado si se cambia al rifle
    /// cambia la animación (activar la animación del rifle)
    /// Cambiar las recargas a las que tenia en esa arma
    /// modificar el hud para que aparezca el sprite de rifle
    /// </summary>
    private void SetRifle()
    {
        if (GameManager.Instance.TieneAK47())
        {
            _anim.SetBool("Rifle", true); // activar la animacion del rifle
            _balasActuales = _balasActualesRifle;
            Cargador = _cargadorRifle;
            Cadencia = _cadenciaRifle;
            _tiempoRecarga = TiempoRecargaRifle;
            Bala = BalaRifle;
            _armaActual = "Rifle";
            Debug.Log("Cambiado a rifle");
            GameManager.Instance.SetMunicion(Cargador, _balasActuales);
            // cambiar el arma en el hud
            SpritePistola.SetActive(false);
            SpriteRifle.SetActive(true);
        }
    }
    #endregion

} // class AimShoot 
// namespace
