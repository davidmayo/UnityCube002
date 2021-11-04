using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotation //: MonoBehaviour
{
    public Vector3 RotationAxis { get; private set; }
    public string FaceLike { get; private set; }
    public float Angle { get; private set; }
    public int QuarterTurns { get; private set; }
    public int Direction { get; private set; }
    public string MoveString { get; private set; }
    public List<GameObject> Pieces { get; private set; }

    private CubeLocator loc;

    public CubeRotation( string moveString)
    {
        // Null move is special
        if( string.IsNullOrEmpty(moveString) || moveString == "0" )
        {
            MoveString = "0";
            RotationAxis = Vector3.zero;
            FaceLike = "0";
            QuarterTurns = 0;
            Angle = 0f;
            Direction = 0;
            Pieces = new List<GameObject>();
            
            return;
        }
        MoveString = moveString;

        loc = new CubeLocator();

        FaceLike = MoveString.Substring(0, 1);

        Pieces = loc.GetPieces(FaceLike);

        if( Pieces.Count == 0 )
        {
            throw new UnityException("Invalid rotation: " + moveString + " [No pieces selected from FaceLike " + FaceLike + ". Pass \"0\" to create a null move.]");
        }

        
        if (MoveString.Contains("'"))
            Direction = -1;
        else
            Direction = 1;

        // Remove the face and the apostrophe (if present), leaving only the number of turns
        // So    "U'3"   becomes    "3"
        string amountString = MoveString.Replace(FaceLike, "").Replace("'", "");

        switch(amountString)
        {
            case "":
            case "1":
                QuarterTurns = 1;
                break;
            case "2":
                QuarterTurns = 2;
                break;
            case "3":
                QuarterTurns = 3;
                break;
            case "4":
                QuarterTurns = 4;
                break;
            default:
                throw new UnityException("Invalid rotation: " + moveString + " [unable to parse number of quarter turns.]");
                //break;
        }

        Angle = 90f * QuarterTurns;

        switch(FaceLike)
        {
            case "U":
            case "u":
            case "y":
                RotationAxis = Vector3.up;
                break;
            case "D":
            case "d":
            case "E":
                RotationAxis = Vector3.down;
                break;

            case "R":
            case "r":
            case "x":
                RotationAxis = Vector3.right;
                break;

            case "L":
            case "l":
            case "M":
                RotationAxis = Vector3.left;
                break;

            case "F":
            case "f":
            case "z":
            case "S":
                RotationAxis = Vector3.back;
                break;

            case "B":
            case "b":
                RotationAxis = Vector3.forward;
                break;

            default:
                RotationAxis = Vector3.zero;
                throw new UnityException("Unable to determine RotationAxis for FaceLike " + FaceLike);
        }
    }

    public CubeRotation GetInverse()
    {
        // Handle null move specially
        // Inverse of null move is null move
        if (MoveString == "0")
            return new CubeRotation("0");

        // If there's an apostrophe, the inverse will be the same string but WITHOUT the apostrophe
        else if (MoveString.Contains("'"))
            return new CubeRotation(MoveString.Replace("'", ""));

        // If there's no apostrophe, the inverse will be the same string but WITH an apostrophe
        else
            return new CubeRotation(MoveString + "'");
    }

    public static CubeRotation GetInverse( CubeRotation rotation )
    {
        return rotation.GetInverse();
    }

    public override string ToString()
    {
        return $"CubeRotation: {MoveString}   [Direction={Direction}, RotationAxis={RotationAxis}, Angle={Angle}, QuarterTurns={QuarterTurns}, FaceLike={FaceLike}, Pieces.Count={Pieces.Count}]";
    }
}
