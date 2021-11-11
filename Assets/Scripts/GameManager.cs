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
    public PhysicalCube.Cube Cube;
    public InputField MoveInputField;
    public InputField CanonicalInputField;
    public List<Button> colorButtons;
    public Image CurrentColorDisplay;



    // Rotation Sequence stuff
    [Header("Debug stuff")]
    public string DebugString;
    private string currentRotationStartPosition;
    //[SerializeField]
    //private RotationSequence unalteredRotationSequence;
    [SerializeField]
    private PhysicalCube.RotationSequence currentRotationSequence;
    [SerializeField]
    private bool isPaused;

    [SerializeField]
    private bool rotationSequenceInProgress;
    [SerializeField]
    bool isCurrentlyReversed = false;
    [SerializeField]
    bool isMidRotationSequence = false;

    [Header("User input mode")]
    [SerializeField]
    private Color currentColor;

    // Start is called before the first frame update
    void Start()
    {
        currentRotationSequence = new PhysicalCube.RotationSequence("");
        currentRotationStartPosition = StartingPosition;
        rotationSequenceInProgress = false;
        isPaused = false;

        Cube.SetCubeFromCanonicalString(StartingPosition);
        //Cube.SetCubeFromCanonicalString("GGWOYYGWY/RGBWRYOGW/ROOBGRRRB/GOOGOYRRY/YWYRBBBBW/BYGOWWOBW");
        SetGameState(GameState.FreeRotation);

        MoveInputField.text = DefaultSequence;
        //MoveInputField.text = "x2 y D D D F2 y D F2 y y y F D F' y y D F2 y D D D D R F' R' x2 R U R' U' R U R' U' R U R' U' R U R' U' R U R' U' y y y R U R' U' R U R' U' y y y U U R U R' U' R U R' U' R U R' U' y y y U U R U R' U' R U R' U' R U R' U' R U R' U' R U R' U' y y y U2 F' U' F U R U R' y y U2 F' U' F U R U R' y U U U2 F' U' F U R U R' y y F R U R' U' F' F R U R' U' F' U U U U y R U R' U R U2 R' U U R U' L' U R' U' L y U R U' L' U R' U' L x2 y y y y R U R' U' R U R' U' D R U R' U' R U R' U' D R U R' U' R U R' U' D D x2 y2 z2 x2 y2 z2";
        UpdateCanonicalString();

        foreach( Button button in colorButtons)
        {
            button.image.color = PhysicalCube.StickerColors.BlueSticker;
            Color target;
            switch (button.name)
            {
                case "ColorWhiteButton":
                    target = PhysicalCube.StickerColors.WhiteSticker;
                    break;
                case "ColorYellowButton":
                    target = PhysicalCube.StickerColors.YellowSticker;
                    break;

                case "ColorGreenButton":
                    target = PhysicalCube.StickerColors.GreenSticker;
                    break;
                case "ColorBlueButton":
                    target = PhysicalCube.StickerColors.BlueSticker;
                    break;

                case "ColorRedButton":
                    target = PhysicalCube.StickerColors.RedSticker;
                    break;
                case "ColorOrangeButton":
                    target = PhysicalCube.StickerColors.OrangeSticker;
                    break;

                default:
                    target = PhysicalCube.StickerColors.UnknownSticker;
                    break;
            }
            button.image.color = target;
        }

        SetColor("unknown");
    }

    public void SetCube(string newString )
    {
        if (newString.Trim().ToUpper() == "SOLVED")
            Cube.SetCubeFromCanonicalString("WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY");
        else if( newString.Trim().ToUpper() == "BLANK")
            Cube.SetCubeFromCanonicalString("XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX");
        else if (newString.Trim().ToUpper() == "CENTERS")
            Cube.SetCubeFromCanonicalString("XXXXWXXXX/XXXXGXXXX/XXXXRXXXX/XXXXBXXXX/XXXXOXXXX/XXXXYXXXX");
        else
        {
            try
            {
                Cube.SetCubeFromCanonicalString(newString);
            }
            catch( Exception )
            {
                Debug.Log($"Invalid canonical string {newString}. Setting to blank instead");
                Cube.SetCubeFromCanonicalString("XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX/XXXXXXXXX");
            }
        }

        UpdateCanonicalString();
    }

    public void SetColor(string color = "")
    {
        switch( color )
        {
            case "white":
                currentColor = PhysicalCube.StickerColors.WhiteSticker;
                break;
            case "yellow":
                currentColor = PhysicalCube.StickerColors.YellowSticker;
                break;

            case "green":
                currentColor = PhysicalCube.StickerColors.GreenSticker;
                break;
            case "blue":
                currentColor = PhysicalCube.StickerColors.BlueSticker;
                break;

            case "red":
                currentColor = PhysicalCube.StickerColors.RedSticker;
                break;
            case "orange":
                currentColor = PhysicalCube.StickerColors.OrangeSticker;
                break;

            default:
                currentColor = PhysicalCube.StickerColors.UnknownSticker;
                break;
        }

        CurrentColorDisplay.color = currentColor;
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

    //private void StartRotationSequence(PhysicalCube.RotationSequence newSequence )
    //{
    //    if (rotationSequenceInProgress || !Cube.IsReadyToRotate)
    //        return;
    //    if (newSequence is null || newSequence.Count == 0)
    //    {
    //        rotationSequenceInProgress = false;
    //        //currentRotationSequence = RotationSequence.EmptySequence;
    //        return;
    //    }
    //    currentRotationSequence = newSequence;
    //    isPaused = false;
    //    StartRotationSequence();
    //}

    private void StartRotationSequence()
    {
        if (rotationSequenceInProgress || !Cube.IsReadyToRotate)
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
                RotateCube(currentRotationSequence.GetBackward());
                //Cube.StartRotation(currentRotationSequence.GetBackward());
            }
        }

        // Move forward
        else
        {
            if (!currentRotationSequence.IsEnd)
            {
                rotationSequenceInProgress = true;
                RotateCube(currentRotationSequence.GetForward());
                //Cube.StartRotation(currentRotationSequence.GetForward());
            }
        }

        
        
    }

    private void RotateCube(PhysicalCube.CubeRotation rotation)
    {
        StartCoroutine(RotateCubeCoroutine(rotation));
    }
    IEnumerator RotateCubeCoroutine(PhysicalCube.CubeRotation rotation)
    {
        // Wait until cube is ready
        while ( !Cube.IsReadyToRotate )
        {
            yield return null;
        }

        // Do the rotation coroutine
        yield return StartCoroutine(Cube.RotateCoroutine(rotation));

        // Rotation is finished
        UpdateCanonicalString();
    }
    private void ContinueRotationSequence()
    {
        if (isPaused)
            return;
        if (!rotationSequenceInProgress)
            return;
        if (!Cube.IsReadyToRotate)
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
                RotateCube(currentRotationSequence.GetBackward());
                //Cube.StartRotation(currentRotationSequence.GetBackward());
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
                RotateCube(currentRotationSequence.GetForward());
                //Cube.StartRotation(currentRotationSequence.GetForward());
            }
        }

        
    }

    

    public void HelpButtonClicked()
    {
        Debug.Log($"Help Button Clicked {this}");

        Cube.SetCubeFromCanonicalString("WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY");
        
        
        //Instantiate(ModalPrefab);
        //Canvas canvas = ModalPrefab.GetComponent<Canvas>();
        //canvas.enabled = false;
        //// Code to set the modal stuff goes here
        //canvas.enabled = true;
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;

                // Only want to take any action if the thing is visible
                // This will prevent clicking on invisible projections:
                MeshRenderer meshRenderer = hitObject.GetComponent<MeshRenderer>();

                float alphaValue = meshRenderer.material.color.a;
        
                if ( (alphaValue > .99f) &&
                     (hitObject.CompareTag("Sticker") || hitObject.CompareTag("Projection")))
                {
                    Cube.SetStickerColor(hitObject, currentColor);
                    UpdateCanonicalString();

                    try
                    {
                        string canon = Cube.CanonicalString;
                        Debug.Log($"canon=[{canon}]");
                        string forcedCube = LogicalCube.CubeValidator.GetForcedCube(Cube.CanonicalString);
                        Debug.Log($"forcedCube=[{forcedCube}]");

                        Cube.SetCubeFromCanonicalString(forcedCube);
                        UpdateCanonicalString();
                    }
                    catch (Exception exc)
                    {
                        Debug.Log($"EXCEPTION: {exc}");
                    }
                }
            }
            

            
        }
        //UpdateCanonicalString();
        //switch (this.State)
        //{
        //    case GameState.StickerInput:
        //        ProcessStickerInput();
        //        break;
        //    case GameState.SolutionView:
        //        ProcessSolutionView();
        //        break;
        //    case GameState.FreeRotation:
        //        ProcessFreeRotation();
        //        break;
        //    default:
        //        break;
        //}

        ProcessFreeRotation();

        // Set Cube adjustable parameters
        // Do this every frame only for debugging
        Cube.DegreesPerSecond = this.RotationSpeedDegreesPerSecond;
        Cube.DelayMilliseconds = this.RotationDelayMilliseconds;
        Cube.ProjectionVisibilityThreshold = this.ProjectionThreshold;
        Cube.ShowProjections = this.ShowProjections;
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
        SetRotationSequence(MoveInputField.text);
        //currentRotationStartPosition = Cube.CanonicalString;
        ////unalteredRotationSequence = new RotationSequence(MoveInputField.text);
        //currentRotationSequence = new PhysicalCube.RotationSequence(MoveInputField.text);
        //isCurrentlyReversed = false;
    }

    public void SetRotationSequence(string moveSequenceString)
    {
        currentRotationStartPosition = Cube.CanonicalString;
        //unalteredRotationSequence = new RotationSequence(MoveInputField.text);
        currentRotationSequence = new PhysicalCube.RotationSequence(moveSequenceString);
        isCurrentlyReversed = false;
    }

    public void UpdateButtonClicked()
    {
        Debug.Log("Update button clicked");
        Debug.Log(CanonicalInputField.text);
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


    public void FlipCube()
    {
        currentRotationSequence = new PhysicalCube.RotationSequence("x2 y'");
        PlaySequenceFromHere();
    }
    public void PlaySequenceFromHere()
    {
        if (!Cube.IsReadyToRotate || rotationSequenceInProgress)
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

        if (!Cube.IsReadyToRotate || rotationSequenceInProgress)
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
        if (!Cube.IsReadyToRotate)
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

        if (!Cube.IsReadyToRotate)
        {
            return;
        }

        UpdateCanonicalString();
        //CanonicalInputField.text = Cube.GetCanonicalString();
        isPaused = false;
        RotateCube(currentRotationSequence.GetForward());
        //Cube.StartRotation(currentRotationSequence.GetForward());
    }

    public void UndoCurrentMove()
    {
        if (currentRotationSequence.PeekBackward() is null)
        {
            return;
        }

        if (!Cube.IsReadyToRotate)
        {
            return;
        }

        UpdateCanonicalString();
        //CanonicalInputField.text = Cube.GetCanonicalString();
        //Cube.StartRotationSequence(null);
        isPaused = false;
        RotateCube(currentRotationSequence.GetBackward());
        //Cube.StartRotation(currentRotationSequence.GetBackward());
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
        if( !string.IsNullOrWhiteSpace(Cube.CanonicalString))
            CanonicalInputField.text = Cube.CanonicalString;
    }
}
