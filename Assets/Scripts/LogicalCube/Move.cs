using System;
using System.Collections.Generic;
using System.Text;


namespace LogicalCube
{
    class Move
    {
        public char Axis { get => axis; }
        public int Rotation { get => rotation; }
        public List<Square[]> Cycles { get => cycles; }

        private char axis;
        private int rotation;
        private List<Square[]> cycles;
        //private string moveString;

        private List<char> validAxes = new List<char> {
            'U', 'D', 'L', 'R', 'F', 'B',   // Face turns
            'M', 'E', 'S',                  // Slice turns
            'x', 'y', 'z',                  // Slice turns
            'u', 'd', 'l', 'r', 'f', 'b',   // Wide face turns
            '0'                             // Null move
        };

        private static readonly Dictionary<char, List<Square[]>> baseMoveCycles = new Dictionary<char, List<Square[]>>()
        {
            // Face turns
            {
                'R',
                new List<Square[]>()
                {
                    new Square[]{ Square.RUF, Square.RUB, Square.RDB, Square.RDF }, // Face corners
                    new Square[]{ Square.RU,  Square.RB,  Square.RD,  Square.RF  }, // Face edges
                    new Square[]{ Square.URF, Square.BUR, Square.DBR, Square.FRD }, // Adjacent corners 1
                    new Square[]{ Square.UBR, Square.BDR, Square.DFR, Square.FUR }, // Adjacent corners 2
                    new Square[]{ Square.FR,  Square.UR,  Square.BR,  Square.DR  }  // Adjacent edges
                }
            },
            {
                'L',
                new List<Square[]>()
                {
                    new Square[]{ Square.LUB, Square.LUF, Square.LDF, Square.LDB }, // Face corners
                    new Square[]{ Square.LU,  Square.LF,  Square.LD,  Square.LB  }, // Face edges
                    new Square[]{ Square.UBL, Square.FUL, Square.DFL, Square.BDL }, // Adjacent corners 1
                    new Square[]{ Square.UFL, Square.FDL, Square.DBL, Square.BUL }, // Adjacent corners 2
                    new Square[]{ Square.UL,  Square.FL,  Square.DL,  Square.BL  }  // Adjacent edges
                }
            },
            {
                'U',
                new List<Square[]>()
                {
                    new Square[]{ Square.UBL, Square.UBR, Square.URF, Square.UFL }, // Face corners
                    new Square[]{ Square.UB,  Square.UR,  Square.UF,  Square.UL  }, // Face edges
                    new Square[]{ Square.FUL, Square.LUB, Square.BUR, Square.RUF }, // Adjacent corners 1
                    new Square[]{ Square.FUR, Square.LUF, Square.BUL, Square.RUB }, // Adjacent corners 2
                    new Square[]{ Square.FU,  Square.LU,  Square.BU,  Square.RU  }  // Adjacent edges
                }
            },
            {
                'D',
                new List<Square[]>()
                {
                    new Square[]{ Square.DFR, Square.DBR, Square.DBL, Square.DFL }, // Face corners
                    new Square[]{ Square.DF,  Square.DR,  Square.DB,  Square.DL  }, // Face edges
                    new Square[]{ Square.FDL, Square.RDF, Square.BDR, Square.LDB }, // Adjacent corners 1
                    new Square[]{ Square.FRD, Square.RDB, Square.BDL, Square.LDF }, // Adjacent corners 2
                    new Square[]{ Square.FD,  Square.RD,  Square.BD,  Square.LD  }  // Adjacent edges
                }
            },
            {
                'F',
                new List<Square[]>()
                {
                    new Square[]{ Square.FUL, Square.FUR, Square.FRD, Square.FDL }, // Face corners
                    new Square[]{ Square.FU,  Square.FR,  Square.FD,  Square.FL  }, // Face edges
                    new Square[]{ Square.UFL, Square.RUF, Square.DFR, Square.LDF }, // Adjacent corners 1
                    new Square[]{ Square.URF, Square.RDF, Square.DFL, Square.LUF }, // Adjacent corners 2
                    new Square[]{ Square.UF,  Square.RF,  Square.DF,  Square.LF  }  // Adjacent edges
                }
            },
            {
                'B',
                new List<Square[]>()
                {
                    new Square[]{ Square.BUR, Square.BUL, Square.BDL, Square.BDR }, // Face corners
                    new Square[]{ Square.BU,  Square.BL,  Square.BD,  Square.BR  }, // Face edges
                    new Square[]{ Square.UBL, Square.LDB, Square.DBR, Square.RUB }, // Adjacent corners 1
                    new Square[]{ Square.UBR, Square.LUB, Square.DBL, Square.RDB }, // Adjacent corners 2
                    new Square[]{ Square.UB,  Square.LB,  Square.DB,  Square.RB  }  // Adjacent edges
                }
            },

            // Slice moves
            {
                'M',
                new List<Square[]>()
                {
                    new Square[]{ Square.U,   Square.F,   Square.D,   Square.B   }, // Slice centers
                    new Square[]{ Square.UB,  Square.FU,  Square.DF,  Square.BD  }, // Slice edges 1
                    new Square[]{ Square.UF,  Square.FD,  Square.DB,  Square.BU  }  // Slice edges 2
                }
            },
            {
                'E',
                new List<Square[]>()
                {
                    new Square[]{ Square.F,   Square.R,   Square.B,   Square.L   }, // Slice centers
                    new Square[]{ Square.FL,  Square.RF,  Square.BR,  Square.LB  }, // Slice edges 1
                    new Square[]{ Square.FR,  Square.RB,  Square.BL,  Square.LF  }  // Slice edges 2
                }
            },
            {
                'S',
                new List<Square[]>()
                {
                    new Square[]{  Square.U,   Square.R,   Square.D,   Square.L   }, // Slice centers
                    new Square[]{  Square.UL,  Square.RU,  Square.DR,  Square.LD  }, // Slice edges 1
                    new Square[]{  Square.UR,  Square.RD,  Square.DL,  Square.LU  }  // Slice edges 2
                }
            },
        };

        public Move( string move )
        {
            //moveString = move;

            if( string.IsNullOrEmpty(move) ||
                move[0] == '0' )
            {
                axis = validAxes[0];
                rotation = 0;
            }
            else if( move.Length == 1)
            {
                axis = move[0];
                rotation = 1;
                
            }
            else if( move.Length == 2)
            {
                axis = move[0];

                char rotationChar = move[1];

                if (rotationChar == '2')
                    rotation = 2;
                else if (rotationChar == '\'')
                    rotation = 3;
                else
                    throw new ArgumentException("Invalid rotation character: " + rotationChar);
            }
            else
            {
                throw new ArgumentException("Invalid move string: " + move);
            }

            cycles = new List<Square[]>();
            CreateCycles();
        }

        public Move(char axis, int rotation)
        {
            this.axis = axis;
            this.rotation = rotation % 4;

            cycles = new List<Square[]>();
            CreateCycles();
        }
        public Move() : this("")
        {
        }

        private void CreateCycles()
        {
            if (baseMoveCycles.ContainsKey(this.axis))
            {
                RepeatBaseMove(axis, rotation);
            }
            else if (axis == 'x')
            {
                // x == R M' L'
                // x == R M M M L L L

                RepeatBaseMove('R', rotation);
                RepeatBaseMove('M', 4 - rotation);
                RepeatBaseMove('L', 4 - rotation);
            }
            else if (axis == 'y')
            {
                // y == U E' D'
                // x == U E E E D D D

                RepeatBaseMove('U', rotation);
                RepeatBaseMove('E', 4 - rotation);
                RepeatBaseMove('D', 4 - rotation);
            }
            else if (axis == 'z')
            {
                // z == F S B'
                // z == F S B B B
                RepeatBaseMove('F', rotation);
                RepeatBaseMove('S', rotation);
                RepeatBaseMove('B', 4 - rotation);
            }

            else if (axis == 'r')
            {
                // r = R M'
                RepeatBaseMove('R', rotation);
                RepeatBaseMove('M', 4 - rotation);
            }
            else if (axis == 'l')
            {
                // l = L M
                RepeatBaseMove('L', rotation);
                RepeatBaseMove('M', rotation);
            }
            else if (axis == 'u')
            {
                // u = U E'
                RepeatBaseMove('U', rotation);
                RepeatBaseMove('E', 4 - rotation);
            }
            else if (axis == 'd')
            {
                // d = D E
                RepeatBaseMove('D', rotation);
                RepeatBaseMove('E', rotation);
            }
            else if (axis == 'f')
            {
                // f = F S
                RepeatBaseMove('F', rotation);
                RepeatBaseMove('S', rotation);
            }
            else if (axis == 'b')
            {
                // b = B S'
                RepeatBaseMove('B', rotation);
                RepeatBaseMove('S', 4 - rotation);
            }
        }

        private void RepeatBaseMove(char baseMove, int rotationCount)
        {
            for( int count = 0; count < rotationCount; count++)
            {
                cycles.AddRange(baseMoveCycles[baseMove]);
            }
        }

        public override string ToString()
        {
            // Null move is special
            if (rotation == 0)
                return "0";

            string rotationString = (rotation) switch
            {
                1 => "",
                2 => "2",
                3 => "'",
                _ => throw new Exception()
            };

            return string.Format("{0}{1}", axis, rotationString);
        }
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            else if (obj is Move)
            {
                Move other = (Move)obj;
                return this.Axis == other.Axis && this.Rotation == other.Rotation;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return 256 * rotation + (int)axis;
        }
    }
}
