using System;
using System.Collections.Generic;
using System.Text;

namespace LogicalCube
{
    public enum Type
    {
        Center = 1,
        Edge   = 2,
        Corner = 3
    }

    class Square
    {
        public int Index { get => index; }
        public string Location { get => location; }
        public Type Type { get => type; }

        public static List<Square> AllSquares
        {
            get
            {
                List<Square> rv = new List<Square>();

                foreach( Square square in allSquares )
                {
                    rv.Add(square);
                }

                return rv;
            }
        }

        public static List<List<Square>> AllPieces
        {
            get
            {
                List<List<Square>> allPieces = new List<List<Square>>()
                {
                    // Centers
                    new List<Square>() {Square.U},
                    new List<Square>() {Square.F},
                    new List<Square>() {Square.R},
                    new List<Square>() {Square.B},
                    new List<Square>() {Square.L},
                    new List<Square>() {Square.D},

                    // Edges
                    new List<Square>() {Square.UF, Square.FU},
                    new List<Square>() {Square.UR, Square.RU},
                    new List<Square>() {Square.UB, Square.BU},
                    new List<Square>() {Square.UL, Square.LU},

                    new List<Square>() {Square.FR, Square.RF},
                    new List<Square>() {Square.BR, Square.RB},
                    new List<Square>() {Square.FL, Square.LF},
                    new List<Square>() {Square.BL, Square.LB},

                    new List<Square>() {Square.DF, Square.FD},
                    new List<Square>() {Square.DR, Square.RD},
                    new List<Square>() {Square.DB, Square.BD},
                    new List<Square>() {Square.DL, Square.LD },

                    // Corners
                    new List<Square>() {Square.UFR, Square.RUF, Square.FUR},
                    new List<Square>() {Square.UBR, Square.BUR, Square.RUB},
                    new List<Square>() {Square.UFL, Square.FUL, Square.LUF},
                    new List<Square>() {Square.UBL, Square.LUB, Square.BUL},

                    new List<Square>() {Square.DFR, Square.FDR, Square.RDF},
                    new List<Square>() {Square.DBR, Square.RDB, Square.BDR},
                    new List<Square>() {Square.DFL, Square.LDF, Square.FDL},
                    new List<Square>() {Square.DBL, Square.BDL, Square.LDB},
                };
                return allPieces;
            }
        }

        public override string ToString()
        {
            return string.Format("{0,-3} [index {1,2}]", this.Location, this.Index, this.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            else if (obj is Square)
                return this.Index == ((Square)obj).Index;
            else if (obj is int)
                return this.Index == (int)obj;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Index;
        }

        public static List<Square> AllCenterSquares
        {
            get
            {
                List<Square> rv = new List<Square>();

                foreach (Square square in allSquares)
                {
                    if( square.IsCenter())
                        rv.Add(square);
                }

                return rv;
            }
        }

        public static List<Square> AllEdgeSquares
        {
            get
            {
                List<Square> rv = new List<Square>();

                foreach (Square square in allSquares)
                {
                    if (square.IsEdge())
                        rv.Add(square);
                }

                return rv;
            }
        }

        public static List<Square> AllCornerSquares
        {
            get
            {
                List<Square> rv = new List<Square>();

                foreach (Square square in allSquares)
                {
                    if (square.IsCorner())
                        rv.Add(square);
                }

                return rv;
            }
        }


        // Constructor is private
        // Only want people using this like "Square = Square.UFR"
        private Square(int index, string location, Type type)
        {
            this.index = index;
            this.location = location.Trim();
            this.type = type;
        }

        public static bool IsCenter( Square square )
        {
            return square.Type == Type.Center;
        }
        public bool IsCenter()
        {
            return IsCenter(this);
        }

        public static bool IsEdge(Square square)
        {
            return square.Type == Type.Edge;
        }
        public bool IsEdge()
        {
            return IsEdge(this);
        }

        public static bool IsCorner(Square square)
        {
            return square.Type == Type.Corner;
        }
        public bool IsCorner()
        {
            return IsCorner(this);
        }
        
        // Upper face
        public static Square UBL = new Square( 0, "UBL", Type.Corner);
        public static Square UBR = new Square( 2, "UBR", Type.Corner);
        public static Square UFL = new Square( 6, "UFL", Type.Corner);
        public static Square UFR = new Square( 8, "UFR", Type.Corner);
        public static Square UB  = new Square( 1, "UB ", Type.Edge  );
        public static Square UL  = new Square( 3, "UL ", Type.Edge  );
        public static Square UR  = new Square( 5, "UR ", Type.Edge  );
        public static Square UF  = new Square( 7, "UF ", Type.Edge  );
        public static Square U   = new Square( 4, "U  ", Type.Center);

        // Front face
        public static Square FUL = new Square( 9, "FUL", Type.Corner);
        public static Square FUR = new Square(11, "FUR", Type.Corner);
        public static Square FDL = new Square(15, "FDL", Type.Corner);
        public static Square FDR = new Square(17, "FDR", Type.Corner);
        public static Square FU  = new Square(10, "FU ", Type.Edge  );
        public static Square FL  = new Square(12, "FL ", Type.Edge  );
        public static Square FR  = new Square(14, "FR ", Type.Edge  );
        public static Square FD  = new Square(16, "FD ", Type.Edge  );
        public static Square F   = new Square(13, "F  ", Type.Center);

        // Right face
        public static Square RUF = new Square(18, "RUF", Type.Corner);
        public static Square RUB = new Square(20, "RUB", Type.Corner);
        public static Square RFD = new Square(24, "RFD", Type.Corner);
        public static Square RBD = new Square(26, "RBD", Type.Corner);
        public static Square RU  = new Square(19, "RU ", Type.Edge  );
        public static Square RF  = new Square(21, "RF ", Type.Edge  );
        public static Square RB  = new Square(23, "RB ", Type.Edge  );
        public static Square RD  = new Square(25, "RD ", Type.Edge  );
        public static Square R   = new Square(22, "R  ", Type.Center);

        // Back face
        public static Square BUR = new Square(27, "BUR", Type.Corner);
        public static Square BUL = new Square(29, "BUL", Type.Corner);
        public static Square BDR = new Square(33, "BDR", Type.Corner);
        public static Square BDL = new Square(35, "BDL", Type.Corner);
        public static Square BU  = new Square(28, "BU ", Type.Edge  );
        public static Square BR  = new Square(30, "BR ", Type.Edge  );
        public static Square BL  = new Square(32, "BL ", Type.Edge  );
        public static Square BD  = new Square(34, "BD ", Type.Edge  );
        public static Square B   = new Square(31, "B  ", Type.Center);

        // Left face
        public static Square LUB = new Square(36, "LUB", Type.Corner);
        public static Square LUF = new Square(38, "LUF", Type.Corner);
        public static Square LDB = new Square(42, "LDB", Type.Corner);
        public static Square LDF = new Square(44, "LDF", Type.Corner);
        public static Square LU  = new Square(37, "LU ", Type.Edge  );
        public static Square LB  = new Square(39, "LB ", Type.Edge  );
        public static Square LF  = new Square(41, "LF ", Type.Edge  );
        public static Square LD  = new Square(43, "LD ", Type.Edge  );
        public static Square L   = new Square(40, "L  ", Type.Center);

        // Down face
        public static Square DFL = new Square(45, "DFL", Type.Corner);
        public static Square DFR = new Square(47, "DFR", Type.Corner);
        public static Square DBL = new Square(51, "DBL", Type.Corner);
        public static Square DBR = new Square(53, "DBR", Type.Corner);
        public static Square DF  = new Square(46, "DF ", Type.Edge  );
        public static Square DL  = new Square(48, "DL ", Type.Edge  );
        public static Square DR  = new Square(50, "DR ", Type.Edge  );
        public static Square DB  = new Square(52, "DB ", Type.Edge  );
        public static Square D   = new Square(49, "D  ", Type.Center);

        //Aliases for corners
        public static Square ULB = UBL;
        public static Square URB = UBR;
        public static Square ULF = UFL;
        public static Square URF = UFR;

        public static Square FLU = FUL;
        public static Square FRU = FUR;
        public static Square FLD = FDL;
        public static Square FRD = FDR;
        
        public static Square RFU = RUF;
        public static Square RBU = RUB;
        public static Square RDF = RFD;
        public static Square RDB = RBD;
        
        public static Square BRU = BUR;
        public static Square BLU = BUL;
        public static Square BRD = BDR;
        public static Square BLD = BDL;
        
        public static Square LBU = LUB;
        public static Square LFU = LUF;
        public static Square LBD = LDB;
        public static Square LFD = LDF;
        
        public static Square DLF = DFL;
        public static Square DRF = DFR;
        public static Square DLB = DBL;
        public static Square DRB = DBR;

        private static readonly List<Square> allSquares = new List<Square>() {
            UBL, UBR, UFL, UFR, UB, UL, UR, UF, U,
            FUL, FUR, FDL, FDR, FU, FL, FR ,FD, F,
            RUF, RUB, RFD, RBD, RU, RF, RB, RD, R,
            BUR, BUL, BDR, BDL, BU, BR, BL, BD, B,
            LUB, LUF, LDB, LDF, LU, LB, LF, LD, L,
            DFL, DFR, DBL, DBR, DF, DL, DR, DB, D  
        };

        private readonly int index;
        private readonly string location;
        private readonly Type type;
        private static List<char> validFaces = new List<char>()
        {
            'U', 'F', 'R', 'B', 'L', 'D'
        };

        public static List<Square> GetAllFaceSquares( char face )
        {
            List<Square> rv = new List<Square>();

            foreach( Square square in allSquares )
            {
                if (square.IsFace(face))
                    rv.Add(square);
            }

            return rv;
        }

        public static List<Square> GetAllLayerSquares(char face)
        {
            List<Square> rv = new List<Square>();

            foreach (Square square in allSquares)
            {
                if (square.IsLayer(face))
                    rv.Add(square);
            }

            return rv;
        }

        public static bool IsFace(char face, Square square)
        {
            // If piece is on a face, its first character will be the face char
            return square.Location[0] == face;
        }
        public bool IsFace(char face)
        {
            return IsFace(face, this);
        }

        /// <summary>
        /// Return true if the piece is in the same layer (but not necessarily same face) as given
        /// </summary>
        /// <param name="face"></param>
        /// <param name="square"></param>
        /// <returns></returns>
        public static bool IsLayer( char face, Square square)
        {
            if (validFaces.Contains(face))
            {
                // If piece is in a face layer, it will have that layer's char in its string
                return square.Location.Contains($"{face}");
            }
            else if( face == 'E')
            {
                // Piece is in E if it's NOT in U AND NOT in D
                return (!IsLayer('U', square) && !(IsLayer('D', square)));
            }
            else if( face == 'S')
            {
                // Piece is in S if it's NOT in F AND NOT in B
                return (!IsLayer('F', square) && !(IsLayer('B', square)));
            }
            else if( face == 'M')
            {
                // Piece is in M if it's NOT in L AND NOT in R
                return (!IsLayer('L', square) && !(IsLayer('R', square)));
            }
            else
            {
                throw new ArgumentException("Invalid face [" + face + "].");
            }
        }

        public bool IsLayer( char face )
        {
            return IsLayer(face, this);
        }

        public static implicit operator int(Square square)
        {
            return square.Index;
        }

        public static Square GetSquare(int squareIndex)
        {
            foreach( Square square in allSquares )
            {
                if (square.Index == squareIndex)
                    return square;
            }
            throw new ArgumentException("Can't find square for squareIndex [" + squareIndex + "].");
        }

        public static Square GetSquare(string locationString)
        {
            foreach (Square square in allSquares)
            {
                if (square.Location == locationString)
                    return square;
            }
            throw new ArgumentException("Can't find square for locationString [" + locationString + "].");
        }

        public static bool IsTopLayer( Square square )
        {
            return IsLayer('U', square);
        }
        public bool IsTopLayer()
        {
            return IsTopLayer(this);
        }

        public static bool IsMiddleLayer(Square square)
        {
            return IsLayer('E', square);
        }
        public bool IsMiddleLayer()
        {
            return IsMiddleLayer(this);
        }

        public static bool IsBottomLayer(Square square)
        {
            return IsLayer('D', square);
        }
        public bool IsBottomLayer()
        {
            return IsBottomLayer(this);
        }
        public static bool IsSamePiece( Square square1, Square square2)
        {
            var pieceSquares = GetAllPieceSquares(square1);

            return pieceSquares.Contains(square2);
        }
        public bool IsSamePiece(Square otherSquare)
        {
            return IsSamePiece(this, otherSquare);
        }

        /// <summary>
        /// Get all other squares on a piece EXCLUDING the given square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public static List<Square> GetOtherPieceSquares(Square square)
        {
            List<Square> rv =  GetAllPieceSquares(square);

            rv.Remove(square);

            return rv;
        }

        /// <summary>
        /// Get all other squares on a piece EXCLUDING this square.
        /// </summary>
        /// <returns></returns>
        public List<Square> GetOtherPieceSquares()
        {
            return GetOtherPieceSquares(this);
        }

        public List<Square> GetAllPieceSquares()
        {
            return GetAllPieceSquares(this);
        }
        public static List<Square> GetAllPieceSquares(Square square)
        {
            List<Square> rv = new List<Square>();

            if (square.IsCenter())
            {
                rv.Add(square);
            }
            else if (square.IsEdge())
            {
                // Check each permutation
                List<string> permutationStrings = new List<string>() {
                    string.Format("{0}{1}", square.Location[0], square.location[1]),
                    string.Format("{1}{0}", square.Location[0], square.location[1])
                };
                foreach (string permutationString in permutationStrings)
                {
                    try
                    {
                        rv.Add(Square.GetSquare(permutationString));
                    }
                    catch (ArgumentException)
                    {
                        ; // Do nothing
                    }
                }
            }
            else
            {
                // Check each permutation
                List<string> permutationStrings = new List<string>() {
                    string.Format("{0}{1}{2}", square.Location[0], square.location[1],square.location[2]),
                    string.Format("{0}{2}{1}", square.Location[0], square.location[1],square.location[2]),
                    string.Format("{1}{0}{2}", square.Location[0], square.location[1],square.location[2]),
                    string.Format("{1}{2}{0}", square.Location[0], square.location[1],square.location[2]),
                    string.Format("{2}{0}{1}", square.Location[0], square.location[1],square.location[2]),
                    string.Format("{2}{1}{0}", square.Location[0], square.location[1],square.location[2]),

                };
                foreach (string permutationString in permutationStrings)
                {
                    try
                    {
                        rv.Add(Square.GetSquare(permutationString));
                    }
                    catch (Exception)
                    {
                        ; // Do nothing
                    }
                }
            }

            return rv;
        }

        

        public static bool IsSameFace(Square square1, Square square2)
        {
            return GetFace(square1) == GetFace(square2);
        }

        public bool IsSameFace(Square otherSquare)
        {
            return IsSameFace(this, otherSquare);
        }

        public static char GetFace( Square sq1 )
        {
            return sq1.location[0];
        }

        public char GetFace()
        {
            return GetFace(this);
        }
        
    }
}
