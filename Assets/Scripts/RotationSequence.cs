using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Container class to navigate through a sequence of CubeRotations
/// </summary>
public class RotationSequence
{
    /// <summary>
    /// The sequence of moves, in order
    /// </summary>
    public List<CubeRotation> Sequence { get; private set; }

    /// <summary>
    /// The total number of moves
    /// </summary>
    public int Count { get => Sequence.Count; }
    
    /// <summary>
    /// The current location in the List
    /// </summary>
    public int Index { get => index; }

    /// <summary>
    /// True if there are no more moves in the forward direction
    /// </summary>
    public bool IsEnd { get => index >= Count; }

    /// <summary>
    /// True if there are no more moves in the backward direction
    /// </summary>
    public bool IsBeginning { get => index <= 0; }

    // field for Index property
    private int index;

    /// <summary>
    /// The empty sequence
    /// </summary>
    public static RotationSequence Empty { get  => new RotationSequence(); }

    /// <summary>
    /// Empty sequence by default
    /// </summary>
    public RotationSequence()
    {
        Sequence = new List<CubeRotation>();
        index = 0;
    }

    /// <summary>
    /// Create a RotationSequence from a List or array of CubeRotation objects
    /// </summary>
    /// <param name="cubeRotations">The cube rotations</param>
    public RotationSequence(IEnumerable<CubeRotation> cubeRotations)
    {
        Sequence = new List<CubeRotation>(cubeRotations);
        index = 0;
    }

    /// <summary>
    /// Create a RotationSequence from a single CubeRotation
    /// </summary>
    /// <param name="cubeRotation">The cube rotation</param>
    public RotationSequence(CubeRotation cubeRotation)
    {
        Sequence = new List<CubeRotation>();
        Sequence.Add(cubeRotation);
        index = 0;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="rotationSequence">RotationSequence to copy</param>
    public RotationSequence(RotationSequence rotationSequence)
    {
        Sequence = new List<CubeRotation>( rotationSequence.Sequence);
        index = rotationSequence.Index;
    }

    /// <summary>
    /// Construct a sequence from a string of standard notation moves separated by ' ' chars.
    /// </summary>
    /// <param name="sequenceString">A string like "U R' x2 F"</param>
    public RotationSequence(string sequenceString)
    {
        Sequence = new List<CubeRotation>();

        foreach( string move in sequenceString.Trim().Split(' '))
        {
            Sequence.Add(new CubeRotation(move));
        }

        index = 0;
    }

    /// <summary>
    /// Get the next CubeRotation in the forward direction and move index, if possible. Return null if not possible.
    /// </summary>
    /// <returns>The next CubeRotation in the sequence</returns>
    public CubeRotation GetForward()
    {
        if (this.index >= Count)
            return null;

        this.index++;
        return Sequence[this.index - 1];
    }

    /// <summary>
    /// Peek the next CubeRotation in the forward direction, if possible. Index is not changed. Return null if not possible.
    /// </summary>
    /// <returns>The next CubeRotation in the sequence</returns>
    public CubeRotation PeekForward()
    {
        if (this.index >= Count)
            return null;

        return Sequence[this.index];
    }

    /// <summary>
    /// Get the INVERSE of the next CubeRotation in the backward direction and move index, if possible. Return null if not possible.
    /// </summary>
    /// <returns>The INVERSE of the previous CubeRotation in the sequence</returns>
    public CubeRotation GetBackward()
    {
        if (Count == 0)
            return null;
        if (Index < 0)
            return null;

        index--;
        return Sequence[Index].GetInverse();
    }

    /// <summary>
    /// Get the INVERSE of the next CubeRotation in the backward direction, if possible. Return null if not possible. Index is not changed.
    /// </summary>
    /// <returns>The INVERSE of the previous CubeRotation in the sequence</returns>
    public CubeRotation PeekBackward()
    {
        if (Count == 0)
            return null;
        if (index - 1 < 0)
            return null;

        return Sequence[index - 1].GetInverse();
    }

    /// <summary>
    /// Return all the properties as a string.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Move Index to the end of the sequence
    /// </summary>
    public void MoveToEnd()
    {
        index = Count - 1;
    }

    /// <summary>
    /// Move Index to the beginning of the sequence
    /// </summary>
    public void MoveToBeginning()
    {
        index = 0;
    }
}
