public class Letter : IElement {
    public readonly string letter;

    public Letter(string letter) => this.letter = letter;
    public string TargetDisplayName => letter;

    public override bool Equals(object obj) => obj is Letter other && letter == other.letter;

    protected bool Equals(Letter other) => letter == other.letter;

    public override int GetHashCode() => letter != null ? letter.GetHashCode() : 0;
}