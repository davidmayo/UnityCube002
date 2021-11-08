using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PhysicalCube
{
    public class CubeLocator //: MonoBehaviour
    {
        private List<GameObject> stickers;
        private List<GameObject> pieces;
        private List<GameObject> projections;

        private readonly List<string> squaresInOrder = new List<string>()
    {
        // UPPER face, Start index = 0
        "UBL", "UB",  "UBR",
        "UL",  "U",   "UR",
        "UFL", "UF",  "UFR",

        // FRONT face, start index = 9
        "FUL", "FU",  "FUR",
        "FL",  "F",   "FR",
        "FDL", "FD",  "FDR",

        // RIGHT face, start index = 18
        "RUF", "RU",  "RUB",
        "RF",  "R",   "RB",
        "RFD", "RD",  "RBD",

        // BACK face, start index = 27
        "BUR", "BU",  "BUL",
        "BR",  "B",   "BL",
        "BDR", "BD",  "BDL",

        // LEFT face, start index = 36
        "LUB", "LU",  "LUF",
        "LB",  "L",   "LF",
        "LDB", "LD",  "LDF",
        
        // DOWN face, start index = 45
        "DFL", "DF",  "DFR",
        "DL",  "D",   "DR",
        "DBL", "DB",  "DBR"
    };

        public CubeLocator()
        {
            stickers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Sticker"));
            pieces = new List<GameObject>(GameObject.FindGameObjectsWithTag("Piece"));
            projections = new List<GameObject>(GameObject.FindGameObjectsWithTag("Projection"));
        }

        public List<GameObject> GetPieces()
        {
            return GetPieces("x");
        }
        public List<GameObject> GetPieces(string faceLike)
        {
            float epsilon = .01f;

            Predicate<GameObject> predicate;

            if (faceLike == "U")
                predicate = delegate (GameObject obj) { return obj.transform.position.y > epsilon; };
            else if (faceLike == "u")
                predicate = delegate (GameObject obj) { return obj.transform.position.y > -epsilon; };
            else if (faceLike == "D")
                predicate = delegate (GameObject obj) { return obj.transform.position.y < -epsilon; };
            else if (faceLike == "d")
                predicate = delegate (GameObject obj) { return obj.transform.position.y < epsilon; };
            else if (faceLike == "E" || faceLike == "e")
                predicate = delegate (GameObject obj) { return obj.transform.position.y > -epsilon && obj.transform.position.y < epsilon; };


            else if (faceLike == "B")
                predicate = delegate (GameObject obj) { return obj.transform.position.z > epsilon; };
            else if (faceLike == "b")
                predicate = delegate (GameObject obj) { return obj.transform.position.z > -epsilon; };
            else if (faceLike == "F")
                predicate = delegate (GameObject obj) { return obj.transform.position.z < -epsilon; };
            else if (faceLike == "f")
                predicate = delegate (GameObject obj) { return obj.transform.position.z < epsilon; };
            else if (faceLike == "S" || faceLike == "s")
                predicate = delegate (GameObject obj) { return obj.transform.position.z > -epsilon && obj.transform.position.z < epsilon; };


            else if (faceLike == "R")
                predicate = delegate (GameObject obj) { return obj.transform.position.x > epsilon; };
            else if (faceLike == "r")
                predicate = delegate (GameObject obj) { return obj.transform.position.x > -epsilon; };
            else if (faceLike == "L")
                predicate = delegate (GameObject obj) { return obj.transform.position.x < -epsilon; };
            else if (faceLike == "l")
                predicate = delegate (GameObject obj) { return obj.transform.position.x < epsilon; };
            else if (faceLike == "M" || faceLike == "m")
                predicate = delegate (GameObject obj) { return obj.transform.position.x > -epsilon && obj.transform.position.x < epsilon; };

            else if (faceLike == "X" || faceLike == "x" ||
                     faceLike == "Y" || faceLike == "y" ||
                     faceLike == "Z" || faceLike == "z")
                predicate = delegate (GameObject obj) { return true; };


            else
                predicate = delegate (GameObject obj) { return false; };


            return pieces.FindAll(predicate);
        }

        public string GetCanonicalString()
        {
            string returnValue = "";

            for (int index = 0; index < 54; index++)
            {
                if (index % 9 == 0 && index != 0)
                    returnValue += '/';
                //string squareString = this.squaresInOrder[index];
                //Debug.Log("INDEX=" + index + "   squareString=" + squareString);


                GameObject sticker = GetStickerGameObject(index);
                char stickerChar = GetStickerChar(sticker);
                //Debug.Log("INDEX=" + index + "   stickerChar=" + '-');

                returnValue += GetStickerChar(GetStickerGameObject(index));

            }
            return returnValue;
        }

        public char GetStickerChar(GameObject sticker)
        {
            if (sticker is null)
                return 'X';

            MeshRenderer mr = sticker.GetComponent<MeshRenderer>();
            Color stickerColor = mr.material.color;

            if (stickerColor == StickerColors.WhiteSticker)
                return 'W';
            else if (stickerColor == StickerColors.YellowSticker)
                return 'Y';

            else if (stickerColor == StickerColors.GreenSticker)
                return 'G';
            else if (stickerColor == StickerColors.BlueSticker)
                return 'B';

            else if (stickerColor == StickerColors.RedSticker)
                return 'R';
            else if (stickerColor == StickerColors.OrangeSticker)
                return 'O';

            else
                return 'X';
        }


        public GameObject GetStickerGameObject(int index)
        {
            try
            {
                return GetStickerGameObject(squaresInOrder[index]);
            }
            catch (UnityException)
            {

                return null;
            }
        }


        public List<GameObject> GetProjections()
        {
            return projections;
        }
        public GameObject GetStickerGameObject(string sticker)
        {
            float epsilon = .01f;

            string threeFace = GetThreeFaceString(sticker);

            Predicate<GameObject> primaryFacePredicate;
            Predicate<GameObject> secondFacePredicate;
            Predicate<GameObject> thirdFacePredicate;

            char primaryFacelike = threeFace[0];
            char secondFacelike = threeFace[1];
            char thirdFacelike = threeFace[2];

            // Find primary stickers
            if (primaryFacelike == 'U')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.y > 1.0f + epsilon; };
            else if (primaryFacelike == 'D')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.y < -1.0f - epsilon; };
            else if (primaryFacelike == 'B')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.z > 1.0f + epsilon; };
            else if (primaryFacelike == 'F')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.z < -1.0f - epsilon; };
            else if (primaryFacelike == 'R')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.x > 1.0f + epsilon; };
            else if (primaryFacelike == 'L')
                primaryFacePredicate = delegate (GameObject obj) { return obj.transform.position.x < -1.0f - epsilon; };
            else
                throw new UnityException("Invalid sticker: " + sticker);

            // Find second stickers
            if (secondFacelike == 'U')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.y > epsilon; };
            else if (secondFacelike == 'D')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.y < -epsilon; };
            else if (secondFacelike == 'E')
                secondFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.y > -epsilon &&
                           obj.transform.position.y < epsilon;
                };
            else if (secondFacelike == 'B')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.z > epsilon; };
            else if (secondFacelike == 'F')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.z < -epsilon; };
            else if (secondFacelike == 'S')
                secondFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.z > -epsilon &&
                           obj.transform.position.z < epsilon;
                };

            else if (secondFacelike == 'R')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.x > epsilon; };
            else if (secondFacelike == 'L')
                secondFacePredicate = delegate (GameObject obj) { return obj.transform.position.x < -epsilon; };
            else if (secondFacelike == 'M')
                secondFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.x > -epsilon &&
                           obj.transform.position.x < epsilon;
                };
            else
                throw new UnityException("Invalid sticker: " + sticker);


            // Find third stickers
            if (thirdFacelike == 'U')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.y > epsilon; };
            else if (thirdFacelike == 'D')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.y < -epsilon; };
            else if (thirdFacelike == 'E')
                thirdFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.y > -epsilon &&
                           obj.transform.position.y < epsilon;
                };
            else if (thirdFacelike == 'B')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.z > epsilon; };
            else if (thirdFacelike == 'F')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.z < -epsilon; };
            else if (thirdFacelike == 'S')
                thirdFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.z > -epsilon &&
                           obj.transform.position.z < epsilon;
                };

            else if (thirdFacelike == 'R')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.x > epsilon; };
            else if (thirdFacelike == 'L')
                thirdFacePredicate = delegate (GameObject obj) { return obj.transform.position.x < -epsilon; };
            else if (thirdFacelike == 'M')
                thirdFacePredicate = delegate (GameObject obj)
                {
                    return obj.transform.position.x > -epsilon &&
                           obj.transform.position.x < epsilon;
                };
            else
                throw new UnityException("Invalid sticker: " + sticker);



            List<GameObject> faceStickers = stickers.FindAll(primaryFacePredicate);
            faceStickers = faceStickers.FindAll(secondFacePredicate);
            faceStickers = faceStickers.FindAll(thirdFacePredicate);

            //Debug.Log("AFTER THIRD ONE");
            //
            //Debug.Log("facePredicate=" + primaryFacePredicate);
            //Debug.Log("stickers count=" + stickers.Count);
            //Debug.Log("faceStickers count=" + faceStickers.Count);

            if (faceStickers.Count > 0)
                return faceStickers[0];

            else
                return null;
        }

        public void Refresh()
        {
            stickers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Sticker"));
            pieces = new List<GameObject>(GameObject.FindGameObjectsWithTag("Piece"));
        }

        private static string GetThreeFaceString(string sticker)
        {
            //Debug.Log("sticker=" + sticker);
            string primaryFacelike;
            string secondFacelike;
            string thirdFacelike;

            if (sticker.Length == 1)
            {
                primaryFacelike = sticker.Substring(0, 1);
                if (primaryFacelike == "U" || primaryFacelike == "D")
                {
                    secondFacelike = "M";
                    thirdFacelike = "S";
                }
                else if (primaryFacelike == "F" || primaryFacelike == "B")
                {
                    secondFacelike = "M";
                    thirdFacelike = "E";
                }
                else if (primaryFacelike == "R" || primaryFacelike == "L")
                {
                    secondFacelike = "S";
                    thirdFacelike = "E";
                }
                else
                {
                    throw new UnityException("Invalid sticker: " + sticker);
                }
            }
            else if (sticker.Length == 2)
            {
                primaryFacelike = sticker.Substring(0, 1);
                secondFacelike = sticker.Substring(1, 1);
                if (primaryFacelike == "U" || primaryFacelike == "D")
                {
                    if (secondFacelike == "F" || secondFacelike == "B")
                        thirdFacelike = "M";
                    else if (secondFacelike == "L" || secondFacelike == "R")
                        thirdFacelike = "S";
                    else
                        throw new UnityException("Invalid sticker: " + sticker);
                }
                else if (primaryFacelike == "F" || primaryFacelike == "B")
                {
                    if (secondFacelike == "U" || secondFacelike == "D")
                        thirdFacelike = "M";
                    else if (secondFacelike == "L" || secondFacelike == "R")
                        thirdFacelike = "E";
                    else
                        throw new UnityException("Invalid sticker: " + sticker);
                }
                else if (primaryFacelike == "L" || primaryFacelike == "R")
                {
                    if (secondFacelike == "U" || secondFacelike == "D")
                        thirdFacelike = "S";
                    else if (secondFacelike == "F" || secondFacelike == "B")
                        thirdFacelike = "E";
                    else
                        throw new UnityException("Invalid sticker: " + sticker);
                }
                else
                {
                    throw new UnityException("Invalid sticker: " + sticker);
                }
            }
            else if (sticker.Length == 3)
            {
                primaryFacelike = sticker.Substring(0, 1);
                secondFacelike = sticker.Substring(1, 1);
                thirdFacelike = sticker.Substring(2, 1);
            }
            else
            {
                throw new UnityException("Invalid sticker: " + sticker);
            }

            return primaryFacelike + secondFacelike + thirdFacelike;
        }
    }
}