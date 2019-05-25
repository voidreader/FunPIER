using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager main = null;

    public static bool itemInput = false;
    public static bool inputLock = false;

    public Camera mainCamera;
    public Camera bgCamera;
    public Camera UICamera;

    Touch touch; // 터치 체크용
    Vector2 pressPos;
    Vector2 endPos;
    Vector2 currentSwipe;


    // 레이케스트용 
    Vector2 point;
    private Ray ray;
    private RaycastHit hit;
    private LayerMask mask;
    GameObject hitTile;
    TileCtrl tile;
    RaycastHit2D hit2D;
    RaycastHit2D hitArea;
    public bool _isAreaHit = false;


    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

        if (itemInput) {
            CheckItemInput();
            return;
        }


        if (!InGame.isPlaying)
            return;

        if (PageManager.main.pageStack.Count > 0)
            return;

        if (LobbyManager.isAnimation)
            return;

        // 레드문 클릭 체크 
        if (CheckRedMoonInput())
            return;

        /*
        if(Application.isEditor)
            CheckMouseSwipe();
        else
            CheckSwipe();
        */

        CheckSwipeArea();





    }

    /// <summary>
    /// 
    /// </summary>
    public void LockUIClose() {
        inputLock = true;
        StartCoroutine(WaitingUnlock());
    }

    IEnumerator WaitingUnlock() {
        yield return new WaitForSeconds(0.3f);
        inputLock = false;
    }


    #region 아이템 입력 대기 

    /// <summary>
    ///  아이템 입력 대기 (업그레이더만)
    /// </summary>
    void CheckItemInput() {

        if(GetInputDown()) {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            mask = 1 << LayerMask.NameToLayer("Tile");

            if(Physics.Raycast(ray, out hit, 20f, mask)) { // 타일 터치 체크 
                hitTile = hit.transform.gameObject;

                if (!hitTile.CompareTag("Tile"))
                    return;

                tile = hitTile.GetComponent<TileCtrl>();

                if (tile.chip == null)
                    return;

                if (tile.chip.id >= 512)
                    return;

                if (PierSystem.main.AdminPlay)
                    tile.UpgradeFinal();
                else 
                    tile.UpgradeChip(); // 업그레이드 처리.
                // InGame에 업그레이드가 완료되었다고 처리
                InGame.main.OnCompleteItemUpgrade();
            }

        }
    }

    #endregion

    #region 레드문 입력 대기 

    /// <summary>
    ///  레드문 입력 
    /// </summary>
    bool CheckRedMoonInput() {

        if (GetInputDown()) {
            point = bgCamera.ScreenPointToRay(GetInputPosition()).origin;
            // mask = 1 << LayerMask.NameToLayer("BG");

            hit2D = Physics2D.Raycast(point, Vector2.zero);

            if (hit2D.transform == null)
                return false;

            if (!hit2D.transform.CompareTag("RedMoon"))
                return false;

            InGame.main.OnClickRedMoon();
            return true;

            /*
            if (Physics2D.Raycast(point, out hit, 20f, mask)) { // 레드문 터치 체크 
                hitTile = hit.transform.gameObject;

                if (!hitTile.CompareTag("RedMoon"))
                    return false;

                InGame.main.OnClickRedMoon();
                return true;

            }
            */

        }

        return false;
    }

    #endregion

    void CheckSwipeArea() {

        if (!Application.isEditor && Input.touches.Length <= 0)
            return;

        if (inputLock)
            return;


        
        if (GetInputDown()) {
            // UI Camera 선 체크
            /*
            point = UICamera.ScreenPointToRay(GetInputPosition()).origin;
            mask = 1 << LayerMask.NameToLayer("UI");
            if (Physics.Raycast(ray, out hit, 20f, mask)) {
                if (hit.transform != null) {
                    _isAreaHit = false;
                    return;
                }
            }
            */
            

            point = bgCamera.ScreenPointToRay(GetInputPosition()).origin;
            hitArea = Physics2D.Raycast(point, Vector2.zero);

            if(hitArea.transform == null || !hitArea.transform.CompareTag("Area")) {
                _isAreaHit = false;
                return;
            }

            _isAreaHit = true;
            pressPos = GetInputPosition(); // 입력 위치 
        }

        if(GetInputUp()) {
            if (!_isAreaHit)
                return;

            endPos = GetInputPosition();
            currentSwipe = endPos - pressPos;

            if (Vector3.Distance(endPos, pressPos) < 10) {
                _isAreaHit = false;
                return;
            }

            if (endPos.x > pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Right);
            else if (endPos.x < pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Up);
            else if (endPos.x < pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Left);
            else if (endPos.x > pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Down);
            

            _isAreaHit = false;

        }

    }


    /// <summary>
    /// 모바일 기기 터치 체크 
    /// </summary>
    void CheckSwipe() {
        if (Input.touches.Length <= 0)
            return;

        touch = Input.GetTouch(0);

        if(touch.phase == TouchPhase.Began) {
            pressPos = touch.position;
        }

        if(touch.phase == TouchPhase.Ended) {
            endPos = touch.position;
            currentSwipe = endPos - pressPos;

            // Debug.Log("Distance Check in Swipe :: " + Vector3.Distance(endPos, pressPos));
            if (Vector3.Distance(endPos, pressPos) < 10)
                return;



            if (endPos.x > pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Right);
            else if (endPos.x < pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Up);
            else if (endPos.x < pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Left);
            else if (endPos.x > pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Down);


            /*
            if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                InGame.main.MoveTiles(Moving.Up);
            else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                InGame.main.MoveTiles(Moving.Down);
            else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                InGame.main.MoveTiles(Moving.Left);
            else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                InGame.main.MoveTiles(Moving.Right);
            */
        }
    }

    void CheckMouseSwipe() {


        if (Input.GetMouseButtonDown(0)) {
            //save began touch 2d point
            pressPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) {
            //save ended touch 2d point
            endPos = Input.mousePosition;

            //create vector from the two points
            currentSwipe = endPos - pressPos;

            //normalize the 2d vector
            // currentSwipe.Normalize();
            Debug.Log("Distance Check in MouseSwipe :: " + Vector3.Distance(endPos, pressPos));
            
            if (Vector3.Distance(endPos, pressPos) < 10)
                return;
            
            


            if (endPos.x > pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Right);
            else if (endPos.x < pressPos.x && endPos.y > pressPos.y)
                InGame.main.MoveTiles(Moving.Up);
            else if (endPos.x < pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Left);
            else if (endPos.x > pressPos.x && endPos.y < pressPos.y)
                InGame.main.MoveTiles(Moving.Down);



        }
    }



    #region INPUT_METHOD
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_WEBGL
    public static Vector2 GetInputPosition(int touchPoint = 0) {
        return Input.mousePosition;
    }

    public static bool GetInputDown(int inputNum = 0) {
        return Input.GetMouseButtonDown(inputNum);
    }

    public static bool GetInputUp(int inputNum = 0) {
        return Input.GetMouseButtonUp(inputNum);
    }

    public static bool GetInput(int inputNum = 0) {
        //Debug.Log("Get Input :: " +  inputNum);
        return Input.GetMouseButton(inputNum);
    }

#elif UNITY_ANDROID || UNITY_IPHONE
	
	public static Vector2 GetInputPosition(int touchPoint = 0)
	{
		return Input.touches[touchPoint].position;
		
	}
	
	public static bool GetInputDown(int inputNum = 0)
	{
		if(Input.touches.Length == 0)
			return false;

		return (Input.touches[inputNum].phase == TouchPhase.Began);
	}
	
	public static bool GetInputUp(int inputNum = 0)
	{
		return (Input.touches[inputNum].phase == TouchPhase.Ended);
	}
	
	public static bool GetInput(int inputNum = 0)
	{
		return (Input.touches[inputNum].phase == TouchPhase.Stationary);
	}
#endif
    #endregion
}
