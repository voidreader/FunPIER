using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDProjectileControl : MonoBehaviour
{

    public TweWeapon weapon; // actual weapon., 

    public bool is2D = true;

    Vector3 rotationSpeed;
    float rotationSpeed2D;
    Vector3 dir2D;
    Rigidbody2D rb2D;


    Vector3 futurePosition;
    Quaternion futureRotation;
    public float speedForward;
    bool stick;
    int bounces;
    float life;
    bool alive = true;//basically a local enabled bool
    public int poolID = 0;
    bool trail;//local variable, variables like this prevent null references on projectiles mid flight that have their master weaponobject changed
    TrailRenderer tr;

    public ParticleSystem particleTrailRef;//reference to the particle trail, just to make it a little easier

    public float currentTime;//timer used for trails

    //for collision
    bool damagehit = false;
    bool bouncehit = false;
    bool breakhit = false;
    bool other = true;
    //
    public Transform aimpoint;
    public int bulletNum; //int used for burst and even fires

    //homing
    public GameObject homingTarget;
    float homingInterval = 0;//timer for homing checks
    float currentHomingAcceleration;
    bool homingGizmoFlash;

    private void OnEnable() {
        Initialize();
        bounces = weapon.bounces;
        speedForward = weapon.initalForwardSpeed * (Random.Range(weapon.randomVelocityMultiplier.x, weapon.randomVelocityMultiplier.y));
        stick = weapon.projectileStick;
        life = weapon.lifetime;
        tr = gameObject.GetComponent<TrailRenderer>();

        //resets
        GetComponent<MeshRenderer>().enabled = true;



        currentTime = 0f;

        if (weapon.randomRotationSpeed == true) {
            rotationSpeed = new Vector3(Random.Range(-weapon.rotationSpeed.x, weapon.rotationSpeed.x), Random.Range(-weapon.rotationSpeed.y, weapon.rotationSpeed.y), Random.Range(-weapon.rotationSpeed.z, weapon.rotationSpeed.z));
        }
        else {
            rotationSpeed = weapon.rotationSpeed;
        }

        alive = true;
    }

    void Start() {
        Initialize();


    }

    private void Update() {
        if (alive) {
            if (trail) {
                currentTime += Time.deltaTime;
                if (currentTime > tr.time && currentTime <= weapon.trailTime) {
                    tr.time = currentTime;
                }
                if (currentTime > weapon.trailTime) {
                    tr.time = weapon.trailTime;
                }
            }

            if (weapon.homing) {
                homingInterval += Time.deltaTime;
                homingGizmoFlash = false;
                if (!weapon.homingCheckInitialOnly && homingTarget == null && homingInterval > weapon.homingCheckInterval) {
                    currentHomingAcceleration = 0;
                    HomingCheck(weapon.homingCheckIntervalRange);
                    homingInterval = 0;
                    homingGizmoFlash = true;
                }
            }

            life -= Time.deltaTime;
            if (life <= 0) {
                ProjectileDestory();
                FizzleLook();
            }

            speedForward += (Time.deltaTime * weapon.forwardAcceleration);

            if (weapon.limitSpeed)//speed clamp
            {
                if (speedForward > weapon.maxForwardSpeed) {
                    speedForward = weapon.maxForwardSpeed;
                }
                if (speedForward < weapon.minForwardSpeed) {
                    speedForward = weapon.minForwardSpeed;
                }
            }


            rotationSpeed *= (1 / Mathf.Pow(2, weapon.angularDrag));

            rotationSpeed += (new Vector3(Random.Range(-weapon.angularTurbulance.x, weapon.angularTurbulance.x), Random.Range(-weapon.angularTurbulance.y, weapon.angularTurbulance.y), Random.Range(-weapon.angularTurbulance.z, weapon.angularTurbulance.z)));
            futurePosition = transform.position + ((transform.forward * speedForward) * (Time.deltaTime / 1));
            futureRotation = Quaternion.Euler(transform.eulerAngles.x + rotationSpeed.x * Time.deltaTime, transform.eulerAngles.y + rotationSpeed.y * Time.deltaTime, transform.eulerAngles.z + rotationSpeed.z * Time.deltaTime);


            if (weapon.gravity != 0) {
                futurePosition += new Vector3(0, -weapon.gravity * Time.deltaTime, 0);
            }

            if (stick == true) {
                rotationSpeed.x = 0f;//x rotation screws it up

                if (weapon.interpolateStick) {
                    futurePosition += ProjectileStick() * (Time.deltaTime * weapon.projectileStickSpeed);
                }
                else {
                    futurePosition += ProjectileStick();
                }
            }

            // 호밍 타겟이 있는 경우..
            if (homingTarget != null && weapon.homing) {
                futureRotation = Quaternion.Slerp(futureRotation, Quaternion.LookRotation((homingTarget.transform.position - transform.position).normalized), Time.deltaTime * (weapon.homingSpeed + currentHomingAcceleration));
                currentHomingAcceleration += (weapon.homingAcceleration * Time.deltaTime);
            }

            newCollision();
            //ProjectileCollision();

            transform.position = futurePosition;
            transform.rotation = futureRotation;
            if (speedForward < 0)//CHANGE
            {
                ProjectileDestory();
                FizzleLook();

            }
        }
    }

    private void newCollision() {
        float collisionLength = Vector3.Distance(transform.position, futurePosition);
        bool breakLoop = false;// used to break, have to do this since we need to break when nested 2 deep.
        do {
            breakLoop = false;
            bouncehit = false;
            RaycastHit[] hits;

            if (weapon.showCollisionDebug) {
                if (other) {
                    Debug.DrawLine(transform.position, futurePosition, Color.red, .5f);
                }
                else {
                    Debug.DrawLine(transform.position, futurePosition, Color.blue, .5f);
                }
            }

            other = !other;
            hits = Physics.SphereCastAll(transform.position, weapon.hitboxRadius, transform.forward, collisionLength);
            //hits = Physics.CapsuleCastAll((transform.position + transform.forward * weapon.hitboxLength), (transform.position + transform.forward * (-weapon.hitboxLength)), weapon.hitboxRadius, (futurePosition - transform.position), collisionLength);

            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));//sort the array by distance, prevents projectile from bouncing off the wrong wall


            //end sort


            if (hits.Length > 0) {
                for (int i = 0; i < hits.Length; i++) {
                    if (breakLoop) {
                        break;
                    }

                    RaycastHit hit = hits[i];

                    //damage
                    for (int j = 0; j < weapon.damageTags.Length; j++) {
                        if (hit.transform.tag == weapon.damageTags[j])//
                        {

                            DamageLook(hit);

                            damagehit = true;

                            TweEnemyStatConrol esc = hit.transform.gameObject.GetComponent<TweEnemyStatConrol>();//THIS IS THE DAMAGE PASSING
                            if (esc != null)//check
                            {
                                esc.Hit(weapon.damage);
                            }
                            else {
                                Debug.Log(hit.transform.name + " is tagged such that is taking damage, but has no enemyStatControl script");
                            }

                            if (weapon.breakOnHit) {
                                ProjectileDestory();
                                break;

                            }

                        }
                    }

                    //break
                    for (int j = 0; j < weapon.breakTags.Length; j++) {
                        if (hit.transform.tag == weapon.breakTags[j] && !breakhit && hit.distance != 0) {
                            BreakLook(hit);//add all look and feel stuff, uses hit normal
                            ProjectileDestory();
                            breakhit = true;
                            break;
                        }
                    }

                    //bounce
                    if (weapon.bounce) {
                        for (int j = 0; j < weapon.bounceTags.Length; j++) {
                            if (hit.transform.tag == weapon.bounceTags[j]) {
                                if (hit.distance != 0) {
                                    if (bounces > 0) {
                                        transform.position = hit.point + (hit.normal * (weapon.hitboxRadius));

                                        if (weapon.showCollisionDebug) {
                                            Debug.DrawLine(hit.point + (hit.normal * (weapon.hitboxRadius)), hit.point + (hit.normal * (weapon.hitboxRadius)) + (Vector3.up * 10), Color.cyan, 1f);
                                            Debug.DrawLine(hit.point + (hit.normal * (weapon.hitboxRadius)), hit.point + (hit.normal * 10), Color.magenta, 1f);
                                        }

                                        //in this part we are getting forward and normal and rotating it based on normal and randombounceangle
                                        collisionLength -= hit.distance;

                                        transform.forward = Vector3.Reflect(transform.forward, hit.normal);

                                        float angle = Vector3.Angle(transform.forward, hit.normal);

                                        Quaternion randomQ = Quaternion.Euler(Mathf.Clamp(Random.Range(-weapon.randomBounceAngle.x, weapon.randomBounceAngle.x), -(90 - angle), 90 - angle), Mathf.Clamp(Random.Range(-weapon.randomBounceAngle.y, weapon.randomBounceAngle.y), -(90 - angle), 90 - angle), 0f);

                                        transform.forward = randomQ * transform.forward;

                                        futureRotation = transform.rotation;

                                        futurePosition = transform.position + transform.forward * collisionLength;//sets new future position, will be used in cascading collision checks

                                        if (weapon.showCollisionDebug) {
                                            Debug.DrawLine(futurePosition, futurePosition + Vector3.up * 10, Color.yellow, .5f);
                                        }

                                        speedForward *= (1 / Mathf.Pow(2f, weapon.bounceFriction));//friction

                                        if (tr != null) {
                                            tr.AddPosition(hit.point + hit.normal * weapon.hitboxRadius);
                                        }

                                        //putting in all look and feel
                                        BounceLook(hit);

                                        if (weapon.stopStickOnBounce)//turn of stick if stopstickonbounce
                                        {
                                            stick = false;
                                        }
                                        bounces--;
                                        bouncehit = true;
                                        breakLoop = true;
                                        break;
                                    }
                                    else //if you run out of bounces you break
                                    {
                                        BreakLook(hit);//do break effects, essentialy break here
                                        ProjectileDestory();
                                        breakhit = true;
                                        breakLoop = true;
                                        break;

                                    }
                                }
                            }
                        }
                    }
                }
            }

        } while (weapon.bounce == true && collisionLength > 0 && bouncehit == true);

        bouncehit = false;
    }

    private Vector3 ProjectileStick() {
        RaycastHit[] hits;
        RaycastHit uhit;
        uhit = new RaycastHit();//
        hits = Physics.RaycastAll(transform.position, -transform.up, Mathf.Infinity);
        bool first = true;//if you know a better way to do this tell me
        bool stick = false;
        if (hits.Length > 0) {
            uhit = hits[0];//pointless, but we have to assign this to something...
        }

        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];

            for (int i2 = 0; i2 < weapon.projectileStickTags.Length; i2++) {
                if (hit.transform.tag == weapon.projectileStickTags[i2]) {
                    stick = true;
                    if (first)//
                    {
                        first = false;//
                        uhit = hit;
                    }
                    if (hit.distance < uhit.distance) {
                        uhit = hit;
                    }
                }
            }
        }

        if (stick) {
            if (uhit.distance > weapon.projectileHeight) {
                return (Vector3.down * (uhit.distance - weapon.projectileHeight));
            }
            if (uhit.distance < weapon.projectileHeight) {
                return (Vector3.up * (weapon.projectileHeight - uhit.distance));
            }
        }
        return new Vector3(0, 0, 0);
    }

    private void OnDrawGizmos() {
        //we show a rough gizmo for the hitboxes of the projectiles.
        if (this.enabled) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, weapon.hitboxRadius);
        }

        if (weapon.showHomingGizmo) {
            if (homingTarget == null) {
                if (homingGizmoFlash) {
                    Gizmos.color = Color.yellow;
                }
                else {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireSphere(transform.position, weapon.homingCheckIntervalRange);
            }
            else {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, weapon.homingCheckIntervalRange);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(homingTarget.transform.position, homingTarget.transform.localScale);
            }
        }
    }

    public void ProjectileDestory() {
        //this method is used to smoothly fade out the trail
        if (trail || particleTrailRef != null) {
            //we disable everything besides the trail, then add the begincountdown script.
            gameObject.GetComponent<MeshRenderer>().enabled = false;


            if (trail) {
                tr.emitting = false;
            }
            if (particleTrailRef != null) {
                particleTrailRef.Stop();
            }

            if (particleTrailRef == null) {
                gameObject.AddComponent<TweCountdownDestroy>().BeginCountdown(tr.time, poolID, weapon);
            }
            else if (trail) {
                gameObject.AddComponent<TweCountdownDestroy>().BeginCountdown(tr.time, poolID, weapon, particleTrailRef);
            }
            else {
                gameObject.AddComponent<TweCountdownDestroy>().BeginCountdown(poolID, weapon, particleTrailRef);
            }



            alive = false;
        }
        else {
            alive = false;

            if (weapon.usePool && poolID == weapon.poolID) {
                weapon.objectPool.Enqueue(gameObject);
                gameObject.SetActive(false);
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 초기화. 발사체 시작시 호출, OnEnable, Start에서만 호출 
    /// </summary>
    public void Initialize()
    {
        homingTarget = null;
        trail = weapon.trail;
        if (weapon.trail) {
            tr = gameObject.GetComponent<TrailRenderer>();
            tr.time = 0;
        }
        bounces = weapon.bounces;
        speedForward = weapon.initalForwardSpeed * (Random.Range(weapon.randomVelocityMultiplier.x, weapon.randomVelocityMultiplier.y));
        stick = weapon.projectileStick;
        life = weapon.lifetime;
        currentTime = 0f;
        damagehit = false;
        breakhit = false;


        /* 일렉트로 시티 한정. 무조건 타겟을 찾도록 설정 */
        if (weapon.is2D) {
            FindHomingTarget();
        }

        /*
        if (weapon.homing) {
            // HomingCheck(weapon.homingCheckInitialRange);
            FindHomingTarget();

        }
        */

        if (weapon.trail) {
            tr.emitting = true;
        }

        if (weapon.randomRotationSpeed == true) {
            rotationSpeed = new Vector3(Random.Range(-weapon.rotationSpeed.x, weapon.rotationSpeed.x), Random.Range(-weapon.rotationSpeed.y, weapon.rotationSpeed.y), Random.Range(-weapon.rotationSpeed.z, weapon.rotationSpeed.z));
        }
        else {
            rotationSpeed = weapon.rotationSpeed;
        }

        //the following is used for accuracy
        int inacBullets;
        int sign;
        if (weapon.burst) {
            inacBullets = weapon.bulletsPerBurst;
            if (weapon.reverseSweep) {
                sign = -1;
            }
            else {
                sign = 1;
            }
        }
        else {
            inacBullets = weapon.bullets;
            sign = 1;
        }



        //ACCURACY
        if (weapon.evenInnacuracy && ((weapon.burst && weapon.bulletsPerBurst > 1) || (!weapon.burst && weapon.bullets > 1))) {
            transform.rotation = Quaternion.Euler(aimpoint.rotation.eulerAngles.x + ((bulletNum * (sign * weapon.innacuracy.x / (inacBullets - 1))) - (sign * weapon.innacuracy.x / 2)), aimpoint.rotation.eulerAngles.y + ((bulletNum * (sign * weapon.innacuracy.y / (inacBullets - 1))) - (sign * weapon.innacuracy.y / 2)), aimpoint.rotation.eulerAngles.z + ((bulletNum * (sign * weapon.innacuracy.z / (inacBullets - 1))) - (sign * weapon.innacuracy.z / 2)));
        }
        else {
            transform.rotation = Quaternion.Euler(aimpoint.rotation.eulerAngles.x + Random.Range(-weapon.innacuracy.x / 2, weapon.innacuracy.x / 2), aimpoint.rotation.eulerAngles.y + Random.Range(-weapon.innacuracy.y / 2, weapon.innacuracy.y / 2), aimpoint.rotation.eulerAngles.z + Random.Range(-weapon.innacuracy.z / 2, weapon.innacuracy.z / 2));//big line, getting aimpoint rotation and adding innacuracy to xyz

            // homing 체크가 안되어있는 경우 시작부터 각을 잡아서 직선으로 발사한다. 
            if (weapon.is2D && homingTarget != null && !weapon.homing) {
                // transform.rotation = Quaternion.LookRotation(homingTarget.transform.position - transform.position);
            }

        }



    }


    /// <summary>
    /// 범위로 찾지 않음. 
    /// </summary>
    void FindHomingTarget() {
        if(!GameManager.main.CurrentEnemy) {
            homingTarget = GameManager.main.FakeTarget.gameObject;
        }
        else {
            homingTarget = GameManager.main.CurrentEnemy.gameObject;
        }

    }

    private void HomingCheck(float range) {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        for (int i = 0; i < hits.Length; i++) {
            for (int j = 0; j < weapon.homingTags.Length; j++) {
                if (hits[i].tag == weapon.homingTags[j]) {
                    homingTarget = hits[i].gameObject;
                }
            }
        }
    }

    //I made these their own method because i was constantly re-writing these parts. Even if they are only referneced once it helps when writing other parts.
    private void DamageLook(RaycastHit hit) {
        if (weapon.damageSound != null && damagehit == false)//only 1 particle,sound,spawn per hit per frame.
        {
            AudioSource.PlayClipAtPoint(weapon.damageSound, this.transform.position);
        }
        if (weapon.damageParticle != null && damagehit == false) {
            Instantiate(weapon.damageParticle, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
        }
        if (weapon.damageSpawn != null && damagehit == false) {
            Instantiate(weapon.damageSpawn, hit.point, this.transform.rotation);
        }
    }
    private void BounceLook(RaycastHit hit) {
        if (weapon.bounceSound != null) {
            AudioSource.PlayClipAtPoint(weapon.bounceSound, this.transform.position);
        }
        if (weapon.bounceParticle != null) {
            Instantiate(weapon.bounceParticle, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
        }
        if (weapon.bounceSpawn != null) {
            Instantiate(weapon.bounceSpawn, hit.point, this.transform.rotation);
        }
    }
    private void BreakLook(RaycastHit hit) {
        if (weapon.breakSound != null) {
            AudioSource.PlayClipAtPoint(weapon.breakSound, this.transform.position);
        }
        if (weapon.breakParticle != null) {
            Instantiate(weapon.breakParticle, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
        }
        if (weapon.breakSpawn != null) {
            Instantiate(weapon.breakSpawn, hit.point, this.transform.rotation);
        }
    }
    private void FizzleLook() {
        if (weapon.fizzleSound != null) {
            AudioSource.PlayClipAtPoint(weapon.fizzleSound, this.transform.position);
        }
        if (weapon.fizzleParticle != null) {
            Instantiate(weapon.fizzleParticle, transform.position, transform.rotation);
        }
        if (weapon.fizzleSpawn != null) {
            Instantiate(weapon.fizzleSpawn, transform.position, transform.rotation);
        }
    }
}
