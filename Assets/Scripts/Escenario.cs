﻿using Construcciones;
using System.Collections.Generic;
using UnityEngine;

public class Escenario : MonoBehaviour {

    public Partidas partidas;

    public Terreno tierra;
    public Terreno rio;

    [SerializeField]
    public Terreno[] terrenos;

    [HideInInspector]
    public Terreno[,] terrenos2 = new Terreno[100,100];

    public Construccion[] arboles;

    public void PonerTerreno(Guardado partida)
    {
        for (int x = 0; x < terrenos2.GetLength(0); x++)
        {
            for (int z = 0; z < terrenos2.GetLength(1); z++)
            {
                if (terrenos2[x, z] != null)
                {
                    Object.Destroy(terrenos2[x, z].gameObject);
                    terrenos2[x, z] = null;
                }
            }
        }

        if (partida != null)
        {
            if (partida.terrenosID.Count > 0)
            {
                int i = 0;
                while (i < partida.terrenosID.Count)
                {
                    Terreno terreno = terrenos[partida.terrenosID[i]];                  
                    Vector3 posicion = new Vector3(partida.terrenosX[i], -0.5f, partida.terrenosZ[i]);
                    GenerarTerreno(terreno, posicion);

                    i += 1;
                }
            }
        }
        else
        {
            if (rio != null)
            {
                int origen = Random.Range(40, 60);
                int primerTramo = Random.Range(20, 30);

                int i = 0;
                while (i < primerTramo)
                {
                    Vector3 posicion = new Vector3(origen, -0.5f, i);
                    GenerarTerreno(rio, posicion);

                    i += 1;
                }

                int positivoNegativo = Random.Range(0, 2);
                int segundoTramo = 0;

                if (positivoNegativo == 0)
                {
                    segundoTramo = Random.Range(-10, -20);
                    i = 0;
                    while (i > segundoTramo)
                    {
                        Vector3 posicion = new Vector3(origen + i, -0.5f, primerTramo);
                        GenerarTerreno(rio, posicion);

                        i -= 1;
                    }
                }
                else
                {
                    segundoTramo = Random.Range(10, 20);
                    i = 0;
                    while (i < segundoTramo)
                    {
                        Vector3 posicion = new Vector3(origen + i, -0.5f, primerTramo);
                        GenerarTerreno(rio, posicion);

                        i += 1;
                    }
                }

                int tercerTramo = Random.Range(30, 40);

                i = 0;
                while (i < tercerTramo)
                {
                    Vector3 posicion = new Vector3(origen + segundoTramo, -0.5f, primerTramo + i);
                    GenerarTerreno(rio, posicion);

                    i += 1;
                }

                positivoNegativo = Random.Range(0, 2);
                int cuartoTramo = 0;

                if (positivoNegativo == 0)
                {
                    cuartoTramo = Random.Range(-10, -20);
                    i = 0;
                    while (i > cuartoTramo)
                    {
                        Vector3 posicion = new Vector3(origen + segundoTramo + i, -0.5f, primerTramo + tercerTramo);
                        GenerarTerreno(rio, posicion);

                        i -= 1;
                    }
                }
                else
                {
                    cuartoTramo = Random.Range(10, 20);
                    i = 0;
                    while (i < cuartoTramo)
                    {
                        Vector3 posicion = new Vector3(origen + segundoTramo + i, -0.5f, primerTramo + tercerTramo);
                        GenerarTerreno(rio, posicion);

                        i += 1;
                    }
                }

                i = 0;
                while ((primerTramo + tercerTramo + i) < 100)
                {
                    Vector3 posicion = new Vector3(origen + segundoTramo + cuartoTramo, -0.5f, primerTramo + tercerTramo + i);
                    GenerarTerreno(rio, posicion);

                    i += 1;
                }
            }

            if (tierra != null)
            {
                int i = 0;
                while (i < 100)
                {
                    int j = 0;
                    while (j < 100)
                    {
                        Vector3 posicion = new Vector3(i, -0.5f, j);
                        GenerarTerreno(tierra, posicion);

                        j += 1;
                    }
                    i += 1;
                }
            }
        }     
    }

    private void GenerarTerreno(Terreno terreno, Vector3 posicion)
    {
        if (terrenos2[(int)posicion.x, (int)posicion.z] == null)
        {
            Terreno terreno2 = Instantiate(terreno, posicion, Quaternion.identity);
            terrenos2[(int)posicion.x, (int)posicion.z] = terreno2;
        }
    }

    public bool ComprobarEdificable(Construccion edificio, Vector3 posicion)
    {
        bool edificable = true;

        for (int x = 0; x < terrenos2.GetLength(0); x++)
        {
            for (int z = 0; z < terrenos2.GetLength(1); z++)
            {
                int i = (int)posicion.x;
                while (i < (int)posicion.x + edificio.dimensiones.x)
                {
                    int j = (int)posicion.z;
                    while (j < (int)posicion.z + edificio.dimensiones.y)
                    {
                        if (i == x && j == z)
                        {
                            if (terrenos2[x, z] != null)
                            {
                                Terreno terreno = terrenos2[x, z];

                                if (terreno.edificable == false)
                                {
                                    edificable = false;
                                }
                            }
                        }
                        j += 1;
                    }
                    i += 1;
                }              
            }
        }

        return edificable;
    }

    public void PonerArboles(Guardado partida, Construir construir)
    {
        if (partida != null)
        {
            if (partida.edificiosID.Count > 0)
            {
                int i = 0;
                while (i < partida.edificiosID.Count)
                {
                    Vector3 posicion = new Vector3(partida.edificiosX[i], 1, partida.edificiosZ[i]);

                    Construccion arbol = null;

                    foreach (Construccion arbol2 in arboles)
                    {
                        if (arbol2.id == partida.edificiosID[i])
                        {
                            arbol = arbol2;
                        }
                    }

                    if (arbol != null)
                    {
                        bool añadir = true;

                        if (ComprobarEdificable(arbol, posicion) == false)
                        {
                            añadir = false;
                        }

                        if (construir.ComprobarPosicion(arbol, posicion) != null)
                        {
                            añadir = false;
                        }

                        if (añadir == true)
                        {
                            construir.AñadirConstruccion(arbol, posicion, false);
                        }
                    }

                    i += 1;
                }
            }
        }
        else
        {
            if (arboles != null)
            {
                if (arboles.Length > 0)
                {
                    int arbolesColocar = 200;

                    int i = 0;
                    while (i < arbolesColocar)
                    {
                        Vector3 posicion = new Vector3(Random.Range(1, 99), 1, Random.Range(1, 99));
                        int j = Random.Range(0, arboles.Length);

                        if (arboles[j] != null)
                        {
                            bool añadir = true;

                            if (ComprobarEdificable(arboles[j], posicion) == false)
                            {
                                añadir = false;
                            }

                            if (construir.ComprobarPosicion(arboles[j], posicion) != null)
                            {
                                añadir = false;
                            }

                            if (añadir == true)
                            {
                                construir.AñadirConstruccion(arboles[j], posicion, false);
                            }
                        }
                        i++;
                    }
                }
            }
        }       
    }

    public Terreno[,] DevolverTerrenos()
    {
        return terrenos2;
    }
}