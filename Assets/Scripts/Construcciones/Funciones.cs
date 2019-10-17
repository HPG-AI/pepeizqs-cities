﻿using UnityEngine;

namespace Construcciones
{
    public static class Funciones
    {      
        public static Construccion[,] RellenarEdificioVacio(Construccion[,] edificios, Construccion edificio, Vector3 posicion, Construccion edificioVacio)
        {
            int topeX = TopeX(edificio);
            int i = SueloX(topeX, edificio);

            int topeZ = TopeZ(edificio);
            int j = SueloZ(topeZ, edificio);

            topeX = TopeX2(topeX, edificio);
            topeZ = TopeZ2(topeZ, edificio);

            int ib = i;
            int jb = j;

            if (topeX > 1)
            {
                while (i < topeX)
                {
                    if (i != 0)
                    {
                        if (edificios[(int)posicion.x + i, (int)posicion.z] == null)
                        {
                            edificios[(int)posicion.x + i, (int)posicion.z] = Object.Instantiate(edificioVacio);
                        }
                    }

                    i += 1;
                }
            }

            if (topeZ > 1)
            {
                while (j < topeZ)
                {
                    if (j != 0)
                    {
                        if (edificios[(int)posicion.x, (int)posicion.z + j] == null)
                        {
                            edificios[(int)posicion.x, (int)posicion.z + j] = Object.Instantiate(edificioVacio);
                        }
                    }

                    j += 1;
                }
            }

            if (topeX > 1 && topeZ > 1)
            {
                while (ib < topeX)
                {
                    int jc = jb;
                    while (jc < topeZ)
                    {
                        if ((ib != 0) && (jc != 0))
                        {
                            if (edificios[(int)posicion.x + ib, (int)posicion.z + jc] == null)
                            {
                                edificios[(int)posicion.x + ib, (int)posicion.z + jc] = Object.Instantiate(edificioVacio);
                            }
                        }

                        jc += 1;
                    }

                    ib += 1;
                }
            }

            return edificios;
        }

        public static Construccion ComprobarPosicion(Construccion[,] edificios, Construccion edificio, Vector3 posicion)
        {
            if (Posicion.Limites(posicion, 100) == true)
            {
                int topeX = 0;
                int i = 0;

                if (edificio != null)
                {
                    if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                    {
                        if ((int)edificio.dimensiones.y > 1)
                        {
                            if (edificio.dimensiones.y < 4)
                            {
                                topeX = (int)Mathf.Floor(edificio.dimensiones.y / 2);
                                i = 0 - ((int)Mathf.Ceil(edificio.dimensiones.y / 2));
                            }
                            else
                            {
                                topeX = (int)Mathf.Round(edificio.dimensiones.y / 2) - 1;
                                i = 0 - ((int)Mathf.Round(edificio.dimensiones.y / 2)) - 1;
                            }
                        }
                        else
                        {
                            topeX = (int)edificio.dimensiones.y;
                        }
                    }
                    else
                    {
                        if ((int)edificio.dimensiones.x > 1)
                        {
                            if (edificio.dimensiones.x < 4)
                            {
                                topeX = (int)Mathf.Floor(edificio.dimensiones.x / 2);
                                i = 0 - ((int)Mathf.Ceil(edificio.dimensiones.x / 2));
                            }
                            else
                            {
                                topeX = (int)Mathf.Round(edificio.dimensiones.x / 2) - 1;
                                i = 0 - ((int)Mathf.Round(edificio.dimensiones.x / 2)) - 1;
                            }
                        }
                        else
                        {
                            topeX = (int)edificio.dimensiones.x;
                        }
                    }
                }

                int topeZ = 0;
                int j = 0;

                if (edificio != null)
                {
                    if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                    {
                        if ((int)edificio.dimensiones.x > 1)
                        {
                            if (edificio.dimensiones.x < 4)
                            {
                                topeZ = (int)Mathf.Floor(edificio.dimensiones.x / 2);
                                j = 0 - ((int)Mathf.Ceil(edificio.dimensiones.x / 2));
                            }
                            else
                            {
                                topeZ = (int)Mathf.Round(edificio.dimensiones.x / 2) - 1;
                                j = 0 - ((int)Mathf.Round(edificio.dimensiones.x / 2)) - 1;
                            }
                        }
                        else
                        {
                            topeZ = (int)edificio.dimensiones.x;
                        }
                    }
                    else
                    {
                        if ((int)edificio.dimensiones.y > 1)
                        {
                            if (edificio.dimensiones.y < 4)
                            {
                                topeZ = (int)Mathf.Floor(edificio.dimensiones.y / 2);
                                j = 0 - ((int)Mathf.Ceil(edificio.dimensiones.y / 2));
                            }
                            else
                            {
                                topeZ = (int)Mathf.Round(edificio.dimensiones.y / 2) - 1;
                                j = 0 - ((int)Mathf.Round(edificio.dimensiones.y / 2)) - 1;
                            }
                        }
                        else
                        {
                            topeZ = (int)edificio.dimensiones.y;
                        }
                    }
                }

                //for (int x = 0; x < edificios.GetLength(0); x++)
                //{
                //    for (int z = 0; z < edificios.GetLength(1); z++)
                //    {
                //        if (edificios[x, z] != null)
                //        {
                //            if (posicion.x == x && posicion.z == z)
                //            {
                //                return edificios[x, z];
                //            }                           
                //        }
                //    }
                //}

                if (edificios[(int)posicion.x, (int)posicion.z] != null)
                {
                    return edificios[(int)posicion.x, (int)posicion.z];
                }

                int ib = i;
                int jb = j;

                while (i < topeX)
                {
                    if (edificios[(int)posicion.x + i, (int)posicion.z] != null)
                    {
                        return edificios[(int)posicion.x + i, (int)posicion.z];
                    }

                    i += 1;
                }

                while (j < topeZ)
                {
                    if (edificios[(int)posicion.x, (int)posicion.z + j] != null)
                    {
                        return edificios[(int)posicion.x, (int)posicion.z + j];
                    }

                    j += 1;
                }

                while (ib < topeX)
                {
                    int jc = jb;
                    while (jc < topeZ)
                    {
                        if (edificios[(int)posicion.x + ib, (int)posicion.z + jc] != null)
                        {
                            return edificios[(int)posicion.x + ib, (int)posicion.z + jc];
                        }

                        jc += 1;
                    }

                    ib += 1;
                }
            }

            return null;
        }

        public static Construccion[,] QuitarEdificios(Construccion[,] edificios, Construccion edificio, Vector3 posicion)
        {
            if (edificio != null)
            {
                int id2 = edificio.id2;

                foreach (Construccion subedificio in edificios)
                {
                    if (subedificio != null)
                    {
                        if (id2 == subedificio.id2)
                        {
                            int x = subedificio.posicionX;
                            int z = subedificio.posicionZ;

                            Object.DestroyImmediate(subedificio.gameObject, true);
                            edificios[x, z] = null;
                        }
                    }
                }
            }

            return edificios;
        }

        //------------------------------------

        private static int SueloX(int topeX, Construccion edificio)
        {
            int sueloX = 0;

            if (topeX == 3)
            {
                sueloX = -1;
            }

            if (topeX == 4)
            {
                if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                {
                    sueloX = -2;
                }
                else
                {
                    sueloX = -1;
                }
            }

            return sueloX;
        }

        private static int TopeX(Construccion edificio)
        {
            int topeX = 0;

            if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
            {
                topeX = (int)edificio.dimensiones.y;
            }
            else
            {
                topeX = (int)edificio.dimensiones.x;
            }

            return topeX;
        }

        private static int TopeX2(int topeX, Construccion edificio)
        {
            if (topeX == 3)
            {
                topeX = 2;
            }

            if (topeX == 4)
            {
                if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                {
                    topeX = 2;
                }
                else
                {
                    topeX = 3;
                }
            }

            return topeX;
        }

        private static int SueloZ(int topeZ, Construccion edificio)
        {
            int sueloZ = 0;

            if (topeZ == 3)
            {
                sueloZ = -1;
            }

            if (topeZ == 4)
            {
                if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                {
                    sueloZ = -1;
                }
                else
                {
                    sueloZ = -2;
                }
            }

            return sueloZ;
        }

        private static int TopeZ(Construccion edificio)
        {
            int topeZ = 0;

            if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
            {
                topeZ = (int)edificio.dimensiones.x;
            }
            else
            {
                topeZ = (int)edificio.dimensiones.y;
            }

            return topeZ;
        }

        private static int TopeZ2(int topeZ, Construccion edificio)
        {
            if (topeZ == 3)
            {
                topeZ = 2;
            }

            if (topeZ == 4)
            {
                if ((edificio.rotacionColocacion == -270) || (edificio.rotacionColocacion == -90))
                {
                    topeZ = 3;
                }
                else
                {
                    topeZ = 2;
                }
            }

            return topeZ;
        }

        public static int RotacionGuardadoX(int x, Construccion edificio)
        {
            if (edificio.dimensiones.x == 2 && edificio.dimensiones.y == 1)
            {
                if (edificio.rotacionColocacion == -180 || edificio.rotacionColocacion == 0)
                {
                    x = x + 1;
                }
            }
            else if (edificio.dimensiones.x == 2 && edificio.dimensiones.y == 2)
            {
                x = x + 1;
            }
            else if (edificio.dimensiones.x == 4 && edificio.dimensiones.y == 3)
            {
                if (edificio.rotacionColocacion == -180 || edificio.rotacionColocacion == 0)
                {
                    x = x + 2;
                }
                else
                {
                    x = x + 1;
                }
            }

            return x;
        }

        public static int RotacionGuardadoZ(int z, Construccion edificio)
        {
            if (edificio.dimensiones.x == 2 && edificio.dimensiones.y == 1)
            {
                if (edificio.rotacionColocacion == -270 || edificio.rotacionColocacion == -90)
                {
                    z = z + 1;
                }
            }
            else if (edificio.dimensiones.x == 2 && edificio.dimensiones.y == 2)
            {
                z = z + 1;
            }
            else if (edificio.dimensiones.x == 4 && edificio.dimensiones.y == 3)
            {
                if (edificio.rotacionColocacion == -180 || edificio.rotacionColocacion == 0)
                {
                    z = z + 1;
                }
                else
                {
                    z = z + 2;
                }
            }

            return z;
        }
    }
}


