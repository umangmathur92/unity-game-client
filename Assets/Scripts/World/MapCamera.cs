using UnityEngine;

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapCamera : MonoBehaviour {
    public MenuScript menuScript;
  bool dragging = false;
  Vector3 mouseDownPos;
  Vector3 oldCameraPos;
  //Holds mouse mode; Mode 0 = normal, Mode 1 = Attack tile selected, Mode 2 = Defender tile selected, Mode 3 = Selected Home Tile, Mode 4 = possible Home Time clicked
  public int mode { get; set;}
  
  //Camera Zoom related variables
  private float cameraZoomed = 10f;
  private float cameraNormal = 60f;
  private float cameraPulled = 70f;
  private float cameraZoomedMin = 10f;
  private float cameraZoomedMax = 120f;
  private float cameraZoomTarget;
  private float cameraSmoothing = 3f;
  private float cameraSmoothingZoom = 9f;
  public bool isPanning { get; set; }
  public bool isZooming { get; set; }
  public bool isZoomed { get; set; }
  public bool isLeaving { get; set; }
  public short cameraStage = 0;

  public float cameraOffset { get; set; }
  private Vector3 cameraNextPos;
  private float deltaTime;
  private Vector3 startPos;
  private float startZoom;

  public Tile CurrentTile { get; set; }
  public Tile AttackerTile { get; set; }
  public Tile DefenderTile { get; set; }
  public Tile FirstTile { get; set; }

  //Type of battle initiated; 0 = none, 1 = proxy, 2 = direct
  public int battleType { get; private set; }
  bool choosingFirstTile = false;


  // Use this for initialization
  void Start() {
    Setup();
  }

  // Update is called once per frame
  void Update() {
    if (isLeaving) {
      switch (cameraStage) {
        case 1:
          if (Vector3.Distance(transform.position, cameraNextPos) > 0.1f || Mathf.Abs(GetComponent<Camera>().fieldOfView - cameraPulled) > 0.1f) {
            deltaTime += Time.deltaTime * cameraSmoothing;
            transform.position = Vector3.Lerp(startPos, cameraNextPos, deltaTime);
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(startZoom, cameraPulled, deltaTime);
          } else {
            cameraStage++;
            deltaTime = 0;
            startZoom = GetComponent<Camera>().fieldOfView;
          }
          break;
        case 2:
          if (Mathf.Abs(GetComponent<Camera>().fieldOfView - cameraZoomed) > 0.1f) {
            deltaTime += Time.deltaTime * 2;
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(startZoom, cameraZoomed, deltaTime);
          } else {
            Game.SwitchScene("Ecosystem");
          }
          break;
      }
    }

    if (isPanning && Vector3.Distance(transform.position, cameraNextPos) > 0.1f) {
      deltaTime += Time.deltaTime * cameraSmoothing;
      transform.position = Vector3.Lerp(startPos, cameraNextPos, deltaTime);
    } else {
      isPanning = false;
    }
    
    if (Input.GetMouseButtonDown(0) ) {
      if (menuScript.menuOpen == false) {
        dragging = true;
        mouseDownPos = Input.mousePosition;
        oldCameraPos = transform.position;
      }
    }
    
    if (Input.GetMouseButtonUp(0)) {
      dragging = false;
    }
    
    if (dragging) {
     PointerEventData pe = new PointerEventData(EventSystem.current);
     pe.position =  Input.mousePosition;
     List<RaycastResult> hits = new List<RaycastResult>();
     EventSystem.current.RaycastAll( pe, hits );
     if (hits.Count > 0) {
       return;
     }
      if (isZoomed) {
        transform.position = new Vector3(oldCameraPos.x + (mouseDownPos.x - Input.mousePosition.x) * .04f, oldCameraPos.y, oldCameraPos.z + (mouseDownPos.y - Input.mousePosition.y) * .04f);
      } else {
        transform.position = new Vector3(oldCameraPos.x + (mouseDownPos.x - Input.mousePosition.x) * .25f, oldCameraPos.y, oldCameraPos.z + (mouseDownPos.y - Input.mousePosition.y) * .25f);
      }
    }

	if (isZooming && !Mathf.Approximately(GetComponent<Camera>().fieldOfView, cameraZoomTarget)) {
	  GetComponent<Camera>().fieldOfView = 
	      Mathf.Lerp(GetComponent<Camera>().fieldOfView, cameraZoomTarget, Time.deltaTime * cameraSmoothingZoom);
	} else {
	  isZooming = false;
	}

  }

  public void Center(int player_id) {
    Vector3 center = GameObject.Find("Map").GetComponent<Map>().GetCenterPoint(player_id);
    center = new Vector3(center.x, transform.position.y, center.z + cameraOffset);

    cameraNextPos = center;
    isPanning = true;
    deltaTime = 0;
  }

  public void Move(int player_id) {
    Move(GameObject.Find("Map").GetComponent<Map>().GetCenterPoint(player_id));
  }

  public void Move(Vector3 position) {
    GetComponent<Camera>().fieldOfView = cameraNormal;
    startPos = GetComponent<Camera>().transform.position;
    startZoom = GetComponent<Camera>().fieldOfView;
    deltaTime = 0;
    cameraStage = 1;
    cameraNextPos = new Vector3(position.x, cameraNextPos.y, position.z + cameraOffset);

    isLeaving = true;
    isPanning = true;
  }

  public void Setup() {
    RaycastHit hit = new RaycastHit();
    if (Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)), out hit)) {
      cameraOffset = transform.position.z - hit.point.z;
    }
    isZooming = false;
  }

  public void reset() {
    if (choosingFirstTile) {
      mode = 3;
    }
    else {
      mode = 0;
      //RoamingCursor.SetActive(true);
    }
    battleType = 0;
    //GameObject.Find("MainObject").GetComponent<Battle>().isHidden = true;
    //DefenderCursor.SetActive(false);
    //AttackerCursor.SetActive(false);
    AttackerTile = null;
    DefenderTile = null;
    FirstTile = null;
  }

  public void FirstTileProcess(bool ongoing) {
    choosingFirstTile = ongoing;
  }


  public void ZoomIn() {
    if (GetComponent<Camera> ().fieldOfView > cameraZoomedMin) {
      cameraZoomTarget = Mathf.Max (Mathf.Floor((GetComponent<Camera> ().fieldOfView - 11) / 10) * 10, cameraZoomedMin);
      // Debug.Log ("MapCamera: fieldOfView, cameraZoomTarget: " + GetComponent<Camera> ().fieldOfView + " " + cameraZoomTarget);
	  isZooming = true;
	}
  }

  public void ZoomOut() {
    if (GetComponent<Camera> ().fieldOfView < cameraZoomedMax) {
      cameraZoomTarget = Mathf.Min (Mathf.Floor((GetComponent<Camera> ().fieldOfView + 20) / 10) * 10, cameraZoomedMax);
      // Debug.Log ("MapCamera: fieldOfView, cameraZoomTarget: " + GetComponent<Camera> ().fieldOfView + " " + cameraZoomTarget);
      isZooming = true;
    }
  }
}
