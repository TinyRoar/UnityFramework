
public enum GameEnvironment
{
    None,
    Default,
}

public enum Layer {
    None,
    MainMenu,
    InGame,
    Debug,
    End,
}

public enum GameplayStatus
{
    None,
    Menu,
    MatchStart,
    MatchStop,
}

public enum UIAction
{
    None,
    Show,
    Hide,
    Toggle,
}

public enum PowerUp
{
    None,
    Flame,
    Bomb,
    Speed,
    Shit,
    Football,
    Shield,
    ChuckNorris,
    Remote,
    Glitch,
    BombDown,
    FlameDown,
    Jump,
    Boxing,
    Joystick,
    BuildCrate,
}

public enum Direction
{
    None = -2,
    Left = -1,
    Right = 1,
    Top = 0,
    Bottom = 2,
}
