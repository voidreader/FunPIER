using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweWeapon))]

public class TweWeaponObjectInspector : Editor
{
    //gui STYLES

    GUIStyle titleStyle;
    GUIStyle helpText = new GUIStyle();
    bool showOriginal = false;
    TweWeapon weapon;
    static string[] tabs = { "Main", "Motion", "Look & Feel", "Collision" , "Pooling", "Help"};

    //help tab bools
    bool setup;
    bool summary;
    bool contact;
    bool archetype;
    bool commonProblems;

    //end help tab bools

    public void OnEnable()
    {
        weapon = (TweWeapon)target;

    }

    int tab;
    public override void OnInspectorGUI()
    {
        //styles
        helpText = new GUIStyle(EditorStyles.label);
        helpText.wordWrap = true;
        helpText.fontSize = 16;
        //endStyes

        serializedObject.Update();

        tab = GUILayout.SelectionGrid(tab, tabs, 3);
        //tab = GUILayout.Toolbar(tab, tabs );  //Previously used toolbar

        if (weapon.bounceTags.Length == 0 && weapon.breakTags.Length == 0 && weapon.damageTags.Length == 0)
        {
            EditorGUILayout.HelpBox("There are no collision tags set", MessageType.Warning);
        }

        EditorGUI.BeginChangeCheck();
        switch (tab)
        {
            case 0://main
                GUILayout.Label("Main", EditorStyles.boldLabel);
                weapon.weaponName = EditorGUILayout.TextField(new GUIContent("Weapon Name", "Sets the name of the projectile objects that are instantiated"), weapon.weaponName);
                weapon.selfTag = EditorGUILayout.TagField(new GUIContent("Tag", "Sets the tag of the projectile, this has no use internally by default"), weapon.selfTag);
                weapon.layer = EditorGUILayout.IntField(new GUIContent("Layer", "Sets the layer index of the projectile, this has no use internally by default"), weapon.layer);
                EditorGUILayout.BeginHorizontal();
                weapon.damage = EditorGUILayout.FloatField(new GUIContent("Damage", "The damage passed through to the hit objects with a EnemyStatControl.cs. Uses the damage tags under the collision section"), weapon.damage);
                weapon.breakOnHit = EditorGUILayout.ToggleLeft(new GUIContent("Destroy On Damage", "When enabled the projectile will destroy when it damages an enemy. Use this if you don't want to mix break and damage together."), weapon.breakOnHit);
                EditorGUILayout.EndHorizontal();
                weapon.fireRate = EditorGUILayout.FloatField(new GUIContent("Fire Rate", "The time between shots fired. This does not have any inherent function and must be implemented inside your combat script. See the PlayerCombat.cs for an example"), weapon.fireRate);
                weapon.bullets = EditorGUILayout.IntField(new GUIContent("Bullets", "How many bullets are fired per shot"), weapon.bullets);
                weapon.lifetime = EditorGUILayout.FloatField(new GUIContent("Lifetime", "How long the bullets lasts before automatically being destroyed. Triggers fizzle particle on time-out"), weapon.lifetime);
                weapon.screenShake = EditorGUILayout.FloatField(new GUIContent("Screen Shake", "Screen shake for each weapon. This does not have any inherent function and must be implemented inside your combat script. See PlayerCombat.cs for an example"), weapon.screenShake);

                EditorGUILayout.Space();

                GUILayout.Label("Accuracy", EditorStyles.boldLabel);
                weapon.innacuracy = EditorGUILayout.Vector3Field(new GUIContent("Innacuracy", "The Inaccuracy of each axis. Y axis is the main axis to be used in top down games"), weapon.innacuracy);
                weapon.randomVelocityMultiplier = EditorGUILayout.Vector2Field(new GUIContent("Random Velocity Multiplier", "The max and min random multiplyer of the velocity on each projectile. Good for creating weapons like a shotgun."), weapon.randomVelocityMultiplier);
                weapon.evenInnacuracy = EditorGUILayout.Toggle(new GUIContent("Even Innacuracy", "When selected inaccuracy will be evenly distributed, Think like a triple machinegun"), weapon.evenInnacuracy);

                EditorGUILayout.Space();
                

                GUILayout.Label("Extra", EditorStyles.boldLabel);
                weapon.bounce = EditorGUILayout.Toggle(new GUIContent("Bounce", "When selected projectile will bounce off objects with the proper bounce tags under the collision tag."), weapon.bounce);
                if (weapon.bounce)
                {
                    weapon.bounces = EditorGUILayout.IntField(new GUIContent("Bounces", "How many times the projectile will bounce before breaking on hit"), weapon.bounces);
                    weapon.bounceFriction = EditorGUILayout.FloatField(new GUIContent("Bounce Friction", "Friction force applied to the velocity of the projectile when it bounces"), weapon.bounceFriction);
                    weapon.randomBounceAngle = EditorGUILayout.Vector2Field(new GUIContent("Random Bounce Angle", "Random rotation applied to the projectile when it bounces, for most top down games you should only use the Y axis"), weapon.randomBounceAngle);
                    EditorGUILayout.Space();
                }

                weapon.burst = EditorGUILayout.Toggle(new GUIContent("Burst", "When enabled turns the weapon into a burst weapon using the BurstSpawn.cs script."), weapon.burst);
                if (weapon.burst)
                {
                    weapon.burstDelay = EditorGUILayout.FloatField(new GUIContent("Burst Fire Rate", "Time between each bullet within the burst"), weapon.burstDelay);
                    weapon.bulletsPerBurst = EditorGUILayout.IntField(new GUIContent("Bullets Per Burst", "How many bullets are in a burst"), weapon.bulletsPerBurst);
                    weapon.reverseSweep = EditorGUILayout.Toggle(new GUIContent("Reverse Even Sweep", "With burst enabled and even innacuracy enabled, the burst will sweep along the innacuracy provided. This will reverse the direction of the sweep."), weapon.reverseSweep);
                }


                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                showOriginal = EditorGUILayout.Toggle(new GUIContent("Show Original Inspector", "Shows the full default inspector for the object. Only used for debug purposes."), showOriginal);
                if (showOriginal)
                {

                    EditorGUILayout.HelpBox("This is just for debugging purposes, all values here are organized in the categories above", MessageType.Warning);
                    base.OnInspectorGUI();
                }
                break;

            case 1://motion

                GUILayout.Label("Motion", EditorStyles.boldLabel); 
                {
                    weapon.initalForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Initial Forward Speed", "Initial forward speed for the projectile, this is in Units per second"), weapon.initalForwardSpeed);
                    weapon.forwardAcceleration = EditorGUILayout.FloatField(new GUIContent("Forward Acceleration", "Forward acceleration for the projectile, negative values can be used."), weapon.forwardAcceleration);
                    weapon.limitSpeed = EditorGUILayout.Toggle(new GUIContent("Limit Speed", "If enabled uses the following values to clamp the speed of the projectile"),weapon.limitSpeed);
                    if (weapon.limitSpeed)
                    {
                        weapon.maxForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Max Forward Speed", "Maximum forward speed of the projectile"), weapon.maxForwardSpeed);
                        weapon.minForwardSpeed = EditorGUILayout.FloatField(new GUIContent("Min Forward Speed", "Minimum forward speed of the projectile"), weapon.minForwardSpeed);
                    }
                    weapon.rotationSpeed = EditorGUILayout.Vector3Field(new GUIContent("Rotational Speed", "The constant rotation speed for the projectile."), weapon.rotationSpeed);
                    weapon.randomRotationSpeed = EditorGUILayout.Toggle(new GUIContent("Random Rotational Speed", "When enabled, rotational speed becomes a random value functioning similar to Innacuracy"), weapon.randomRotationSpeed);
                    weapon.angularDrag = EditorGUILayout.FloatField(new GUIContent("Angular Drag", "Drag force applied to the projectiles rotational speed"), weapon.angularDrag);
                    weapon.angularTurbulance = EditorGUILayout.Vector3Field(new GUIContent("Angular Turbulance", "Adds random rotational speed overtime"), weapon.angularTurbulance);
                }


                EditorGUILayout.Space();
                GUILayout.Label("Sticking", EditorStyles.boldLabel);
                weapon.projectileStick = EditorGUILayout.Toggle(new GUIContent("Projectile Stick", "When enabled projectiles will stick to objects using the following parameters"), weapon.projectileStick);
                if (weapon.projectileStick)
                {
                    
                    weapon.stopStickOnBounce = EditorGUILayout.Toggle(new GUIContent("Stop Stick On Bounce", "When enabled projectiles will stop sticking once they have bounced. This helps prevent particles glitching when the bounce gives them an x rotation"), weapon.stopStickOnBounce);
                    weapon.projectileHeight = EditorGUILayout.FloatField(new GUIContent("Projectile Height", "The height above the ground in units that the projectile sticks to"), weapon.projectileHeight);
                    weapon.interpolateStick = EditorGUILayout.Toggle(new GUIContent("Interpolate Stick", "If enabled projectiles smoothly stick scaling with the following stick speed value"), weapon.interpolateStick);
                    if (weapon.interpolateStick)
                    {
                        weapon.projectileStickSpeed = EditorGUILayout.FloatField(new GUIContent("Stick Speed", "How fast the projectile will lerp to its desired location"), weapon.projectileStickSpeed);
                    }
                    

                    GUILayout.Label(new GUIContent("Projectile Stick Tags", "The object tags that the projectile sticks to"),EditorStyles.boldLabel);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        System.Array.Resize(ref weapon.projectileStickTags, weapon.projectileStickTags.Length + 1);
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (weapon.projectileStickTags.Length > 1)
                        {
                            System.Array.Resize(ref weapon.projectileStickTags, weapon.projectileStickTags.Length - 1);
                        }
                    }
                    GUILayout.EndHorizontal();


                    for (int i = 0; i < weapon.projectileStickTags.Length; i++)
                    {
                        weapon.projectileStickTags[i] = EditorGUILayout.TagField("Tag "+(i+1), weapon.projectileStickTags[i]);
                    }
                }

                EditorGUILayout.Space();

                GUILayout.Label("Projectile Homing", EditorStyles.boldLabel);
                weapon.homing = EditorGUILayout.Toggle(new GUIContent("Enable Homing", "toggles homing for projectiles"), weapon.homing);


                if (weapon.homing)
                {
                    weapon.homingCheckInitialOnly = EditorGUILayout.Toggle(new GUIContent("Check for target on start only", "When selected the projectile only checks for a homing target on start"), weapon.homingCheckInitialOnly);
                    weapon.homingCheckInitialRange = EditorGUILayout.FloatField(new GUIContent("Initial Check Range", "Check range for the first check on start"), weapon.homingCheckInitialRange);
                    if (!weapon.homingCheckInitialOnly)
                    {
                        weapon.homingCheckIntervalRange = EditorGUILayout.FloatField(new GUIContent("Interval Check Range", "Check range for every check after the first"), weapon.homingCheckIntervalRange);
                        weapon.homingCheckInterval = EditorGUILayout.FloatField(new GUIContent("Check Interval", "How often the projectile checks for a target. Setting this to very low values will have a performance impact"), weapon.homingCheckInterval);
                    }
                    weapon.homingSpeed = EditorGUILayout.FloatField(new GUIContent("Homing Speed", "Speed at which the projectile turns toward the target"), weapon.homingSpeed);
                    weapon.homingAcceleration = EditorGUILayout.FloatField(new GUIContent("Homing Acceleration", "Acceleration of homing speed"), weapon.homingAcceleration);

                    GUILayout.Label(new GUIContent("Homing Tags", "The object tags that the projectile homes to"), EditorStyles.label);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        System.Array.Resize(ref weapon.homingTags, weapon.homingTags.Length + 1);
                    }
                    if (GUILayout.Button("-"))
                    {
                        if (weapon.homingTags.Length > 1)
                        {
                            System.Array.Resize(ref weapon.homingTags, weapon.homingTags.Length - 1);
                        }
                    }
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < weapon.homingTags.Length; i++)
                    {
                        weapon.homingTags[i] = EditorGUILayout.TagField("Tag " + (i + 1), weapon.homingTags[i]);
                    }

                    weapon.showHomingGizmo = EditorGUILayout.Toggle(new GUIContent("Show Debug Gizmo", "Shows a gizmo in the editor when enabled"), weapon.showHomingGizmo);
                }


                EditorGUILayout.Space();
                GUILayout.Label("Misc", EditorStyles.boldLabel);
                weapon.destoryOnZeroSpeed = EditorGUILayout.Toggle(new GUIContent("Destroy on Zero Speed", "When enabled the projectile will die when its velocity is <= zero"), weapon.destoryOnZeroSpeed);
                weapon.gravity = EditorGUILayout.FloatField(new GUIContent("Gravity", "Constant downward motion applied to the projectile."), weapon.gravity);
                if (weapon.gravity != 0)
                {
                    EditorGUILayout.HelpBox("Gravity is not properly supported and may cause problems with other options. I reccomend using another physics based solution if you need gravity", MessageType.Info);
                }

                break;

            case 2://look and feel
                GUILayout.Label("Basic", EditorStyles.boldLabel);

                weapon.projectileMesh = EditorGUILayout.ObjectField(new GUIContent("Mesh","The mesh of the projectile, this field is OPTIONAL"),weapon.projectileMesh, typeof(Mesh), false) as Mesh;
                weapon.material = EditorGUILayout.ObjectField(new GUIContent("Material","The material applied to the Mesh of the projectile"),weapon.material, typeof(Material), false) as Material;

                weapon.receiveShadows = EditorGUILayout.Toggle(new GUIContent("Receive Shadows", "Modifies the receive shadows variable in the mesh renderer of the projectile"), weapon.receiveShadows);
                weapon.projectileScale = EditorGUILayout.Vector3Field(new GUIContent("Projectile Scale", "Scale applied to the projectile game object"), weapon.projectileScale);

                weapon.fireSound = EditorGUILayout.ObjectField(new GUIContent("Fire Sound", "Audio Clip played when the weapon is fired"), weapon.fireSound, typeof(AudioClip), false) as AudioClip;
                weapon.fireParticle = EditorGUILayout.ObjectField(new GUIContent("Fire Particle", "Particle spawned when the weapon is fired"), weapon.fireParticle, typeof(ParticleSystem), false) as ParticleSystem;

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.Label("Trails", EditorStyles.boldLabel);
                weapon.particleTrail = EditorGUILayout.ObjectField(new GUIContent("Particle Trail", "If filled adds the particle system to the projectile. Make sure the particle system attached functions like the example trails provided"), weapon.particleTrail, typeof(ParticleSystem), false) as ParticleSystem;
                EditorGUILayout.Space();

                weapon.trail = EditorGUILayout.Toggle(new GUIContent("Unity Trail Renderer", "Enables the use of a Unity trail renderer on the projectile weapon, for more detailed documentation on how this functions look up \"Unity Trail Renderer\""), weapon.trail);
                if (weapon.trail)
                {
                        weapon.trailMaterial = EditorGUILayout.ObjectField(new GUIContent("Material", "Material of the Trail"), weapon.trailMaterial, typeof(Material), false) as Material;
                        weapon.trailTime = EditorGUILayout.FloatField(new GUIContent("Time", "Defines the length of the trail, measured in seconds"), weapon.trailTime);
                        weapon.trailMinVertexDistance = EditorGUILayout.FloatField(new GUIContent("Min Vertex Distance", "The minimum distance between anchor points of the trail"), weapon.trailMinVertexDistance);
                        weapon.trailAutodestruct = EditorGUILayout.Toggle(new GUIContent("Autodestruct", "Enable this to destroy the GameObject once it has been idle for Time seconds. Be careful using this option, you will want this DISABLED in most cases"), weapon.trailAutodestruct);
                        weapon.trailEmitting = EditorGUILayout.Toggle(new GUIContent("Emitting", "Toggles whether the trail is emitting or not"), weapon.trailEmitting);
                        weapon.trailWidthCurve = EditorGUILayout.CurveField(new GUIContent("Width", "A width value and a curve to control the width of your trail at various points between its start and end. The curve is applied from the beginning to the end of the trail, and sampled at each vertex. The overall width of the curve is controlled by the width value"), weapon.trailWidthCurve);
                        weapon.trailColorGradient = EditorGUILayout.GradientField(new GUIContent("Color", "A gradient to control the color of the trail along its length"), weapon.trailColorGradient);
                        weapon.trailAlignment = (LineAlignment)EditorGUILayout.EnumPopup(new GUIContent("Alignment", "Set to View to make the Trail face the camera, or Local to align it based on the orientation of its Transform component"), weapon.trailAlignment);
                        weapon.trailTextureMode = (LineTextureMode)EditorGUILayout.EnumPopup(new GUIContent("Texture Mode", "Control how the Texture is applied to the Trail. Use Stretch to apply the Texture map along the entire length of the trail, or use Wrap to repeat the Texture along the length of the Trail. Use the Tilingparameters in the Material to control the repeat rate"), weapon.trailTextureMode);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                GUILayout.Label("Damage", EditorStyles.boldLabel);
                weapon.damageSound = EditorGUILayout.ObjectField(new GUIContent("Damage Sound","Audio Clip that plays when the projectile damages"), weapon.damageSound, typeof(AudioClip), false) as AudioClip;
                weapon.damageParticle = EditorGUILayout.ObjectField(new GUIContent("Damage Particle","Particle System that spawns when the projectile damages"), weapon.damageParticle, typeof(ParticleSystem), false) as ParticleSystem;
                weapon.damageSpawn = EditorGUILayout.ObjectField(new GUIContent("Damage Spawn","Game Object that spawns when the projectile damages, this can be anything"), weapon.damageSpawn, typeof(GameObject), false) as GameObject;

                EditorGUILayout.Space();
                GUILayout.Label("Break", EditorStyles.boldLabel);
                weapon.breakSound = EditorGUILayout.ObjectField(new GUIContent("Break Sound", "Audio Clip that plays when the projectile breaks"), weapon.breakSound, typeof(AudioClip), false) as AudioClip;
                weapon.breakParticle = EditorGUILayout.ObjectField(new GUIContent("Break Particle", "Particle System that spawns when the projectile breaks"), weapon.breakParticle, typeof(ParticleSystem), false) as ParticleSystem;
                weapon.breakSpawn = EditorGUILayout.ObjectField(new GUIContent("Break Spawn", "Game Object that spawns when the projectile breaks, this can be anything"), weapon.breakSpawn, typeof(GameObject), false) as GameObject;


                EditorGUILayout.Space();
                GUILayout.Label("Bounce", EditorStyles.boldLabel);
                weapon.bounceSound = EditorGUILayout.ObjectField(new GUIContent("Bounce Sound", "Audio Clip that plays when the projectile bounces"), weapon.bounceSound, typeof(AudioClip), false) as AudioClip;
                weapon.bounceParticle = EditorGUILayout.ObjectField(new GUIContent("Bounce Particle", "Particle System that spawns when the projectile bounces"), weapon.bounceParticle, typeof(ParticleSystem), false) as ParticleSystem;
                weapon.bounceSpawn = EditorGUILayout.ObjectField(new GUIContent("Bounce Spawn", "Game Object that spawns when the projectile bounces, this can be anything"), weapon.bounceSpawn, typeof(GameObject), false) as GameObject;

                EditorGUILayout.Space();
                GUILayout.Label(new GUIContent("Fizzle","Fizzle occurs when a projectile reaches 0 speed with destroy on zero speed enabled, OR when its lifetime reaches 0"), EditorStyles.boldLabel);
                weapon.fizzleSound = EditorGUILayout.ObjectField(new GUIContent("Fizzle Sound", "Audio Clip that plays when the projectile fizzles"), weapon.fizzleSound, typeof(AudioClip), false) as AudioClip;
                weapon.fizzleParticle = EditorGUILayout.ObjectField(new GUIContent("Fizzle Particle", "Particle spawned with the projectile “fizzles”, When a projectile reaches its max life or reaches 0 speed with Destroy On Zero Speed enabled"), weapon.fizzleParticle, typeof(ParticleSystem), false) as ParticleSystem;
                weapon.fizzleSpawn = EditorGUILayout.ObjectField(new GUIContent("Fizzle Spawn", "Game Object that spawns when the projectile fizzles, this can be anything"), weapon.fizzleSpawn, typeof(GameObject), false) as GameObject;


                break;

            case 3://collision

                EditorGUILayout.HelpBox("Collision is calculated through a sphere cast", MessageType.Info);

                weapon.hitboxRadius = EditorGUILayout.FloatField(new GUIContent("Hitbox Radius", "Radius of the collision capsule cast"), weapon.hitboxRadius);
                weapon.showCollisionDebug = EditorGUILayout.Toggle(new GUIContent("Show Collision Debug", "When enbled shows some debug.lines that show the separate collison checks and other things like the bounce normals"), weapon.showCollisionDebug);

                GUILayout.Label(new GUIContent("Damage Tags","Tags used in decided whether to damage in collision detection. A tag is located on the top left of a game object right under its name"), EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    System.Array.Resize(ref weapon.damageTags, weapon.damageTags.Length + 1);
                }
                if (GUILayout.Button("-"))
                {
                    if (weapon.damageTags.Length > 0)
                    {
                        System.Array.Resize(ref weapon.damageTags, weapon.damageTags.Length - 1);
                    }
                }
                GUILayout.EndHorizontal();


                for (int i = 0; i < weapon.damageTags.Length; i++)
                {
                    weapon.damageTags[i] = EditorGUILayout.TagField("Tag " + (i + 1), weapon.damageTags[i]);
                }

                EditorGUILayout.Space();
                GUILayout.Label(new GUIContent("Break Tags", "Tags used in decided whether to break in collision detection. A tag is located on the top left of a game object right under its name"), EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    System.Array.Resize(ref weapon.breakTags, weapon.breakTags.Length + 1);
                }
                if (GUILayout.Button("-"))
                {
                    if (weapon.breakTags.Length > 0)
                    {
                        System.Array.Resize(ref weapon.breakTags, weapon.breakTags.Length - 1);
                    }
                }
                GUILayout.EndHorizontal();


                for (int i = 0; i < weapon.breakTags.Length; i++)
                {
                    weapon.breakTags[i] = EditorGUILayout.TagField("Tag " + (i + 1), weapon.breakTags[i]);
                }


                EditorGUILayout.Space();
                GUILayout.Label(new GUIContent("Bounce Tags", "Tags used in decided whether to bounce in collision detection. A tag is located on the top left of a game object right under its name"), EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    System.Array.Resize(ref weapon.bounceTags, weapon.bounceTags.Length + 1);
                }
                if (GUILayout.Button("-"))
                {
                    if (weapon.bounceTags.Length > 0)
                    {
                        System.Array.Resize(ref weapon.bounceTags, weapon.bounceTags.Length - 1);
                    }
                }
                GUILayout.EndHorizontal();


                for (int i = 0; i < weapon.bounceTags.Length; i++)
                {
                    weapon.bounceTags[i] = EditorGUILayout.TagField("Tag " + (i + 1), weapon.bounceTags[i]);
                }

                EditorGUILayout.HelpBox("Collision is calculated in the following order: Damage -> Break -> Bounce", MessageType.Info);


                break;

            case 4://pooling

                GUILayout.Label("Pooling", EditorStyles.boldLabel);
                weapon.usePool = EditorGUILayout.Toggle(new GUIContent("Use Object Pooling","Toggles the use of object pooling"), weapon.usePool);

                if (weapon.trail)
                {
                    GUILayout.Label(new GUIContent("Reccomended Pool Size: "+ Mathf.RoundToInt(weapon.bullets + (weapon.bullets * (1f / weapon.fireRate) * (weapon.lifetime + weapon.trailTime))), "A very ROUGH estimate of what your pool size should be, this is just to give you a general idea of how big it should be.") , EditorStyles.label);
                }
                else
                {
                    GUILayout.Label(new GUIContent("Reccomended Pool Size: " + Mathf.RoundToInt(weapon.bullets + (weapon.bullets * (1f / weapon.fireRate) * (weapon.lifetime))), "A very ROUGH estimate of what your pool size should be, this is just to give you a general idea of how big it should be."), EditorStyles.label);
                }

                if (Application.isPlaying)
                {
                    GUILayout.Label("Current pool size: " + weapon.objectPool.Count);
                }

                weapon.poolSize = EditorGUILayout.IntField(new GUIContent("Start Pool Size", "The initial pool size"), weapon.poolSize);
                weapon.autoExpansion = EditorGUILayout.IntField(new GUIContent("Pool Auto Expansion", "How much to expand the pool by each time the pool reaches its limit. HIGHLY recommend keeping this value greater than 0"), weapon.autoExpansion);

                EditorGUILayout.Space();

                break;

            case 5://help

                EditorGUILayout.Space();

                //common problems
                if (GUILayout.Button("Common Problems"))
                {
                    commonProblems = !commonProblems;
                }
                if (commonProblems)
                {
                    GUILayout.Label("Performance is terrible in the editor", helpText);
                    EditorGUILayout.Space();
                    GUILayout.Label("The problem is most likely there are to many gizoms on screen. More specifically, the gizmos on each particle effect. To fix simply disable gizmos.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Otherwise make sure that unity is updated. While developing I was having performance issues related to LWRP that I had no control over that have since been fixed.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Everything is pink", helpText);
                    EditorGUILayout.Space();
                    GUILayout.Label("The assets provided with this package are designed for LWRP. That does not mean that you must be using LWRP to use this editor, it only means you must use LWRP if you want to use the default assest provided.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Null reference with Tags", helpText);
                    EditorGUILayout.Space();
                    GUILayout.Label("Collision is handled with tags on objects with this editor. The default weapons use the tags \"Wall\" and \"Enemy\". If these tags are not present you will get null errors when using the default weapons. Either add the tags to your project or change the tags of the weapon in the collision tab", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                }


                //setup
                if (GUILayout.Button("Setup"))
                {
                    setup = !setup;
                }
                if (setup)
                {
                    GUILayout.Label("This section covers the basic setup and utilization of this weapon edtior", helpText);
                    EditorGUILayout.Space();
                    GUILayout.Label("1.  Create a new weapon object by creating a new asset and selecting “Weapon”", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("2.  Customize the parameters of the new weapon. The most important to change initially will be the collision tags in the collision tab. If you don't know what something does, remember that every variable has a tooltip explaining it's purpose. If you still can't figure out what something means contact me personally.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("3.  If not using the provided PlayerCombat.cs, inside your combat script you must create a Weapon variable, then use wepName.FireWeapon(transform) to fire the weapon how you see fit on your own script.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("4.  If using the PlayerCombat.cs you only have to drag your new weapon into the public Weapon variable slot.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("5.  INPUT. The provided PlayerCombat.cs uses the “Fire1” input axis. If you have renamed this you will need to adjust accordingly", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                }

                //main
                if (GUILayout.Button("Tabs Summary"))
                {
                    summary = !summary;
                }
                if (summary)
                {
                    GUILayout.Label("Main",helpText);
                    GUILayout.Label("The Main tab covers all the essential variables that come together to create the archetype of the weapon. ",EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Motion", helpText);
                    GUILayout.Label("The Motion tab covers all varibles that effect the motion of the projectile after it has been fired. This includes special functions like projectiles sticking and projectiles homing.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Look and Feel", helpText);
                    GUILayout.Label("The Look and Feel tab covers all visual aspects of the projectile as well as any particle effects or sounds it creates in various scenarios.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Collision", helpText);
                    GUILayout.Label("The Collision tab contains all the tags for each type of collision. It also contains the controls for the hitbox of the projectile.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Pooling", helpText);
                    GUILayout.Label("Object pooling is the act of creating many innactive game objects that get enabled and disabled when needed instead of instantiating ever projectile. This has a great performance boost. The controls for pooling are in this tab. Pooling is enabled by default.", EditorStyles.wordWrappedLabel);
                }

                //archetypes
                if (GUILayout.Button("Archetypes Guide"))
                {
                    archetype = !archetype;
                }
                if (archetype)
                {
                    GUILayout.Label("This guide covers the fundamentals of creating popular weapon archetypes.",helpText);
                    EditorGUILayout.Space();
                    GUILayout.Label("Shotgun", helpText);
                    GUILayout.Label("1. Set bullets to 5+ in the main tab", EditorStyles.wordWrappedLabel);
                    GUILayout.Label("2. Set y value for accuracy to the spread you desire for the shotgun", EditorStyles.wordWrappedLabel);
                    GUILayout.Label("3. Set x value for random velocity multiplier to any value below 1", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Burst Rifle", helpText);
                    GUILayout.Label("The key to creating burst weapons is using the \"Burst\" options under the Main tab",EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Triple Machine Gun", helpText);
                    GUILayout.Label("Using the \"Even Innacuracy\" option under the Accuracy section in the Main tab with any number of bullets and a spread value in the y of Innacuracy will create an X Machine Gun", EditorStyles.wordWrappedLabel);
                }

                //contact
                if (GUILayout.Button("Contact / Extra Help"))
                {
                    contact = !contact;
                }
                if (contact)
                {
                    GUILayout.Label("If you are having problems or need any help PLEASE contact me personally at either of the following places", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    GUILayout.Label("Email", helpText);
                    EditorGUILayout.SelectableLabel("alansherba@gmail.com", EditorStyles.wordWrappedLabel);
                    GUILayout.Label("Twitter", helpText);
                    EditorGUILayout.SelectableLabel("@AlanSherba", EditorStyles.wordWrappedLabel);
                }
                break;
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty((TweWeapon)target);
            serializedObject.ApplyModifiedProperties();
            weapon.OnInspectorChange();//this updates the object pool if parameters get changed while playing
            //AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();
        }
    }
}
