using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cube : MonoBehaviour
{
    public enum CubeStatus
    {
        Rotating,

    }
    public bool ShowProjections;
    public Color currentColor;

    private List<GameObject> stickers;

    public InputField canonicalInputField;
    public InputField moveInputField;

    //[Range(0f,1080f)]
    public float DegreesPerSecond = 135f;

    //[Range(0f, 1000f)]
    public float DelayMilliseconds = 100f;

    //[SerializeField]
    //private string testString;

    [Range(0f, 6f)]
    public float ProjectionVisibilityThreshold = 4.0f;

    private CubeLocator loc;

    private RotationSequence currentRotationSequence;

    /// <summary>
    /// Is the cube currently rotating
    /// </summary>
    public bool IsRotating { get; private set; }
    public bool ReadyToRotate { get => (!IsRotating && DateTime.Now > NextRotationTime); }
    public DateTime NextRotationTime { get; private set; }

    private CubeRotation currentRotation;
    private float currentRotationActualDegrees;

    // Start is called before the first frame update
    void Start()
    {
        ShowProjections = false;
        loc = new CubeLocator();

        currentColor = Color.white;

        stickers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Sticker"));
        Debug.Log("stickers length=" + stickers.Count);
        for( int i = 0; i < 54; i++ )
        {
            //Debug.Log(string.Format("{0} sticker position={1}", i, stickers[i].transform.position));
        }

        NextRotationTime = DateTime.Now.AddMilliseconds(DelayMilliseconds);
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

                if (hitObject.CompareTag("Sticker") || hitObject.CompareTag("Projection"))
                {
                    SetStickerColor(hitObject);
                    UpdateCanonicalString();
                }
            }
        }

        if (IsRotating)
            ContinueCurrentRotation();
        else
            ContinueRotationSequence();

        UpdateProjectionOpacity();
    }

    void ContinueRotationSequence()
    {
        if (!ReadyToRotate || currentRotationSequence is null)
            return;
        if (!(currentRotationSequence.PeekForward() is null))
        {
            StartRotation(currentRotationSequence.GetForward());
        }
    }

    public void StartRotationSequence(RotationSequence sequence )
    {
        currentRotationSequence = sequence;
        StartRotationSequence();
    }
    void StartRotationSequence()
    {
        CubeRotation rotation = currentRotationSequence.GetForward();

        if (rotation is null)
            return;

        StartRotation(rotation);
    }

    public void StartRotation( CubeRotation rotation, bool cancelRotationSequence = false )
    {
        if( !ReadyToRotate )
        {
            return;
        }
        if (cancelRotationSequence)
            currentRotationSequence = new RotationSequence("");
        currentRotation = rotation;
        currentRotationActualDegrees = 0f;
        IsRotating = true;
    }

    void ContinueCurrentRotation()
    {
        if( currentRotation is null )
        {
            return;
        }
        if( currentRotationActualDegrees < currentRotation.Angle )
        {
            float angleToRotate = Time.deltaTime * DegreesPerSecond;

            foreach ( var piece in loc.GetPieces(currentRotation.FaceLike) )
            {
                //piece.transform.Rotate(currentRotation.RotationAxis, angleToRotate, Space.World);
                piece.transform.RotateAround(Vector3.zero, currentRotation.RotationAxis, angleToRotate * currentRotation.Direction);
                //piece.transform.Rotate()
                //piece.transform.Rotate()
                //Debug.Log($"currentDegrees = {currentRotationActualDegrees}   angleToRotate = {angleToRotate}");
            }

            currentRotationActualDegrees += angleToRotate;
            
        }
        else
        {
            FinishRotation();
        }
    }

    void FinishRotation()
    {
        if (!IsRotating)
            return;

        

        foreach( var piece in loc.GetPieces())
        {
            piece.transform.position = GetSquaredPosition(piece);
            piece.transform.eulerAngles = GetSquaredRotation(piece);
        }

        UpdateCanonicalString();

        loc.Refresh();

        IsRotating = false;
        NextRotationTime = DateTime.Now.AddMilliseconds(DelayMilliseconds);

        
    }
    private Vector3 GetSquaredRotation(GameObject piece)
    {
        Vector3 vector = piece.transform.rotation.eulerAngles;

        for( int index = 0; index < 3; index++)
        {
            float value = vector[index];

            if (value < 45f)
                value = 0f;
            else if (value < 135f)
                value = 90f;
            else if (value < 225f)
                value = 180f;
            else if (value < 315f)
                value = 270f;
            else
                value = 0f;

            vector[index] = value;
        }

        return vector;
    }

    public string GetCanonicalString()
    {
        return loc.GetCanonicalString();
    }
    private void UpdateCanonicalString()
    {
        canonicalInputField.text = loc.GetCanonicalString();
    }

    private Vector3 GetSquaredPosition(GameObject piece)
    {
        Vector3 vector = piece.transform.position;

        for (int index = 0; index < 3; index++)
        {
            float value = vector[index];

            if (value < -.5f)
                value = -1f;
            else if (value < .5f)
                value = 0f;
            else
                value = 1f;

            vector[index] = value;
        }

        return vector;
    }

    public void SetStickerColor(GameObject obj)
    {
        SetStickerColor(obj, currentColor);
        //if( obj.CompareTag("Sticker"))
        //{
        //    // Set sticker color
        //    obj.GetComponent<MeshRenderer>().material.color = currentColor;
        //
        //    // Set projection color
        //    foreach (MeshRenderer meshRenderer in obj.GetComponentsInChildren<MeshRenderer>())
        //    {
        //        meshRenderer.material.color = currentColor;
        //    }
        //}
        //else if(obj.CompareTag("Projection"))
        //{
        //    // Set sticker color
        //    //MeshRenderer parentMeshRenderer = obj.GetComponentInParent<MeshRenderer>();
        //    //parentMeshRenderer.material.color = currentColor;
        //
        //    foreach (MeshRenderer meshRenderer in obj.GetComponentsInParent<MeshRenderer>())
        //    {
        //        Debug.Log($"Found MeshRenderer {meshRenderer}");
        //        if( meshRenderer.name.Contains("face") )
        //            meshRenderer.material.color = currentColor;
        //    }
        //
        //    // Set projection color.
        //    obj.GetComponent<MeshRenderer>().material.color = currentColor;
        //}
    }

    public void RotateButtonClicked()
    {
        RotationSequence sequence = new RotationSequence(moveInputField.text);

        Debug.Log(sequence);

        currentRotationSequence = sequence;
        StartRotationSequence();

        //CubeRotation rotation = new CubeRotation(moveInputField.text);
        //Debug.Log(rotation);
        //StartRotation(rotation);
    }

    public void UpdateButtonClicked( string argument = "")
    {
        SetCubeFromCanonicalString(canonicalInputField.text);

        //foreach( var piece in loc.GetPieces(testString))
        //{
        //    piece.transform.position *= 1.1f;
        //}
        //
        //CubeRotation rotation = new CubeRotation("U2");
        //Debug.Log(rotation);
    }

    public void SetCubeFromCanonicalString( string canonicalString )
    {
        string strippedCanonicalString = canonicalString.Trim().Replace("/", "").ToUpper();

        if( strippedCanonicalString.Length != 54 )
        {
            throw new UnityException("Invalid canonical string length");
        }

        for( int index = 0; index < 54; index++ )
        {
            GameObject sticker = loc.GetStickerGameObject(index);
            Color color;

            switch( strippedCanonicalString[index])
            {
                case 'W':
                    color = StickerColors.WhiteSticker;
                    break;
                case 'Y':
                    color = StickerColors.YellowSticker;
                    break;
                case 'G':
                    color = StickerColors.GreenSticker;
                    break;
                case 'B':
                    color = StickerColors.BlueSticker;
                    break;
                case 'R':
                    color = StickerColors.RedSticker;
                    break;
                case 'O':
                    color = StickerColors.OrangeSticker;
                    break;
                default:
                    color = StickerColors.UnknownSticker;
                    break;
            }

            SetStickerColor(sticker, color);
        }
        UpdateCanonicalString();
    }

    public void SetStickerColor( GameObject sticker, Color color )
    {
        if (sticker.CompareTag("Sticker"))
        {
            // Set sticker color
            sticker.GetComponent<MeshRenderer>().material.color = color;

            // Set projection color
            foreach (MeshRenderer meshRenderer in sticker.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material.color = color;
            }
        }
        else if (sticker.CompareTag("Projection"))
        {
            // Set sticker color
            //MeshRenderer parentMeshRenderer = obj.GetComponentInParent<MeshRenderer>();
            //parentMeshRenderer.material.color = currentColor;

            foreach (MeshRenderer meshRenderer in sticker.GetComponentsInParent<MeshRenderer>())
            {
                Debug.Log($"Found MeshRenderer {meshRenderer}");
                if (meshRenderer.name.Contains("face"))
                    meshRenderer.material.color = color;
            }

            // Set projection color.
            sticker.GetComponent<MeshRenderer>().material.color = color;
        }
        //sticker.GetComponent<MeshRenderer>().material.color = color;
    }

    public void UpdateProjectionOpacity()
    {
        
        foreach( var projection in loc.GetProjections())
        {
            MeshRenderer meshRenderer = projection.GetComponent<MeshRenderer>();
            Color color = meshRenderer.material.color;

            if ( projection.transform.position.y > ProjectionVisibilityThreshold ||
                 projection.transform.position.x > ProjectionVisibilityThreshold ||
                 projection.transform.position.z < -ProjectionVisibilityThreshold
                )
            {
                color.a = 0f;
            }
            else
            {
                color.a = 1f;
            }

            if (!ShowProjections)
            {
                color.a = 0f;
            }
            meshRenderer.material.color = color;

        }
    }

    public void SetCurrentColor(string color)
    {
        switch ( color.ToLower() )
        {
            case "white":
                currentColor = StickerColors.WhiteSticker;
                break;
            case "yellow":
                currentColor = StickerColors.YellowSticker;
                break;
            case "green":
                currentColor = StickerColors.GreenSticker;
                break;
            case "blue":
                currentColor = StickerColors.BlueSticker;
                break;
            case "red":
                currentColor = StickerColors.RedSticker;
                break;
            case "orange":
                currentColor = StickerColors.OrangeSticker;
                break;
            case "blank":
                currentColor = StickerColors.UnknownSticker;
                break;
            default:
                currentColor = StickerColors.UnknownSticker;
                throw new UnityException("Invalid color: " + color);
        }
        
    }
}
