using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalCube
{
    /// <summary>
    /// Class to represent a single rotation of a Cube
    /// </summary>
    public class CubeRotation //: MonoBehaviour
    {
        /// <summary>
        /// The axis to rotate around, using transform.RotateAround()
        /// Unity has a left-handed coordinate system, so this might be unexpected
        /// </summary>
        public Vector3 RotationAxis { get; private set; }

        /// <summary>
        /// A single character string representing which face (or face-like collection of pieces)
        /// should be rotated.
        /// 
        /// Valid facelike:
        /// U, D, F, B, L, R = face turns
        /// u, d, f, b, l, r = wide face turns
        /// M, E, S = slices
        /// x, y, z = whole cube
        /// </summary>
        public string FaceLike { get; private set; }

        /// <summary>
        /// The nonnegative number of degrees needed to make the turn. 0, 90, 180, 270, or 360.
        /// </summary>
        public float Angle { get; private set; }

        /// <summary>
        /// The nonnegative number of quarter turns (90 degree turns) needed to make the turn. 0, 1, 2, 3, or 4.
        /// </summary>
        public int QuarterTurns { get; private set; }

        /// <summary>
        /// Multiplier for which way to rotate
        /// +1 for clockwise
        /// -1 for counterclockwise
        /// 0 for null move
        /// </summary>
        public int Direction { get; private set; }

        /// <summary>
        /// A string representing the move in something like standard notation:
        /// [facelike char] [OPTIONAL: ' char for inverse] [OPTIONAL: quarterturn count modifier char: 1, 2, 3, or 4]
        /// </summary>
        public string MoveString { get; private set; }

        /// <summary>
        /// Construct a new rotation from a standard notation string
        /// </summary>
        /// <param name="moveString">A move in standard notation, or 0 for a null move</param>
        public CubeRotation(string moveString)
        {
            // Null move is special
            if (string.IsNullOrEmpty(moveString) || moveString == "0")
            {
                MoveString = "0";
                RotationAxis = Vector3.zero;
                FaceLike = "0";
                QuarterTurns = 0;
                Angle = 0f;
                Direction = 0;
                return;
            }
            MoveString = moveString;

            // FaceLike will always be the first char of MoveString
            FaceLike = MoveString.Substring(0, 1);

            // If MoveString contains a ' char, it is inverted direction
            // Otherwise, it's normal
            if (MoveString.Contains("'"))
                Direction = -1;
            else
                Direction = 1;

            // Remove the face and the apostrophe (if present), leaving only the number of turns
            // if specified.
            // So    "U'3"   becomes    "3"
            // If nothing specified, it's one quarter turn.
            string amountString = MoveString.Replace(FaceLike, "").Replace("'", "");

            // Determine number of quarter turns.
            switch (amountString)
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

            // Quarter turn = 90 degrees/
            Angle = 90f * QuarterTurns;

            // Determing RotationAxis, which is a product of Facelike
            // These are all empirically correct, though they don't always make physical sense
            // because Unity has a left-handed coordinate system with right-handed rotations.
            switch (FaceLike)
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

                // Default set to <0,0,0>, which will result in no rotation
                // But also throw an error
                default:
                    RotationAxis = Vector3.zero;
                    throw new UnityException("Unable to determine RotationAxis for FaceLike " + FaceLike);
            }
        }

        /// <summary>
        /// Get the inverse of the rotation. I.E., the rotation that would undo this rotation.
        /// </summary>
        /// <returns>The inverted rotation</returns>
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

        /// <summary>
        /// Get the inverse of the rotation. I.E., the rotation that would undo the given rotation.
        /// </summary>
        /// <param name="rotation">Rotation to invert</param>
        /// <returns>The inverted rotation</returns>
        public static CubeRotation GetInverse(CubeRotation rotation)
        {
            return rotation.GetInverse();
        }

        /// <summary>
        /// Return all the Properties in a single string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"CubeRotation: {MoveString} [Direction={Direction}, RotationAxis={RotationAxis}, Angle={Angle}, QuarterTurns={QuarterTurns}, FaceLike={FaceLike}]";
        }
    }
}