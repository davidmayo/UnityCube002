using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;  

namespace LogicalCube
{
    class CubeValidator
    {
        public bool IsValid { get; private set; }

        private Cube givenCube;
        private Cube resultCube;

        private List<string> availableEdges;
        private List<string> availableCorners;

        public CubeValidator( Cube cube )
        {
            IsValid = true;
            givenCube = cube;
            resultCube = cube.Copy();

            availableEdges = new List<string>()
            {
                // White edges (white first)
                "WG",
                "WR",
                "WB",
                "WO",

                // Equatorial edges (green or blue first)
                "GR",
                "BR",
                "BO",
                "GO",
                
                // Yellow edges (yellow first)
                "YG",
                "YO",
                "YB",
                "YR"
            };

            availableCorners = new List<string>()
            {
                // White corners (white first, then clockwise)
                "WRG",
                "WBR",
                "WOB",
                "WGO",

                // Yellow corners (yellow first, then clockwise)
                "YOG",
                "YBO",
                "YRB",
                "YGR"
            };

            // Add the rotated edges
            int size = availableEdges.Count;
            for( int index = 0; index < size; index++ )
            {
                availableEdges.Add(RotateString(availableEdges[index], 1));
            }

            // Add the rotated corners
            size = availableCorners.Count;
            for (int index = 0; index < size; index++)
            {
                availableCorners.Add(RotateString(availableCorners[index], 1));
                availableCorners.Add(RotateString(availableCorners[index], 2));
            }

            ValidatePieces();
        }
        
        public static string GetForcedCube( string cubeString )
        {
            CubeValidator validator = new CubeValidator(cubeString);
            
            if( validator.IsValid) 
               return validator.resultCube.ToCanonicalString();
            else
            {
                throw new Exception($"Invalid cube: {cubeString}");
            }
        }
        

        public CubeValidator( string cubeString ) : this(new Cube(cubeString))
        {
        }

        private void ValidatePieces()
        {
            // Centers only called once
            ValidateCenters();
            
            // Call edges for baseline
            ValidateEdges();
            
            // Keep calling edges until no changes are made
            string oldCanonicalString;
            string newCanonicalString;
            do
            {
                oldCanonicalString = resultCube.ToCanonicalString();
                ValidateEdges(true);
                newCanonicalString = resultCube.ToCanonicalString();
            } while (newCanonicalString != oldCanonicalString);
            
            
            // Call corners for baseline
            ValidateCorners();
            
            // Keep calling Corners until no changes are made
            do
            {
                oldCanonicalString = resultCube.ToCanonicalString();
                ValidateCorners(true);
                newCanonicalString = resultCube.ToCanonicalString();
            } while (newCanonicalString != oldCanonicalString);
        }

        private void ValidateCorners(bool ignoreDeterminedPieces = false)
        {
            List<(Square, Square, Square)> cornerLocationTuples = new List<(Square, Square, Square)>()
            {
                (Square.UFR, Square.RUF, Square.FUR),
                (Square.UBR, Square.BUR, Square.RUB),
                (Square.UFL, Square.FUL, Square.LUF),
                (Square.UBL, Square.LUB, Square.BUL),

                (Square.DFR, Square.FDR, Square.RDF),
                (Square.DBR, Square.RDB, Square.BDR),
                (Square.DFL, Square.LDF, Square.FDL),
                (Square.DBL, Square.BDL, Square.LDB),
            };

            foreach (var cornerLocationTuple in cornerLocationTuples)
            {
                Square corner0 = cornerLocationTuple.Item1;
                Square corner1 = cornerLocationTuple.Item2;
                Square corner2 = cornerLocationTuple.Item3;

                string actualPiece = $"{resultCube[corner0]}{resultCube[corner1]}{resultCube[corner2]}";

                if (ignoreDeterminedPieces && !actualPiece.Contains("X"))
                {
                    continue;
                }

                Console.WriteLine($"DEBUG: corner0={corner0}");

                Console.WriteLine($"  DEBUG: actualPiece={actualPiece}");

                List<string> possibleCorners = new List<string>(availableCorners);

                Predicate<string> predicateAllRotations = delegate (string str) { return EqualUnderRotationAndWildcards(str, actualPiece); };
                Predicate<string> predicateSpecific = delegate (string str) { return EqualUnderWildcards(str, actualPiece); };

                List<string> possibleSpecificCorners = possibleCorners.FindAll(predicateSpecific);
                if (possibleSpecificCorners.Count == 0)
                {
                    // Invalid cube
                    IsValid = false;
                    return;
                }

                string consensusString = GetConsensusString(possibleSpecificCorners);
                Console.WriteLine($"  DEBUG: Consensus is {consensusString}   Possibilities={possibleSpecificCorners.Count}");

                // Apply the consensus to the Cube
                resultCube[corner0] = consensusString[0];
                resultCube[corner1] = consensusString[1];
                resultCube[corner2] = consensusString[2];

                if (possibleSpecificCorners.Count == 1)
                {
                    // Forced edge.

                    // Match on predicateAllRotations and remove them from available
                    List<string> candidatesToRemove = possibleCorners.FindAll(predicateAllRotations);

                    // Remove from the available edges
                    foreach (string candidateToRemove in candidatesToRemove)
                    {
                        availableCorners.Remove(candidateToRemove);
                    }
                }
            }
        }
















        // Determine if piece matches under rotation with X's as wildcard
        // So "BXX" will match
        //     BOY,OYB,YBO
        //     BYR,YRB,RBY 
        //     BRW,RWB,WBR 
        //     BWO,WOB,OBW
        //
        // And "BOX" will match
        //     BOY,OYB,YBO
        //
        // "BOY" will also match the same ones
        //     BOY,OYB,YBO
        //
        // "XXX" will match any three char string
        public static bool EqualUnderRotationAndWildcards( string candidatePiece, string actualPiece)
        {
            if (candidatePiece.Length != actualPiece.Length)
                return false;

            candidatePiece = candidatePiece.ToUpper();
            actualPiece = actualPiece.ToUpper();
            int size = actualPiece.Length;

            // Iterate over offsets and generate rotated string
            for( int offset = 0; offset < size; offset++ )
            {
                string rotatedCandidateString = RotateString(candidatePiece, offset);

                if (EqualUnderWildcards(rotatedCandidateString, actualPiece))
                    return true;
                else
                    continue;
            }
            return false;
        }

        public static bool EqualUnderWildcards(string rotatedCandidateString, string actualPiece)
        {
            if (rotatedCandidateString.Length != actualPiece.Length)
                return false;

            for( int index = 0; index < rotatedCandidateString.Length; index++)
            {
                if (actualPiece[index] == 'X')
                    continue;

                else if (actualPiece[index] != rotatedCandidateString[index])
                    return false;
            }

            return true;
        }

        private void ValidateEdges(bool ignoreDeterminedPieces = false)
        {
            List<(Square, Square)> edgeLocationTuples = new List<(Square, Square)>()
            {
                (Square.UF, Square.FU),
                (Square.UR, Square.RU),
                (Square.UB, Square.BU),
                (Square.UL, Square.LU),

                (Square.FR, Square.RF),
                (Square.BR, Square.RB),
                (Square.FL, Square.LF),
                (Square.BL, Square.LB),

                (Square.DF, Square.FD),
                (Square.DR, Square.RD),
                (Square.DB, Square.BD),
                (Square.DL, Square.LD),
            };

            //List<string> validEdges = new List<string>(availableEdges);

            foreach( var edgeLocationTuple in edgeLocationTuples)
            {
                Square edge0 = edgeLocationTuple.Item1;
                Square edge1 = edgeLocationTuple.Item2;

                string actualPiece = $"{resultCube[edge0]}{resultCube[edge1]}";

                if( ignoreDeterminedPieces && !actualPiece.Contains("X"))
                {
                    continue;
                }

                Console.WriteLine($"DEBUG: edge0={edge0}");

                Console.WriteLine($"  DEBUG: actualPiece={actualPiece}");

                List<string> possibleEdges = new List<string>(availableEdges);

                Predicate<string> predicateAllRotations = delegate (string str) { return EqualUnderRotationAndWildcards(str,actualPiece); };
                Predicate<string> predicateSpecific = delegate (string str) { return EqualUnderWildcards(str, actualPiece); };

                List<string> possibleSpecificEdges = possibleEdges.FindAll(predicateSpecific);
                if (possibleSpecificEdges.Count == 0)
                {
                    // Invalid cube
                    IsValid = false;
                    return;
                }

                string consensusString = GetConsensusString(possibleSpecificEdges);
                Console.WriteLine($"  DEBUG: Consensus is {consensusString}   Possibilities={possibleSpecificEdges.Count}");

                // Apply the consensus to the Cube
                resultCube[edge0] = consensusString[0];
                resultCube[edge1] = consensusString[1];

                if (possibleSpecificEdges.Count == 1)
                {
                    // Forced edge.

                    // Match on predicateAllRotations and remove them from available
                    List<string> candidatesToRemove = possibleEdges.FindAll(predicateAllRotations);

                    // Remove from the available edges
                    foreach( string candidateToRemove in candidatesToRemove )
                    {
                        availableEdges.Remove(candidateToRemove);
                    }
                }
            }
        }

        private static string GetConsensusString(List<string> possibleStrings)
        {
            if (possibleStrings is null || possibleStrings.Count == 0)
                return null;

            int stringLength = possibleStrings[0].Length;
            char[] charArray = new char[stringLength];

            for( int charIndex = 0; charIndex < stringLength; charIndex++ )
            {
                char candidateChar = possibleStrings[0][charIndex];

                foreach( string possibleString in possibleStrings )
                {
                    if( possibleString[charIndex] != candidateChar)
                    {
                        candidateChar = 'X';
                        break;
                    }
                }
                charArray[charIndex] = candidateChar;
            }

            return new string(charArray);
        }

        /// <summary>
        /// Determine if two strings are equivalent after rotating through some number of positions. So "BOY" is equivalent to "OYB" but not "BYO"
        /// </summary>
        /// <param name="string1"></param>
        /// <param name="string2"></param>
        /// <returns></returns>
        public static bool EqualsUnderRotation(string string1, string string2)
        {
            if (string1.Length != string2.Length)
                return false;

            for (int offset = 0; offset < string1.Length; offset++)
            {
                string string2offset = RotateString(string2, offset);
                if (string1 == string2offset)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Rotate a string forward by a given amount. Default amount is +1. So "BOY" becomes "YBO"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rotationAmount"></param>
        /// <returns></returns>
        public static string RotateString(string str, int rotationAmount = 1)
        {
            char[] charArray = str.ToCharArray();

            int length = str.Length;

            for (int index = 0; index < length; index++)
            {
                int forwardIndex = PositiveMod((index + rotationAmount), length);
                charArray[forwardIndex] = str[index];
            }

            return new string(charArray);
        }

        /// <summary>
        /// Get the integer in [0, divisor) that is congruent to dividend (mod divisor)
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static int PositiveMod(int dividend, int divisor)
        {
            int mod = dividend % divisor;
            if (mod < 0)
                mod += Math.Abs(divisor);
            return mod;
        }

        private void ValidateCenters()
        {
            Debug.Log($"Start of ValidateCenters, resultCube={resultCube}");
            // Orientations: six ways for white, then 4 ways for green == 6*4 == 24 orientations

            // Create the 12 "base" orientations.
            // (The other 12 are the reverse of these strings)
            List<string> possibleCenterOrientations = new List<string>()
            {
                // White UPPER (Yellow DOWN)
                "WGRBOY",
                "WRBOGY",
                "WBOGRY",
                "WOGRBY",

                // Green UPPER (Blue DOWN)
                "GYRWOB",
                "GRWOYB",
                "GWOYRB",
                "GOYRWB",

                // Red UPPER (Orange DOWN)
                "RWGYBO",
                "RGYBWO",
                "RYBWGO",
                "RBWGYO",
            };

            for( int i = 0; i < 12; i++)
            {
                string orientation = possibleCenterOrientations[i];
                possibleCenterOrientations.Add(ReverseString(orientation));
            }

            List<string> centerPieces = new List<string>() { "W", "G", "R", "B", "O", "Y" };
            List<(string, int)> foundCenterPieces = new List<(string, int)>();

            // Find all the centers
            foreach( var centerPiece in centerPieces )
            {
                try
                {
                    Square centerLocation = resultCube.FindPiece(centerPiece);

                    // If the center isn't found, continue.
                    if (centerLocation is null)
                        continue;

                    // Center is always at offset 4 on its face.  [Indexes are 4, 13, 22, 31, 40, 49]
                    // So face index is (loc - 4) / 9
                    // Range is   0 <= face index < 6
                    int faceIndex = ((int)centerLocation - 4) / 9;
                    foundCenterPieces.Add( (centerPiece, faceIndex) );
                }
                catch( Exception exc )
                {
                    Debug.Log(exc);
                    // Multiple results found
                    // So cube is invalid, and we can short-circuit the whole function
                    IsValid = false;
                    return;
                }
            }

            // If there are no centers, it's valid, and nothing else is forced
            if( foundCenterPieces.Count == 0)
            {
                IsValid = true;
                return;
            }

            // If there is 1 center, it's valid, and the opposite center is forced
            else if (foundCenterPieces.Count == 1)
            {
                // TODO
                var foundCenterPiece = foundCenterPieces[0];

                string foundCenterPieceString = foundCenterPiece.Item1;
                int foundFaceIndex = foundCenterPiece.Item2;
                int oppositeFaceIndex = GetOppositeFaceIndex(foundFaceIndex);
                int oppositeCenterSquareIndex = 9 * oppositeFaceIndex + 4;
                Square oppositeCenterSquare = Square.GetSquare(oppositeCenterSquareIndex);

                char oppositeCenterColor = foundCenterPieceString switch
                {
                    "W" => 'Y',
                    "Y" => 'W',
                    "B" => 'G',
                    "G" => 'B',
                    "R" => 'O',
                    "O" => 'R',
                    _ => 'X'
                };

                resultCube[oppositeCenterSquare] = oppositeCenterColor;
            }

            //// If there are 2 opposite centers, they need to be checked against each other,
            ////   and nothing else is forced
            //else if ( foundCenterPieces.Count == 2 && GetOppositeFaceIndex(foundCenterPieces[0].Item2) == foundCenterPieces[1].Item2)
            //{
            //    ;
            //}


            // Otherwise, there are [2, 3, 4, 5, 6] centers, at least two of which are adjacent
            //   They need to be checked against each other
            //   And they will all be forced (unless there are exactly two, and they're across from each other)

            // Whittle down the possible orientations
            foreach( var foundCenterPiece in foundCenterPieces)
            {
                string centerPiece = foundCenterPiece.Item1;
                int faceIndex = foundCenterPiece.Item2;
                Predicate<string> predicate = delegate (string obj) { return MatchCenterOrientation(obj,faceIndex, centerPiece); };

                possibleCenterOrientations = possibleCenterOrientations.FindAll(predicate);
            }

            // If possibleCenterOrientations is length 0, the centers are in an impossible configuration
            if ( possibleCenterOrientations.Count == 0)
            {
                Debug.Log($"Possible Center Orientation Count is ZERO");
                IsValid = false;
                return;
            }

            // If it's length 1, then that is the forced location of all six centers
            else if( possibleCenterOrientations.Count == 1)
            {
                IsValid = true;
                string centerOrientation  = possibleCenterOrientations[0];
                
                // Iterate over all faces, calculate the center Square for each face,
                // and set the center to the correct color
                for( int faceIndex = 0; faceIndex < 6; faceIndex++)
                {
                    int squareIndex = faceIndex * 9 + 4;
                    Square square = Square.GetSquare(squareIndex);

                    resultCube[square] = centerOrientation[faceIndex];
                }
            }

            // This case is when there are only two opposing centers defined
            else if( possibleCenterOrientations.Count == 4)
            {
                IsValid = true;

                // Nothing is forced, so return
                return;
            }

            // No other result should be possible
            else
            {
                throw new Exception($"Multiple possible center orientations found, which shouldn't be possible. Found {possibleCenterOrientations.Count}.");
            }
        }

        // Meant to be called as a delegate
        private static bool MatchCenterOrientation(string testOrientation, int faceIndex, string faceCenter)
        {
            return testOrientation.ToUpper()[faceIndex] == faceCenter.ToUpper()[0];
        }
        private static int GetOppositeFaceIndex(int faceIndex)
        {
            switch (faceIndex)
            {
                case 0: return 5;
                case 1: return 3;
                case 2: return 4;
                case 3: return 1;
                case 4: return 2;
                case 5: return 0;
                default: throw new ArgumentException($"Invalid face index: {faceIndex}. Must be in [0,5]");
            }

        }

        // Simple and performant
        // https://stackoverflow.com/a/228060
        static string ReverseString( string str )
        {
            var arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
