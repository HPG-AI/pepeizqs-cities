﻿using System.Collections.Generic;
using UnityEngine;

public class Colocar : MonoBehaviour
{
    private int contadorIdsConstrucciones;

    [HideInInspector]
    public Construccion[,] edificios = new Construccion[100, 100];

    public Construccion edificioVacio;

    public Vehiculo[] vehiculos;

    [HideInInspector]
    public List<Vehiculo> vehiculosGenerados = new List<Vehiculo>();

    private int contadorIdsVehiculos;

    private void Start()
    {
        contadorIdsConstrucciones = 0;
        contadorIdsVehiculos = 0;      
    }

    public void AñadirConstruccion(Construccion edificio, Vector3 posicion, bool encender)
    {
        edificio.GetComponent<Renderer>().receiveShadows = true;
        edificio.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        posicion = ColocarFunciones.PosicionEdificio(edificio, posicion, edificio.rotacionColocacion);
        ComprobarLucesEdificio(encender, edificio);

        Construccion edificioFinal = Instantiate(edificio, posicion, Quaternion.identity);
        edificioFinal.transform.Rotate(Vector3.up, edificio.rotacionAdicional + edificio.rotacionColocacion, Space.World);
        edificioFinal.posicionX = (int)posicion.x;
        edificioFinal.posicionZ = (int)posicion.z;
        edificioFinal.id2 = contadorIdsConstrucciones;
        contadorIdsConstrucciones += 1;

        edificioVacio.id2 = edificioFinal.id2;

        edificios[(int)posicion.x, (int)posicion.z] = edificioFinal;
        edificios = ColocarFunciones.RellenarEdificioVacio(edificios, edificio, posicion, edificioVacio);
    }

    public Construccion ComprobarConstruccionesPosicion(Construccion edificio, Vector3 posicion)
    {
        return ColocarFunciones.ComprobarPosicion(edificios, edificio, posicion);
    }

    public void QuitarEdificio(Construccion edificio, Vector3 posicion)
    {
        edificios = ColocarFunciones.QuitarEdificios(edificios, edificio, posicion);
    }

    public void QuitarTodosEdificios()
    {
        for (int x = 0; x < edificios.GetLength(0); x++)
        {
            for (int z = 0; z < edificios.GetLength(1); z++)
            {
                if (edificios[x, z] != null)
                {
                    Object.Destroy(edificios[x, z].gameObject);
                    edificios[x, z] = null;
                }
            }
        }
    }

    public Construccion[,] DevolverConstrucciones()
    {
        return edificios;
    }

    public void DemolerColorRojo(int id2)
    {
        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                if (subedificio.id != 99)
                {
                    if (subedificio.id2 == id2)
                    {
                        subedificio.gameObject.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 98f / 255f, 98f / 255f);
                    }
                }
            }
        }
    }

    public void DemolerColorQuitar()
    {
        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                subedificio.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public void ComprobarLuces(bool encender)
    {
        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                ComprobarLucesEdificio(encender, subedificio);
            }
        }
    }

    public void ComprobarLucesEdificio(bool encender, Construccion edificio)
    {
        bool modificar = false;
        Material material = edificio.gameObject.GetComponent<Renderer>().sharedMaterial;

        if (encender == true)
        {
            if (edificio.categoria == 1)
            {
                bool lucesCarretera = true;

                if (edificio.categoria == 1)
                {
                    if (edificio.id == 10)
                    {
                        lucesCarretera = false;
                    }
                    else if (edificio.id == 11)
                    {
                        lucesCarretera = false;
                    }
                }

                if (lucesCarretera == true)
                {
                    Light[] luces = edificio.GetComponentsInChildren<Light>();

                    foreach (Light luz in luces)
                    {
                        luz.intensity = edificio.luzIntesidad;
                        luz.range = edificio.luzRango;
                    }
                }
            }

            if (edificio.categoria == 2)
            {
                modificar = true;
            }
            else if (edificio.categoria == 4)
            {
                modificar = true;
            }
            else if (edificio.categoria == 5)
            {
                modificar = true;
            }

            if (modificar == true)
            {              
                material.EnableKeyword("_EMISSION");
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
        }
        else
        {
            if (edificio.categoria == 1)
            {
                bool lucesCarretera = true;

                if (edificio.categoria == 1)
                {
                    if (edificio.id == 10)
                    {
                        lucesCarretera = false;
                    }
                    else if (edificio.id == 11)
                    {
                        lucesCarretera = false;
                    }
                }

                if (lucesCarretera == true)
                {
                    Light[] luces = edificio.GetComponentsInChildren<Light>();

                    foreach (Light luz in luces)
                    {
                        luz.intensity = 0;
                        luz.range = 0;
                    }
                }
            }

            if (edificio.categoria == 2)
            {
                modificar = true;
            }
            else if (edificio.categoria == 4)
            {
                modificar = true;
            }
            else if (edificio.categoria == 5)
            {
                modificar = true;
            }

            if (modificar == true)
            {
                material.DisableKeyword("_EMISSION");
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }

    public void CambiarLucesSemaforos(int accion)
    {
        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                bool lucesCarretera = false;

                if (subedificio.categoria == 1)
                {
                    if (subedificio.id == 10)
                    {
                        lucesCarretera = true;
                    }
                    else if (subedificio.id == 11)
                    {
                        lucesCarretera = true;
                    }
                }
             
                if (lucesCarretera == true)
                {
                    Light[] luces = subedificio.GetComponentsInChildren<Light>();

                    foreach (Light luz in luces)
                    {
                        if (accion == 0)
                        {
                            bool activarLuz = false;

                            if (luz.name.ToLower().Contains("luzrojo1"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzrojo3"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzverde2"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzverde4"))
                            {
                                activarLuz = true;
                            }

                            if (activarLuz == true)
                            {
                                luz.intensity = 1.0f;
                                luz.range = 0.3f;
                            }
                            else
                            {
                                luz.intensity = 0f;
                                luz.range = 0f;
                            }
                        }
                        else if (accion == 1)
                        {
                            bool activarLuz = false;

                            if (luz.name.ToLower().Contains("luzrojo2"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzrojo4"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzverde1"))
                            {
                                activarLuz = true;
                            }
                            else if (luz.name.ToLower().Contains("luzverde3"))
                            {
                                activarLuz = true;
                            }

                            if (activarLuz == true)
                            {
                                luz.intensity = 1.0f;
                                luz.range = 0.3f;
                            }
                            else
                            {
                                luz.intensity = 0f;
                                luz.range = 0f;
                            }
                        }
                        else
                        {
                            luz.intensity = 0f;
                            luz.range = 0f;
                        }
                    }
                }
            }
        }
    }

    public void GenerarVehiculos()
    {
        int cantidadEdificios = 0;

        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                if (subedificio.categoria == 2)
                {
                    cantidadEdificios += 1;
                }
            }
        }

        cantidadEdificios = cantidadEdificios / 4;

        int i = 0;
        while (i < cantidadEdificios)
        {
            GenerarVehiculo();
            i += 1;
        }   
    }

    public void GenerarVehiculo()
    {
        List<Construccion> carreteras = new List<Construccion>();

        foreach (Construccion subedificio in edificios)
        {
            if (subedificio != null)
            {
                if (subedificio.categoria == 1)
                {
                    if (subedificio.id == 6 || subedificio.id == 12)
                    {
                        carreteras.Add(subedificio);
                    }
                }
            }
        }

        if (carreteras.Count > 0)
        {
            int azarCarretera = Random.Range(0, carreteras.Count);

            int rotacionAzar = Random.Range(0, 2);
            string direccion = null;
            Vector3 posicion = carreteras[azarCarretera].gameObject.transform.position;
            posicion.y = 0.51f;

            if (carreteras[azarCarretera].rotacionColocacion == -270 || carreteras[azarCarretera].rotacionColocacion == -90)
            {
                if (rotacionAzar == 0)
                {
                    posicion.z = posicion.z - 0.15f;
                    direccion = "x+";
                }
                else if (rotacionAzar == 1)
                {
                    posicion.z = posicion.z + 0.15f;
                    direccion = "x-";
                }
            }
            else if (carreteras[azarCarretera].rotacionColocacion == -180 || carreteras[azarCarretera].rotacionColocacion == 0)
            {
                if (rotacionAzar == 0)
                {
                    posicion.x = posicion.x + 0.15f;
                    direccion = "z+";
                }
                else if (rotacionAzar == 1)
                {
                    posicion.x = posicion.x - 0.15f;
                    direccion = "z-";
                }
            }

            int vehiculoAzar = Random.Range(0, vehiculos.Length);

            Vehiculo vehiculo = Instantiate(vehiculos[vehiculoAzar], posicion, Quaternion.identity);
            vehiculo.edificios = edificios;
            vehiculo.direccion = direccion;
            vehiculo.id2 = contadorIdsVehiculos;

            contadorIdsVehiculos += 1;

            if (rotacionAzar == 0)
            {
                vehiculo.transform.Rotate(Vector3.up, carreteras[azarCarretera].rotacionColocacion, Space.World);
            }
            else if (rotacionAzar == 1)
            {
                vehiculo.transform.Rotate(Vector3.up, carreteras[azarCarretera].rotacionColocacion - 180, Space.World);
            }

            vehiculosGenerados.Add(vehiculo);
        }        
    }
}