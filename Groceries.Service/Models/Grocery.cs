using System;

public class Grocery : IEquatable<Grocery>
{
    public string Name { get; set; }
    public DateTime Created { get; set; }

    public bool Equals(Grocery other)
    {
        return Name == other?.Name;
    }
}