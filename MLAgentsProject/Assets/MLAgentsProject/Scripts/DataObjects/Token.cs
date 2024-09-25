using System;
using System.Collections.Generic;

[Serializable]
public class Token
{
    public int Id;
    public string Name;
    public List<double> Vector;

    public Token(int id, string name, double[] vector)
    {
        if (vector.Length != Constants.TokenSize && vector.Length != Constants.VectorSize)
        {
            throw new ArgumentException($"Token vector must be of size {Constants.TokenSize} or {Constants.VectorSize}.");
        }

        Id = id;
        Name = name;
        Vector = new List<double>(vector);
    }
}
