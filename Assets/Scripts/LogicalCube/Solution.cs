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
        string currentStep;
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
            string captionHeader = "6: Place yellow corners";
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
                AddMoveToSolution("U R U' L' U R' U' L",
                    captionHeader,
                    "All corners are in the wrong position.\n\n"+
                    "Do sequence until a corner is positioned correctly.");

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
                    AddMoveToSolution("y",
                        captionHeader,
                        "One corner is in the correct position.\n\n"+
                        "Rotate the cube until the correct corner is in the UPPER / FRONT position.");

                    correctCornerString = "" + _cube[Square.U] + _cube[Square.F] + _cube[Square.R];
                    isUFRcorrect = Square.UFR.IsSamePiece(_cube.FindPiece(correctCornerString));
                }


                // Do U R U' L' U R' U' L
                AddMoveToSolution("U R U' L' U R' U' L",
                    captionHeader,
                    "Do the sequence until all yellow corners are positioned correctly.");

                // Recursive call
                PermuteYellowCorners();
            }
            else if (correctlyPositionedCorners == 2)
            {
                // Impossible position.
            }
            else if (correctlyPositionedCorners == 3)
            {
                // Impossible position.
            }
            else
            {
                // All 4 corners correctly positioned.
                return;
            }
        }

        private void OrientYellowCorners()
        {
            string captionHeader = "7: Solve the cube!";
            // Flip cube so YELLOW is DOWN
            if ( _cube[Square.D] == 'Y')
            {
                ; // Do nothing
            }
            else if( _cube[Square.D] == 'W')
            {
                AddMoveToSolution("x2",
                    captionHeader,
                    "Flip the cube so that the YELLOW face is DOWN.");
            }
            else
            {
                throw new Exception("Invalid cube orientation. DOWN is " + _cube[Square.D]);
            }

            // Find count of unsolved corners
            int unsolvedCorners = 0;
            for ( int rotationCount = 0; rotationCount < 4; rotationCount++)
            {
                AddMoveToSolution("y",
                    captionHeader,
                    "Check each yellow corner to see if it's already solved.");
                if (!IsDFRsolved())
                {
                    unsolvedCorners++;
                    AddMoveToSolution("0",
                        captionHeader,
                        $"This corner is unsolved.\n\nTotal unsolved corners found: {unsolvedCorners}.");
                }
                else
                {
                    AddMoveToSolution("0",
                        captionHeader,
                        $"This corner is unsolved.\n\nTotal unsolved corners found: {unsolvedCorners}.");
                }
            }


            if( unsolvedCorners == 0)
            {
                // Cube is solved
                return;
            }

            else if( unsolvedCorners == 1 )
            {
                // IMPOSSIBLE POSITION
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
                        AddMoveToSolution("R U R' U'",
                            captionHeader,
                            "Do four move sequence until the FRONT RIGHT corner is solved");
                    }

                    // Rotate D to stage the next corner
                    AddMoveToSolution("D",
                        captionHeader,
                        "Do not rotate the whole cube!!\n\nRotate the DOWN layer until the next unsolved piece is in the FRONT / RIGHT corner");
                }

                // Do final D turn, if necessary
                while(_cube[Square.F] != _cube[Square.FD])
                {
                    AddMoveToSolution("D",
                        captionHeader,
                        "All corners are now done.\n\nRotate the DOWN layer until the cube is solved!");
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
        }

        private void PermuteYellowEdges()
        {
            string captionHeader = "5: Solve yellow edges";
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
                AddMoveToSolution("U",
                    captionHeader,
                    "Rotate UPPER face until at least two edges are correctly positioned.");

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
                // Either way, don't do anything.
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
                        AddMoveToSolution("y",
                            captionHeader,
                            "Two edges next to each other are in the correct position.\n\nRotate the cube until the solved edges are on the BACK and RIGHT faces.");
                    }

                    // Do R U R' U R U2 R' U to solve the edges
                    AddMoveToSolution("R U R' U R U2 R' U",
                        captionHeader,
                        "Do the algorithm to solve the edges.");


                }
                else
                {
                    // I case

                    // Do R U R' U R U2 R' U to transform to V case
                    AddMoveToSolution("R U R' U R U2 R' U",
                        captionHeader,
                        "Two edges across from each other are in the correct position.\n\n" +
                            "Do the algorithm to transform to having two edges next to each other in correct position.");

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
            string captionHeader = "4: Make yellow cross";
            // Make UPPER be YELLOW
            Square yellowCenter = _cube.FindPiece("Y");

            if( yellowCenter == Square.U )
            {
                ; // Do nothing
            }
            else if( yellowCenter == Square.D)
            {
                // Do an x2
                AddMoveToSolution("x2",
                    captionHeader,
                    "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.F)
            {
                // Do an x
                AddMoveToSolution("x",
                    captionHeader,
                    "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.R)
            {
                // Do a z'
                AddMoveToSolution("z'",
                    captionHeader,
                    "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.B)
            {
                // Do an x'
                AddMoveToSolution("x'",
                    captionHeader,
                    "Flip the cube so that the YELLOW side is UP");
            }
            else if (yellowCenter == Square.L)
            {
                // Do a z
                AddMoveToSolution("z",
                    captionHeader,
                    "Flip the cube so that the YELLOW side is UP");
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
                AddMoveToSolution("F R U R' U' F'",
                    captionHeader,
                    "None of the YELLOW edges are oriented. Do the algorithm to get two of them right.");

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
                    AddMoveToSolution("y",
                        captionHeader,
                        "This is an 'I' case. Turn the cube until the solved edges are on the LEFT and RIGHT side.");
                }

                // Do the alg
                AddMoveToSolution("F R U R' U' F'",
                    captionHeader,
                    "Do the algorithm to get all four edges aligned.");

                // Return
                return;
            }
            else
            {
                // It's case V
                // rotate until solved edges are UB and UL
                while (!(_cube[Square.UB] == 'Y' && _cube[Square.UL] == 'Y'))
                {
                    AddMoveToSolution("y",
                        captionHeader,
                        "This is a 'V' case. Turn the cube until the solved edges are on the LEFT and BACK side.");
                }

                // Do alg
                AddMoveToSolution("F R U R' U' F'",
                    captionHeader,
                    "Do the algorithm to convert the 'V' case into an 'I' case.");

                // Call recursively
                OrientYellowEdges();
            }
        }

        private void SolveWhiteCorners()
        {
            currentStep = "2: Solve the White Corners";

            // Flip the cube over
            AddMoveToSolution("x2", currentStep, "Flip the cube over so that YELLOW faces UP.");

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
                AddMoveToSolution("y", currentStep, "Rotate cube until the " + GetPieceString( frontColor )+ " side is in front.");
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
                        AddMoveToSolution("y", currentStep, "Rotate cube until " + GetPieceString(cornerPiece) + " corner is in DFR position.");
                    }

                    // Do R U R' U' to place in top layer
                    AddMoveToSolution("R U R' U'", currentStep, "Do the four move sequence until " + GetPieceString(cornerPiece) + " corner is solved.");

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
                    AddMoveToSolution("U", currentStep, "Do the four move sequence until " + GetPieceString(cornerPiece) + " corner is solved.");

                }

                AddMoveToSolution("R U R' U'", currentStep, "Do the four move sequence until " + GetPieceString(cornerPiece) + " corner is solved.");
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
            string captionHeader = "3: Solve middle layer edges";
            char desiredFrontColor = edgePiece[0];
            bool isMiddleLayer = _cube.FindPiece(edgePiece).IsMiddleLayer();
            if ( isMiddleLayer )
            {
                // Rotate until PIECE is in FR position
                while (!_cube.FindPiece(edgePiece).IsSamePiece(Square.FR))
                {
                    AddMoveToSolution("y",
                        captionHeader,
                        "Rotate the cube until " + GetPieceString( edgePiece )+ " is in FRONT / RIGHT position.");
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
                        AddMoveToSolution("y",
                            captionHeader,
                            "Rotate the cube until " + GetPieceString(edgePiece) + " is in FRONT / RIGHT position.");
                    }

                    // Do R U R' U' F' U' F to put it into the top layer
                    AddMoveToSolution("R U R' U' F' U' F",
                        captionHeader,
                        "Move target piece " + GetPieceString(edgePiece) + " to TOP layer.");

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
                    AddMoveToSolution("y",
                        captionHeader,
                        "Rotate cube until the slot for " + GetPieceString(edgePiece) + " is in FRONT / RIGHT position.");
                }

                // Rotate upper face until piece is in FU position
                while (!_cube.FindPiece(edgePiece).IsSamePiece(Square.FU))
                {
                    AddMoveToSolution("U",
                        captionHeader, 
                        "Move target piece " + GetPieceString(edgePiece) + " into FRONT / UPPER position.");
                }


                // if FU.color == F.color, it's aligned to be inserted from LEFT upper position.
                // otherwise, it's aligned to be inserted from BACK upper position
                bool isAlignedForLeftUpperInsertion = _cube[Square.FU] == _cube[Square.F];

                if( isAlignedForLeftUpperInsertion)
                {
                    // Do U to place in LEFT upper position
                    AddMoveToSolution("U",
                        captionHeader,
                        "Move target piece " + GetPieceString(edgePiece) + " into position to be inserted in the front.");


                    // Do R U R' U' F' U' F to solve edge
                    AddMoveToSolution("R U R' U' F' U' F",
                        captionHeader,
                        "Solve " + GetPieceString(edgePiece) + " edge.");
                }
                else
                {
                    // Do U2 to place in BACK upper position
                    AddMoveToSolution("U2",
                        captionHeader,
                        "Move target piece " + edgePiece + " into position to be inserted in the front.");

                    // Do F' U' F U R U R' to solve edge
                    AddMoveToSolution("F' U' F U R U R'",
                        captionHeader,
                        "Solve " + GetPieceString(edgePiece) + " edge.");
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

        //private void AddMoveToSolution(MoveSequence moves, string caption = "", string longCaption = "")
        //{
        //    foreach (Move move in moves.Moves)
        //    {
        //        AddMoveToSolution( move, caption, longCaption);
        //    }
        //}

        private void AddMoveToSolution(string moveString, string caption = "", string longCaption = "")
        {
            MoveSequence moves = new MoveSequence(moveString,caption, longCaption);
            foreach (Move move in moves.Moves)
            {
                AddMoveToSolution(move, caption, longCaption);
            }
        }


        private void SolveWhiteCross()
        {
            currentStep = "1: Solve the White Cross";

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
                AddMoveToSolution("y", currentStep, $"Rotate the cube until the {GetPieceString(destinationSide)} side faces FRONT.");
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
                    AddMoveToSolution("D", currentStep, $"Rotating D until {GetPieceString(pieceString)} edge piece is in FRONT / DOWN slot.");
                }

                if ( !isOriented )
                {
                    AddMoveToSolution("D R F' R'", currentStep, $"Flip {GetPieceString(pieceString)} piece and put it into FRONT / DOWN slot.");
                }
                else
                {
                    // Do an F2 to place edge
                    AddMoveToSolution("F2", currentStep, $"Solve {GetPieceString(pieceString)} piece by putting it into the FRONT / UP slot.");
                }
            }

            else if( isInMiddleLayer )
            {
                // Rotate cube so white is U and piece is in FR slot
                while( !Square.IsSamePiece(Square.FR,_cube.FindPiece(pieceString)))
                {
                    AddMoveToSolution("y", currentStep, $"Rotate until {GetPieceString(pieceString)} edge is in FRONT/RIGHT slot.");
                }

                pieceSquare_ = _cube.FindPiece(pieceString);
                bool whiteSideFront = Square.IsFace('F', pieceSquare_);

                if(whiteSideFront)
                {
                    AddMoveToSolution("R' D' R", currentStep, $"Place {GetPieceString(pieceString)} edge into BOTTOM layer, properly oriented.");

                    // Call this function recursively
                    SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);

                }
                else
                {
                    // Do F D F' to place piece in bottom layer, properly oriented
                    AddMoveToSolution("F D F'", currentStep, $"Place {GetPieceString(pieceString)} edge into BOTTOM layer, properly oriented.");

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
                    AddMoveToSolution("y", currentStep, $"Rotate cube until {GetPieceString(pieceString)} edge is in UPPER / FRONT slot.");
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
                        AddMoveToSolution("F2", currentStep, $"Solve {GetPieceString(pieceString)} edge in UPPER / FRONT slot.");

                        // Call this function recursively
                        SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);
                    }
                }
                else
                {
                    // Do F R' D' R to put piece in bottom layer correctly oriented
                    AddMoveToSolution("F R' D' R",
                        currentStep,
                        $"Move {pieceString} edge to bottom layer, properly oriented");

                    // Call this function recursively
                    SolveWhiteCrossPiece(pieceString, destinationSquareIndex, algorithmMoves);
                }
            }


            this.moveList.AddRange(algorithmMoves);
        }

        private void MoveCubeToStandardOrientation()
        {
            string currentStep = "0: Get the cube ready";
            // Move white side to UP
            Square whiteSquare = _cube.FindPiece("W");

            //Move whiteOrientationMove;

            if (whiteSquare == Square.U)
            {
                //whiteOrientationMove = new SingleMove("");
                //AddMoveToSolution("");
            }
            else if (whiteSquare == Square.F)
                AddMoveToSolution("x", currentStep, "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.R)
                AddMoveToSolution("z'", currentStep, "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.B)
                AddMoveToSolution("x'", currentStep, "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.L)
                AddMoveToSolution("z", currentStep, "Rotate cube to put WHITE face UP");
            else if (whiteSquare == Square.D)
                AddMoveToSolution("x2", currentStep, "Rotate cube to put WHITE face UP");
            else
                throw new Exception("Unable to find white center.");    // SHould never happen - FindPiece() would have thrown.

            // Move green side to FRONT
            Square greenSquare = _cube.FindPiece("G");

            if (greenSquare == Square.F)
            {
                AddMoveToSolution("");
            }
            else if (greenSquare == Square.R)
                AddMoveToSolution("y", currentStep, "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.B)
                AddMoveToSolution("y2", currentStep, "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.L)
                AddMoveToSolution("y'", currentStep, "Rotate cube to put GREEN face FRONT");
            else if (greenSquare == Square.D)
                throw new Exception("Green center can't be down if white is up.");
            else if (greenSquare == Square.U)
                throw new Exception("Green center can't be up if white is up.");
            else
                throw new Exception("Unable to find green center.");    // Should never happen - FindPiece() would have thrown.
        }

        private static string GetPieceString(string piece)
        {
            string returnValue = "";

            for (int index = 0; index < piece.Length; index++)
            {
                returnValue += piece[index] switch
                {
                    'W' => "WHITE",
                    'Y' => "YELLOW",
                    'G' => "GREEN",
                    'B' => "BLUE",
                    'R' => "RED",
                    'O' => "ORANGE",
                    _ => piece[index],
                };

                // Add a / unless this is the last entry
                if ( index < piece.Length - 1)
                    returnValue += "/";
            }
            return returnValue;
        }

        //private static string GetLocationString(string location)
        //{
        //    string returnValue = "";
        //
        //    for (int index = 0; index < location.Length; index++)
        //    {
        //        returnValue += location[index] switch
        //        {
        //            'U' => "UPPER",
        //            'D' => "DOWN",
        //            'F' => "FRONT",
        //            'B' => "BACK",
        //            'R' => "RIGHT",
        //            'L' => "LEFT",
        //            _ => location[index],
        //        };
        //
        //        // Add a / unless this is the last entry
        //        if (index < location.Length - 1)
        //            returnValue += " / ";
        //    }
        //    return returnValue;
        //}
    }
}
