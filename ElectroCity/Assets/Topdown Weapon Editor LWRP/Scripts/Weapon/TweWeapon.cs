using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class TweWeapon : ScriptableObject
{
    public int openTab;//this is used to keep same inspector tab open when you open game and stuff. no longet used delete

    public string weaponName = "NewWeapon";
    public string selfTag;//what tag the projectile will be given
    public int layer = 0;
    public int bullets = 1;
    public bool burst = false;
    public float burstDelay = .1f;//time between bullets in a single burst
    public int bulletsPerBurst = 3;//size of burst  
    public int muzzles = 1; // 총구 카운트 
    public float fireRate = .1f;
    public int fireRateGrade = 1; // 발사속도 등급(상점용도)
    public float damage = 1;
    public bool breakOnHit = true;
    public float lifetime = .5f;//max duration of the projectile
    public Vector3 innacuracy;
    public bool evenInnacuracy;
    public bool reverseSweep;//used to reverse the direction of the sweep on burst even
    public Vector2 randomVelocityMultiplier = new Vector2(1,1);

    public float bulletRate = 0.1f; // 총알 사이의 간격 - 0이면 한번에 발사 



    public bool bounce = false;
    public bool stopStickOnBounce = true;//when true projectile will stop sticking when bouncing. This is perfered as bouncing can give x rotation which is not good with stick
    public int bounces = 1;
    public float bounceFriction = 0;//speedVector *= (1/Mathf.Pow(2,weapon.bounceFriction))
    public Vector2 randomBounceAngle = new Vector3(0, 0);
    public string[] bounceTags = { };
    public AudioClip bounceSound;
    public ParticleSystem bounceParticle;
    public GameObject bounceSpawn;// x-spanws are used to create game object when something happens, probably wont be used often
    public string[] damageTags = {};//tag types that you deal damage to
    public AudioClip damageSound;
    public ParticleSystem damageParticle;
    public GameObject damageSpawn;
    public string[] breakTags = {};//tag types that you break on, if you dont break on who ou do damage you will damage and pass through
    public AudioClip breakSound;
    public ParticleSystem breakParticle;
    public GameObject breakSpawn;//game object that spawn when projectile breaks, optional

    public ParticleSystem fizzleParticle;//fizzle happens when projectile reaches max lifetime or when speed = 0 with destroy on zero speed enabled
    public GameObject fizzleSpawn;
    public AudioClip fizzleSound;

    //homing stuff
    public bool homing = false;
    public string[] homingTags = {};
    public float homingCheckInterval = .5f;//how often to check for homing target. .5 = twice per second
    public bool homingCheckInitialOnly = false;
    public float homingCheckInitialRange = 10f;//range check on startup
    public float homingCheckIntervalRange = 5f;//range check on each interval
    public float homingSpeed = 1f;//how fast the projectile will turn towards the homing target
    public float homingAcceleration = 1f;//homing speed acceleration
    public bool showHomingGizmo = false;

    public AudioClip fireSound;
    public ParticleSystem fireParticle;
    public Vector3 fireMuzzleRotation = Vector3.zero;
    public float screenShake;
    public Mesh projectileMesh;
    public Vector3 projectileScale = new Vector3(1,1,1);
    //public float hitboxLength = 1;//was used when collision was a capsule cast
    public float hitboxRadius = 1;
    public bool showCollisionDebug = false;
    public Material material;
    public bool receiveShadows = true;
    public bool trail;

    public string weaponSpriteID = string.Empty;
    public ParticleSystem particleTrail;//one of two trail types

    public Material trailMaterial;
    public float trailTime = 5;
    public float trailMinVertexDistance = .1f;
    public bool trailAutodestruct = false;
    public bool trailEmitting = true;
    public AnimationCurve trailWidthCurve;
    public Gradient trailColorGradient = new Gradient();
    public LineAlignment trailAlignment = LineAlignment.View;
    public LineTextureMode trailTextureMode = LineTextureMode.Stretch;


    //projectileControl
    public float initalForwardSpeed;
    public bool limitSpeed = false;
    public float maxForwardSpeed = 1000f;
    public float minForwardSpeed = -1000f;
    public float forwardAcceleration;
    public Vector3 rotationSpeed;
    public float rotationSpeed2D; // 2D 회전값
    public bool is2D = false; // 2D인지 아닌지.
    public bool randomRotationSpeed;
    public float angularDrag;
    public Vector3 angularTurbulance;//randomly adds this to rotationspeed

    public float gravity;

    public bool destoryOnZeroSpeed = true;//destroy when forward speed is less than 0

    //stick
    public bool projectileStick;//keep in mind this isnt a fool-proof setting. this wont work well with every projectile.
    public bool interpolateStick = true;//if true uses projectileStickSpeed to lerp
    public string[] projectileStickTags;
    public float projectileHeight = 1f;
    public float projectileStickSpeed = 10;//how fast the projectile lerps to the ground

    //for pooling
    public bool usePool = true;
    public Queue<GameObject> objectPool = new Queue<GameObject>();
    public int poolSize = 20;
    public int autoExpansion = 15;//auto expansion, min should be the amount of bullets

    //poolID is left out of the inspector just to prevent any confusion
    public int poolID = 1;//poolID is used to track if the projectile is updated to the current Weapon version. If not it gets deleted instead of being put back into pool
    //end for pooling



    //CHANGE HERE IF USING CUSTOM CAMERA CONTROL
    private TweCameraControl cameraControlRefernece;


    public float FireWeapon(Transform aimpoint, TweCameraControl cc = null)//this is the method you should call when firing a weapon in another script
    {
        if (cc != null)
        {
            cameraControlRefernece = cc;
        }

        if (!burst)
        {
            if (fireSound != null)
            {
                AudioSource.PlayClipAtPoint(fireSound, aimpoint.transform.position);
                
            }
            if (fireParticle != null)
            {

                if(fireMuzzleRotation != Vector3.zero)
                    Instantiate(fireParticle, aimpoint.transform.position, Quaternion.Euler(fireMuzzleRotation));
                // Instantiate(fireParticle, aimpoint.transform.position, aimpoint.transform.rotation);
                else
                    Instantiate(fireParticle, aimpoint.transform.position, aimpoint.transform.rotation);


                
            }

            ScreenShake();
        }

        if (usePool) {
            if (bulletRate == 0) {
                for (int i = 0; i < bullets; i++) {
                    if (burst) {
                        for (int j = 0; j < bulletsPerBurst; j++) {
                            aimpoint.gameObject.AddComponent<TweBurstSpawn>().Initialize(burstDelay * j, this, j);
                        }
                    }
                    else {
                        if (usePool) {
                            if (objectPool.Count == 0) {
                                AddToPool();
                            }
                        }
                        GameObject spawn = objectPool.Dequeue();


                        spawn.transform.rotation = aimpoint.rotation;//basic position set, we do accuracy in Initialize() on projectileControl now
                        spawn.transform.position = aimpoint.position;
                        // spawn.transform.position = new Vector2(aimpoint.position.x, aimpoint.position.y);


                        TweProjectileControl pc = spawn.GetComponent<TweProjectileControl>();
                        pc.aimpoint = aimpoint;
                        pc.bulletNum = i;

                        spawn.SetActive(true);
                    }
                } // end of for (int i = 0; i < bullets; i++)
            }
            else { // 그 외에는 무조건 1발씩 쏜다. 
                if (burst) {
                    for (int j = 0; j < bulletsPerBurst; j++) {
                        aimpoint.gameObject.AddComponent<TweBurstSpawn>().Initialize(burstDelay * j, this, j);
                    }
                }
                else {
                    if (usePool) {
                        if (objectPool.Count == 0) {
                            AddToPool();
                        }
                    }
                    GameObject spawn = objectPool.Dequeue();


                    spawn.transform.rotation = aimpoint.rotation;//basic position set, we do accuracy in Initialize() on projectileControl now
                    spawn.transform.position = aimpoint.position;
                    // spawn.transform.position = new Vector2(aimpoint.position.x, aimpoint.position.y);


                    TweProjectileControl pc = spawn.GetComponent<TweProjectileControl>();
                    pc.aimpoint = aimpoint;
                    pc.bulletNum = 0;

                    spawn.SetActive(true);
                }
            }

        } // end of if usePool
        else
        {
            for (int i = 0; i < bullets; i++)
            {
                if (burst)//burst weapons create a script on the aimpoint object that waits to instantiate the bullet.
                {
                    for (int i2 = 0; i2 < bulletsPerBurst; i2++)
                    {
                        aimpoint.gameObject.AddComponent<TweBurstSpawn>().Initialize(burstDelay * i2, this, i2);
                    }
                }
                else
                {

                    GameObject spawn = Projectile();//the difference between pool an non pooled is we instantiate the object HERE instead oh having the pooll.....
                    spawn.transform.position = aimpoint.position;
                    spawn.transform.rotation = aimpoint.rotation;
                    TweProjectileControl pc = spawn.GetComponent<TweProjectileControl>();
                    pc.aimpoint = aimpoint;
                    pc.bulletNum = i;
                    spawn.SetActive(true);
                }
            }
        }


        return screenShake;

    }



    /// <summary>
    /// 투사체 생성.
    /// </summary>
    /// <returns></returns>
    public GameObject Projectile()//essentially a very specific instantiate, creates an inactive game object
    {
        GameObject projectile;
        projectile = new GameObject(weaponName + " Projectile");
        projectile.SetActive(false);
        if (!string.IsNullOrEmpty(selfTag))
        {
            projectile.tag = selfTag;
        }

        projectile.layer = layer;

        ParticleSystem pt = null;
        if (particleTrail != null)
        {
           pt  = Instantiate(particleTrail, projectile.transform);//adds the particle trail
            pt.transform.localPosition = Vector3.zero;
            pt.transform.localEulerAngles = new Vector3(0, 90, 0);
        }


        projectile.AddComponent<MeshFilter>();
        MeshFilter projMeshFlt = projectile.GetComponent<MeshFilter>();//
        projMeshFlt.mesh = projectileMesh;//set the mesh, if mesh = null don't worry it works fine.

        projectile.AddComponent<MeshRenderer>();
        MeshRenderer projMeshRnd = projectile.GetComponent<MeshRenderer>();
        projMeshRnd.material = material;
        projMeshRnd.receiveShadows = receiveShadows;

        projectile.transform.localScale = projectileScale;

        //trail, 
        if (trail)//setup all trail values.
        {
            projectile.AddComponent<TrailRenderer>();
            TrailRenderer projTR = projectile.GetComponent<TrailRenderer>();
            projTR.material = trailMaterial;
            projTR.time = 0f;
            projTR.minVertexDistance = trailMinVertexDistance;
            projTR.autodestruct = trailAutodestruct;
            projTR.emitting = trailEmitting;
            projTR.widthCurve = trailWidthCurve;
            projTR.colorGradient = trailColorGradient;
            projTR.alignment = trailAlignment;
            projTR.textureMode = trailTextureMode;
        }

        // 2D인 경우 추가 
        /* 2D의 경우 Trail도 파티클 시스템을 사용하기 때문에 기존 툴과 조금 다른 방식 */
        if(is2D) {
            projectile.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            projectile.AddComponent<CircleCollider2D>();
            CircleCollider2D circle = projectile.GetComponent<CircleCollider2D>();
            circle.radius = hitboxRadius;
            circle.isTrigger = true;
        }



        projectile.transform.position = Vector3.zero;


        projectile.AddComponent<TweProjectileControl>();
        TweProjectileControl projCntrl = projectile.GetComponent<TweProjectileControl>();
        if (particleTrail != null)
        {
            projCntrl.particleTrailRef = pt;
        }
        projCntrl.poolID = poolID;
        projCntrl.weapon = this;

        return projectile;
    }



    public void CreatePool()
    {
        DeletePool();
        poolID++;
        if (poolID > 1000000)//reset the pool id eventually
        {
            poolID = 1;
        }
        for (int i = 0; i < poolSize; i++)
        {
            objectPool.Enqueue(Projectile());
        }
    }

    public void AddToPool(int input)//not used by default, used to manually add to pool
    {
        for (int i = 0; i < input; i++)
        {
            objectPool.Enqueue(Projectile());
        }
    }

    public void AddToPool()//overload uses autoExpansion by default
    {
        for (int i = 0; i < autoExpansion; i++)
        {
            objectPool.Enqueue(Projectile());
        }
    }

    public void DeletePool()
    {
        int count = objectPool.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(objectPool.Dequeue());
        }
        objectPool.Clear();//just in case
        poolID++;
        if (poolID > 1000000)//reset the pool id eventually
        {
            poolID = 1;
        }
    }

    public void OnInspectorChange()
    {
        if (Application.isPlaying && usePool)//only execute during play
        {
            CreatePool();
        }
        else if(Application.isPlaying && !usePool && objectPool.Count > 0)
        {
            DeletePool();
        }
    }

    public void ScreenShake()
    {
        //THIS IS WHERE YOU HANDLE SCREENSHAKE, if you are not using the provided player control scripts you will need to change this

        //ALSO You can use this as a generic "On shoot" method

        //KEEP IN MIND, if you have the same weapon on a player and npc the players screen will shake when the npc fires. I reccomend seperating the weapons.
        if (cameraControlRefernece != null)
        {
            cameraControlRefernece.ScreenShake(screenShake);

        }

    }
}