using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private struct TutorialEntry
    {
        public string HeaderText;
        public string CaptionText;
        public string CubeCanonicalString;
        public string CubeSequence;
    }

    public Text HeaderTextField;
    public Text MainTextField;
    public PhysicalCube.Cube Cube;
    public GameManager manager;

    public int CurrentIndex { get; private set; }
    public int TotalEntries{ get; private set; }

    private List<TutorialEntry> entries;

    private string solvedCanonicalString = "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY";

    public void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();




        entries = new List<TutorialEntry>();

        TutorialEntry entry;

        // Intro
        entry = new TutorialEntry
        {
            HeaderText = "Intro",
            CaptionText = "This tutorial will explain some facts about the cube.\n\nAnd show you the steps involved in solving it.",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "The cube has six sides, called \"Faces\".\n\nThey're referred to by their position on the cube as seen by you, the person holding the cube.",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 1
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the UPPER face.\n\nIt is abbreviated U",
            CubeCanonicalString = "WWWWWWWWW/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 2
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the RIGHT face.\n\nIt is abbreviated R",
            CubeCanonicalString = "XXXXXXXXX/XXXXXXXXX/RRRRRRRRR/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 3
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the FRONT face.\n\nIt is abbreviated F",
            CubeCanonicalString = "XXXXXXXXX/GGGGGGGGG/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 4
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the DOWN face.\n\nIt is abbreviated D",
            CubeCanonicalString = "XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/YYYYYYYYY",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 5
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the LEFT face.\n\nIt is abbreviated L",
            CubeCanonicalString = "XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/OOOOOOOOO/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Face 6
        entry = new TutorialEntry
        {
            HeaderText = "Faces",
            CaptionText = "This is the BACK face.\n\nIt is abbreviated B",
            CubeCanonicalString = "XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/BBBBBBBBB/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);


        // Structure
        entry = new TutorialEntry
        {
            HeaderText = "Structure",
            CaptionText = "The cube is made of different kinds of pieces. Some people call these pieces \"cubies.\"",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Centers",
            CaptionText = "There are six center pieces, and they do not move in relation to each other.\n\nThe WHITE center is always opposite the YELLOW center.\nThe GREEN center is always opposite the BLUE center.\nThe RED center is always opposite the ORANGE center.",
            CubeCanonicalString = "XXXXWXXXX/XXXXGXXXX/XXXXRXXXX/XXXXBXXXX/XXXXOXXXX/XXXXYXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Centers",
            CaptionText = "Don't believe me? I'll prove it.\n\nWatch all the faces rotate, and notice how the centers don't change location at all. They just spin around in place.",
            CubeCanonicalString = "XXXXWXXXX/XXXXGXXXX/XXXXRXXXX/XXXXBXXXX/XXXXOXXXX/XXXXYXXXX",
            CubeSequence = "0 R U R' U' 0 L F L' F' 0 D B D' B'"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Centers",
            CaptionText = "A center piece has ONE sticker.\n\nWe identify a specific center piece by the color of its sticker, and a specific center piece location by the face it's in the middle of.\n\nFor example, this is the GREEN center, which is in the FRONT position.",
            CubeCanonicalString = "XXXXXXXXX/XXXXGXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Edges",
            CaptionText = "There are twelve edge pieces.",
            CubeCanonicalString = "XWXWXWXWX/XGXGXGXGX/XRXRXRXRX/XBXBXBXBX/XOXOXOXOX/XYXYXYXYX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Edges",
            CaptionText = "Unlike the centers, the edges can move around all over the place.\n\nBut they will still always be in one of the 12 edge slots.",
            CubeCanonicalString = "XWXWXWXWX/XGXGXGXGX/XRXRXRXRX/XBXBXBXBX/XOXOXOXOX/XYXYXYXYX",
            CubeSequence = "0 R U R' U' 0 L F L' F' 0 D B D' B'"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Edges",
            CaptionText = "An edge has two stickers, and we refer to a specific piece by its color.\n\nFor example, this is the GREEN/RED piece (or, equivalently, the RED/GREEN piece).\n\nWe refer to its position by which two faces it's straddling. So we could say that the RED/GREEN piece is in the RIGHT/FRONT slot.",
            CubeCanonicalString = "XXXXXXXXX/XXXXXGXXX/XXXRXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);




        entry = new TutorialEntry
        {
            HeaderText = "Structure - Corners",
            CaptionText = "There are eight corner pieces.",
            CubeCanonicalString = "WXWXXXWXW/GXGXXXGXG/RXRXXXRXR/BXBXXXBXB/OXOXXXOXO/YXYXXXYXY",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Corners",
            CaptionText = "Like the edges, the corners can move around all over the place.\n\nBut they will still always be in one of the eight corner slots.",
            CubeCanonicalString = "WXWXXXWXW/GXGXXXGXG/RXRXXXRXR/BXBXXXBXB/OXOXXXOXO/YXYXXXYXY",
            CubeSequence = "0 R U R' U' 0 L F L' F' 0 D B D' B'"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Structure - Corners",
            CaptionText = "A corner has three stickers, and we refer to a specific piece by its color.\n\nFor example, this is the WHITE/RED/GREEN piece.\n\nWe refer to its position by which three faces it's straddling. So we UPPER/RIGHT/FRONT slot.",
            CubeCanonicalString = "XXXXXXXXW/XXGXXXXXX/RXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        // Movement
        entry = new TutorialEntry
        {
            HeaderText = "Movement",
            CaptionText = "Let's start turning the cube!\n\nWe refer to a turn by listing the FACE to be turned, and the DIRECTION.",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Movement",
            CaptionText = "So \"RIGHT CLOCKWISE\" means turn the cube so that the face on the RIGHT turns in a CLOCKWISE direction.",
            CubeCanonicalString = "XXWXXWXXW/XXGXXGXXG/RRRRRRRRR/BXXBXXBXX/XXXXXXXXX/XXYXXYXXY",
            CubeSequence = "0 R"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Movement",
            CaptionText = "\"UPPER CLOCKWISE\"",
            CubeCanonicalString = "WWWWWWWWW/GGGXXXXXX/RRRXXXXXX/BBBXXXXXX/OOOXXXXXX/XXXXXXXXX",
            CubeSequence = "0 U"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Movement",
            CaptionText = "\"RIGHT COUNTERCLOCKWISE\"",
            CubeCanonicalString = "XXWXXWXXW/XXGXXGXXG/RRRRRRRRR/BXXBXXBXX/XXXXXXXXX/XXYXXYXXY",
            CubeSequence = "0 R'"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Movement",
            CaptionText = "\"UPPER COUNTERCLOCKWISE\"",
            CubeCanonicalString = "WWWWWWWWW/GGGXXXXXX/RRRXXXXXX/BBBXXXXXX/OOOXXXXXX/XXXXXXXXX",
            CubeSequence = "0 U'"
        };
        entries.Add(entry);


        // Four move sequence

        entry = new TutorialEntry
        {
            HeaderText = "Sequences",
            CaptionText = "The basic idea in solving the cube is to do these turns in a specific order to accomplish some task on the cube.\n\nWe call this a \"sequence.\" Some other people call it an \"algorithm.\"\n\nFor example, this sequence makes a checkerboard pattern on the cube.",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0 U U D' D' 0 L L R' R' 0 B' B' F F"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Four move sequence",
            CaptionText = "The most important sequence for you is \"The Four Move Sequence.\" Do those four moves in a row:\n\n    RIGHT face CLOCKWISE\n    UPPER face CLOCKWISE\n    RIGHT face COUNTERCLOCKWISE\n    UPPER face COUNTERCLOCKWISE\n\nIn traditional notation, that's:   R U R' U'",
            CubeCanonicalString = solvedCanonicalString,
            CubeSequence = "0 R U R' U'"
        };
        entries.Add(entry);

        //entry = new TutorialEntry
        //{
        //    HeaderText = "Four move sequence",
        //    CaptionText = "Get used to this sequence, because you'll be using it a lot to solve your cube. Practice it!\n\nIf you do it 6 times in a row, the cube will end up back in the same state it started in.\n\n",
        //    CubeCanonicalString = solvedCanonicalString,
        //    CubeSequence = "0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U'"
        //};
        //entries.Add(entry);
        //
        //entry = new TutorialEntry
        //{
        //    HeaderText = "Four move sequence",
        //    CaptionText = "Good job!\n\nProbably better practice it a few more times, though. Again, you're going to be doing this A LOT.",
        //    CubeCanonicalString = solvedCanonicalString,
        //    CubeSequence = "0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U' 0 R U R' U'"
        //};
        //entries.Add(entry);

        // Solution outline
        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube",
            CaptionText = "All right, time to talk about solving this thing.\n\nWe're going to solve it in seven steps, (mostly) just using the four move sequence.\n\nYou start with a totally scrambled cube:",
            CubeCanonicalString = "YOROWYWBB/RWOYGYWOG/WBYRRWYWW/BBRGBRBYY/GGGGOOBBO/GWORYROGR",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube",
            CaptionText = "STEP 0: The centers are already done.\n\nRemember how the centers don't move? Well, we can just go ahead and call them done. That was easy!",
            CubeCanonicalString = "XXXXWXXXX/XXXXGXXXX/XXXXRXXXX/XXXXBXXXX/XXXXOXXXX/XXXXYXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 1",
            CaptionText = "STEP 1:\n\nYou will solve the four edge pieces with a WHITE sticker.\n\nThis is called \"The white cross\" because you will have solved a cross-shaped portion of the cube",
            CubeCanonicalString = "XWXWWWXWX/XGXXGXXXX/XRXXRXXXX/XBXXBXXXX/XOXXOXXXX/XXXXYXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 1",
            CaptionText = "STEP 1:\n\nAfter step 1, your cube might look something like this.\n\nSee all those white stickers on the top? It's a good start!",
            CubeCanonicalString = "YWYWWWOWW/BOGROGOGG/RGGRGYRYY/OROBROROB/BBWBBYRBG/WOYRYGWYB",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 2",
            CaptionText = "STEP 2:\n\nYou will solve the four corner pieces with a WHITE sticker.\n\nThis is called \"The first layer\" because you will have solved the white face (congrats!) and also all the stickers next to the white face, which form a layer",
            CubeCanonicalString = "WWWWWWWWW/GGGXGXXXX/RRRXRXXXX/BBBXBXXXX/OOOXOXXXX/XXXXYXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 2",
            CaptionText = "STEP 2:\n\nAfter step 2, your cube might look something like this.\n\nOne whole layer done. Nice!",
            CubeCanonicalString = "WWWWWWWWW/OOOBOYYOO/GGGRGGGBY/RRRRROGGR/BBBBBYBYO/BYYGYRYOR",
            CubeSequence = "0"
        };
        entries.Add(entry);


        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 3",
            CaptionText = "STEP 3:\n\nYou will solve the four edge pieces in the MIDDLE layer.\n\nThese will be the four edges that don't have a WHITE sticker or a YELLOW sticker.",
            CubeCanonicalString = "WWWWWWWWW/GGGGGGXXX/RRRRRRXXX/BBBBBBXXX/OOOOOOXXX/XXXXYXXXX",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 3",
            CaptionText = "STEP 3:\n\nAfter step 3, your cube might look something like this.\n\nIt's really coming together now.",
            CubeCanonicalString = "WWWWWWWWW/GGGGGGRYY/RRRRRRGBY/BBBBBBOGB/OOOOOOOYY/BORRYYYYG",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Last Layer",
            CaptionText = "Now we'll flip the cube over and start focusing on the YELLOW squares\n\nThe bad news is that it's a little harder from here, and we're going to need a little more than the Four Move Sequence.",
            CubeCanonicalString = "WWWWWWWWW/GGGGGGXXX/RRRRRRXXX/BBBBBBXXX/OOOOOOXXX/XXXXYXXXX",
            CubeSequence = "0 x2 y'"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Last Layer",
            CaptionText = "The YELLOW side of your actual cube might look like this.\n\nStill a lot of work to do.",
            CubeCanonicalString = "WWWWWWWWW/GGGGGGRYY/RRRRRRGBY/BBBBBBOGB/OOOOOOOYY/BORRYYYYG",
            CubeSequence = "0 x2 y'"
        };
        entries.Add(entry);


        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 4",
            CaptionText = "STEP 4:\n\nAlign the YELLOW edges.\n\nIn this step, you will get all four edges with YELLOW stickers to have their YELLOW sticker on the UPPER face.\n\nThey won't be in the correct position, but that's OK.",
            CubeCanonicalString = "XYXYYYXYX/XXXGGGGGG/XXXOOOOOO/XXXBBBBBB/XXXRRRRRR/WWWWWWWWW",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 4",
            CaptionText = "STEP 4:\n\nAfter step 4, your cube might look like this",
            CubeCanonicalString = "YYBYYYGYR/RRYGGGGGG/BGYOOOOOO/OOGBBBBBB/OBYRRRRRR/WWWWWWWWW",
            CubeSequence = "0"
        };
        entries.Add(entry);


        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 5",
            CaptionText = "STEP 5:\n\nPosition the YELLOW edges.\n\nIn this step, you will solve all four edges with YELLOW stickers.",
            CubeCanonicalString = "XYXYYYXYX/XGXGGGGGG/XOXOOOOOO/XBXBBBBBB/XRXRRRRRR/WWWWWWWWW",
            CubeSequence = "0"
        };
        entries.Add(entry);

        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 5",
            CaptionText = "STEP 5:\n\nAfter step 5, your cube might look like this",
            CubeCanonicalString = "GYYYYYYYB/GGYGGGGGG/OOBOOOOOO/RBOBBBBBB/YRRRRRRRR/WWWWWWWWW",
            CubeSequence = "0"
        };
        entries.Add(entry);



        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 6",
            CaptionText = "STEP 6:\n\nPut the YELLOW corners in the correct location.\n\nSome/all of them will be pointed the wrong way, like the BLUE/YELLOW/RED corner is here, but they will all be in the correct slot.",
            CubeCanonicalString = "OYYYYYYYB/BBRBBBBBB/YRRRRRRRR/GGYGGGGGG/GOOOOOOOO/WWWWWWWWW",
            CubeSequence = "0"
        };
        entries.Add(entry);


        entry = new TutorialEntry
        {
            HeaderText = "Solving the Cube - Step 7",
            CaptionText = "STEP 7:\n\nSolve the cube!\n\nYou will do the Four Move Sequence until the whole cube is solved!",
            CubeCanonicalString = "YYYYYYYYY/OOOOOOOOO/BBBBBBBBB/RRRRRRRRR/GGGGGGGGG/WWWWWWWWW",
            CubeSequence = "0 x2 y2 z2 x'2 y'2 z'2"
        };
        entries.Add(entry);


        CurrentIndex = -1;
        TotalEntries = entries.Count;

        //SetNext();
    }

    private void MoveForward()
    {
        CurrentIndex++;

        if (CurrentIndex >= entries.Count)
            CurrentIndex = entries.Count - 1;
        else if (CurrentIndex < 0)
            CurrentIndex = 0;
    }

    private void MoveBackward()
    {
        CurrentIndex--;

        if (CurrentIndex >= entries.Count)
            CurrentIndex = entries.Count - 1;
        else if (CurrentIndex < 0)
            CurrentIndex = 0;
    }

    public void SetNext()
    {
        // Move forward
        MoveForward();

        // Show current entry
        SetTutorial(entries[CurrentIndex]);
    }

    public void SetPrevious()
    {
        // Move backward
        MoveBackward();

        // Show current entry
        SetTutorial(entries[CurrentIndex]);
    }

    private void SetTutorial(TutorialEntry entry)
    {
        if (Cube is null || manager is null)
            return;

        HeaderTextField.text = entry.HeaderText;
        MainTextField.text = entry.CaptionText;
        Cube.SetCubeFromCanonicalString(entry.CubeCanonicalString);

        manager.SetRotationSequence(entry.CubeSequence);
        manager.PlaySequenceFromHere();
    }
}
