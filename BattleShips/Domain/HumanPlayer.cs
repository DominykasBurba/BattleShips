namespace BattleShips.Domain;

public class HumanPlayer(string name, int boardSize = 10)
    : Player(name, PlayerKind.Human, boardSize) { }