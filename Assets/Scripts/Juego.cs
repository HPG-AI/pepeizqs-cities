﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    public Idiomas idioma;

    public Canvas canvas;

    public KeyCode teclaMenu;
    public KeyCode teclaRotacionEdificioIzquierda;
    public KeyCode teclaRotacionEdificioDerecha;
    public KeyCode teclaArrastrarConstruccion;

    public Interfaz.MenuPrincipal menuPrincipal;
    public Interfaz.Opciones opciones;
    public Interfaz.Opciones2.General opcionesGeneral;
    public Interfaz.Opciones2.Graficos opcionesGraficos;
    public Interfaz.Opciones2.Teclado opcionesTeclado;
    public Interfaz.Ayuda ayuda;
    public Interfaz.MenuJuego menuJuego;

    public Text versionTexto;

    public Button botonCargarPartida;

    public AudioSource musicaFondo;
    public AudioSource sonidoBoton;
    public AudioSource sonidoBotonConstruir;
    public AudioSource sonidoBotonDemoler;

    [SerializeField]
    private Ciudad ciudad;

    [SerializeField]
    private Construccion[] edificios;

    [SerializeField]
    private Colocar colocar;

    [SerializeField]
    private ColocarPrevio colocarPrevio;

    [SerializeField]
    private DiaNoche diaNoche;

    private Construccion edificioSeleccionado;
    private List<Construccion2> edificiosSeleccionados = new List<Construccion2>();

    private int rotacionColocar = -180;
    private int rotacionesPosicion = 0;

    private bool construirActivar;
    private bool demolerActivar;

    public Button botonDemoler; 

    public Camera camara;

    public ArbolesInicio arbolesInicio;

    public Panel panelConstruir;
    public Panel panelConstruirSub;
    public Panel panelDemoler;
    public Panel panelDemolerSub;
    public Panel panelDatos;
    public Panel panelEdificios;
    public Panel panelGuardar;
    public Panel panelTiempo;

    public Interfaz.Edificios panelEdificios2;
    public Panel panelCarreteras;

    private bool mantenerEjeX;
    private bool mantenerEjeZ;

    private void Start()
    {
        opciones.CargarInicio();
        opcionesGeneral.CargarInicio();
        opcionesGraficos.CargarInicio();
        opcionesTeclado.CargarInicio();

        opcionesGeneral.Sonido();
        idioma.CargarTextos();

       //File.Delete(Application.persistentDataPath + "/guardado.save");
        diaNoche.VelocidadMarchas(0);
        versionTexto.text = "v" + Application.version;

        if (DetectarPartidaGuardada() != null)
        {
            botonCargarPartida.interactable = true;
            CargarEdificios();
            ayuda.Cargar(false);
        }
        else
        {
            botonCargarPartida.interactable = false;
            arbolesInicio.Colocar(colocar);
        }            
    }

    public void NuevaPartida()
    {
        sonidoBoton.Play();

        if (File.Exists(Application.persistentDataPath + "/guardado.save"))
        {
            File.Delete(Application.persistentDataPath + "/guardado.save");
        }

        diaNoche.tiempoDia = 0.4f;
        //ciudad.Dinero = 2000000;
        ciudad.Dinero = 200;
        ciudad.PoblacionActual = 0f;
        ciudad.PoblacionTope = 0f;
        ciudad.TrabajosActual = 0;
        ciudad.TrabajosTope = 0;
        ciudad.Comida = 0f;

        camara.transform.position = new Vector3(10, 60, 10);
        colocar.QuitarTodosEdificios();
        arbolesInicio.Colocar(colocar);
        CargarInterfaz();
    }

    public void CargarPartida()
    {
        sonidoBoton.Play();
        colocar.QuitarTodosEdificios();
        CargarEdificios();
        CargarInterfaz();      
    }

    private void CargarInterfaz()
    {
        menuPrincipal.MostrarJuego();
        ayuda.Cargar(true);
        ayuda.EstadoCajas(true);
        diaNoche.VelocidadMarchas(1);
        colocarPrevio.QuitarTodosEdificios();
        panelEdificios2.Arranque(edificios);

        construirActivar = false;
        ConstruirCambiarColor();

        demolerActivar = false;
        DemolerCambiarColor();

        musicaFondo.loop = true;
        musicaFondo.Play();   
    }

    void Update ()
    {
        if (canvas.gameObject.GetComponent<CanvasGroup>().alpha == 1)
        {
            if (Input.GetKeyDown(teclaMenu))
            {
                menuJuego.MostrarMenu();
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (construirActivar == true)
                {
                    construirActivar = false;
                    ConstruirCambiarColor();
                    ConstruirOcultarPanelEdificios();
                    colocarPrevio.QuitarTodosEdificios();
                }

                if (demolerActivar == true)
                {
                    demolerActivar = false;
                    DemolerCambiarColor();
                }
            }

            if (edificioSeleccionado != null)
            {
                if (construirActivar == true)
                {
                    int[] rotaciones = new int[4];

                    rotaciones[0] = -180;
                    rotaciones[1] = -270;
                    rotaciones[2] = 0;
                    rotaciones[3] = -90;

                    if (Input.GetKeyDown(teclaRotacionEdificioDerecha))
                    {
                        rotacionesPosicion += 1;

                        if (rotacionesPosicion == 4)
                        {
                            rotacionesPosicion = 0;
                        }

                        rotacionColocar = rotaciones[rotacionesPosicion];
                    }

                    if (Input.GetKeyDown(teclaRotacionEdificioIzquierda))
                    {
                        rotacionesPosicion -= 1;

                        if (rotacionesPosicion == -1)
                        {
                            rotacionesPosicion = 3;
                        }

                        rotacionColocar = rotaciones[rotacionesPosicion];
                    }
                    Debug.Log(rotacionColocar);
                    if (Input.GetKey(teclaArrastrarConstruccion))
                    {
                        if ((edificioSeleccionado.id == 6) || (edificioSeleccionado.id == 12))
                        {
                            ColocarEdificioPrevio(true, 1);
                        }                           
                    }
                    else
                    {
                        mantenerEjeX = true;
                        mantenerEjeZ = true;

                        foreach (Construccion2 edificio in edificiosSeleccionados)
                        {
                            colocarPrevio.QuitarEdificio(edificio.edificio, edificio.posicion);
                        }

                        edificiosSeleccionados.Clear();
                        ColocarEdificioPrevio(false, 0);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    ColocarEdificio(0);
                }
            }

            if (demolerActivar == true)
            {
                DemolerPrevio();

                if (Input.GetMouseButtonDown(0))
                {
                    ColocarEdificio(1);
                }
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                InterfazOcultar();
            }
        }      
    }

    public void Construir()
    {
        sonidoBoton.Play();

        demolerActivar = false;
        DemolerCambiarColor();

        colocarPrevio.QuitarTodosEdificios();
        menuJuego.CerrarPanel();

        if (construirActivar == true)
        {
            construirActivar = false;
        }
        else
        {
            construirActivar = true;
        }

        if (construirActivar == true)
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(118f / 255f, 118f / 255f, 118f / 255f, 255f);
        }
        else
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }

        if (panelEdificios.gameObject.GetComponent<CanvasGroup>().alpha == 0)
        {
            panelEdificios.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            panelEdificios.gameObject.GetComponent<CanvasGroup>().interactable = true;
            panelEdificios.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            panelEdificios.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            panelEdificios.gameObject.GetComponent<CanvasGroup>().interactable = false;
            panelEdificios.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        panelEdificios2.ConstruirMostrarPanel(panelCarreteras);
    }

    public void ConstruirRatonEntra()
    {
        if (construirActivar == false)
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(118f / 255f, 118f / 255f, 118f / 255f, 255f);
        }
        else
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }
    }

    public void ConstruirCambiarColor()
    {
        if (construirActivar == true)
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(118f / 255f, 118f / 255f, 118f / 255f, 255f);
        }
        else
        {
            panelConstruirSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }
    }

    public void ConstruirOcultarPanelEdificios()
    {
        panelEdificios.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        panelEdificios.gameObject.GetComponent<CanvasGroup>().interactable = false;
        panelEdificios.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void ConstruirSeleccionarEdificio(int edificio)
    {
        demolerActivar = false;
        DemolerCambiarColor();

        construirActivar = true;
        ConstruirOcultarPanelEdificios();

        edificioSeleccionado = edificios[edificio]; 
        sonidoBoton.Play();
    }

    void ColocarEdificio(int accion)
    {
        Vector3 raton = Posicion.Raton();

        if (raton != Vector3.zero)
        {
            Vector3 gridPosicion = Posicion.Redondear(raton, edificioSeleccionado);

            if (Posicion.Limites(gridPosicion, 100) == true)
            {
                if (edificiosSeleccionados.Count > 0)
                {
                    if (edificiosSeleccionados[0].edificio.categoria == 1)
                    {
                        Carreteras.Colocar(edificiosSeleccionados, ciudad, colocar, diaNoche, edificios, sonidoBotonConstruir);
                    }
                }     
                else
                {
                    if (edificioSeleccionado != null)
                    {
                        edificioSeleccionado.rotacionColocacion = rotacionColocar;
                    }

                    if (accion == 0 && colocar.ComprobarConstruccionesPosicion(edificioSeleccionado, gridPosicion) == null)
                    {
                        if (ciudad.Dinero >= edificioSeleccionado.coste)
                        {
                            ciudad.DepositoDinero(-edificioSeleccionado.coste);
                            ciudad.ActualizarUI(false);
                            colocar.AñadirConstruccion(edificioSeleccionado, gridPosicion, diaNoche.EstadoEncendidoLuces());
                            sonidoBotonConstruir.Play();
                        }
                    }

                    if (edificioSeleccionado != null)
                    {
                        if (accion == 1)
                        {
                            Construccion edificioEliminar = edificioSeleccionado;

                            if (edificioEliminar.categoria != 0)
                            {
                                ciudad.DepositoDinero(edificioEliminar.coste / 3);
                            }

                            ciudad.ActualizarUI(false);
                            colocar.QuitarEdificio(edificioEliminar, gridPosicion);
                            sonidoBotonDemoler.Play();
                        }
                    }
                }               
            }
        }
    }

    void ColocarEdificioPrevio(bool mantener, int modo)
    {
        Vector3 raton = Posicion.Raton();

        if (raton != Vector3.zero)
        {
            Vector3 gridPosicion = Posicion.Redondear(raton, edificioSeleccionado);

            if (Posicion.Limites(gridPosicion, 100) == true)
            {
                colocarPrevio.QuitarTodosEdificios();

                if (mantener == true)
                {
                    if (modo == 1)
                    {
                        Carreteras.ColocarPrevio(edificiosSeleccionados, ciudad, edificioSeleccionado, gridPosicion, colocar, colocarPrevio, edificios, mantenerEjeX, mantenerEjeZ);
                    }       
                }
                else
                {
                    if (edificioSeleccionado != null)
                    {
                        edificioSeleccionado.rotacionColocacion = rotacionColocar;
                    }

                    if (colocar.ComprobarConstruccionesPosicion(edificioSeleccionado, gridPosicion) == null)
                    {
                        if (colocarPrevio.ComprobarConstruccionesPosicion(edificioSeleccionado, gridPosicion) == null)
                        {
                            colocarPrevio.AñadirConstruccion(edificioSeleccionado, gridPosicion);
                        }
                        else
                        {
                            colocarPrevio.QuitarEdificio(edificioSeleccionado, gridPosicion);
                            colocarPrevio.AñadirConstruccion(edificioSeleccionado, gridPosicion);
                        }
                    }
                }                
            }
        }
    }

    public void Demoler()
    {
        sonidoBoton.Play();

        construirActivar = false;
        ConstruirCambiarColor();
        ConstruirOcultarPanelEdificios();

        colocarPrevio.QuitarTodosEdificios();
        menuJuego.CerrarPanel();

        if (demolerActivar == true)
        {
            demolerActivar = false;
        }
        else
        {
            demolerActivar = true;
        }

        if (demolerActivar == true)
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f / 255f, 98f / 255f, 98f / 255f, 255f);
        }
        else
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }
    }

    public void DemolerRatonEntra()
    {
        if (demolerActivar == false)
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f / 255f, 98f / 255f, 98f / 255f, 255f);
        }
        else
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }
    }

    public void DemolerCambiarColor()
    {
        if (demolerActivar == true)
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f / 255f, 98f / 255f, 98f / 255f, 255f);
        }
        else
        {
            panelDemolerSub.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        }
    }

    void DemolerPrevio()
    {
        Vector3 raton = Posicion.Raton();

        if (raton != Vector3.zero)
        {
            Vector3 gridPosicion = Posicion.Redondear(raton, edificioSeleccionado);

            if (Posicion.Limites(gridPosicion, 100) == true)
            {
                edificioSeleccionado = colocar.ComprobarConstruccionesPosicion(null, gridPosicion);
                colocar.DemolerColorQuitar();

                if (edificioSeleccionado != null)
                {
                    colocar.DemolerColorRojo(edificioSeleccionado.id2);
                }
            }
        }
    }

    public Guardado DetectarPartidaGuardada()
    {
        if (File.Exists(Application.persistentDataPath + "/guardado.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fichero = File.Open(Application.persistentDataPath + "/guardado.save", FileMode.Open);
            Guardado guardado = (Guardado)bf.Deserialize(fichero);
            fichero.Close();

            if (guardado.edificiosID.Count > 0)
            {
                return guardado;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void CargarEdificios()
    {
        if (File.Exists(Application.persistentDataPath + "/guardado.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fichero = File.Open(Application.persistentDataPath + "/guardado.save", FileMode.Open);
            Guardado guardado = (Guardado)bf.Deserialize(fichero);
            fichero.Close();

            diaNoche.tiempoTotalDias = guardado.dia;
            diaNoche.tiempoDia = guardado.hora;
            diaNoche.ActualizarLuces();

            int i = 0;
            while (i < guardado.edificiosID.Count)
            {
                Construccion edificioGuardado = edificios[guardado.edificiosID[i]];
                edificioGuardado.rotacionColocacion = guardado.edificiosRotacion[i];

                Vector3 vector = new Vector3(guardado.edificiosX[i], 1, guardado.edificiosZ[i]);
                colocar.AñadirConstruccion(edificioGuardado, vector, diaNoche.encender);

                i++;
            }

            //camara.transform.position = new Vector3(guardado.camaraPosicionX, guardado.camaraPosicionY, guardado.camaraPosicionZ);
            //camara.transform.Rotate(new Vector3(guardado.camaraRotacionX, guardado.camaraRotacionY, guardado.camaraRotacionZ));

            ciudad.Dinero = guardado.dinero;
            ciudad.PoblacionActual = guardado.poblacionActual;
            ciudad.PoblacionTope = guardado.poblacionTope;
            ciudad.TrabajosActual = guardado.trabajosActual;
            ciudad.TrabajosTope = guardado.trabajosTope;
            ciudad.Comida = guardado.comida;
        }
    }

    public void GuardarPartida()
    {
        Guardado guardado = new Guardado();

        Construccion[,] edificiosGuardar = colocar.DevolverConstrucciones();

        for (int x = 0; x < edificiosGuardar.GetLength(0); x++)
        {
            for (int z = 0; z < edificiosGuardar.GetLength(1); z++)
            {
                if (edificiosGuardar[x,z] != null)
                {
                    if (edificiosGuardar[x, z].id != 99)
                    {
                        guardado.edificiosID.Add(edificiosGuardar[x, z].id);                       
                        guardado.edificiosRotacion.Add(edificiosGuardar[x,z].rotacionColocacion);
                        guardado.edificiosX.Add(ColocarFunciones.RotacionGuardadoX(x, edificiosGuardar[x, z]));
                        guardado.edificiosZ.Add(ColocarFunciones.RotacionGuardadoZ(z, edificiosGuardar[x, z]));
                    }                    
                }
            }
        }

        guardado.camaraPosicionX = camara.transform.position.x;
        guardado.camaraPosicionY = camara.transform.position.y;
        guardado.camaraPosicionZ = camara.transform.position.z;

        guardado.camaraRotacionX = camara.transform.rotation.x;
        guardado.camaraRotacionY = camara.transform.rotation.y;
        guardado.camaraRotacionZ = camara.transform.rotation.z;

        guardado.dia = (int)diaNoche.tiempoTotalDias;
        guardado.hora = diaNoche.tiempoDia;

        guardado.dinero = ciudad.Dinero;
        guardado.poblacionActual = ciudad.PoblacionActual;
        guardado.poblacionTope = ciudad.PoblacionTope;
        guardado.trabajosActual = ciudad.TrabajosActual;
        guardado.trabajosTope = ciudad.TrabajosTope;
        guardado.comida = ciudad.Comida;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fichero = File.Create(Application.persistentDataPath + "/guardado.save");
        bf.Serialize(fichero, guardado);
        fichero.Close();
    }

    public void InterfazOcultar()
    {
        if (canvas.gameObject.GetComponent<CanvasGroup>().alpha == 1)
        {
            canvas.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            canvas.gameObject.GetComponent<CanvasGroup>().interactable = false;
            canvas.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            canvas.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            canvas.gameObject.GetComponent<CanvasGroup>().interactable = true;
            canvas.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
