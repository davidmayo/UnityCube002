using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The main coordinating class of the program, handling program state, user interface, etc.
/// Very much a work in progress
/// -David 11/4/21
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        StickerInput,
        SolutionView,
        FreeRotation
    }
    public GameState State { get; private set; }
    [Header("Cube")]
    public string StartingPosition = "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY";
    public string DefaultSequence = "M'2 U M'2 U2 M'2 U M'2";
    [Header("Rotation")]
    [Range(0f,1080f)]
    public float RotationSpeedDegreesPerSecond = 135f;
    [Range(0f, 1000f)]
    public float RotationDelayMilliseconds = 100f;
    public bool ShowProjections = true;
    [Range(0f,8f)]
    public float ProjectionThreshold = 1.5f;

    [Header("References")]
    public GameObject ModalPrefab;
    public Cube Cube;
    public InputField MoveInputField;
    public InputField CanonicalInputField;



    // Rotation Sequence stuff
    [Header("Debug stuff")]
    public string DebugString;
    private string currentRotationStartPosition;
    //[SerializeField]
    //private RotationSequence unalteredRotationSequence;
    [SerializeField]
    private RotationSequence currentRotationSequence;
    [SerializeField]
    private bool isPaused;

    [SerializeField]
    private bool rotationSequenceInProgress;
    [SerializeField]
    bool isCurrentlyReversed = false;

    // Start is called before the first frame update
    void Start()
    {
        currentRotationSequence = new RotationSequence("");
        currentRotationStartPosition = StartingPosition;
        rotationSequenceInProgress = false;
        isPaused = false;

        Cube.SetCubeFromCanonicalString(StartingPosition);
        Cube.SetCubeFromCanonicalString("GGWOYYGWY/RGBWRYOGW/ROOBGRRRB/GOOGOYRRY/YWYRBBBBW/BYGOWWOBW");
        SetGameState(GameState.FreeRotation);

        MoveInputField.text = DefaultSequence;
        MoveInputField.text = "x2 y D D D F2 y D F2 y y y F D F' y y D F2 y D D D D R F' R' x2 R U R' U' R U R' U' R U R' U' R U R' U' R U R' U' y y y R U R' U' R U R' U' y y y U U R U R' U' R U R' U' R U R' U' y y y U U R U R' U' R U R' U' R U R' U' R U R' U' R U R' U' y y y U2 F' U' F U R U R' y y U2 F' U' F U R U R' y U U U2 F' U' F U R U R' y y F R U R' U' F' F R U R' U' F' U U U U y R U R' U R U2 R' U U R U' L' U R' U' L y U R U' L' U R' U' L x2 y y y y R U R' U' R U R' U' D R U R' U' R U R' U' D R U R' U' R U R' U' D D x2 y2 z2 x2 y2 z2";
        UpdateCanonicalString();
    }

    private void FinishRotation()
    {
        // Update the canonical field
        UpdateCanonicalString();
    }

    private void FinishRotationSequence()
    {
        rotationSequenceInProgress = false;

        if (isCurrentlyReversed)
        {
            isCurrentlyReversed = false;
            //currentRotationSequence = unalteredRotationSequence;
            //currentRotationSequence.MoveToBeginning();
        }
        else
        {
            //currentRotationSequence.MoveToEnd();
        }
    }

    private void StartRotationSequence(RotationSequence newSequence )
    {
        if (rotationSequenceInProgress || !Cube.ReadyToRotate)
            return;
        if (newSequence is null || newSequence.Count == 0)
        {
            rotationSequenceInProgress = false;
            //currentRotationSequence = RotationSequence.EmptySequence;
            return;
        }
        currentRotationSequence = newSequence;
        isPaused = false;
        StartRotationSequence();
    }

    private void StartRotationSequence()
    {
        if (rotationSequenceInProgress || !Cube.ReadyToRotate)
            return;

        if (currentRotationSequence is null || currentRotationSequence.Count == 0)
        {
            //currentRotationSequence = RotationSequence.EmptySequence;
            rotationSequenceInProgress = false;

            return;
        }

        isPaused = false;

        // Move backward
        if( isCurrentlyReversed)
        {
            if (!currentRotationSequence.IsBeginning)
            {
                rotationSequenceInProgress = true;
                Cube.StartRotation(currentRotationSequence.GetBackward());
            }
        }

        // Move forward
        else
        {
            if (!currentRotationSequence.IsEnd)
            {
                rotationSequenceInProgress = true;
                Cube.StartRotation(currentRotationSequence.GetForward());
            }
        }

        
        
    }

    private void ContinueRotationSequence()
    {
        if (isPaused)
            return;
        if (!rotationSequenceInProgress)
            return;
        if (!Cube.ReadyToRotate)
            return;

        FinishRotation();
        // Move backwards
        if( isCurrentlyReversed )
        {
            if (currentRotationSequence.IsBeginning)
            {
                FinishRotationSequence();
            }
            else
            {
                Cube.StartRotation(currentRotationSequence.GetBackward());
            }
        }

        // Move forward
        else
        {
            if (currentRotationSequence.IsEnd)
            {
                FinishRotationSequence();
            }
            else
            {
                Cube.StartRotation(currentRotationSequence.GetForward());
            }
        }

        
    }

    

    public void HelpButtonClicked()
    {
        Debug.Log($"Help Button Clicked {this}");
        Instantiate(ModalPrefab);
        Canvas canvas = ModalPrefab.GetComponent<Canvas>();


        canvas.enabled = false;

        // Code to set the modal stuff goes here

        canvas.enabled = true;
    }

    

    // Update is called once per frame
    void Update()
    {
        switch (this.State)
        {
            case GameState.StickerInput:
                ProcessStickerInput();
                break;
            case GameState.SolutionView:
                ProcessSolutionView();
                break;
            case GameState.FreeRotation:
                ProcessFreeRotation();
                break;
            default:
                break;
        }

        Cube.DegreesPerSecond = this.RotationSpeedDegreesPerSecond;
        Cube.DelayMilliseconds = this.RotationDelayMilliseconds;
        Cube.ProjectionVisibilityThreshold = this.ProjectionThreshold;
        Cube.ShowProjections = this.ShowProjections;

        if( ShowProjections )
        {
        }
    }

    void SetGameState( GameState newState )
    {
        switch (newState)
        {
            case GameState.StickerInput:
                break;
            case GameState.SolutionView:
                break;
            case GameState.FreeRotation:
                break;
            default:
                break;
        }
        State = newState;
    }

    public void PauseRotationSequence()
    {
        isPaused = true;
        rotationSequenceInProgress = false;
    }

    // Called when "set sequence" button is clicked
    // Or "Update" is clicked
    public void SetRotationSequence()
    {
        currentRotationStartPosition = Cube.CanonicalString;
        //unalteredRotationSequence = new RotationSequence(MoveInputField.text);
        currentRotationSequence = new RotationSequence(MoveInputField.text);
        isCurrentlyReversed = false;
    }

    public void UpdateButtonClicked()
    {
        Debug.Log("Update button clicked");
        Cube.SetCubeFromCanonicalString(CanonicalInputField.text);
        SetRotationSequence();
    }

    public void GenerateSolutionButtonClicked()
    {
        Debug.Log("Generate solution button clicked");
        LogicalCube.Cube logicalCube = new LogicalCube.Cube(Cube.CanonicalString);
        LogicalCube.Solution solution = new LogicalCube.Solution(logicalCube);

        MoveInputField.text = solution.ToString();
        SetRotationSequence();
        

        Debug.Log($"SOLUTION length: {solution.moveList.Count}    ToString: {solution}");
    }

    public void PlaySequenceFromHere()
    {
        if (!Cube.ReadyToRotate || rotationSequenceInProgress)
        {
            return;
        }
        //RotationSequence sequenceToPlay = currentRotationSequence.GetRemainingSequence();
        //Debug.Log(sequenceToPlay);
        isPaused = false;
        isCurrentlyReversed = false;
        StartRotationSequence();

        LogicalCube.Square test = LogicalCube.Square.FUR;
        LogicalCube.Move move = new LogicalCube.Move("U2");
        LogicalCube.MoveSequence seq = new LogicalCube.MoveSequence("M2");
        LogicalCube.Cube logicalCube = new LogicalCube.Cube();
        logicalCube.MakeMove(move);
        Debug.Log($"TEST: square is {test}");
        Debug.Log($"TEST: move is {move}");
        Debug.Log($"TEST: seq is {seq}");
        Debug.Log($"TEST: logicalCube is {logicalCube}");

    }

    public void ReverseSequenceFromHere()
    {

        if (!Cube.ReadyToRotate || rotationSequenceInProgress)
        {
            return;
        }
        //RotationSequence sequenceToPlay = currentRotationSequence.GetInverseSoFar();
        isCurrentlyReversed = true;
        //Debug.Log(sequenceToPlay);
        isPaused = false;
        StartRotationSequence();
    }

    public void ResetSequence()
    {
        if (!Cube.ReadyToRotate)
        {
            return;
        }
        
        //Cube.StartRotation(null, true);
        Cube.SetCubeFromCanonicalString(currentRotationStartPosition);
        UpdateCanonicalString();
        currentRotationSequence.MoveToBeginning();
    }

    public void ResetAndPlaySequence()
    {
        ResetSequence();
        PlaySequenceFromHere();
    }

    // Called when >> button is clicked
    public void DoCurrentMove()
    {
        if( currentRotationSequence.PeekForward() is null)
        {
            return;
        }

        if (!Cube.ReadyToRotate)
        {
            return;
        }

        UpdateCanonicalString();
        //CanonicalInputField.text = Cube.GetCanonicalString();
        isPaused = false;
        Cube.StartRotation(currentRotationSequence.GetForward());
    }

    public void UndoCurrentMove()
    {
        if (currentRotationSequence.PeekBackward() is null)
        {
            return;
        }

        if (!Cube.ReadyToRotate)
        {
            return;
        }

        UpdateCanonicalString();
        //CanonicalInputField.text = Cube.GetCanonicalString();
        //Cube.StartRotationSequence(null);
        isPaused = false;
        Cube.StartRotation(currentRotationSequence.GetBackward());
    }

    private void ProcessFreeRotation()
    {
        ContinueRotationSequence();
    }

    private void ProcessSolutionView()
    {
    }

    private void ProcessStickerInput()
    {
    }

    private void UpdateCanonicalString()
    {
        CanonicalInputField.text = Cube.CanonicalString;
    }
}
