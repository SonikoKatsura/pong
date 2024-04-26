using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongController : MonoBehaviour
{
    public QTableManager qTableManager;
    public GameObject ball;
    public GameObject playerPaddle;
    public GameObject opponentPaddle;

    // Parámetros del juego
    private float ballSpeed = 5f;
    private float paddleSpeed = 5f;
    private float bounceAngleHalfRange = 60f; // Ángulo máximo de rebote
    private float rewardHit = 1f; // Recompensa por golpear la pelota
    private float rewardMiss = -1f; // Recompensa por perder la pelota

    // Parámetro de exploración vs. explotación
    private float epsilon = 0.1f; // Valor inicial de epsilon

    // Estados
    private Vector2 ballStartPosition;
    private Vector2 playerPaddleStartPosition;
    private Vector2 opponentPaddleStartPosition;
    private string currentState;
    private string nextState;

    void Start()
    {
        ballStartPosition = ball.transform.position;
        playerPaddleStartPosition = playerPaddle.transform.position;
        opponentPaddleStartPosition = opponentPaddle.transform.position;
        ResetGame(); // Mover la pelota al iniciar el juego
    }

    void Update()
    {
        // Control del jugador
        float verticalInput = Input.GetAxisRaw("Vertical");
        MovePlayerPaddle(verticalInput);

        // Control del oponente (controlado por IA)
        MoveOpponentPaddle();

        // Movimiento de la pelota
        Vector2 ballVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        ballVelocity.Normalize(); // Normalizar la velocidad para mantener una velocidad constante
        ball.GetComponent<Rigidbody2D>().velocity = ballVelocity * ballSpeed;

        // Actualizar el estado actual y el siguiente
        currentState = GetState();
        nextState = GetNextState();

        // Controlar si la pelota se sale de la pantalla
        Vector2 ballPosition = ball.transform.position;
        if (ballPosition.x < -11f || ballPosition.x > 10f || ballPosition.y < -7.5f || ballPosition.y > 7.5f)
        {
            ResetGame();
        }
        // Actualizar la tabla Q
        UpdateQTable();
    }

    // Función para reiniciar el juego
    private void ResetGame()
    {
        ball.transform.position = ballStartPosition;
        playerPaddle.transform.position = playerPaddleStartPosition;
        opponentPaddle.transform.position = opponentPaddleStartPosition;
        // Mover la pelota
        MoveBall();
        // Reiniciar otros estados si es necesario
    }

    // Función para mover la pelota
    private void MoveBall()
    {
        // Ajustar la velocidad y dirección de la pelota según tus necesidades
        Vector2 direction = new Vector2(1f, 0.5f).normalized; // Ejemplo de dirección (derecha y arriba)
        ball.GetComponent<Rigidbody2D>().velocity = direction * ballSpeed;
    }

    // Función para mover la paleta del jugador
    private void MovePlayerPaddle(float direction)
    {
        float moveAmount = direction * paddleSpeed * Time.deltaTime;
        playerPaddle.transform.Translate(0f, moveAmount, 0f);
    }

    // Función para mover la paleta del oponente (controlado por IA)
    private void MoveOpponentPaddle()
    {
        // Mover la paleta del oponente hacia la posición Y de la pelota
        Vector2 targetPosition = new Vector2(opponentPaddle.transform.position.x, ball.transform.position.y);
        opponentPaddle.transform.position = Vector2.MoveTowards(opponentPaddle.transform.position, targetPosition, paddleSpeed * Time.deltaTime);
    }

    // Detectar colisiones con las paletas
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerPaddle || collision.gameObject == opponentPaddle)
        {
            // Calcular el ángulo de rebote basado en la posición de impacto en la paleta
            float y = CalculateHitFactor(transform.position,
                                         collision.transform.position,
                                         collision.collider.bounds.size.y);
            // Calcular la dirección del rebote
            Vector2 direction = new Vector2(collision.gameObject == playerPaddle ? 1 : -1, y).normalized;
            // Aplicar el rebote a la pelota
            ball.GetComponent<Rigidbody2D>().velocity = direction * ballSpeed;

            // Ajustar las recompensas y actualizar el estado actual y el siguiente
            if (collision.gameObject == playerPaddle)
            {
                currentState = nextState; // Actualizar el estado actual
                nextState = GetNextState(); // Calcular el siguiente estado

                qTableManager.UpdateQValue(currentState, 1, qTableManager.GetQValue(currentState, 1) + rewardHit); // Recompensa por golpear la pelota
            }
            else if (collision.gameObject == opponentPaddle)
            {
                currentState = nextState; // Actualizar el estado actual
                nextState = GetNextState(); // Calcular el siguiente estado

                qTableManager.UpdateQValue(currentState, 1, qTableManager.GetQValue(currentState, 1) + rewardMiss); // Recompensa por perder la pelota
            }
        }
    }

    // Calcular el factor de impacto en la paleta
    private float CalculateHitFactor(Vector2 ballPosition, Vector2 paddlePosition, float paddleHeight)
    {
        return (ballPosition.y - paddlePosition.y) / (paddleHeight / 2);
    }

    // Obtener el estado actual del juego
    private string GetState()
    {
        Vector2 ballPosition = ball.transform.position;
        Vector2 playerPaddlePosition = playerPaddle.transform.position;
        return $"{ballPosition.y},{playerPaddlePosition.y}";
    }

    // Calcular el siguiente estado del juego
    private string GetNextState()
    {
        Vector2 ballPosition = ball.transform.position;
        Vector2 opponentPaddlePosition = opponentPaddle.transform.position;
        return $"{ballPosition.y},{opponentPaddlePosition.y}";
    }

    // Actualizar la tabla Q
    private void UpdateQTable()
    {
        // Obtener los valores Q del estado actual y siguiente
        float qCurrent = qTableManager.GetQValue(currentState, 1); // Valor Q para el estado actual y la acción de golpear la pelota
        float qNext = qTableManager.GetQValue(nextState, 1); // Valor Q para el siguiente estado y la acción de golpear la pelota

        // Actualizar el valor Q para el estado actual y la acción de golpear la pelota
        qTableManager.UpdateQValue(currentState, 1, qCurrent + qTableManager.alpha * (rewardHit + qTableManager.gamma * qNext - qCurrent));
        qTableManager.SaveQTable();
    }

    // Función para mover la paleta del jugador usando Q-Learning
    private void MovePlayerPaddle()
    {
        // Obtener el estado actual del juego
        Vector2 ballPosition = ball.transform.position;
        Vector2 playerPaddlePosition = playerPaddle.transform.position;

        // Definir el estado
        string state = $"{ballPosition.y},{playerPaddlePosition.y}";

        // Elegir la acción utilizando la política ɛ-greedy
        float action;
        if (Random.value < epsilon) // Exploración: elegir una acción aleatoria
        {
            action = Random.Range(0, 3); // 0: no hacer nada, 1: mover hacia arriba, 2: mover hacia abajo
        }
        else // Explotación: elegir la mejor acción según la tabla Q
        {
            float[] qValues = new float[3];
            qValues[0] = qTableManager.GetQValue(state, 0); // No hacer nada
            qValues[1] = qTableManager.GetQValue(state, 1); // Mover hacia arriba
            qValues[2] = qTableManager.GetQValue(state, 2); // Mover hacia abajo

            action = Mathf.Max(qValues); // Obtener el índice de la acción con el valor Q más alto
        }

        // Ejecutar la acción seleccionada
        if (action == 1)
        {
            MovePlayerPaddle(1f); // Mover hacia arriba
        }
        else if (action == 2)
        {
            MovePlayerPaddle(-1f); // Mover hacia abajo
        }
        // No hacer nada si action == 0
    }

}
