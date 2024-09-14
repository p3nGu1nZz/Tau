using System;
using System.Collections.Generic;

[Serializable]
public class Result<TResult>
{
    public string original_text;
    public List<TResult> responses;
}
