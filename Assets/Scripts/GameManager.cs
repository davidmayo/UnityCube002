using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
        FreeRotation,
        Tutorial
    }
    public GameState State { get; private set; }
    public GameState PreviousState { get; private set; }


    [Header("Cube")]

    public string StartingPosition = "WWWWWWWWW/GGGGGGGGG/RRRRRRRRR/BBBBBBBBB/OOOOOOOOO/YYYYYYYYY";
    public string DefaultSequence = "M'2 U M'2 U2 M'2 U M'2";
    [Header("Rotation")]
    [Range(0f,10800f)]
    public float RotationSpeedDegreesPerSecond = 135f;
    [Range(0f, 1000f)]
    public float RotationDelayMilliseconds = 100f;
    public bool ShowProjections = true;
    [Range(0f,8f)]
    public float ProjectionThreshold = 1.5f;

    [Header("References")]

    public GameObject ModalPrefab;
    public GameObject CubePrefab;
    public PhysicalCube.Cube Cube;
    public InputField MoveInputField;
    public InputField CanonicalInputField;
    public List<Button> colorButtons;
    public Image CurrentColorDisplay;
    public Text TutorialHeader;
    public Text TutorialContent;
    public Button HelpButton;
    public Slider SpeedSlider;
    public Toggle ProjectionToggle;

    public GameObject UIBase;
    public GameObject UICubeControls;
    public GameObject UICaptionArea;
    public GameObject UICaptionControls;
    public GameObject UIInputControls;
    public GameObject UIGameOverPanel;


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
    private bool IsTutorial;
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
        PreviousState = GameState.FreeRotation;

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

    public void SetShowProjections()
    {
        ShowProjections = ProjectionToggle.isOn;
    }

    public void ExplodeAndExit()
    {
        PauseRotationSequence();

        this.StopAllCoroutines();
        //Cube.StopAllCoroutines();
        UIGameOverPanel.SetActive(true);
        StartCoroutine(ExplodeCubeCoroutine(true));
    }

    // Kaaaa-BOOOOOOM!!
    //public void ExplodeCube()
    //{
    //    PauseRotationSequence();
    //
    //    this.StopAllCoroutines();
    //    //Cube.StopAllCoroutines();
    //    StartCoroutine(ExplodeCubeCoroutine());
    //}

    public IEnumerator FadeOutAndExit()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        Image image = UIGameOverPanel.GetComponent<Image>();
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        //UIGameOverPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        while (color.a <= 1f)
        {
            color = image.color;
            color.a = color.a + .01f;
            image.color = color;

            yield return new WaitForSeconds(.01f);
        }

        yield return new WaitForSecondsRealtime(.2f);

        Text text = UIGameOverPanel.GetComponentInChildren<Text>();

        text.text = "THANK YOU\nFOR PLAYING";

        color = text.color;

        while (color.a <= 1f)
        {
            color = text.color;
            color.a = color.a + .01f;
            text.color = color;

            yield return new WaitForSeconds(.01f);
        }

        yield return new WaitForSecondsRealtime(1f);


        //GameObject newPanel = Instantiate(UIGameOverPanel);
        //newPanel.SetActive(true);
        Application.Quit();
        yield break;
    }
    public IEnumerator ExplodeCubeCoroutine(bool exitAfterExploding = true)
    {
        UIBase.SetActive(false);
        this.ShowProjections = false;

        PhysicalCube.CubeLocator loc = new PhysicalCube.CubeLocator();

        foreach( var projection in loc.GetProjections() )
        {
            //Destroy(projection.gameObject);

            foreach( BoxCollider boxCollider in projection.GetComponentsInChildren<BoxCollider>())
            {
                boxCollider.enabled = false;
            }

        }

        foreach( var piece in loc.GetPieces())
        {
            //piece.SetActive(false);
            foreach( Rigidbody rigidbody in piece.GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = false;

                //rigidbody.AddExplosionForce(1, Vector3.zero, 3);
                rigidbody.velocity = (piece.transform.position + Vector3.up * 5) + 5*UnityEngine.Random.insideUnitSphere;
                rigidbody.angularVelocity = piece.transform.position + 5*UnityEngine.Random.insideUnitSphere;


            }
        }

        if (exitAfterExploding)
        {
            StartCoroutine(FadeOutAndExit());
            yield break;
        }
        


        //var newCube = Instantiate(Cube);

        int maxCubes = 10;

        Color[] possibleCubeColors = {
            PhysicalCube.StickerColors.WhiteSticker,
            PhysicalCube.StickerColors.YellowSticker,
            PhysicalCube.StickerColors.GreenSticker,
            PhysicalCube.StickerColors.BlueSticker,
            PhysicalCube.StickerColors.RedSticker,
            PhysicalCube.StickerColors.OrangeSticker,

        };
        System.Random rand = new System.Random();

        Queue<GameObject> cubeClones = new Queue<GameObject>();

        do
        {
            yield return new WaitForSeconds(.7f);

            // Make a new cube

            Vector3 position = UnityEngine.Random.insideUnitSphere * 1.5f;
            position.y = position.y * 3 + 1;
            position.x += -1f;
            position.z += 1f;

            Quaternion rotation = UnityEngine.Random.rotationUniform;

            var newCube = Instantiate(CubePrefab, position, rotation);
            cubeClones.Enqueue(newCube);

            // If too many cubes, destroy the oldest one.
            if( cubeClones.Count > maxCubes)
            {
                var toDestroy = cubeClones.Dequeue();

                Destroy(toDestroy);
            }


            // Set all the stickers on the new cube
            Color newStickerColor = possibleCubeColors[rand.Next(possibleCubeColors.Length)];

            foreach( GameObject gameObject in GameObject.FindGameObjectsWithTag("Sticker"))
            {
                float threshold = 2.07f; // Distance from origin to <1, 1, 1.5> = sqrt(4.25) = 2.06155
                if( Vector3.Distance(gameObject.transform.position, position) > threshold)
                {
                    continue;
                }

                //if (gameObject.transform.position.x < positionOffset.x - threshold || gameObject.transform.position.x > positionOffset.x + threshold ||
                //    gameObject.transform.position.y < positionOffset.y - threshold || gameObject.transform.position.y > positionOffset.y + threshold ||
                //    gameObject.transform.position.z < positionOffset.z - threshold || gameObject.transform.position.z > positionOffset.z + threshold)
                //    continue;

                gameObject.GetComponent<MeshRenderer>().material.color = newStickerColor;
            }

            // Turn off the new projections
            foreach (var projection in GameObject.FindGameObjectsWithTag("Projection"))
            {
                MeshRenderer mr = projection.GetComponent<MeshRenderer>();
                Color newColor = mr.material.color;
                newColor.a = 0;
                mr.material.color = newColor;

                BoxCollider bc = projection.GetComponent<BoxCollider>();
                bc.enabled = false;
            }

            foreach (var rigidbody in newCube.GetComponentsInChildren<Rigidbody>())
            {
                //piece.SetActive(false);


                //foreach (Rigidbody rigidbody in piece.GetComponentsInChildren<Rigidbody>())
                {
                    rigidbody.isKinematic = false;

                    //rigidbody.AddExplosionForce(500f, position, 3f);
                    //rigidbody.velocity = (rigidbody.transform.position + Vector3.up * 5) + 5 * UnityEngine.Random.insideUnitSphere;
                    rigidbody.angularVelocity = rigidbody.transform.position + 5 * UnityEngine.Random.insideUnitSphere;
                    rigidbody.AddExplosionForce(rand.Next(1000), (position + new Vector3(0, -.75f + (float)rand.NextDouble(), 0)), 5f);

                }
            }
        } while (true);
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

    public void StartStickerInput()
    {
        if (State == GameState.StickerInput)
            SetGameState(PreviousState);
        else
            SetGameState(GameState.StickerInput);
    }
    public void FinishStickerInput()
    {
        SetGameState(this.PreviousState);
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
                var nextRotation = currentRotationSequence.GetBackward();
                Debug.Log($"Starting rotation {nextRotation} HEADER: {nextRotation.CaptionHeaderText} BODY: {nextRotation.CaptionBodyText}");
                RotateCube(nextRotation);
                //Cube.StartRotation(currentRotationSequence.GetBackward());
            }
        }

        // Move forward
        else
        {
            if (!currentRotationSequence.IsEnd)
            {
                rotationSequenceInProgress = true;
                var nextRotation = currentRotationSequence.GetForward();
                Debug.Log($"Starting rotation {nextRotation} HEADER: {nextRotation.CaptionHeaderText} BODY: {nextRotation.CaptionBodyText}");
                RotateCube(nextRotation);
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

        // Ready to start the rotation
        Debug.Log($"Starting rotation {rotation} HEADER: {rotation.CaptionHeaderText} BODY: {rotation.CaptionBodyText}");

        //// Update the caption, UNLESS it's in tutorial mode.
        //if (!IsTutorial)
        //{
        //    if (!string.IsNullOrWhiteSpace(rotation.CaptionHeaderText))
        //    {
        //        TutorialHeader.text = rotation.CaptionHeaderText;
        //    }
        //    else
        //    {
        //        TutorialHeader.text = rotation.MoveString;
        //    }
        //
        //
        //    if (!string.IsNullOrWhiteSpace(rotation.CaptionBodyText))
        //    {
        //        TutorialContent.text = rotation.CaptionBodyText;
        //    }
        //    else
        //    {
        //        TutorialContent.text = "Do move: " + rotation.MoveString;
        //    }
        //
        //}
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

    // Set rotation speed from 1 to 5
    public void SetRotationSpeedFromSlider()
    {
        float speed = SpeedSlider.value;
        float epsilon = .01f;
        if( speed < 1.0 + epsilon)
        {
            this.RotationSpeedDegreesPerSecond = 135f;
            this.RotationDelayMilliseconds = 250f;
        }
        else if( speed < 2.0 + epsilon)
        {
            this.RotationSpeedDegreesPerSecond = 180f;
            this.RotationDelayMilliseconds = 100f;
        }
        else if( speed < 3.0 + epsilon )
        {
            this.RotationSpeedDegreesPerSecond = 270f;
            this.RotationDelayMilliseconds = 50f;
        }
        else if ( speed < 4.0 + epsilon)
        {
            this.RotationSpeedDegreesPerSecond = 360;
            this.RotationDelayMilliseconds = 25f;
        }
        else
        {
            this.RotationSpeedDegreesPerSecond = 720f;
            this.RotationDelayMilliseconds = 10f;
        }
    }

    PhysicalCube.RotationSequence GenerateScramble(int scrambleLength = 15, System.Random random = null )
    {
        System.Random rand = random;

        if (rand is null)
            rand = new System.Random();

        string scrambleString = "";

        string[] axisSource = { "UD", "LR", "FB"};
        string[] modifierSource = { "", "'" };

        int previousAxis = -1;
        int axis;
        for ( int i = 0; i < scrambleLength; i++)
        {
            // Make sure this axis is different from previous axis.
            do
            {
                axis = rand.Next(3);
            } while (axis == previousAxis);
            previousAxis = axis;

            // Get one character from the axis
            scrambleString += axisSource[axis][rand.Next(0, 2)];

            // Add ' half the time
            if (rand.NextDouble() > 0.5)
                scrambleString += "'";

            scrambleString += " ";
        }

        scrambleString = scrambleString.Trim();

        return new PhysicalCube.RotationSequence(scrambleString);
    }

    public void ScrambleCube()
    {
        SetCube(StartingPosition);
        SetRotationSequence(GenerateScramble());
        PlaySequenceFromHere();
    }


    public void HelpButtonClicked()
    {
        if (State == GameState.Tutorial)
            SetGameState(this.PreviousState);
        else
            SetGameState(GameState.Tutorial);
        //Debug.Log($"Help Button Clicked {this}");
        //
        //if( IsTutorial)
        //{
        //    IsTutorial = false;
        //    HelpButton.GetComponentInChildren<Text>().text = "Start Tutorial";
        //    UICaptionControls.SetActive(false);
        //}
        //else
        //{
        //    HelpButton.GetComponentInChildren<Text>().text = "Stop Tutorial";
        //    IsTutorial = true;
        //    UICaptionControls.SetActive(true);
        //}
    }

    void UpdateStickerColor()
    {
        if (Input.GetMouseButtonDown(0) && State == GameState.StickerInput)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //hit.collider.gameObject;
                GameObject hitObject = hit.collider.gameObject;// hit.transform.gameObject;



                // Only want to take any action if the thing is visible
                // This will prevent clicking on invisible projections:
                MeshRenderer meshRenderer = hitObject.GetComponent<MeshRenderer>();

                float alphaValue = meshRenderer.material.color.a;

                if ((alphaValue > .99f) &&
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

                        int unknownCount = Cube.CanonicalString.Count(f => f == 'X');
                        int knownCount = 54 - unknownCount;

                        TutorialContent.text = $"So far so good.\n\n{knownCount} stickers determined.\n\n{unknownCount} stickers to go.";

                    }
                    catch (Exception exc)
                    {
                        TutorialContent.text = $"Uh-oh!\n\nThis cube is impossible.";
                        Debug.Log($"EXCEPTION: {exc}");
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Easter egg: SHIFT + ALT + X = infinitely exploding cube
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.X))
            StartCoroutine(ExplodeCubeCoroutine(false));

        UpdateStickerColor();
        ContinueRotationSequence();



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

    public void FreeRotateButtonClicked()
    {
        if (State == GameState.FreeRotation)
            SetGameState(PreviousState);
        else
            SetGameState(GameState.FreeRotation);
    }
    void SetGameState( GameState newState )
    {
        if (newState == this.State)
            return;
        
        PreviousState = State;
        State = newState;

        switch (newState)
        {
            case GameState.StickerInput:
                TutorialHeader.text = "Edit cube state";
                TutorialContent.text = "Set the colors of these stickers to match your actual cube.\n\n" + 
                    "Start with a preset state on the left, then select a color from the right and start clicking on the cube.\n\n" +
                    "I will start to magically figure out some stickers as you go.";

                UICubeControls.SetActive(false);
                UICaptionArea.SetActive(true);
                UICaptionControls.SetActive(false);
                UIInputControls.SetActive(true);
                break;
            case GameState.SolutionView:
                UICubeControls.SetActive(true);
                UICaptionArea.SetActive(true);
                UICaptionControls.SetActive(false);

                UIInputControls.SetActive(false);
                FindSolution();

                break;
            case GameState.FreeRotation:
                TutorialHeader.text = "Free Rotation";
                TutorialContent.text = "Rotate the cube however you'd like. Controls:\n\n" +
                    "Hold SHIFT to rotate counterclockwise. Otherwise, rotation will be clockwise.\n\n" +
                    "          U = Upper        D = Down face\n" +
                    "          F = Front          B = Back face\n" +
                    "          R = Right          L = Left face\n";
                UICubeControls.SetActive(true);
                UICaptionArea.SetActive(true);
                UICaptionControls.SetActive(false);
                UIInputControls.SetActive(false);
                break;
            case GameState.Tutorial:
                UICubeControls.SetActive(false);
                UICaptionArea.SetActive(true);
                UICaptionControls.SetActive(true);

                UIInputControls.SetActive(false);
                break;
            default:
                break;
        }
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

    public void SetRotationSequence(PhysicalCube.RotationSequence solutionSequence)
    {
        currentRotationStartPosition = Cube.CanonicalString;
        //unalteredRotationSequence = new RotationSequence(MoveInputField.text);
        currentRotationSequence = solutionSequence;
        isCurrentlyReversed = false;
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

    private void FindSolution()
    {
        TutorialHeader.text = "Finding solution . . . ";
        TutorialContent.text = "";

        LogicalCube.Cube logicalCube = new LogicalCube.Cube(Cube.CanonicalString);

        LogicalCube.Solution solution;
        try
        {
            solution = new LogicalCube.Solution(logicalCube);
            TutorialContent.text = "Found a valid solution.\n\nLet's get started!";
        }
        catch
        {
            TutorialContent.text = "That cube is not solvable.\n\nPlease double-check that you entered in your cube correctly. If you did, then you will need to take your cube apart and reassemble it.";
            return;
        }

        List<PhysicalCube.CubeRotation> solutionSteps = new List<PhysicalCube.CubeRotation>();

        // Create new CubeRotation for each move, preserving the captions
        foreach (var move in solution.moveList)
        {
            solutionSteps.Add(new PhysicalCube.CubeRotation(move.MoveString, move.CaptionHeader, move.CaptionText));
        }

        PhysicalCube.RotationSequence solutionSequence = new PhysicalCube.RotationSequence(solutionSteps);
        SetRotationSequence(solutionSequence);

        MoveInputField.text = solution.ToString();
    }
    public void GenerateSolutionButtonClicked()
    {
        SetGameState(GameState.SolutionView);
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
        if (State != GameState.FreeRotation || !Cube.IsReadyToRotate)
            return;

        bool invertDirection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        string moveString = "";

        if (Input.GetKeyDown(KeyCode.U))
            moveString = "U";
        else if (Input.GetKeyDown(KeyCode.D))
            moveString = "D";

        else if (Input.GetKeyDown(KeyCode.F))
            moveString = "F";
        else if (Input.GetKeyDown(KeyCode.B))
            moveString = "B";

        else if (Input.GetKeyDown(KeyCode.L))
            moveString = "L";
        else if (Input.GetKeyDown(KeyCode.R))
            moveString = "R";

        if (moveString == "")
            return;

        if (invertDirection)
            moveString += "'";

        PhysicalCube.CubeRotation rotation = new PhysicalCube.CubeRotation(moveString);
        this.RotateCube(rotation);
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
