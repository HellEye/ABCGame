using System;

public interface IGameManager {
    event Action OnGameComplete;
    void RestartGame();
}