using UnityEngine;

public class GameManager : MonoBehaviour
{
    public QTableManager qTableManager; // Asigna el componente PongController desde el Editor

    private void OnApplicationQuit()
    {
        // Llama a la función SaveQTable() del PongController cuando la aplicación se cierra
        qTableManager.SaveQTable();
    }
}
