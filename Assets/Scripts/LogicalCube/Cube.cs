using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;

namespace LogicalCube
{
    class Cube
    {
        public char[] Squares { get => squares; }
        private char[] squares;

        private static readonly string solvedPositionCanonicalString = "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY";

        /// <summary>
        /// Cube in solved state, standard orientation
        /// </summary>
        public Cube() : this(solvedPositionCanonicalString )
        { }

        /// <summary>
        /// New cube from a canonical string
        /// </summary>
        /// <param name="canonicalString"></param>
        public Cube( string canonicalString)
        {
            ProcessCanonicalString(canonicalString);
        }

        /// <summary>
        /// Get the color of a square
        /// </summary>
        /// <param name="square">The square to check</param>
        /// <returns>char of the color</returns>
        public char GetSquareColor( Square square )
        {
            return squares[square.Index];
        }

        /// <summary>
        /// Get the color of a square
        /// </summary>
        /// <param name="key">The square to check</param>
        /// <returns>char of the color</returns>
        public char this[Square key]
        {
            get => squares[key.Index];
            set => squares[key.Index] = value;
        }

        /// <summary>
        /// Get the canonical string, like "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY"
        /// </summary>
        /// <returns></returns>
        public string ToCanonicalString()
        {
            string rv = string.Join("", squares);

            rv = rv.Insert(9 * 5, "/");
            rv = rv.Insert(9 * 4, "/");
            rv = rv.Insert(9 * 3, "/");
            rv = rv.Insert(9 * 2, "/");
            rv = rv.Insert(9 * 1, "/");

            return rv;
        }

        /// <summary>
        /// Set the Cube to a given canonical string, like "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY"
        /// </summary>
        /// <param name="canonicalString"></param>
        private void ProcessCanonicalString(string canonicalString)
        {
            canonicalString = canonicalString.Replace("/", "").ToUpper();
            if (canonicalString.Length != 54)
            {
                throw new ArgumentException("String must have 54 letters: " + canonicalString);
            }

            foreach (char ch in canonicalString.ToCharArray())
            {
                if (ch != 'W' &&
                     ch != 'Y' &&
                     ch != 'G' &&
                     ch != 'B' &&
                     ch != 'R' &&
                     ch != 'O' &&
                     ch != 'X')
                {
                    throw new ArgumentException("String contained invalid character");
                }
            }

            squares = canonicalString.ToCharArray();
        }

        /// <summary>
        /// Find a piece on the cube. Throws an error if piece is not found
        /// (This doesn't check piece validity. "BOY" and "BYO" are both considered the same piece.)
        /// </summary>
        /// <param name="pieceString">A string representing the colors of the piece, like "BOY", "WR", or "G"</param>
        /// <returns>The Square where the first char of pieceString is found</returns>
        public Square FindPiece( string pieceString )
        {
            if (string.IsNullOrEmpty(pieceString))
                throw new ArgumentException("Piece string was empty.");
            if (pieceString.Length == 1)
                return FindCenterPiece(pieceString);
            else if (pieceString.Length == 2)
                return FindEdgePiece(pieceString);
            else if (pieceString.Length == 3)
                return FindCornerPiece(pieceString);

            throw new ArgumentException("Piece string was wrong size.");
        }

        /// <summary>
        /// Find a corner piece
        /// (This doesn't check piece validity. "BOY" and "BYO" are both considered the same piece.)
        /// </summary>
        /// <param name="pieceString">A string like "BOY"</param>
        /// <returns>The square where the "B" of "BOY" is</returns>
        private Square FindCornerPiece(string pieceString)
        {
            List<Square> cornerSquares = Square.AllCornerSquares;

            // Loop over edge squares
            foreach (Square cornerSquare in cornerSquares)
            {
                if (squares[cornerSquare] == pieceString[0])
                {
                    // If we find the primary face color, Check the other face on that piece
                    var otherCornerSquares = cornerSquare.GetOtherPieceSquares();

                    // if the other faces on that piece match the other face colors,
                    // We've found it.
                    // Check both orders
                    if (squares[otherCornerSquares[0]] == pieceString[1] && squares[otherCornerSquares[1]] == pieceString[2])
                        return cornerSquare;
                    else if (squares[otherCornerSquares[0]] == pieceString[2] && squares[otherCornerSquares[1]] == pieceString[1])
                        return cornerSquare;
                }
            }

            // If we didn't find it in the loop, throw an exception
            throw new Exception("Could not find piece [" + pieceString + "].");
        }

        /// <summary>
        /// Find an edge piece
        /// </summary>
        /// <param name="pieceString">A string like "WR"</param>
        /// <returns>The location of the "W" side of "WR"</returns>
        private Square FindEdgePiece(string pieceString)
        {
            List<Square> edgeSquares = Square.AllEdgeSquares;

            // Loop over edge squares
            foreach (Square edgeSquare in edgeSquares)
            {
                if (squares[edgeSquare] == pieceString[0])
                {
                    // If we find the primary face color, Check the other face on that piece
                    var otherEdgeSquares = edgeSquare.GetOtherPieceSquares();

                    // if the other face on that piece matches the other face color,
                    // We've found it
                    if( squares[otherEdgeSquares[0]] == pieceString[1])
                        return edgeSquare;
                }
            }

            // If we didn't find it in the loop, throw an exception
            throw new Exception("Could not find piece [" + pieceString + "].");
        }

        /// <summary>
        /// Find a center square
        /// </summary>
        /// <param name="pieceString">A string like "G"</param>
        /// <returns>The Square where "G" is</returns>
        private Square FindCenterPiece(string pieceString)
        {
            List<Square> foundLocations = new List<Square>();
            List<Square> centerSquares = Square.AllCenterSquares;

            foreach (Square centerSquare in centerSquares)
            {
                if (squares[centerSquare] == pieceString[0])
                    foundLocations.Add(centerSquare);
            }

            // If we found one, then all is good.
            if (foundLocations.Count == 1)
                return foundLocations[0];

            // If we didn't find it in the loop, return null
            else if (foundLocations.Count == 0)
                return null;

            // If we found more than one, throw an exception, Since there's an invlid cube.
            else
                throw new Exception("Found multiples of [" + pieceString + "] found " + foundLocations.Count + ".");
        }

        /// <summary>
        /// Cycle squares. For four squares, it goes 0 -> 1, 1 -> 2, 2 -> 3, and 3 -> 0
        /// </summary>
        /// <param name="cycleSquares">The squares to be cycled</param>
        private void CycleSquares(params Square[] cycleSquares)
        {
            // if null, do nothing
            if (cycleSquares is null)
                return;

            // If 0 or 1, do nothing
            if (cycleSquares.Length <= 1)
                return;


            int count = cycleSquares.Length;
            int sourceIndex;
            int destinationIndex;

            // Create a temp one from the end of the indexes
            destinationIndex = cycleSquares[count - 1].Index;
            char tempChar = this.squares[destinationIndex];

            // Iterate over squares
            // In REVERSE order, "pushing" forward
            for (int i = count - 2; i >= 0; i--)
            {
                sourceIndex = cycleSquares[i].Index;
                destinationIndex = cycleSquares[i + 1].Index;

                this.squares[destinationIndex] = this.squares[sourceIndex];
            }

            // Copy temp into the first one
            destinationIndex = cycleSquares[0].Index;

            this.squares[destinationIndex] = tempChar;
        }

        public void MakeMove(Move move )
        {
            MakeMove(move.Cycles);
        }

        public void MakeMove(List<Move> moves)
        {
            foreach( Move move in moves)
            {
                MakeMove(move);
            }
        }

        public void MakeMove(MoveSequence moves )
        {
            foreach( Move move in moves.Moves)
            {
                MakeMove(move);
            }
        }

        /// <summary>
        /// Transform the Cube according to a List<> of arrays of Square objects
        /// </summary>
        /// <param name="cycles"></param>
        private void MakeMove(List<Square[]> cycles)
        {
            if (cycles is null)
                return;
            foreach (var cycle in cycles)
                CycleSquares(cycle);
        }

        /// <summary>
        /// Make a move from a string like "R2 U' x y2"
        /// </summary>
        /// <param name="moveString"></param>
        public void MakeMove(string moveString)
        {
            MoveSequence moves = new MoveSequence(moveString);
            
            foreach (Move move in moves.Moves)
                MakeMove( move);
        }

        public override string ToString()
        {
            return ToCanonicalString();
        }

        public override bool Equals(object obj)
        {
            return this.ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public Cube Copy()
        {
            return new Cube(this.ToCanonicalString());
        }

        /// <summary>
        /// Get a string of the cube net, with U on top, L F R B below, and U below that
        /// </summary>
        /// <returns></returns>
        public string ToNetString()
        {
            string rv = "";

            string[] squareStringArray = new string[54];

            for( int index = 0; index < 54; index++ )
            {
                squareStringArray[index] = new string(squares[index], 1);
            }

            rv += string.Format("          ┌─────────┐\n");
			rv += string.Format("        U │ {0}  {1}  {2} │   " + ToCanonicalString() + "\n", squareStringArray);
			rv += string.Format("          │ {3}  {4}  {5} │\n", squareStringArray);
			rv += string.Format("  L       │ {6}  {7}  {8} │       R         B\n", squareStringArray);
            rv += string.Format("┌─────────┼─────────┼─────────┬─────────┐\n");
			rv += string.Format("│ {36}  {37}  {38} │ {09}  {10}  {11} │ {18}  {19}  {20} │ {27}  {28}  {29} │\n", squareStringArray);
			rv += string.Format("│ {39}  {40}  {41} │ {12}  {13}  {14} │ {21}  {22}  {23} │ {30}  {31}  {32} │\n", squareStringArray);
			rv += string.Format("│ {42}  {43}  {44} │ {15}  {16}  {17} │ {24}  {25}  {26} │ {33}  {34}  {35} │\n", squareStringArray);
            rv += string.Format("└─────────┼─────────┼─────────┴─────────┘\n");
            rv += string.Format("          │ {45}  {46}  {47} │\n", squareStringArray);
            rv += string.Format("          │ {48}  {49}  {50} │\n", squareStringArray);
            rv += string.Format("        D │ {51}  {52}  {53} │\n", squareStringArray);
            rv += string.Format("          └─────────┘\n");

            return rv;
        }

        /// <summary>
        /// Print the cube to the Console with fancy colors and all.
        /// </summary>
        public void WriteColoredCube()
        {
            Debug.Log(this);
        }
        //public void WriteColoredCube()
        //{
        //    if (Console.CursorLeft != 0)
        //        Console.WriteLine();
        //
        //    string netString = ToNetString();
        //
        //    int row = 0;
        //    int col = 0;
        //    bool isSquareChar;
        //
        //    foreach (char ch in netString.ToCharArray())
        //    {
        //        if (ch == '\n')
        //        {
        //            row++;
        //            col = 0;
        //        }
        //        else
        //        {
        //            col++;
        //        }
        //
        //        // Top and bottom faces
        //        if ((1 <= row && row <= 3) || (9 <= row && row <= 11))
        //        {
        //            if (12 <= col && col <= 20)
        //                isSquareChar = true;
        //            else
        //                isSquareChar = false;
        //        }
        //
        //        // Middle row of faces
        //        else if (5 <= row && row <= 7)
        //        {
        //            // First face
        //            if ((2 <= col && col <= 10))
        //                isSquareChar = true;
        //
        //            // Second face
        //            else if ((12 <= col && col <= 20))
        //                isSquareChar = true;
        //
        //            // Third face
        //            else if ((22 <= col && col <= 30))
        //                isSquareChar = true;
        //
        //            // Fourth face
        //            else if ((32 <= col && col <= 40))
        //                isSquareChar = true;
        //
        //            else
        //                isSquareChar = false;
        //        }
        //        else
        //        {
        //            isSquareChar = false;
        //        }
        //
        //        System.Drawing.Color foreground = ColorTranslator.FromHtml("#808080");
        //        if (isSquareChar)
        //        {
        //            foreground = ch switch
        //            {
        //                'W' => DisplayColors.White,
        //                'G' => DisplayColors.Green,
        //                'R' => DisplayColors.Red,
        //                'B' => DisplayColors.Blue,
        //                'O' => DisplayColors.Orange,
        //                'Y' => DisplayColors.Yellow,
        //                'X' => DisplayColors.Unspecified,
        //                _ => DisplayColors.Default
        //            };
        //        }
        //        else
        //        {
        //            //; = ConsoleColor.Black;
        //        }
        //
        //        string str = new string(ch, 1);
        //        Console.Write(AnsiColor.Convert(str, foreground));
        //
        //    }
        //}
    } // Class Cube
}
