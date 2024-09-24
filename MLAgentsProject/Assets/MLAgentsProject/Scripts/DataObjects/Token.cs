using System;
using System.Collections.Generic;
using System.Numerics;

[System.Serializable]
public class Token
{
    public int Id;
    public string Name;
    public List<double> Vector;

    public Token(int id, string name, double[] vector)
    {
        if (vector.Length != DatabaseConstants.TokenSize)
        {
            throw new ArgumentException("Token vector must be of size 3.");
        }

        Id = id;
        Name = name;
        Vector = new List<double>(vector);
    }
}
