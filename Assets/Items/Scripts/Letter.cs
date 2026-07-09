public class Letter : IElement {
    public string letter;

    public Letter(string letter) => this.letter = letter;

    public override bool Equals(object obj) => obj is Letter other && letter == other.letter;
}