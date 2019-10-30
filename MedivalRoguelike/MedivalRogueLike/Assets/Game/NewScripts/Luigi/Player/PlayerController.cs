using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour
{
    public float _mSpeed;

    [SerializeField]
    private Transform _rig;
    [SerializeField]
    private Transform _camRig;
    [SerializeField]
    private GameObject _pointer;
    [SerializeField]
    private GameObject _leftArm;
    [SerializeField]
    private GameObject _rightArm;

    private bool _teleport;
    private Vector2 _moveAxis;
    private bool _run;
    private float _runSpeed;
    private bool _hasPosition = false;
    private bool _isTeleporting = false;
    private float _fadeTime = 1f;

    private void Start()
    {
        _runSpeed = 2;
    }

    void Update()
    {
        if (SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport").GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            _teleport = true;
        }
        _hasPosition = UpdatePointer();
        _pointer.SetActive(_hasPosition);

        if (_teleport)
        {
            TryTeleport();
        }
    }

    private void FixedUpdate()
    {
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
        if (!_hasPosition || _isTeleporting)
        {
            return;
        }

        Vector3 groundPosition = new Vector3(_rig.position.x, _rig.position.y, _rig.position.z);
        Vector3 translateVector = _pointer.transform.position - groundPosition;
        
        StartCoroutine(Move(_rig, translateVector));
        _teleport = false;
    }

    private IEnumerator Move(Transform camRig, Vector3 translation)
    {
        _isTeleporting = true;
        SteamVR_Fade.Start(Color.black, _fadeTime, true);

        yield return new WaitForSeconds(_fadeTime);

        camRig.position += translation;
        SteamVR_Fade.Start(Color.clear, _fadeTime, true);
        _isTeleporting = false;
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(_rightArm.transform.position, _rightArm.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _pointer.transform.position = hit.point;
            return true;
        }
        return false;
    }
}
