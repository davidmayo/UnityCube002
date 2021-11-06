using System;
using System.Collections.Generic;
using System.Text;

namespace LogicalCube
{
    class MoveSequence
    {
        public List<Move> Moves { get => moves; }

        private List<Move> moves;
        public int Length => moves.Count;

        public MoveSequence(string moves)
        {
            this.moves = new List<Move>();
            foreach( var move in moves.Split(' '))
            {
                this.moves.Add(new Move(move));
            }
        }

        public MoveSequence( List<Move> moves)
        {
            this.moves = moves;
        }

        public override string ToString()
        {
            return string.Join(" ", moves);
        }

        public void RemoveRedundancy()
        {
            int leftIndex = 0;
            //for( int leftIndex = 0; leftIndex < moves.Count - 1; leftIndex++)
            while( leftIndex < moves.Count - 1)
            {
                int rightIndex = leftIndex + 1;
                Move leftMove = moves[leftIndex];
                Move rightMove = moves[rightIndex];

                if( leftMove.Axis == rightMove.Axis)
                {
                    moves[leftIndex] = AddMoves(leftMove, rightMove);
                    moves[rightIndex] = new Move("0");

                    moves.RemoveAt(rightIndex);
                    continue;
                }
                else
                {
                    leftIndex++;
                    ; // Do nothing
                }
            }

            // Remove any null moves
            leftIndex = 0;
            while (leftIndex < moves.Count)
            {
                Move leftMove = moves[leftIndex];

                if( leftMove.Rotation == 0)
                {
                    moves.RemoveAt(leftIndex);
                }
                else
                {
                    leftIndex++;
                }
            }
        }

        private Move AddMoves( Move move1, Move move2 )
        {
            if (move1.Axis != move2.Axis)
                throw new ArgumentException("Moves not on same axis: " + move1 + ", " + move2);

            int rotation = move1.Rotation + move2.Rotation;

            if (rotation == 0)
                return new Move("0");
            else
                return new Move(move1.Axis, rotation);
        }

        public MoveSequence Clone()
        {
            return new MoveSequence(this.ToString());
        }

        public void InvertSequence()
        {
            moves.Reverse();

            for (int index = 0; index < moves.Count; index++)
            {
                Move originalMove = moves[index];
                Move inverseMove = new Move(originalMove.Axis, 4 - originalMove.Rotation);
                moves[index] = inverseMove;
            }
        }

        public MoveSequence GetInverseSequenced()
        {
            MoveSequence reversed = this.Clone();
            reversed.InvertSequence();
            return reversed;
        }
    }
}
