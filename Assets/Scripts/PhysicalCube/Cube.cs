using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhysicalCube
{

    /// <summary>
    /// Class to control the physical motions of the 3D cube,
    /// Including setting colors rotating faces.
    /// </summary>
    public class Cube : MonoBehaviour
    {
        /// <summary>
        /// The cube's canonical string: 54 chars representing the color of each sticker, with faces separated by '/'
        /// </summary>
        public string CanonicalString { get; private set; }
        //public string CanonicalString { get => loc.GetCanonicalString(); set => SetCubeFromCanonicalString(value); }

        /// <summary>
        /// Whether or not the projections should be displayed
        /// </summary>
        public bool ShowProjections;

        // TODO: Move color picker stuff to GameManager or a new UserInput class
        /// <summary>
        /// For the color picker
        /// </summary>
        public Color currentColor;

        // Field to display/read the canonical string
        // TODO: delete this
        public InputField canonicalInputField;

        // Field to display/read the canonical string
        // TODO: delete this
        public InputField moveInputField;

        // Recommended values:
        // Slow: 135
        // Moderate: 270
        // Fast: 540
        // Very fast: 1080
        // Anything above ~2500 is too fast for interpolation to work consistently,
        // which will lead to pieces being misaligned and putting the cube into an
        // unrecoverable state.
        /// <summary>
        /// Desired rotational speed of the cube. HIGHLY recommended to set well below 2500.
        /// </summary>
        public float DegreesPerSecond = 135f;

        // Recommended values:
        // Slow: 250
        // Moderate: 100
        // Fast: 50
        // Very fast: 0 or 25
        /// <summary>
        /// The time to delay between rotations
        /// </summary>
        public float DelayMilliseconds = 100f;

        // To have the the projections always visible: greater than 6
        // To hide the U, F, and R projections "at rest", stay in this range: 
        // Minimum travel visible: sqrt(2) = 1.414
        // Maximum travel visible: 6 / sqrt(2) = 4.242
        // Don't use any other value, or you will get glitches.
        /// <summary>
        /// Threshold value for determining when to hide the projections. Recommend 1.5 < value < 4, or value > 6
        /// </summary>
        [Range(0f, 7f)]
        public float ProjectionVisibilityThreshold = 1.5f;

        // CubeLocator object to find specific stickers/projections/pieces
        private CubeLocator loc;

        /// <summary>
        /// Is cube ready to accept input?
        /// </summary>
        public bool IsReadyToRotate { get; private set; }

        /// <summary>
        /// The time when the delay 
        /// </summary>
        //public DateTime NextRotationTime { get; private set; }

        // The rotation that is currently happening
        private CubeRotation currentRotation;

        // How far the current rotation has actually gone
        // Starts at 0 and is updated once per frame in ContinueCurrentRotation() function
        // Until desired rotation angle is achieved
        private float currentRotationAngle;


        /// <summary>
        /// Initialized at scene creation
        /// </summary>
        void Start()
        {
            // Initialize stuff
            ShowProjections = false;
            loc = new CubeLocator();
            currentColor = Color.white;
            //NextRotationTime = DateTime.Now.AddMilliseconds(DelayMilliseconds);
            IsReadyToRotate = true;

            CanonicalString = loc.GetCanonicalString();
        }

        public void InitializeStuff()
        {
            Debug.Log($"InitializeStuff() called on {this}");
            // Initialize stuff
            ShowProjections = false;
            loc = new CubeLocator();
            currentColor = Color.white;
            //NextRotationTime = DateTime.Now.AddMilliseconds(DelayMilliseconds);
            IsReadyToRotate = true;

            CanonicalString = loc.GetCanonicalString();
        }

        /// <summary>
        /// Run every frame update
        /// </summary>
        void Update()
        {
            // TODO: Move the color selection stuff to GameManager class
            // (Don't delete the commented out code yet though) - Mayo 11/4/21
            //if (Input.GetMouseButtonDown(0))
            //{
            //    RaycastHit hit;
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //
            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        GameObject hitObject = hit.transform.gameObject;
            //
            //        if (hitObject.CompareTag("Sticker") || hitObject.CompareTag("Projection"))
            //        {
            //            SetStickerColor(hitObject);
            //        }
            //    }
            //}

            // Update projection visibility every frame, because the user could change the
            // setting at any point
            //UpdateProjectionOpacity();
        }

        /// <summary>
        /// The main coroutine to rotate the cube smoothly over multiple frames, implemented as a Unity Coroutine.
        /// </summary>
        public IEnumerator RotateCoroutine( CubeRotation rotation )
        {
            while( !IsReadyToRotate )
            {
                //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} Not ready to rotate. Waiting. ");
                yield return null;
            }

            // Re-establish the lockout
            IsReadyToRotate = false;

            // Initialize
            currentRotation = rotation;
            currentRotationAngle = 0f;

            //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} Starting rotation {currentRotation}. ");

            // As long as the cube hasn't turned the target number of degrees, there's more
            // rotation still to be done
            while ( currentRotationAngle < currentRotation.Angle)
            {
                // Angle to move per frame (in degrees) is
                // (desired speed in degrees/second) * (length of frame in seconds) * (direction of rotation [-1 or 1])
                float frameAngleToRotate = Time.deltaTime * DegreesPerSecond * currentRotation.Direction;

                // Iterate over the moving pieces and rotate them
                // Stickers and Projections are children of pieces, so only the pieces need to be moved
                foreach (var piece in loc.GetPieces(currentRotation.FaceLike))
                {
                    // Rotate each piece about the origin (center of cube), around its rotation axis,
                    // by the calculated angle

                    // TODO: Change this to Rotate(), since RotateAround() is deprecated
                    piece.transform.RotateAround(Vector3.zero, currentRotation.RotationAxis, frameAngleToRotate);
                }

                // Update the actual angle rotated
                currentRotationAngle += Mathf.Abs(frameAngleToRotate);

                //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} Rotated by {frameAngleToRotate} deg.   Current={currentRotationAngle} deg    Target={currentRotation.Angle} deg. Deferring to next frame");
                yield return null;
            }

            // Rotation is now done.
            // Call FinishRotation to square up all the pieces.
            //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} Rotation is done. Calling FinishRotation()");
            FinishRotation();
            
            //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} FinishRotation() completed. Beginning delay");
            yield return new WaitForSeconds(DelayMilliseconds / 1000f);

            // After delay, cube is ready to rotate again.
            //Debug.Log($"{DateTime.Now:HH:mm:ss.FFFF} Delay is done. Cube is ready to rotate.");
            IsReadyToRotate = true;

            // End couroutine
            yield break;
        }

        public void StartRotation(CubeRotation rotation)
        {
            StartCoroutine(RotateCoroutine(rotation));
        }

        /// <summary>
        /// Reset things and square up the piece locations when a rotation finishes
        /// </summary>
        private void FinishRotation()
        {
            // Iterate over all pieces and set their position and rotation precisely
            // To deal with floating point drift
            // 
            // Stickers and Projections are children of pieces, so only need to
            // fix the Pieces themselves
            foreach (var piece in loc.GetPieces())
            {
                piece.transform.position = GetSquaredPosition(piece);
                piece.transform.eulerAngles = GetSquaredRotation(piece);
            }


            // Update the canonical string
            CanonicalString = loc.GetCanonicalString();
        }

        /// <summary>
        /// Get a Vector3 where the values are all rounded to the nearest orthogonal angle (0, 90, 180, or 270 degrees)
        /// </summary>
        /// <param name="piece">A GameObject whose .transform.rotation.eulerAngles will be rounded</param>
        /// <returns></returns>
        private Vector3 GetSquaredRotation(GameObject piece)
        {
            Vector3 returnValue = piece.transform.rotation.eulerAngles;

            // Round each value to the closest angle
            for (int index = 0; index < 3; index++)
            {
                float value = returnValue[index];

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

                returnValue[index] = value;
            }
            return returnValue;
        }

        /// <summary>
        /// Get a Vector3 where the values are all rounded to the nearest of -1, 0, or 1.
        /// </summary>
        /// <param name="piece">A GameObject with a .transform.position</param>
        /// <returns></returns>
        private Vector3 GetSquaredPosition(GameObject piece)
        {
            Vector3 returnValue = piece.transform.position;

            // Round each value to the closest angle
            for (int index = 0; index < 3; index++)
            {
                float value = returnValue[index];

                if (value < -.5f)
                    value = -1f;
                else if (value < .5f)
                    value = 0f;
                else
                    value = 1f;

                returnValue[index] = value;
            }

            return returnValue;
        }

        /// <summary>
        /// Set the cube from a given canonical string
        /// </summary>
        /// <param name="canonicalString">The canonical string</param>
        public void SetCubeFromCanonicalString(string canonicalString)
        {
            // Remove leading/trailing whitespace and '/', and convert to upper case
            string strippedCanonicalString = canonicalString.Trim().Replace("/", "").ToUpper();

            if (strippedCanonicalString.Length != 54)
                throw new UnityException($"Invalid canonical string length {strippedCanonicalString.Length }. Original string: {canonicalString}");

            // Iterate over the 54 chars in the string
            // the index of the char in the string
            // is the same as the index of the sticker
            for (int index = 0; index < 54; index++)
            {
                // Get the sticker referred to by the given index
                // This is the object that's going to get its color set this iteration
                GameObject sticker = loc.GetStickerGameObject(index);

                Color color;

                // Determine what the color should be
                switch (strippedCanonicalString[index])
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

                // Actually set the color
                // This will also set the Projection color
                SetStickerColor(sticker, color);
            }

            CanonicalString = loc.GetCanonicalString();
        }

        /// <summary>
        /// Set the color of a sticker/projection combo. If the gameObject is not a Sticker or Projection, nothing happens.
        /// </summary>
        /// <param name="gameObject">A Sticker or Projection</param>
        /// <param name="color">Desired color. Alpha channel WILL be used</param>
        public void SetStickerColor(GameObject gameObject, Color color)
        {
            if (gameObject is null)
                return;

            // If the object is a sticker, we need to set the object's color
            // and set the object's child's color.
            if (gameObject.CompareTag("Sticker"))
            {
                // Set sticker color
                gameObject.GetComponent<MeshRenderer>().material.color = color;

                // Set projection color
                // GetComponentInChildren() includes self, it will (usually) return its own component
                // Therefore, get them all, iterate over them, and find the one where the name contains
                // "projection", which is the name of the projection prefab
                // (There really doesn't seem to be an efficient way to select a GameObject's DIRECT child.)
                foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    if (meshRenderer.name.Contains("projection"))
                        meshRenderer.material.color = color;
                }
            }

            // If the object is a sticker, we need to set the object's color
            // and set the object's parent's color.
            else if (gameObject.CompareTag("Projection"))
            {
                // Set sticker color
                // GetComponentsInParent() will return ALL MeshRenderers in ALL parents back to the root,
                // so iterate over all of them and only adjust the ones where the name contains "face",
                // since that's what the sticker prefabs are called.
                // (There really doesn't seem to be an efficient way to select a GameObject's DIRECT parent.)
                foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInParent<MeshRenderer>())
                {
                    if (meshRenderer.name.Contains("face"))
                        meshRenderer.material.color = color;
                }

                // Set projection color.
                gameObject.GetComponent<MeshRenderer>().material.color = color;
            }

            CanonicalString = loc.GetCanonicalString();
        }

        /// <summary>
        /// Update each Projection's opacity, based on the class's VisibilityThreshold and ShowProjections properties
        /// </summary>
        public void UpdateProjectionOpacity()
        {
            foreach (var projection in loc.GetProjections())
            {
                MeshRenderer meshRenderer = projection.GetComponent<MeshRenderer>();

                // Get the current color of the Projection
                // The RGB values will not be modified, but the A value will.
                Color color = meshRenderer.material.color;

                // if piece is outside the threshold OR ShowProjections is false,
                // set alpha to 0 (fully transparent)
                if (!ShowProjections ||
                     projection.transform.position.y > ProjectionVisibilityThreshold ||
                     projection.transform.position.x > ProjectionVisibilityThreshold ||
                     projection.transform.position.z < -ProjectionVisibilityThreshold
                    )
                {
                    color.a = 0f;
                }

                // Otherwise, set alpha to 1 (fully opaque)
                else
                {
                    color.a = 1f;
                }

                // Set the color
                meshRenderer.material.color = color;
            }
        }
    }
}