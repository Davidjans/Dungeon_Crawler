using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _rig;
    [SerializeField]
    private Transform _camRig;
    //Assign Movement variables
    private bool _teleport;
    private Vector2 _moveAxis;
    private bool _run;
    private float _runSpeed;
    public float _mSpeed;
    //Assign Teleport variables
    private bool _hasPosition = false;
    private bool _isTeleporting = false;
    private float _fadeTime = 1f;
    //assign Pointer
    [SerializeField]
    private GameObject _pointer;
    //Assign Hands
    [SerializeField]
    private GameObject _leftArm;
    [SerializeField]
    private GameObject _rightArm;


    private void Start()
    {
        _runSpeed = 2;
    }

    void Update()
    {
        //Assign teleport to vr input
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("cast").GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            print("Righthand Grabbing");
        }
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip").GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            print("Righthand Releasing");
            _teleport = true;
        }
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch").GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            print("Lefthand Grabbing");
        }
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch").GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            print("Leftthand Releasing");
        }

        //Pointer
        _hasPosition = UpdatePointer();
        _pointer.SetActive(_hasPosition);

        //Teleport
        if (_teleport)
        {
            TryTeleport();
        }
    }

    private void FixedUpdate()
    {
        //Assign Movement to vr input
        _moveAxis = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("move").GetAxis(SteamVR_Input_Sources.RightHand);
        _run = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("run").GetState(SteamVR_Input_Sources.RightHand);
        if (_rig != null)
        {
            _rig.position += (_camRig.transform.right * _moveAxis.x + _camRig.transform.forward * _moveAxis.y) * Time.deltaTime;
            _rig.position = new Vector3(_rig.position.x, 0, _rig.position.z);
        }
    }

    private void TryTeleport()
    {
        //Check for valid position, annd already teleporting
        if (!_hasPosition || _isTeleporting)
        {
            return;
        }
        //figure out translation
        Vector3 groundPosition = new Vector3(_rig.position.x, _rig.position.y, _rig.position.z);
        Vector3 translateVector = _pointer.transform.position - groundPosition;
        //Move
        StartCoroutine(Move(_rig, translateVector));
        _teleport = false;
    }

    private IEnumerator Move(Transform camRig, Vector3 translation)
    {
        //Flag
        _isTeleporting = true;
        //Fase to black
        SteamVR_Fade.Start(Color.black, _fadeTime, true);
        //Apply translation
        yield return new WaitForSeconds(_fadeTime);
        camRig.position += translation;
        //Fade to clear
        SteamVR_Fade.Start(Color.clear, _fadeTime, true);
        //De-Flag
        _isTeleporting = false;
    }

    private bool UpdatePointer()
    {
        //Ray from the controller
        Ray ray = new Ray(_rightArm.transform.position, _rightArm.transform.forward);
        RaycastHit hit;

        //if hit
        if (Physics.Raycast(ray, out hit))
        {
            _pointer.transform.position = hit.point;
            return true;
        }

        //if not
        return false;
    }
}
