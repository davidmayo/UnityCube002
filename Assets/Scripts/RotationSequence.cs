using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RotationSequence
{
    public List<CubeRotation> Sequence { get; private set; }
    public int Count { get => Sequence.Count; }
    public int Index { get => index; }
    public bool IsEnd { get => index >= Count; }
    public bool IsBeginning { get => index <= 0; }

    private int index;
    public static RotationSequence EmptySequence
    {
        get
        {
            return new RotationSequence();
        }
    }

    /// <summary>
    /// Empty sequence by default
    /// </summary>
    public RotationSequence()
    {
        Sequence = new List<CubeRotation>();
        index = 0;
    }
    public RotationSequence(IEnumerable<CubeRotation> moves)
    {
        Sequence = new List<CubeRotation>(moves);
        index = 0;
    }
    public RotationSequence(CubeRotation move)
    {
        Sequence = new List<CubeRotation>();
        Sequence.Add(move);
        index = 0;
    }
    public RotationSequence(RotationSequence rotationSequence)
    {
        Sequence = rotationSequence.Sequence;
        index = rotationSequence.Index;
    }
    public RotationSequence(string moves)
    {
        Sequence = new List<CubeRotation>();

        foreach( string move in moves.Trim().Split(' '))
        {
            Sequence.Add(new CubeRotation(move));
        }

        index = 0;
    }

    //public RotationSequence GetRemainingSequence()
    //{
    //    RotationSequence returnValue = new RotationSequence(this);
    //
    //    returnValue.Index = this.Index;
    //    return returnValue;
    //}
    //
    //public RotationSequence GetInverse()
    //{
    //    List<CubeRotation> moves = new List<CubeRotation>();
    //    for (int index = Count - 1; index >= 0; index--)
    //    {
    //        moves.Add(Sequence[index].GetInverse());
    //    }
    //    RotationSequence returnValue = new RotationSequence(moves);
    //    returnValue.Index = Count - Index - 1;
    //    return returnValue;
    //}
    //public RotationSequence GetInverseSoFar()
    //{
    //    return GetInverse();
    //}

    public CubeRotation GetForward()
    {
        if (this.index >= Count)
            return null;

        Debug.Log($"GetNextRotation() BEFORE INCREMENTING index={index}   Count={Count}\n{this.ToString()}");
        this.index++;

        Debug.Log($"GetNextRotation() AFTER INCREMENTING index={index}   Count={Count}\n{this.ToString()}");
        return Sequence[this.index - 1];
    }

    public CubeRotation PeekForward()
    {
        if (this.index >= Count)
            return null;

        return Sequence[this.index];
    }

    public CubeRotation GetBackward()
    {
        if (Count == 0)
            return null;
        if (Index < 0)
            return null;

        index--;
        return Sequence[Index].GetInverse();
    }

    public CubeRotation PeekBackward()
    {
        if (Count == 0)
            return null;
        if (index - 1 < 0)
            return null;

        return Sequence[index - 1].GetInverse();
    }

    public override string ToString()
    {
        string returnValue = "";

        returnValue += $"RotationSequence [Count={Count}, Index={Index}, Sequence=(";
        foreach( var rotation in Sequence)
        {
            returnValue += rotation.MoveString + " ";
        }
        returnValue += ")]";

        return returnValue;
    }

    public void MoveToEnd()
    {
        index = Count - 1;
    }

    public void MoveToBeginning()
    {
        index = 0;
    }
}
