using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro
{
    /// <summary>
    /// A camera which smoothly centers on the character and also allows movement between zones.
    /// </summary>
    public class PlatformerProStandardCamera : PlatformCamera
    {
        #region members

        /// <summary>
        /// How fast to move to the new camera zone.
        /// </summary>
        [Tooltip ("How fast to move to the new camera zone.")]
        public float transitionSpeed;

        /// <summary>
        /// How to move to the new camera zone.
        /// </summary>
        [Tooltip ("How to move to the new camera zone.")]
        public TweenMode tweenMode;


        /// <summary>
        /// If true the character will be completely frozen during the transition.
        /// </summary>
        [Tooltip ("If true the character will be completely frozen during transition.")]
        public bool freezePlayerDuringTransition;

        [Header ("Camera Movement")]
        /// <summary>
        /// Follow settings for xAxis;
        /// </summary>
        public CameraAxisSettings xAxis;

        /// <summary>
        /// Follow settings for yAxis;
        /// </summary>
        public CameraAxisSettings yAxis;

        /// <summary>
        /// If the character is teleported or respawned should be snap to that position or animate to it?
        /// </summary>
        [Tooltip ("If the character is teleported or respawned should be snap to that position or animate to it?")]
        public bool snapOnRespawn;


        [Header ("Camera Zooming")]
        [Tooltip("Can this camera zoom in and out?")]
        /// <summary>
        /// Can this camera zoom in and out.
        /// </summary>
        public bool allowZoom;


        /// <summary>
        /// Maximum zoom level.
        /// </summary>
        [DontShowWhenAttribute("allowZoom", true)]
        [Tooltip("Maximum zoom level.")]
        public float maximumZoom = 10.0f;

        /// <summary>
        /// How should we implement the zoom?
        /// </summary>
        [DontShowWhenAttribute("allowZoom", true)]
        [Tooltip("How should we implement the zoom?")]
        public CameraZoomMode zoomMode;

        /// <summary>
        /// How big should the border around the characters be (to enure they can see in 'front' of themselves)?
        /// </summary>
        [DontShowWhenAttribute("allowZoom", true)]
        [Tooltip("How big should the border around the characters be (to enure they can see in 'front' of themselves)?")]
        public Vector2 borderSize = Vector2.one;

        /// <summary>
        /// The default zoom which comes form the cameras initial value. We cannot zoom in 
        /// more than the default unless you use code to manually force a zoom level.
        /// </summary>
        protected float defaultZoom;

        /// <summary>
        /// Gets the header string used to describe the component.
        /// </summary>
        /// <value>The header.</value>
        override public string Header
        {
            get
            {
                return "The Standard Platformer PRO camera with options for following one or more characters, " +
                       "locking to and transitioning between zones, and zooming in and out.";
            }
        }

        /// <summary>
        /// Is camera moving in x.
        /// </summary>
        protected bool movingInX;

        /// <summary>
        /// Is camera moving in y.
        /// </summary>
        protected bool movingInY;
        
        /// <summary>
        /// Reference to the character loaders.
        /// </summary>
        protected List<PlatformerProGameManager> characterLoaders;
    
        /// <summary>
        /// How far did we move last frame in X
        /// </summary>
        protected float distanceMovedLastFrameX;

        /// <summary>
        /// How far did we move last frame in X
        /// </summary>
        protected float distanceMovedLastFrameY;

        /// <summary>
        /// The actual smoothing factor in X.
        /// </summary>
        protected float actualSmoothingFactorX;

        /// <summary>
        /// The actual smoothing factor in Y.
        /// </summary>
        protected float actualSmoothingFactorY;

        /// <summary>
        /// Never allow frames to move more than this factor relative to previous frame.
        /// </summary>
        protected const float SmoothingFactor = 1.125f;

        /// <summary>
        /// True while the camera is moving.
        /// </summary>
        protected bool isInTransition;

        /// <summary>
        /// Tweener which handles any moves.
        /// </summary>
        protected PositionTweener tweener;

        /// <summary>
        /// Zone we are moving to.
        /// </summary>
        protected CameraZone targetZone;

        /// <summary>
        /// Where are we moving to?
        /// </summary>
        protected Vector3 targetPosition;

        /// <summary>
        /// Reference to the character being followed.
        /// </summary>
        protected List<Character> characters;

        #endregion

        #region constants

        /// <summary>
        /// If camera movement is smaller than this it is considered still.
        /// </summary>
        protected const float MAX_SPEED_FOR_STILL = 0.4f;

        #endregion 

        /// <summary>
        /// Unity OnDestroy hook.
        /// </summary>
        void OnDestroy()
        {
            DoDestroy ();
        }

        /// <summary>
        /// Initialise this instance.
        /// </summary>
        override public void Init()
        {
            base.Init ();
            if (allowZoom) {
                switch (zoomMode)
                {
                case CameraZoomMode.ZOOM_WITH_ORTHO_SIZE:
                    defaultZoom = myCamera.orthographicSize;
                    break;
                }
            
            }
            characters = new List<Character> ();
            tweener = GetComponent<PositionTweener> ();
            if (tweener == null) {
                tweener = gameObject.AddComponent<PositionTweener> ();
                tweener.UseGameTime = true;
            }
            PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
            PlatformerProGameManager.Instance.CharacterRemoved += HandleCharacterRemoved;
            actualSmoothingFactorX = 1.0f + (SmoothingFactor - 1.0f) * xAxis.acceleration;
            actualSmoothingFactorY = 1.0f + (SmoothingFactor - 1.0f) * yAxis.acceleration;
        }


        /// <summary>
        /// Do the destroy actions.
        /// </summary>
        override protected void DoDestroy()
        {
            base.DoDestroy ();
            if (PlatformerProGameManager.Instance != null)
            {
                PlatformerProGameManager.Instance.CharacterLoaded -= HandleCharacterLoaded;
                PlatformerProGameManager.Instance.CharacterRemoved -= HandleCharacterRemoved;
            }
            if (characters != null && characters.Count > 0 && snapOnRespawn)
            {
                foreach (Character c in characters)
                {
                    c.Respawned -= HandleRespawned;
                }
            }
        }

        /// <summary>
        /// Handles a character being loaded.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
        {
            if (!characters.Contains (e.Character))
            {
                characters.Add (e.Character);
                e.Character.Respawned += HandleRespawned;
            }
            // TODO Phase check
            // MULTIPLAYER TODO: What do we do on spawn
            if (characters.Count == 1)
            {
                transform.Translate (e.Character.transform.position.x - transform.position.x, e.Character.transform.position.y - transform.position.y, 0);
            }
            else
            {
                MoveInX ();
                MoveInY ();
            }
        }

        /// <summary>
        /// Handles a character being removed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        virtual protected void HandleCharacterRemoved (object sender, CharacterEventArgs e)
        {
            if (characters.Contains (e.Character))
            {
                characters.Remove (e.Character);
            }
            // TODO Phase check
            // MULTIPLAYER TODO: What do we do on spawn
            MoveInX();
            MoveInY();
        }


        /// <summary>
        /// Handles the respawned event
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Even details.</param>
        virtual protected void HandleRespawned (object sender, CharacterEventArgs e)
        {
            // MULTIPLAYER TODO: What do we do on spawn
            if (characters.Count == 1)
            {
                transform.Translate(e.Character.transform.position.x - transform.position.x, e.Character.transform.position.y - transform.position.y, 0);
                // Update zone
                for (int i = 0; i < CameraZone.cameraZones.Count; i++)
                {
                    if (CameraZone.cameraZones[i].IsInZone(transform))
                    {
                        currentZone = CameraZone.cameraZones[i];
                        break;
                    }
                }
            }
            else
            {
                MoveInX ();
                MoveInY ();
            }
        }

        /// <summary>
        /// Unity LateUpdate hook.
        /// </summary>
        void LateUpdate() 
        {
            if (enabled && !isInTransition) DoMove ();
            if (isInTransition)
            {

            }
        }

        virtual protected Vector2 AverageTransformPosition
        {
            get
            {
                if (characters.Count == 0) return Vector2.zero;
                Vector2 transform = Vector2.zero;
                for (int i = 0; i < characters.Count; i++)
                {
                    // TODO Weighting?
                    transform += (Vector2)characters [i].transform.position;
                }
                transform = transform / characters.Count;
                return transform;
            }
        }

        /// <summary>
        /// Move the camera
        /// </summary>
        protected void DoMove() 
        {
            if (characters.Count > 0)
            {
                if (currentZone != null && currentZone.ZoneSmallerThanView) return;
                if (allowZoom) DoZoom ();

                if (xAxis.moveOnAxis) MoveInX ();
                if (yAxis.moveOnAxis) MoveInY ();
                if (currentZone != null) LimitToBounds ();
            }
        }

        /// <summary>
        /// Handles the zoom of the camera.
        /// </summary>
        protected void DoZoom() 
        {
            // TODO Allow developers to define a specified zoom override
            float desiredZoom = 0;
            if (characters.Count < 2)
            {
                desiredZoom = defaultZoom;
            }
            if (desiredZoom == 0)
            {
                // Get parameters
                Vector2 position = AverageTransformPosition;
                Vector2 min = new Vector2 (float.MaxValue, float.MaxValue);
                Vector2 max = new Vector2 (float.MinValue, float.MinValue);
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters [i].transform.position.x > max.x) max.x = characters [i].transform.position.x;
                    if (characters [i].transform.position.x < min.x) min.x = characters [i].transform.position.x;
                    if (characters [i].transform.position.y > max.y) max.y = characters [i].transform.position.y;
                    if (characters [i].transform.position.y < min.x) min.y = characters [i].transform.position.y;
                }
                Vector2 distance = (max - min) + borderSize;

                // Calculate desired zoom
                switch (zoomMode)
                { // x = (distance.y / 2.0f) * aspect
                case CameraZoomMode.ZOOM_WITH_ORTHO_SIZE:
                    float yZoom = distance.y / 2.0f;
                    float xZoom = (distance.x / myCamera.aspect) / 2.0f;
                    desiredZoom = (yZoom > xZoom) ? yZoom : xZoom;
                    break;
                case CameraZoomMode.ZOOM_WITH_FOV:
                    break;
                case CameraZoomMode.ZOOM_WITH_Z_POS:
                    break;
                }

            }
            // Limit
            if (desiredZoom < defaultZoom) desiredZoom = defaultZoom;
            if (desiredZoom > maximumZoom) desiredZoom = maximumZoom;

            // Apply desired zoom (separate to calcualtion so we can also handle developers specifying a desired zoom)
            switch (zoomMode)
            {
            case CameraZoomMode.ZOOM_WITH_ORTHO_SIZE:
                myCamera.orthographicSize = desiredZoom;
                break;
            case CameraZoomMode.ZOOM_WITH_FOV:
                break;
            case CameraZoomMode.ZOOM_WITH_Z_POS:
                break;
            }
        }

        /// <summary>
        /// Moves the camera in x.
        /// </summary>
        protected void MoveInX() 
        {
            Vector2 position = AverageTransformPosition;
            if (xAxis.hardFollow)
            {
                
                transform.Translate(position.x - transform.position.x, 0, 0);
            }
            else
            {
                if (movingInX)
                {
                    float xDiff = position.x - transform.position.x;
                    if (Mathf.Abs (xDiff) < (xAxis.minOffsetForMove * 0.5f))
                    {
                        movingInX = false;
                        distanceMovedLastFrameX = 0.0f;
                    } 
                    else
                    {
                        // Alternative way to do this but it doesn't work so well.
//                        float newPosition = Mathf.SmoothDamp(transform.position.x, character.transform.position.x, ref xVel, 0.75f);
//                        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);

                        float dist = xDiff * TimeManager.FrameTime * xAxis.acceleration;
                        if (dist < 0 && dist < xDiff) dist = xDiff;
                        else if (dist > 0 && dist > xDiff) dist = xDiff;
                        // Try to smooth out frame time jumps (mainly for web player)
                        if (distanceMovedLastFrameX < 0 && dist < (distanceMovedLastFrameX * actualSmoothingFactorX)) dist = distanceMovedLastFrameX * actualSmoothingFactorX;
                        else if (distanceMovedLastFrameX > 0 && dist > (distanceMovedLastFrameX * actualSmoothingFactorX)) dist = distanceMovedLastFrameX * actualSmoothingFactorX;
                        else if (distanceMovedLastFrameX < -0.05f && dist > (distanceMovedLastFrameX / actualSmoothingFactorX)) dist = distanceMovedLastFrameX / actualSmoothingFactorX;
                        else if (distanceMovedLastFrameX > 0.05f && dist < (distanceMovedLastFrameX / actualSmoothingFactorX)) dist = distanceMovedLastFrameX / actualSmoothingFactorX;
                        transform.Translate(dist, 0, 0);
                        distanceMovedLastFrameX = dist;
                    }
                }
                else if (Mathf.Abs (position.x - transform.position.x) > xAxis.minOffsetForMove)
                {
                    movingInX = true;
                }
            }
        }

        /// <summary>
        /// Moves the camera in y.
        /// </summary>
        protected void MoveInY() 
        {
            Vector2 position = AverageTransformPosition;
            if (yAxis.hardFollow)
            {
                transform.Translate(0, position.y - transform.position.y, 0);
            }
            else
            {
                if (movingInY)
                {
                    float yDiff = position.y - transform.position.y;
                    if (Mathf.Abs (yDiff) < (yAxis.minOffsetForMove * 0.5f))
                    {
                        movingInY = false;
                        distanceMovedLastFrameY = 0.0f;
                    } 
                    else
                    {
                        float dist = yDiff * TimeManager.FrameTime * yAxis.acceleration;
                        if (dist < 0 && dist < yDiff) dist = yDiff;
                        else if (dist > 0 && dist > yDiff) dist = yDiff;
                        // Try to smooth out frame time jumps (mainly for web player)
                        if (distanceMovedLastFrameY < 0 && dist < (distanceMovedLastFrameY * actualSmoothingFactorY)) dist = distanceMovedLastFrameY * actualSmoothingFactorY;
                        else if (distanceMovedLastFrameY > 0 && dist > (distanceMovedLastFrameY * actualSmoothingFactorY)) dist = distanceMovedLastFrameY * actualSmoothingFactorY;
                        else if (distanceMovedLastFrameY < -0.05f && dist > (distanceMovedLastFrameY / actualSmoothingFactorY)) dist = distanceMovedLastFrameY / actualSmoothingFactorY;
                        else if (distanceMovedLastFrameY > 0.05f && dist < (distanceMovedLastFrameY / actualSmoothingFactorY)) dist = distanceMovedLastFrameY / actualSmoothingFactorY;
                        transform.Translate(0, dist, 0);
                        distanceMovedLastFrameY = dist;
                    }
                }
                else if (Mathf.Abs (position.y - transform.position.y) > yAxis.minOffsetForMove)
                {
                    movingInY = true;
                }
            }
        }

        /// <summary>
        /// Limits camera position to the current zones bounds.
        /// </summary>
        protected void LimitToBounds()
        {
            if (currentZone != null)
            {
                // X Bounds
                if (transform.position.x > currentZone.Max(myCamera).x)
                {
                    transform.Translate (currentZone.Max(myCamera).x - transform.position.x, 0, 0);
                    movingInX = false;
                }
                else if (transform.position.x < currentZone.Min(myCamera).x)
                {
                    transform.Translate (currentZone.Min(myCamera).x - transform.position.x, 0, 0);
                    movingInX = false;
                }
                // Y Bounds
                if (transform.position.y > currentZone.Max(myCamera).y)
                {
                    transform.Translate (0, currentZone.Max(myCamera).y - transform.position.y, 0);
                    movingInY = false;
                }
                else if (transform.position.y < currentZone.Min(myCamera).y)
                {
                    transform.Translate (0, currentZone.Min(myCamera).y - transform.position.y, 0);
                    movingInY = false;
                }
            }
        }


        /// <summary>
        /// Changes the zone by smoothly animating to the new zone.
        /// </summary>
        /// <param name="newZone">The zone to move to.</param>
        /// <param name="triggeringCharacter">Character that triggered the transition./param> 
        override public void ChangeZone(CameraZone newZone, Character triggeringCharacter = null)
        {
            if (tweener != null && tweener.Active) tweener.Stop();
            if (transitionSpeed > 0 && tweener != null)
            {
                currentZone = null;
                targetZone = newZone;
                isInTransition = true;
                targetPosition = newZone.GetBestPositionForCharacter (myCamera, triggeringCharacter);
                tweener.TweenWithRate (tweenMode, transform, targetPosition, transitionSpeed, ZoneHasBeenChanged);
                movingInX = false;
                movingInY = false;
                distanceMovedLastFrameX = 0.0f;
                distanceMovedLastFrameY = 0.0f;
                if (freezePlayerDuringTransition && characters.Count > 0)
                {
                    foreach (Character c in characters)
                    {
                        c.enabled = false;
                    }
                }
            } 
            else
            {
                currentZone = newZone;
                isInTransition = false;
                transform.position = newZone.GetBestPositionForCharacter (myCamera, triggeringCharacter);
            }
        }

        /// <summary>
        /// Called when the zones the been changed.
        /// </summary>
        virtual public void ZoneHasBeenChanged(Transform t, Vector3 p)
        {
            if (targetZone == null) return;
            currentZone = ((CameraZone)targetZone).ActualZone;
            isInTransition = false;
            // TODO Unfreeze all characters
            if (freezePlayerDuringTransition && characters.Count > 0)
            {
                foreach (Character c in characters)
                {
                    c.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Settings defining how a camera moves along an axis.
    /// </summary>
    [System.Serializable]
    public class CameraAxisSettings 
    {
        /// <summary>
        /// Does the camera move along this axis.
        /// </summary>
        public bool moveOnAxis = true;

        /// <summary>
        /// Does the camera transform exactly follow the character transform on this axis.
        /// </summary>
        public bool hardFollow;

        /// <summary>
        /// How much does the character need to move before the camera starts to move.
        /// Ignored if hardFollow is true;
        /// </summary>
        [DontShowWhenAttribute ("hardFollow")]
        public float minOffsetForMove;

        /// <summary>
        /// The acceleration applied to the camera until it catches up to characer.
        /// Ignored if hardFollow is true;
        /// </summary>
        [DontShowWhenAttribute ("hardFollow")]
        public float acceleration;

    }

    /// <summary>
    /// How do we zoom?
    /// </summary>
    public enum CameraZoomMode 
    {
        ZOOM_WITH_ORTHO_SIZE,
        ZOOM_WITH_FOV,
        ZOOM_WITH_Z_POS
    }
}
