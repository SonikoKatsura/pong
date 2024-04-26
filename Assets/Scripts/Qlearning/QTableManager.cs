using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QTableManager : MonoBehaviour
{
    private Dictionary<string, float[]> qTable; // Tabla Q: (estado, acciones)
    private string csvFilePath = "Assets/QTable.csv"; // Ruta del archivo CSV
    public float gamma = 4;
    public float alpha = 6;
    public int actionSpaceSize = 10;
    void Start()
    {
        // Inicializar la tabla Q
        qTable = new Dictionary<string, float[]>();

        // Si el archivo CSV existe, cargar la tabla Q desde él
        if (File.Exists(csvFilePath))
        {
            LoadQTable();
        }
        else
        {
            CreateCSVFile();
        }
    }

    private void CreateCSVFile()
    {
        StreamWriter writer = new StreamWriter(csvFilePath);

        // Escribir encabezados
        writer.WriteLine("State,Action1,Action2,Action3");

        // Escribir datos iniciales si es necesario
        // Por ejemplo:
        writer.WriteLine("0.0,0.0,0.0,0.0"); // Para un estado inicial con valores Q inicializados en 0

        writer.Close();
    }
    // Guardar la tabla Q en el archivo CSV
    public void SaveQTable()
    {
        StreamWriter writer = new StreamWriter(csvFilePath);

        foreach (KeyValuePair<string, float[]> entry in qTable)
        {
            string state = entry.Key;
            float[] actions = entry.Value;

            // Escribir cada fila en el formato "estado,acción1,acción2,..."
            writer.WriteLine(state + "," + string.Join(",", actions));
        }

        writer.Close();
    }

    // Cargar la tabla Q desde el archivo CSV
    private void LoadQTable()
    {
        StreamReader reader = new StreamReader(csvFilePath);

        // Leer la primera línea (encabezados) sin procesarla
        string headersLine = reader.ReadLine();

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] parts = line.Split(',');

            string state = parts[0];
            float[] actions = new float[parts.Length - 1];

            for (int i = 1; i < parts.Length; i++)
            {
                string valueString = parts[i].Trim(); // Eliminar espacios en blanco alrededor del valor
                if (float.TryParse(valueString, out float actionValue))
                {
                    actions[i - 1] = actionValue;
                }
                else
                {
                    // Manejar el caso de conversión fallida
                    Debug.LogError($"Error al convertir '{valueString}' en un número flotante en la línea '{line}'");
                    // Asignar un valor predeterminado o manejar el error de otra manera según sea necesario
                    actions[i - 1] = 0f; // Valor predeterminado
                }
            }

            qTable[state] = actions;
        }

        reader.Close();
    }



    // Obtener el valor Q para un estado y una acción específicos
    public float GetQValue(string state, int action)
    {
        if (qTable.ContainsKey(state))
        {
            return qTable[state][action];
        }
        else
        {
            return 0f; // Valor Q predeterminado si el estado no está en la tabla
        }
    }

    // Actualizar el valor Q para un estado y una acción específicos
    public void UpdateQValue(string state, int action, float newValue)
    {
        if (!qTable.ContainsKey(state))
        {
            // Crear una nueva entrada en la tabla Q si el estado no existe
            qTable[state] = new float[actionSpaceSize]; // actionSpaceSize es el número de acciones posibles
        }

        // Actualizar el valor Q para la acción específica en el estado dado
        qTable[state][action] = newValue;
    }

}
