using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float Gravity = 15.0f;
    private float JumpSpeed = 15.0f;
    private float JumpFalloff = 6.0f;
    private float StartingJumpSpeed = 15.0f;
    private float EndingJumpSpeed = 1.0f;
    private float Speed = 10.0f;
    private float AirSpeed = 15.0f;
    private float CameraSpeed = 6.0f;
    public CharacterController Player;
    private InputController InputController;
    public Camera PlayerCamera;
    public GameObject PlayerCameraRig;
    public GameObject RobotBottom;
    public bool Jumping;
    void Awake()
    {
        InputController = FindObjectOfType<InputController>();
    }

    void Update()
    {
       JumpSpeed = Mathf.Lerp(JumpSpeed,EndingJumpSpeed,JumpFalloff*Time.deltaTime); 
       MovePlayerRelativeToCamera();
       RotateCamera();
       MoveCamera();
       Jump();
    }


    private void MoveCamera(){
            PlayerCameraRig.transform.position = Vector3.Lerp(  PlayerCameraRig.transform.position ,Player.transform.position, CameraSpeed *Time.deltaTime);
    }

    private void RotateCamera(){
        var lookVector = InputController.LookInputVector;
        var oldRotation = PlayerCameraRig.transform.rotation;
        var deltaRotation = InputController.LookInputVector.x * Time.deltaTime;
        var newAngles = new Vector3(0,oldRotation.eulerAngles.y+deltaRotation,0);
        PlayerCameraRig.transform.eulerAngles = newAngles;
    }

    private void MovePlayerRelativeToCamera(){
        var movementVector =  InputController.MovementInputVector; // x and y
   
    

        var PlayerForwardVector = PlayerCameraRig.transform.forward *-1.0f;
        var PlayerLeftVector = PlayerCameraRig.transform.right *-1.0f;
        
        PlayerForwardVector *= movementVector.y;
        PlayerLeftVector *= movementVector.x;
        var newPlayerDirection = PlayerForwardVector + PlayerLeftVector;
        if(movementVector.magnitude != 0){
            Player.transform.forward = PlayerCameraRig.transform.forward *-1;
        RobotBottom.transform.forward = newPlayerDirection;
        }
        var gravityVector = Vector3.up* Time.deltaTime;
        var newMovementVector =(newPlayerDirection  * Time.deltaTime);

        if(Jumping){
            gravityVector *= JumpSpeed;  
            newMovementVector *= AirSpeed;
        }
        else{
            gravityVector *= -1 * Gravity;  
            newMovementVector *= Speed;

        }
        Player.Move(newMovementVector + gravityVector);
        
    }

    private void Jump(){
        if (Player.isGrounded && InputController.WasJumpPressed  && Jumping == false) {
            StartCoroutine(JumpAndFall());
        }
        if(InputController.WasJumpReleased  && Jumping == true){
              StartCoroutine(ShortJump());
        }
    }

    IEnumerator ShortJump()
    {
        Jumping = true;
        yield return new WaitForSeconds(0.05f);
        Jumping = false;
    }
    IEnumerator JumpAndFall()
    {

        GameController.Instance.SoundController.PlayJumpSFX();
        JumpSpeed = StartingJumpSpeed;
        Jumping = true;
        yield return new WaitForSeconds(0.4f);
        Jumping = false;
    }
}
