﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    public TextAsset ficheroIdiomas;
    public Idiomas idioma;

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

    private int rotacionColocar = -180;
    private int rotacionesPosicion = 0;

    private bool enseñarPrevio;
    private bool activarDemoler;

    public Button botonDemoler; 

    public Camera camara;

    public ArbolesInicio arbolesInicio;

    public Panel panelConstruir;
    public Panel panelDemoler;
    public Panel panelDatos;
    public Panel panelEdificios;
    public Panel panelGuardar;
    public Panel panelTiempo;

    public GameObject botonEdificiosPrefab;

    public Panel panelEdificiosCarreteras;
    public Button botonEdificiosCarreteras;
    public Sprite botonEdificiosCarreterasSprite1;
    public Sprite botonEdificiosCarreterasSprite2;

    public Panel panelEdificiosCasas;
    public Button botonEdificiosCasas;
    public Sprite botonEdificiosCasasSprite1;
    public Sprite botonEdificiosCasasSprite2;

    public Panel panelEdificiosComida;
    public Button botonEdificiosComida;
    public Sprite botonEdificiosComidaSprite1;
    public Sprite botonEdificiosComidaSprite2;

    public Panel panelEdificiosTiendas;
    public Button botonEdificiosTiendas;
    public Sprite botonEdificiosTiendasSprite1;
    public Sprite botonEdificiosTiendasSprite2;

    public Panel panelEdificiosIndustria;
    public Button botonEdificiosIndustria;
    public Sprite botonEdificiosIndustriaSprite1;
    public Sprite botonEdificiosIndustriaSprite2;

    public Panel panelEdificiosDecoracion;
    public Button botonEdificiosDecoracion;
    public Sprite botonEdificiosDecoracionSprite1;
    public Sprite botonEdificiosDecoracionSprite2;

    public EdificiosInfo panelEdificiosInfo;

    public Panel volverMenu;
    public Text volverMenuTexto;
    public Text volverMenuTextoSi;
    public Text volverMenuTextoNo;
    public Text volverMenuTextoCancelar;

    public Panel ayuda1;
    public Text ayuda1Texto;
    public Panel ayuda2;
    public Text ayuda2Texto;
    public Panel ayuda3;
    public Text ayuda3Texto;

    private void Start()
    {
        idioma.CargarIdioma(ficheroIdiomas, PlayerPrefs.GetString("idioma"));

        panelEdificiosInfo.Arranque();

        foreach (Construccion edificio in edificios)
        {
            GameObject botonObjeto = Instantiate(botonEdificiosPrefab);

            if (edificio.categoria == 0)
            {
                botonObjeto.transform.SetParent(panelEdificiosDecoracion.transform, false);
            }
            else if(edificio.categoria == 1)
            {
                botonObjeto.transform.SetParent(panelEdificiosCarreteras.transform, false);
            }
            else if (edificio.categoria == 2)
            {
                botonObjeto.transform.SetParent(panelEdificiosCasas.transform, false);
            }
            else if (edificio.categoria == 3)
            {
                botonObjeto.transform.SetParent(panelEdificiosComida.transform, false);
            }
            else if (edificio.categoria == 4)
            {
                botonObjeto.transform.SetParent(panelEdificiosTiendas.transform, false);
            }
            else if (edificio.categoria == 5)
            {
                botonObjeto.transform.SetParent(panelEdificiosIndustria.transform, false);
            }

            Image imagen = botonObjeto.GetComponent<Image>();
            imagen.sprite = edificio.botonImagen;

            Button boton = botonObjeto.GetComponent<Button>();
            boton.onClick.AddListener(() => SeleccionarEdificio(edificio.id));

            EventTrigger evento = botonObjeto.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };

            pointerEnter.callback.AddListener((data) => { panelEdificiosInfo.OnPointerEnter((PointerEventData)data, edificio); });
            evento.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };

            pointerExit.callback.AddListener((data) => { panelEdificiosInfo.OnPointerExit((PointerEventData)data); });
            evento.triggers.Add(pointerExit);
        }

        CargarPartida();

        volverMenuTexto.text = idioma.CogerCadena("exitQuestion");
        volverMenuTextoSi.text = idioma.CogerCadena("yes");
        volverMenuTextoNo.text = idioma.CogerCadena("no");
        volverMenuTextoCancelar.text = idioma.CogerCadena("cancel");     

        musicaFondo.loop = true;
        musicaFondo.Play();
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VolverMenu();
        }

        if (edificioSeleccionado != null)
        {
            if (enseñarPrevio == true)
            {
                int[] rotaciones = new int[4];

                rotaciones[0] = -180;
                rotaciones[1] = -270;
                rotaciones[2] = 0;
                rotaciones[3] = -90;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    rotacionesPosicion += 1;

                    if (rotacionesPosicion == 4)
                    {
                        rotacionesPosicion = 0;
                    }

                    rotacionColocar = rotaciones[rotacionesPosicion];
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    rotacionesPosicion -= 1;

                    if (rotacionesPosicion == -1)
                    {
                        rotacionesPosicion = 3;
                    }

                    rotacionColocar = rotaciones[rotacionesPosicion];
                }

                ColocarEdificioPrevio();
            }
           
            if (Input.GetMouseButtonDown(0))
            {
                ColocarEdificio(0);
            }
        }
                  
        if (activarDemoler == true)
        {
            DemolerPrevio();

            if (Input.GetMouseButtonDown(0))
            {
                ColocarEdificio(1);                
            }
        }       
    }

    public void Construir()
    {
        DemolerBoton(false);
        enseñarPrevio = false;
        colocarPrevio.QuitarTodosEdificios();

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

        sonidoBoton.Play();
       
        MostrarPanelEdificios(panelEdificiosCarreteras);
    }

    public void MostrarPanelEdificios(Panel panelVisible)
    {        
        panelEdificiosCarreteras.gameObject.SetActive(false);
        panelEdificiosCasas.gameObject.SetActive(false);
        panelEdificiosComida.gameObject.SetActive(false);
        panelEdificiosTiendas.gameObject.SetActive(false);
        panelEdificiosIndustria.gameObject.SetActive(false);
        panelEdificiosDecoracion.gameObject.SetActive(false);

        panelVisible.gameObject.SetActive(true);

        botonEdificiosCarreteras.GetComponent<Image>().sprite = botonEdificiosCarreterasSprite2;
        botonEdificiosCasas.GetComponent<Image>().sprite = botonEdificiosCasasSprite2;
        botonEdificiosComida.GetComponent<Image>().sprite = botonEdificiosComidaSprite2;
        botonEdificiosTiendas.GetComponent<Image>().sprite = botonEdificiosTiendasSprite2;
        botonEdificiosIndustria.GetComponent<Image>().sprite = botonEdificiosIndustriaSprite2;
        botonEdificiosDecoracion.GetComponent<Image>().sprite = botonEdificiosDecoracionSprite2;

        if (panelVisible.nombre == "carreteras")
        {
            botonEdificiosCarreteras.GetComponent<Image>().sprite = botonEdificiosCarreterasSprite1;
        }
        else if (panelVisible.nombre == "casas")
        {
            botonEdificiosCasas.GetComponent<Image>().sprite = botonEdificiosCasasSprite1;
        }
        else if (panelVisible.nombre == "comida")
        {
            botonEdificiosComida.GetComponent<Image>().sprite = botonEdificiosComidaSprite1;
        }
        else if(panelVisible.nombre == "tiendas")
        {
            botonEdificiosTiendas.GetComponent<Image>().sprite = botonEdificiosTiendasSprite1;
        }
        else if (panelVisible.nombre == "industria")
        {
            botonEdificiosIndustria.GetComponent<Image>().sprite = botonEdificiosIndustriaSprite1;
        }
        else if (panelVisible.nombre == "decoracion")
        {
            botonEdificiosDecoracion.GetComponent<Image>().sprite = botonEdificiosDecoracionSprite1;
        }        
    }

    public void SeleccionarEdificio(int edificio)
    {
        panelEdificios.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        panelEdificios.gameObject.GetComponent<CanvasGroup>().interactable = false;
        panelEdificios.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

        DemolerBoton(false);

        edificioSeleccionado = edificios[edificio];
        enseñarPrevio = true;
        ColocarEdificioPrevio();
        sonidoBoton.Play();
    }

    void ColocarEdificio(int accion)
    {     
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {            
            Vector3 gridPosicion = RedondearPosicion.Buscar(hit.point);

            if (((int)gridPosicion.x > 0) && ((int)gridPosicion.x < 100) && ((int)gridPosicion.z > 0) && ((int)gridPosicion.z < 100))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
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
                            colocar.AñadirConstruccion(edificioSeleccionado, gridPosicion);
                            sonidoBotonConstruir.Play();
                        }
                    }
                    else if (accion == 1 && colocar.ComprobarConstruccionesPosicion(edificioSeleccionado, gridPosicion) != null)
                    {
                        Construccion edificioEliminar = colocar.ComprobarConstruccionesPosicion(edificioSeleccionado, gridPosicion);

                        if (edificioEliminar.categoria != 0)
                        {
                            ciudad.DepositoDinero(edificioEliminar.coste / 3);
                        }
                        
                        ciudad.ActualizarUI(false);
                        colocar.QuitarEdificio(edificioEliminar, gridPosicion);
                        DemolerBoton(false);
                        sonidoBotonDemoler.Play();
                    }
                }
            }                 
        }
    }

    void ColocarEdificioPrevio()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 gridPosicion = RedondearPosicion.Buscar(hit.point);

                if (((int)gridPosicion.x > 0) && ((int)gridPosicion.x < 100) && ((int)gridPosicion.z > 0) && ((int)gridPosicion.z < 100))
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
        enseñarPrevio = false;
        colocarPrevio.QuitarTodosEdificios();

        DemolerBoton(true);
    }

    void DemolerPrevio()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 gridPosicion = RedondearPosicion.Buscar(hit.point);

            if (((int)gridPosicion.x > 0) && ((int)gridPosicion.x < 100) && ((int)gridPosicion.z > 0) && ((int)gridPosicion.z < 100))
            {
                colocar.LimpiarColorEdificios();

                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    edificioSeleccionado = colocar.ComprobarConstruccionesPosicion(null, gridPosicion);

                    if (edificioSeleccionado != null)
                    {
                        edificioSeleccionado.gameObject.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0, 0.1f);
                    }                    
                }
            }
        }
    }

    void DemolerBoton(bool estado)
    {
        activarDemoler = estado;

        ColorBlock color = botonDemoler.colors;

        if (estado == true)
        {           
            color.normalColor = new Color32(159, 0, 0, 255);        
        }
        else
        {
            color.normalColor = Color.white;
        }

        botonDemoler.colors = color;
    }

    public void CargarPartida()
    {
        if (File.Exists(Application.persistentDataPath + "/guardado.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fichero = File.Open(Application.persistentDataPath + "/guardado.save", FileMode.Open);
            Guardado guardado = (Guardado)bf.Deserialize(fichero);
            fichero.Close();

            int i = 0;
            while (i < guardado.edificiosID.Count)
            {
                Construccion edificioGuardado = edificios[guardado.edificiosID[i]];
                edificioGuardado.rotacionColocacion = guardado.edificiosRotacion[i];

                Vector3 vector = new Vector3(guardado.edificiosX[i], 1, guardado.edificiosZ[i]);
                colocar.AñadirConstruccion(edificioGuardado, vector);

                i++;
            }

            camara.transform.position = new Vector3(guardado.camaraX, guardado.camaraY, guardado.camaraZ);

            diaNoche.tiempoTotalDias = guardado.dia;
            diaNoche.tiempoDia = guardado.hora;
            diaNoche.ActualizarLuces();

            ciudad.Dinero = guardado.dinero;
            ciudad.PoblacionActual = guardado.poblacionActual;
            ciudad.PoblacionTope = guardado.poblacionTope;
            ciudad.TrabajosActual = guardado.trabajosActual;
            ciudad.TrabajosTope = guardado.trabajosTope;
            ciudad.Comida = guardado.comida;
        }
        else
        {
            arbolesInicio.Colocar(colocar);
        }

        if (PlayerPrefs.GetString("ayuda") == "true")
        {
            ayuda1.GetComponent<CanvasGroup>().alpha = 1;
            ayuda1.GetComponent<CanvasGroup>().interactable = true;
            ayuda1.GetComponent<CanvasGroup>().blocksRaycasts = true;
            ayuda1.gameObject.SetActive(true);

            ayuda1Texto.text = idioma.CogerCadena("help1");
            ayuda2Texto.text = idioma.CogerCadena("help2");
            ayuda3Texto.text = idioma.CogerCadena("help3");

            diaNoche.ArrancarParar();
        }
        else
        {
            ayuda1.gameObject.SetActive(false);
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
                        
                        if (edificiosGuardar[x, z].dimensiones.x == 2 && edificiosGuardar[x, z].dimensiones.y == 2)
                        {
                            if (edificiosGuardar[x, z].rotacionColocacion == -180)
                            {
                                guardado.edificiosX.Add(x);
                                guardado.edificiosZ.Add(z);
                            }
                            else if (edificiosGuardar[x, z].rotacionColocacion == -270)
                            {
                                guardado.edificiosX.Add(x + 1);
                                guardado.edificiosZ.Add(z);
                            }
                            else if (edificiosGuardar[x, z].rotacionColocacion == 0)
                            {
                                guardado.edificiosX.Add(x + 1);
                                guardado.edificiosZ.Add(z + 1);
                            }
                            else if (edificiosGuardar[x, z].rotacionColocacion == -90)
                            {
                                guardado.edificiosX.Add(x);
                                guardado.edificiosZ.Add(z + 1);
                            }
                        }
                        else
                        {
                            guardado.edificiosX.Add(x);
                            guardado.edificiosZ.Add(z);
                        }
                    }                    
                }
            }
        }

        guardado.camaraX = (int)camara.transform.position.x;
        guardado.camaraY = (int)camara.transform.position.y;
        guardado.camaraZ = (int)camara.transform.position.z;

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

    public void VolverMenu()
    {
        panelConstruir.gameObject.SetActive(false);
        panelDemoler.gameObject.SetActive(false);
        panelDatos.gameObject.SetActive(false);
        panelEdificios.gameObject.SetActive(false);
        panelGuardar.gameObject.SetActive(false);
        panelTiempo.gameObject.SetActive(false);

        ayuda1.gameObject.SetActive(false);
        ayuda2.gameObject.SetActive(false);
        ayuda3.gameObject.SetActive(false);

        volverMenu.GetComponent<CanvasGroup>().alpha = 1;
        volverMenu.GetComponent<CanvasGroup>().interactable = true;
        volverMenu.GetComponent<CanvasGroup>().blocksRaycasts = true;
        volverMenu.gameObject.SetActive(true);

        if (diaNoche.parar == false)
        {
            diaNoche.ArrancarParar();
        }

        enseñarPrevio = false;
        sonidoBoton.Play();
    }

    public void VolverMenuSi()
    {
        sonidoBoton.Play();
        GuardarPartida();
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void VolverMenuNo()
    {
        sonidoBoton.Play();
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void VolverMenuCancelar()
    {
        sonidoBoton.Play();

        panelConstruir.gameObject.SetActive(true);
        panelDemoler.gameObject.SetActive(true);
        panelDatos.gameObject.SetActive(true);
        panelEdificios.gameObject.SetActive(true);
        panelGuardar.gameObject.SetActive(true);
        panelTiempo.gameObject.SetActive(true);

        ayuda1.gameObject.SetActive(true);
        ayuda2.gameObject.SetActive(true);
        ayuda3.gameObject.SetActive(true);

        volverMenu.GetComponent<CanvasGroup>().alpha = 0;
        volverMenu.GetComponent<CanvasGroup>().interactable = false;
        volverMenu.GetComponent<CanvasGroup>().blocksRaycasts = false;
        volverMenu.gameObject.SetActive(false);

        if (diaNoche.parar == true)
        {
            diaNoche.ArrancarParar();
        }
    }

    public void CerrarAyuda1()
    {
        sonidoBoton.Play();

        ayuda1.GetComponent<CanvasGroup>().alpha = 0;
        ayuda1.GetComponent<CanvasGroup>().interactable = false;
        ayuda1.GetComponent<CanvasGroup>().blocksRaycasts = false;
        ayuda1.gameObject.SetActive(false);

        ayuda2.GetComponent<CanvasGroup>().alpha = 1;
        ayuda2.GetComponent<CanvasGroup>().interactable = true;
        ayuda2.GetComponent<CanvasGroup>().blocksRaycasts = true;
        ayuda2.gameObject.SetActive(true);
    }

    public void CerrarAyuda2()
    {
        sonidoBoton.Play();

        ayuda2.GetComponent<CanvasGroup>().alpha = 0;
        ayuda2.GetComponent<CanvasGroup>().interactable = false;
        ayuda2.GetComponent<CanvasGroup>().blocksRaycasts = false;
        ayuda2.gameObject.SetActive(false);

        ayuda3.GetComponent<CanvasGroup>().alpha = 1;
        ayuda3.GetComponent<CanvasGroup>().interactable = true;
        ayuda3.GetComponent<CanvasGroup>().blocksRaycasts = true;
        ayuda3.gameObject.SetActive(true);
    }

    public void CerrarAyuda3()
    {
        sonidoBoton.Play();

        ayuda3.GetComponent<CanvasGroup>().alpha = 0;
        ayuda3.GetComponent<CanvasGroup>().interactable = false;
        ayuda3.GetComponent<CanvasGroup>().blocksRaycasts = false;
        ayuda3.gameObject.SetActive(false);

        if (diaNoche.parar == true)
        {
            diaNoche.ArrancarParar();
        }
    }
}
