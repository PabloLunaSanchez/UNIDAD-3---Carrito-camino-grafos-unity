using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ManagerReadFile : MonoBehaviour
{
    public string[] contenido_archivo;    
    public int[,] graph;

    public int [,]M_distancias;
    public int [,]T_trayectorias;

    public int n;

    int i, j;

    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/Resources/GRAFO_NAVEGACION_PROYECTO.csv";                
        contenido_archivo = File.ReadAllLines(path);              
                
        n = contenido_archivo.Length;
        graph = new int[n,n];
               
        string []tempElementos;
        for (i = 0; i < n; i++)
        {                            
            tempElementos = contenido_archivo[i].Split(",");
            for (j = 0; j < n; j++)
            {
                int c = int.Parse(tempElementos[j]);
                if (c == 0 && i != j) c = 9999;
                graph[i,j] = c;
            }
        }
        //n = 11;  //only test


        T_trayectorias = new int[n,n];
        M_distancias = new int[n,n];
        

        for (i = 0; i < n; i++)
        {
            for (j = 0; j < n; j++)
            {
                M_distancias[i,j] = graph[i,j];

                // Si no hay camino ente i y j
                if (graph[i,j] == 9999)
                    T_trayectorias[i,j] = -1; //
                else
                    T_trayectorias[i,j] = j;

            }
        }

        //System.out.println("Inicia FloydWarshall:");

        floydWarshall();

    }


    public TextMeshProUGUI origen;
    public TextMeshProUGUI destino;
    public TextMeshProUGUI camino;

    public GameObject carrito;

    public List<string> newCamino;
    public bool iniciaRecorrido;

    int indexCamino;

    public void ejecutaAlgoritmo() {

        int org = (int)origen.text[0]  - 65; //.... ** Incompleto para AA...
        int dst = (int)destino.text[0] - 65; //.... ** Incompleto para AA...


        List<int> ruta = getPath(org, dst);
        newCamino = convierteToLetras(ruta);

        string c = printPath(newCamino);
        camino.text = c;

        GameObject temp = GameObject.Find(newCamino[0]);
        carrito.transform.position = temp.transform.position;

        iniciaRecorrido = true;
        indexCamino = 1;

    }


    void floydWarshall()
    {
        int i, j, k;

        for (k = 0; k < n; k++)
        { //nodos intermedios
            for (i = 0; i < n; i++)
            { //origen
                for (j = 0; j < n; j++)
                { //destino

                    if (M_distancias[i,k] == 9999 || M_distancias[k,j] == 9999)
                    {
                        continue; //continua si no existe un enlace entre origen y destino
                    }

                    //si k es un nodo intermedio que mejora la distancia entre i y j
                    if (M_distancias[i,k] + M_distancias[k,j] < M_distancias[i,j])
                    {
                        M_distancias[i,j] = M_distancias[i,k] + M_distancias[k,j];
                        T_trayectorias[i,j] = T_trayectorias[i,k];
                    }
                }
            }
        }
    }

    List<int> getPath(int u, int v)
    {
        //si no existe un path regresa null
        if (T_trayectorias[u,v] == -1)
            return null;

        // Inicio del camino
        List<Int32> path = new  List<int>();
        path.Add(u);

        while (u != v)
        {
            u = T_trayectorias[u,v];
            path.Add(u);
        }
        return path;
    }


    
    static string printPath(List<string> path)
    {
        string camino = "";
        int n = path.Count;
        for (int i = 0; i < n - 1; i++)
        {
            camino += path[i] + "->\n";
        }
        camino +=path[n - 1];
        return camino;
    }
    


    public static List<string> convierteToLetras(List<int> camino)
    {
        List<string> newCamino = new List<string>();

        for (int i = 0; i < camino.Count; i++)
        {
            int temp_i = camino[i] + 65;
            String letraInicio = (char)(temp_i > 90 ? 65 : temp_i) + "";
            letraInicio += temp_i <= 90 ? "" : (char)(temp_i % 91 + 65);

            newCamino.Add(letraInicio);
        }


        return newCamino;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (iniciaRecorrido)
        {
            if (indexCamino < newCamino.Count)
            {
                GameObject destino = GameObject.Find(newCamino[indexCamino]);
                
                carrito.transform.LookAt(destino.transform);

                carrito.transform.position = Vector3.MoveTowards(
                    carrito.transform.position, destino.transform.position, 20f * Time.deltaTime
                    );                
                if (Vector3.Distance(carrito.transform.position, destino.transform.position) < 0.05f)
                {
                    indexCamino++;
                }
            }
            else {
                iniciaRecorrido = false;
            }
        }
    }
}
