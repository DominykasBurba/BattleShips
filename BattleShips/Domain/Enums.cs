namespace BattleShips.Domain;

public enum CellStatus { Empty, Ship, Hit, Miss, Sunk }
public enum Orientation { Horizontal, Vertical }
public enum ShotResult { Invalid, AlreadyTried, Miss, Hit, Sunk }
public enum Phase { Preparation, Playing, Finished }
public enum PlayerKind { Human, AI }
public enum DrawState { None, ProposedByP1, ProposedByP2, Accepted }
public enum ShootingMode { Single, Salvo3x3 }