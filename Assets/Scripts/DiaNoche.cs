﻿using Construcciones;
using UnityEngine;
using UnityEngine.UI;

public class DiaNoche : MonoBehaviour {

    public Idiomas idioma;

    [HideInInspector]
    public KeyCode teclaArrancarPararTiempo;

    public Light sol;

    private float amanecer = 48000;
    private float atardecer = 120000;

    private float segundosDia = 0;
    private float segundosDiaTope = 172800;

    public float velocidad1 = 360;
    public float velocidad2 = 1440;

    public float arranqueDia = 90000;

    public float contadorDias = 1;
    private int contadorHoras = 0;

    [SerializeField]
    private Construir construir = null;

    [SerializeField]
    private Text dias = null;

    [SerializeField]
    private Text reloj = null;

    public int velocidad = 0;

    [SerializeField]
    private Ciudad ciudad = null;

    public bool encender;

    public Panel panelPausa;
    public Panel panelPlay1;
    public Panel panelPlay2;

    private float tiempoSemaforos = 0;
    private float tiempoSemaforosTope = 4000;

    private int accionSemaforos = 0;

    private float tiempoVehiculos = 0;
    private float tiempoVehiculosTope = 5000;

    private Vehiculos vehiculos;

    void Update()
    {
        ciudad.ActualizarUI(false);

        if (Input.GetKeyDown(teclaArrancarPararTiempo))
        {
            if (velocidad != 0)
            {
                velocidad = 0;
            }
            else
            {
                velocidad = 1;
            }
        }
       
        if (velocidad != 0)
        {
            if (velocidad == 1)
            {
                segundosDia = velocidad1;
            }
            else if (velocidad == 2)
            {
                segundosDia = velocidad2;
            }
   
            arranqueDia += (Time.deltaTime * segundosDia);

            if (arranqueDia > segundosDiaTope)
            {
                arranqueDia = 0;
                contadorDias += 1;
            }
         
            if (encender == true)
            {    
                if (arranqueDia > atardecer || arranqueDia < amanecer)
                {
                    construir.ComprobarLuces(encender);
                    encender = false;               
                }
            }
            else
            {
                if (arranqueDia <= atardecer && arranqueDia >= amanecer)
                {
                    construir.ComprobarLuces(encender);
                    encender = true;
                }
            }

            tiempoSemaforos += (Time.deltaTime * segundosDia);

            if (tiempoSemaforos > tiempoSemaforosTope)
            {
                tiempoSemaforos = 0;
                construir.CambiarLucesSemaforos(accionSemaforos);
                accionSemaforos += 1;

                if (accionSemaforos > 1)
                {
                    accionSemaforos = 0;
                }
            }

            tiempoVehiculos += (Time.deltaTime * segundosDia);

            if (tiempoVehiculos > tiempoVehiculosTope)
            {
                tiempoVehiculos = 0;

                if (vehiculos.listaVehiculos != null)
                {
                    if (vehiculos.listaVehiculos.Length > 0)
                    {
                        int cantidadEdificios = 0;

                        foreach (Construccion subedificio in construir.edificios)
                        {
                            if (subedificio != null)
                            {
                                if (subedificio.categoria == 2)
                                {
                                    cantidadEdificios += 1;
                                }
                            }
                        }

                        int i = 0;
                        while (i <= cantidadEdificios)
                        {
                            vehiculos.GenerarVehiculo();
                            i += 1;
                        }
                    }
                }
            }
        }

        if (idioma.CogerCadena("day") != null)
        {
            dias.text = string.Format(idioma.CogerCadena("day").ToLower() + " {0}", Mathf.Round(contadorDias));
        }       

        ActualizarReloj();
        ActualizarSol();
        vehiculos.ArrancarPararVehiculos(velocidad);
    }

    void ActualizarReloj()
    {
        float hora = Mathf.Round((arranqueDia * 24) / segundosDiaTope);
        float minutos = Mathf.Round((arranqueDia * 1440) / segundosDiaTope);
        minutos = minutos - (hora * 60) + 30;

        if (contadorHoras != (int)Mathf.Round(hora))
        {
            ciudad.ActualizarUI(true);
        }

        contadorHoras = (int)Mathf.Round(hora);

        string horaS = string.Format("{0}", Mathf.Round(hora));

        if (horaS == "24")
        {
            horaS = "0";        
        }

        string minutosS = string.Format("{0}",minutos);

        if (minutosS.Length == 1)
        {
            minutosS = "0" + minutosS;
        }

        reloj.text = string.Format("{0}:{1}",horaS,minutosS);
    }

    void ActualizarSol()
    {
        sol.transform.rotation = Quaternion.Euler(new Vector3((arranqueDia - (segundosDiaTope / 4)) / segundosDiaTope * 360, 0, 0));

        if (arranqueDia < (segundosDiaTope / 2))
        {
            sol.intensity = 1f - ((segundosDiaTope / 2) - arranqueDia) / (segundosDiaTope / 2);
        }
        else
        {
            sol.intensity = 1f - (((segundosDiaTope / 2) - arranqueDia) / (segundosDiaTope / 2) * - 0.8f);
        }
    }

    public void VelocidadMarchas(int nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;

        panelPausa.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        panelPlay1.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);
        panelPlay2.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 50f / 255f);

        panelPausa.volverColor = false;
        panelPlay1.volverColor = false;
        panelPlay2.volverColor = false;

        if (nuevaVelocidad == 0)
        {
            panelPausa.gameObject.GetComponent<Image>().color = new Color(0.08f, 0.4f, 0.58f);
            panelPausa.volverColor = true;
        }
        else if (nuevaVelocidad == 1)
        {
            panelPlay1.gameObject.GetComponent<Image>().color = new Color(0.08f, 0.4f, 0.58f);
            panelPlay1.volverColor = true;
        }
        else if (nuevaVelocidad == 2)
        {
            panelPlay2.gameObject.GetComponent<Image>().color = new Color(0.08f, 0.4f, 0.58f);
            panelPlay2.volverColor = true;
        }
    }

    public void ActualizarLuces()
    {
        if (arranqueDia > atardecer || arranqueDia < amanecer)
        {
            encender = true;
        }
        else
        {
            encender = false;
        }
    }

    public bool EstadoEncendidoLuces()
    {
        if (arranqueDia > atardecer || arranqueDia < amanecer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
