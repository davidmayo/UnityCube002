using System;
using System.Collections.Generic;
using System.Text;

namespace LogicalCube
{
    /// <summary>
    /// Generate solution
    /// 
    /// Work in progress
    /// </summary>
    class Solution
    {
        public List<Move> moveList;
        public Cube _cube;
        //public Cube initialCube;

        private static readonly bool debugToConsole = false;
        public Solution( Cube cube )
        {
            this._cube = new Cube( cube.ToCanonicalString() );
            //initialCube = new Cube(cube.ToCanonicalString());
            moveList = new List<Move>();

            if (debugToConsole)
            {
                Console.WriteLine(("DEBUG: Starting state"));
                this._cube.WriteColoredCube();
            }
            MoveCubeToStandardOrientation();

            if (debugToConsole)
            {
                Console.WriteLine(("DEBUG: After moving to standard orientation"));
                this._cube.WriteColoredCube();
            }
            // First layer
            
            SolveWhiteCross();

            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After white cross");
                this._cube.WriteColoredCube();
            }
            SolveWhiteCorners();
            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After white corners", "FF0000");
                this._cube.WriteColoredCube();
            }
            // Second layer
            SolveMiddleLayerEdges();
            

            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After middle layer");
                this._cube.WriteColoredCube();
            }
            // Final layer

            OrientYellowEdges();



            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After OrientYellowEdges");
                this._cube.WriteColoredCube();
            }
            PermuteYellowEdges();


            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After PermuteYellowEdges");
                this._cube.WriteColoredCube();
            }
            PermuteYellowCorners();

            //MoveCubeToStandardOrientation();

            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After PermuteYellowCorners");
                this._cube.WriteColoredCube();
            }

            OrientYellowCorners();
            if (debugToConsole)
            {
                Console.WriteLine("\n\nDEBUG: After OrientYellowCorners");
                this._cube.WriteColoredCube();

                Console.WriteLine("\n\nSOLUTION: {0}\nSOLUTION LENGTH {1}", string.Join(" ", moveList), moveList.Count);
            }
        }

        private void PermuteYellowCorners()
        {
            // TODO: Count correct corners
            int correctlyPositionedCorners = 0;

            int rotationOffset = 0;
            switch( _cube[Square.F])
            {
                case 'G':
                    rotationOffset = 0;
                    break;
                case 'O':
                    rotationOffset = 1;
                    break;
                case 'B':
                    rotationOffset = 2;
                    break;
                case 'R':
                    rotationOffset = 3;
                    break;
                default:
                    throw new InvalidOperationException("Invalid front face: " + _cube[Square.F]);
            }

            List<(Square, string)> cornerPositions = new List<(Square,string)>()
            {
                (Square.UFR, "YOG"),
                (Square.UBR, "YBO"),
                (Square.UBL, "YRB"),
                (Square.UFL, "YGR")
            };

            for ( int index = 0; index < 4; index++ )
            {
                int offsetIndex = (index + rotationOffset) % 4;
                Square correctLocation = cornerPositions[index].Item1;
                Square actualLocation = _cube.FindPiece(cornerPositions[offsetIndex].Item2);

                if( correctLocation.IsSamePiece(actualLocation))
                {
                    correctlyPositionedCorners++;
                }
            }

            //Console.WriteLine("DEBUG: correctlyPositionedCorners = " + correctlyPositionedCorners);

            if (correctlyPositionedCorners == 0)
            {
                // Do U R U' L' U R' U' L
                AddMoveToSolution("U R U' L' U R' U' L", "Do sequence until a corner is positioned correctly.");

                // Recursive call
                PermuteYellowCorners();
            }
            else if (correctlyPositionedCorners == 1)
            {
                // Rotate cube so correctly positioned corner is in UFR
                bool isUFRcorrect = false;
                string correctCornerString = "" + _cube[Square.U] + _cube[Square.F] + _cube[Square.R];
                isUFRcorrect = Square.UFR.IsSamePiece(_cube.FindPiece(correctCornerString));
                while( !isUFRcorrect )
                {
                    // Rotate cube
                    AddMoveToSolution("y", "Rotate the cube until the UFR cubie is in the right position.");

                    correctCornerString = "" + _cube[Square.U] + _cube[Square.F] + _cube[Square.R];
                    isUFRcorrect = Square.UFR.IsSamePiece(_cube.FindPiece(correctCornerString));
                }


                // Do U R U' L' U R' U' L
                AddMoveToSolution("U R U' L' U R' U' L", "Do sequence until all corners are positioned correctly.");

                // Recursive call
                PermuteYellowCorners();
            }
            else if (correctlyPositionedCorners == 2)
            {
                // Impossible position.
                // TODO
            }
            else if (correctlyPositionedCorners == 3)
            {
                // Impossible position.
                // TODO
            }
            else
            {
                // All 4 corners correctly positioned.
                return;
            }
        }

        private void OrientYellowCorners()
        {
            // Flip cube so YELLOW is DOWN
            if( _cube[Square.D] == 'Y')
            {
                ; // Do nothing
            }
            else if( _cube[Square.D] == 'W')
            {
                AddMoveToSolution("x2", "Move YELLOW face to DOWN");
            }
            else
            {
                throw new Exception("Invalid cube orientation. DOWN is " + _cube[Square.D]);
            }

            // Find count of unsolved corners
            int unsolvedCorners = 0;
            for ( int rotationCount = 0; rotationCount < 4; rotationCount++)
            {
                AddMoveToSolution("y", "Rotate cube looking for solved DFR corners [This will be optimized away].");
                if (!IsDFRsolved())
                    unsolvedCorners++;
            }


            if( unsolvedCorners == 0)
            {
                // Cube is solved
                return;
            }

            else if( unsolvedCorners == 1 )
            {
                // IMPOSSIBLE POSITION
                // TODO
            }
            else
            {
                /*
                 * There are unsolved corners, and they all have to be solved at basically the same time
                 * 
                 * The method:
                 *    Iterate over all four corners, moving them into the DFR position one by one.
                 *       Orient DFR corner, even though it disrupts rest of cube. It will be fixed by the end.
                 *       Do D turn
                 *    Cube will now be solved, excet for possibly a D turn
                 *    Do final turn, if necessary
                 */ 

                // Iterate over the corners
                for( int cornerCount = 0; cornerCount < 4; cornerCount++)
                {
                    // If DFR is not solved, do R U R' U' until it is.
                    while( !IsDFRsolved() )
                    {
                        // If DFR is not solved, do R U R' U' until it is.
                        AddMoveToSolution("R U R' U'", "Keep doing four move sequence until the FRONT RIGHT corner is solved");
                    }

                    // Rotate D to stage the next corner
                    AddMoveToSolution("D", "Rotate the DOWN layer until the next unsolved piece is in the FRONT RIGHT corner");
                }

                // Do final D turn, if necessary
                while(_cube[Square.F] != _cube[Square.FD])
                {
                    AddMoveToSolution("D", "Rotate the DOWN layer until the next unsolved piece is in the FRONT RIGHT corner");
                }
            }
        }

        private bool IsDFRsolved()
        {
            // Determine if it's the correct piece
            // It is the correct piece (in the context of solving OrientYellowCorners iff
            //     1. Its colors are the same as the D, FD, and RD square
            //  && 2. Its primary facet is DFR
            string targetPieceString = "" + _cube[Square.D] + _cube[Square.FD] + _cube[Square.RD];

            Square targetPieceLocation = _cube.FindPiece(targetPieceString);

            return targetPieceLocation == Square.DFR;

            // Determine if it's the correct orientation
        }

        private void PermuteYellowEdges()
        {
            // These are the pairs of squares to look at for counting how many 
            // HACKY: List of tuples
            List<(Square, Square)> edgeAdjacentFaceTuples = new List<(Square,Square)>()
            {
                (Square.RU, Square.R),
                (Square.FU, Square.F),
                (Square.LU, Square.L),
                (Square.BU, Square.B)
            };

            int solvedEdges = 0;

            bool isCorrectAlignment = false;

            while (!isCorrectAlignment)
            {
                AddMoveToSolution("U", "Rotate U until at least two edges are correctly positioned.");

                solvedEdges = 0;
                foreach (var edgeAdjacentFace in edgeAdjacentFaceTuples)
                {
                    Square topSquare = edgeAdjacentFace.Item1;
                    Square botSquare = edgeAdjacentFace.Item2;

                    if (_cube[topSquare] == _cube[botSquare])
                        solvedEdges++;
                }

                isCorrectAlignment = solvedEdges >= 2;
            }

            // CONDITION AT THIS POINT (assuming legal cube):
            //    Cube is YELLOW up
            //    2 or 4 yellow squares are correctly permuted


            if ( solvedEdges == 0 || solvedEdges == 1 || solvedEdges == 3)
            {
                // UNSOLVABLE CUBE
                // I don't think either case can ever happen on a cube that has passed piece validation
                // But I'm not totally sure
                // TODO
            }
            else if( solvedEdges == 2 )
            {
                // V or I case


                // It's an I case IFF it's one of these scenarios:
                // I case #1: FRONT and BACK correct:  (BU == B) && (FU == F)
                // I case #2: LEFT and RIGHT correct:  (LU == L) && (RU == R)
                bool isCaseI = (_cube[Square.BU] == _cube[Square.B] && _cube[Square.FU] == _cube[Square.F]) ||
                               (_cube[Square.LU] == _cube[Square.L] && _cube[Square.RU] == _cube[Square.R]);


                // If it's not an I case, it's a V case (since we already know that exactly two edges are solved)
                bool isCaseV = !isCaseI;

                
                if( isCaseV)
                {
                    // Rotate cube until solved edges are in BU and RU
                    // i.e., until BU.color == B.color && RU.color == R.color
                    while (!(_cube[Square.BU] == _cube[Square.B] && _cube[Square.RU] == _cube[Square.R]))
                    {
                        AddMoveToSolution("y", "Two edges next to each other are in the correct position. Rotate the cube until the solved edges are BU and RU.");
                    }

                    // Do R U R' U R U2 R' U to solve the edges
                    AddMoveToSolution("R U R' U R U2 R' U", "Solve the edges.");


                }
                else
                {
                    // I case

                    // Do R U R' U R U2 R' U to transform to V case
                    AddMoveToSolution("R U R' U R U2 R' U", "Two edges across from each other are in the correct position. Transform to having two edges next to each other in correct position.");

                    // Recursive call
                    PermuteYellowEdges();
                }
            }

            // 4 solved edges, so this step is done
            else
            {
                // Done
                return;
            }
        }

        // Precondition: UP is YELLOW
        private void OrientYellowEdges()
        {
            // Make UPPER be YELLOW
            Square yellowCenter = _cube.FindPiece("Y");

            if( yellowCenter == Square.U )
            {
                ; // Do nothing
            }
            else if( yellowCenter == Square.D)
            {
                // Do an x2
                AddMoveToSolution("x2", "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.F)
            {
                // Do an x
                AddMoveToSolution("x", "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.R)
            {
                // Do a z'
                AddMoveToSolution("z'", "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.B)
            {
                // Do an x'
                AddMoveToSolution("x'", "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.L)
            {
                // Do a z
                AddMoveToSolution("z", "Flip the cube so that the YELLOW side is UP");
            }
            else
            {
                // Throw an error
                throw new Exception($"Can't find yellow center. Y is at {yellowCenter}");
            }


            // Count correct edges
            int correctlyOrientedEdgeCount = 0;

            if (_cube[Square.UF] == 'Y')
                correctlyOrientedEdgeCount++;
            if (_cube[Square.UR] == 'Y')
                correctlyOrientedEdgeCount++;
            if (_cube[Square.UB] == 'Y')
                correctlyOrientedEdgeCount++;
            if (_cube[Square.UL] == 'Y')
                correctlyOrientedEdgeCount++;

            if( correctlyOrientedEdgeCount == 1 || correctlyOrientedEdgeCount == 3)
            {
                // Invalid cube
                throw new Exception($"Unsolvable cube.");
            }
            else if( correctlyOrientedEdgeCount == 4 )
            {
                // Done
                return;
            }
            else if( correctlyOrientedEdgeCount == 0)
            {
                // Do the alg
                AddMoveToSolution("F R U R' U' F'", "Do the edge OLL algorithm");

                // Call recursively
                OrientYellowEdges();

                return;
            }

            // If we got here, there are 2 correctly oriented edges. Determine if it's an I or V case
            // It's an I case IFF (UF == UB == YELLOW) || (UR == UL == YELLOW)
            bool isCaseI = (_cube[Square.UF] == 'Y' && _cube[Square.UB] == 'Y') ||
                           (_cube[Square.UR] == 'Y' && _cube[Square.UL] == 'Y');


            if( isCaseI )
            {
                // Rotate until solved edges are UR and UL
                while(!(_cube[Square.UR] == 'Y' && _cube[Square.UL] == 'Y'))
                {
                    AddMoveToSolution("y", "This is an 'I' case. Turn the cube until the solved edges are on the LEFT and RIGHT side.");
                }

                // Do the alg
                AddMoveToSolution("F R U R' U' F'", "Do the edge OLL algorithm");

                // DEBUG: Does this fix it?
                //OrientYellowEdges();


                // Return
                return;
            }
            else
            {
                // It's case V
                // rotate until solved edges are UB and UL
                while (!(_cube[Square.UB] == 'Y' && _cube[Square.UL] == 'Y'))
                {
                    AddMoveToSolution("y", "This is a 'V' case. Turn the cube until the solved edges are on the LEFT and BACK side.");
                }

                // Do alg
                AddMoveToSolution("F R U R' U' F'", "Do the edge OLL algorithm");

                // Call recursively
                OrientYellowEdges();
            }
        }

        private void SolveWhiteCorners()
        {
            // Flip the cube over
            AddMoveToSolution(new Move("x2"), "Flip the cube over.");

            //MoveCubeToStandardOrientation();
            ;
            List<string> pieceDestinations = new List<string>();
            pieceDestinations.Add("WRG");
            pieceDestinations.Add("WBR");
            pieceDestinations.Add("WOB");
            pieceDestinations.Add("WGO");

            foreach (string cornerPiece in pieceDestinations)
            {
                SolveWhiteCornerPiece(cornerPiece);
            }
        }

        // Precondition: D is white
        // Precondition: cornerPiece lists the WHTIE square first.
        //    So only WRG, WBR, WOB, and WGO are valid
        private void SolveWhiteCornerPiece(string cornerPiece)
        {
            // Rotate cube until target corner is in BFR position
            // Cube is in correct position if and only if:
            // F is cornerPiece[1] && R is cornerPiece[2]
            // (That's actually redundant. Only need to test one or the other.)
            string frontColor = cornerPiece.Substring(1, 1);
            while( _cube.FindPiece(frontColor) != Square.F )
            {
                AddMoveToSolution("y", "Rotate cube until the " + frontColor + " side is in front.");
            }

            Square location = _cube.FindPiece(cornerPiece);

            if ( location.IsBottomLayer())
            {
                // isSolved := (DFR == White)
                bool isSolved = location.IsFace('D') && location.IsSamePiece(Square.DFR);

                //Console.WriteLine( "Piece is in face: " + location.GetFace() );

                if (isSolved)
                {
                    // Do nothing
                    // Piece is solved
                    return;
                }
                else
                {
                    //location = cube.FindPiece(cornerPiece);

                    // Rotate cube until PIECE is in DFR
                    while (!_cube.FindPiece(cornerPiece).IsSamePiece(Square.DFR))
                    {
                        AddMoveToSolution("y", "Rotate cube until " + cornerPiece + " corner is in DFR position.");
                    }

                    // Do R U R' U' to place in top layer
                    AddMoveToSolution("R U R' U'", "Bring " + cornerPiece + " corner into top layer.");

                    //Console.WriteLine("At recursive call, cornerPiece is in " + cube.FindPiece(cornerPiece));

                    // Call recursively
                    SolveWhiteCornerPiece(cornerPiece);
                }
            }
            else
            {
                // Do U until piece is in UFR position
                while (!_cube.FindPiece(cornerPiece).IsSamePiece(Square.UFR))
                {
                    AddMoveToSolution("U", "Rotate upper face until " + cornerPiece + " corner is in UFR position.");

                }

                AddMoveToSolution("R U R' U'", "Bring " + cornerPiece + " corner into bottom layer.");
                // Do R U R' U' to place in bottom layer

                //Console.WriteLine("At recursive call, cornerPiece is in " + cube.FindPiece(cornerPiece));

                // Call recursively
                SolveWhiteCornerPiece(cornerPiece);

            }
        }

        // Precondition: D is white
        private void SolveMiddleLayerEdges()
        {
            // List the side that's in FR first
            // Assuming WHITE is DOWN
            List<string> pieceDestinations = new List<string>();
            pieceDestinations.Add("RG");
            pieceDestinations.Add("GO");
            pieceDestinations.Add("OB");
            pieceDestinations.Add("BR");

            foreach (string edgePiece in pieceDestinations)
            {
                SolveMiddleLayerEdge(edgePiece);
            }
        }

        // Precondition: D is white
        // Precondition: edgePiece is given to be solved where [0] is the FRONT side
        private void SolveMiddleLayerEdge(string edgePiece)
        {
            char desiredFrontColor = edgePiece[0];
            bool isMiddleLayer = _cube.FindPiece(edgePiece).IsMiddleLayer();
            //Console.WriteLine("  isMiddleLayer = {0}", isMiddleLayer);
            if ( isMiddleLayer )
            {
                // Rotate until PIECE is in FR position
                while (!_cube.FindPiece(edgePiece).IsSamePiece(Square.FR))
                {
                    AddMoveToSolution("y", "Move target piece " + edgePiece + " to FR position.");
                }

                // Piece is in correct position IFF := (F.color == desiredFrontColor)
                // Piece is in correct orientation IFF := (FR.color == desiredFrontColor)
                bool isSolved = _cube[Square.F] == desiredFrontColor && _cube[Square.FR] == desiredFrontColor;
                //Console.WriteLine("  isMiddleLayer = {0}", isMiddleLayer);

                if ( isSolved )
                {
                    // Do nothing, piece is solved
                    return;
                }
                else
                {
                    // Rotate until PIECE is in FR position
                    while (!_cube.FindPiece(edgePiece).IsSamePiece(Square.FR))
                    {
                        AddMoveToSolution("y", "Rotate cube until target piece " + edgePiece + " to FR position.");
                    }

                    // Do R U R' U' F' U' F to put it into the top layer
                    AddMoveToSolution("R U R' U' F' U' F", "Move target piece " + edgePiece + " to TOP layer position.");

                    // Recursively call
                    SolveMiddleLayerEdge(edgePiece);
                }
            }
            else
            {
                // Rotate cube until target DESTINATION is in FR position.
                // This will occur when F.color == desiredFrontColor
                while (_cube[Square.F] != desiredFrontColor)
                {
                    AddMoveToSolution("y", "Rotate cube until " + edgePiece + " DESTINATION is in FR position.");
                }

                // Rotate upper face until piece is in FU position
                while (!_cube.FindPiece(edgePiece).IsSamePiece(Square.FU))
                {
                    AddMoveToSolution("U", "Move target piece " + edgePiece + " into FU position.");
                }


                // if FU.color == F.color, it's aligned to be inserted from LEFT upper position.
                // otherwise, it's aligned to be inserted from BACK upper position
                bool isAlignedForLeftUpperInsertion = _cube[Square.FU] == _cube[Square.F];

                if( isAlignedForLeftUpperInsertion)
                {
                    // Do U to place in LEFT upper position
                    AddMoveToSolution("U", "Move target piece " + edgePiece + " into position to be inserted in the front.");


                    // Do R U R' U' F' U' F to solve edge
                    AddMoveToSolution("R U R' U' F' U' F", "Solve " + edgePiece + ".");

                    // TODO
                }
                else
                {
                    // Do U2 to place in BACK upper position
                    AddMoveToSolution("U2", "Move target piece " + edgePiece + " into position to be inserted in the front.");

                    // Do F' U' F U R U R' to solve edge
                    AddMoveToSolution("F' U' F U R U R'", "Solve " + edgePiece + ".");
                    // TODO
                }


            }
        }

        public override string ToString()
        {
            return string.Join(" ", moveList);
        }

        private void AddMoveToSolution( Move move, string caption = "", string longCaption = "")
        {
            moveList.Add(move);
            
            if( moveList.Count > 500)
            {
                throw new Exception("Max move count exceeded:" + _cube.ToCanonicalString());
            }
            if (debugToConsole)
            {
                Console.WriteLine("Move #{2,-3} : {0,-2} : {1}", move, caption, moveList.Count);
            }
            _cube.MakeMove(move);
        }

        private void AddMoveToSolution(MoveSequence moves, string caption = "", string longCaption = "")
        {
            foreach (Move move in moves.Moves)
            {
                AddMoveToSolution( move, caption, longCaption);
            }
        }

        private void AddMoveToSolution(string moveString, string caption = "", string longCaption = "")
        {
            MoveSequence moves = new MoveSequence(moveString);
            foreach (Move move in moves.Moves)
            {
                AddMoveToSolution(move, caption, longCaption);
            }
        }


        private void SolveWhiteCross()
        {
            

            Dictionary<string, int> pieceDestinations = new Dictionary<string, int>();
            pieceDestinations.Add("WG", Square.UF.Index);
            pieceDestinations.Add("WR", Square.UR.Index);
            pieceDestinations.Add("WB", Square.UB.Index);
            pieceDestinations.Add("WO", Square.UL.Index);

            foreach( var pair in pieceDestinations)
            {
                SolveWhiteCrossPiece(pair.Key, pair.Value);
            }
        }

        private void SolveWhiteCrossPiece(string pieceString, int destinationSquareIndex, List<Move> prepMoves = null)
        {
            List<Move> algorithmMoves = new List<Move>();

            if( !(prepMoves is null))
            {
                algorithmMoves.AddRange(prepMoves);
            }


            // A string like "G";
            string destinationSide = pieceString.Substring(1, 1);
            

            Square pieceSquare_ = _cube.FindPiece(pieceString);

            // Prep: Rotate cube so white is U and the side being solved is F
            while (!Square.IsSamePiece(Square.F, _cube.FindPiece(destinationSide)))
            {
                AddMoveToSolution("y", "Rotating so green side is in front");
            }

            pieceSquare_ = _cube.FindPiece(pieceString);

            bool isAlreadySolved = false;
            bool isInBottomLayer = Square.IsBottomLayer( pieceSquare_ );
            bool isInMiddleLayer = Square.IsMiddleLayer(pieceSquare_);
            bool isInTopLayer = Square.IsTopLayer(pieceSquare_);

            if( isInBottomLayer )
            {
                // If the piece is white-facet down, it's oriented
                bool isOriented = Square.IsFace( 'D', pieceSquare_);


                // Rotate D to put target below destination center square
                while( !Square.IsSamePiece(Square.FD, _cube.FindPiece(pieceString)))
                {
                    AddMoveToSolution("D", $"Rotating D until {pieceString} edge piece is in FRONT-DOWN position");
                }

                if ( !isOriented )
                {
                    AddMoveToSolution("D R F' R'", $"Flip {pieceString} piece and put it into FRONT-UP position.");
                }
                else
                {
                    // Do an F2 to place edge
                    AddMoveToSolution("F2", $"Put {pieceString} piece into FRONT-UP position.");
                }
            }

            else if( isInMiddleLayer )
            {
                // Rotate cube so white is U and piece is in FR slot
                while( !Square.IsSamePiece(Square.FR,_cube.FindPiece(pieceString)))
                {
                    AddMoveToSolution("y", $"Rotate until {pieceString} edge is in FRONT-RIGHT location.");
                }

                pieceSquare_ = _cube.FindPiece(pieceString);
                bool whiteSideFront = Square.IsFace('F', pieceSquare_);

                if(whiteSideFront)
                {
                    AddMoveToSolution("R' D' R", $"I THINK THIS IS WRONG");

                    // Call this function recursively
                    SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);

                }
                else
                {
                    // Do F D F' to place piece in bottom layer, properly oriented
                    AddMoveToSolution("F D F'", $"Place {pieceString} edge into BOTTOM layer, properly oriented.");

                    // Call this function recursively
                    SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);
                }
            }

            // Top layer
            else
            {
                // Rotate cube so piece is in UF position
                while (!Square.IsSamePiece(Square.UF, _cube.FindPiece(pieceString)))
                {
                    AddMoveToSolution("y", $"Rotate cube until {pieceString} edge is in UPPER-FRONT position.");
                }

                pieceSquare_ = _cube.FindPiece(pieceString);

                // Piece is oriented if white side is U
                bool isOriented = Square.IsFace('U', pieceSquare_);

                if( isOriented)
                {
                    isAlreadySolved = false;
                    if (isAlreadySolved)
                    {
                        // Do nothing
                    }
                    else
                    {
                        // Solve the edge with F2
                        AddMoveToSolution("F2", $"Solve {pieceString} edge in UPPER-FRONT position.");

                        // Call this function recursively
                        SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);
                    }
                }
                else
                {
                    // Do F R' D' R to put piece in bottom layer correctly oriented
                    AddMoveToSolution("F R' D' R", $"Move {pieceString} edge to bottom layer, properly oriented");

                    // Call this function recursively
                    SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);
                }
            }


            this.moveList.AddRange(algorithmMoves);
        }

        private void MoveCubeToStandardOrientation()
        {
            // Move white side to UP
            Square whiteSquare = _cube.FindPiece("W");

            //Move whiteOrientationMove;

            if (whiteSquare == Square.U)
            {
                //whiteOrientationMove = new SingleMove("");
                //AddMoveToSolution("");
            }
            else if (whiteSquare == Square.F)
                AddMoveToSolution("x", "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.R)
                AddMoveToSolution("z'", "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.B)
                AddMoveToSolution("x'", "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.L)
                AddMoveToSolution("z", "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.D)
                AddMoveToSolution("x2", "Rotate cube to put WHITE face UP");
            else
                throw new Exception("Unable to find white center.");    // SHould never happen - FindPiece() would have thrown.

            // Move green side to FRONT
            Square greenSquare = _cube.FindPiece("G");

            if (greenSquare == Square.F)
            {
                AddMoveToSolution("");
            }
            else if (greenSquare == Square.R)
                AddMoveToSolution("y", "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.B)
                AddMoveToSolution("y2", "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.L)
                AddMoveToSolution("y'", "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.D)
                throw new Exception("Green center can't be down if white is up.");
            else if (greenSquare == Square.U)
                throw new Exception("Green center can't be up if white is up.");
            else
                throw new Exception("Unable to find green center.");    // Should never happen - FindPiece() would have thrown.
        }
    }
}
