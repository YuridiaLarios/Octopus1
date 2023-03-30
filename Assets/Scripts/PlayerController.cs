using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;

    public float mouseSensitivity = 4f; // how fast to move left right camera with the mouse
    private float verticalRotStore; // rotation up and down limit
    private Vector2 mouseInput; // vector2 = x and y axes values

    public bool invertLook;

    public float walkSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed; // to control how much movement we are applying to player (either walking or running speed)
    private Vector3 moveDirection, movement;
    private Camera camera; // when character dies, the respawn object is a new one, so we don't want to manually have to assign the camera to that player, cuz the game is already running

    public float jumpForce = 12f, gravityModifier = 2.5f;
    public CharacterController characterController;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    // Start is called before the first frame update
    void Start()    
    {
 /************************************************************************************************************************************************************************/

        /* cursor dissapears and it is centered to focus on the player itself
           in unity click inside game to active cursorLocked
           in unity press escape key to see your mouse again and keep developing.
         */
        Cursor.lockState = CursorLockMode.Locked;
        /************************************************************************************************************************************************************************/

        /************************************************************************************************************************************************************************/
        camera = Camera.main;

 
/************************************************************************************************************************************************************************/
    }










    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            /************************************************************************************************************************************************************************/

            /*
             Get Mouse Input
             this is how we get mouseInput and multiply by mouse sensitivity for camera to move faster left and right
            */

            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            /************************************************************************************************************************************************************************/

            /* 
             Horizontal camera rotation movement
             This is how we apply mouseInput to our player. 
             we can directly access the transform component of the player since this script is attached to him.
             we don't want to change x or z only y angle. 
             we change y value only because in unity when we manually move our character horizontally that's the value that changes.
             this moves player camera left and right (horizontal rotation) 
             we use Quaternion.Eurler because it allows us to treat this as a vector3 object so we can just affect based on the x,y,z values.
             pronounced as "oiler angles" 
            */
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + mouseInput.x,
                transform.rotation.eulerAngles.z
                );

            /************************************************************************************************************************************************************************/

            /*
             Vertical camera rotation movement
             We use a Clamp function to limit the angle view to 60 degrees only (common standard in gaming)
            */
            verticalRotStore += mouseInput.y; // equals to whatever it currently is on unity UI + mouseInput.y
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            if (invertLook)
            {
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            }
            /************************************************************************************************************************************************************************/
            /*
              Player's movement (x,z, y axis)
              In unity we move the x axis for side to side movement and z axis for forward back movement. y controls up and down movement.
              for the moment we define 0f to vertical y axis because we don't take jumping/up/down movement
            */
            moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            /* Player's speed
               while leftShift key is pressed down (GetKey method) make player run otherwise walk
            */
            if (Input.GetKey(KeyCode.LeftShift))
            {
                activeMoveSpeed = runSpeed;
            }
            else
            {
                activeMoveSpeed = walkSpeed;
            }

            // when you create a variable like this in the update() loop, you DO NOT make it public or private cuz it doesn't matter, it won't appear on the inspector
            float yVelocity = movement.y;

            // basing movement based on z value and rotation of the player
            // it allows movement in all directions by adding both axes
            // use .normalized to prevent user from traveling at a faster speed within diagonal movement
            // use activeMoveSpeed to get player's speed
            movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeMoveSpeed;

            // in real life once one starts falling, the velocity increases over time.
            // When the update() loop starts, the movement.y will be storing whatever value we set last time
            movement.y = yVelocity;

            // if we don't break y velocity will keep increasing, so reset it back to 0f once the character has landed
            if (characterController.isGrounded)
            {
                movement.y = 0f;
            }

            // a raycast is an invisible line that checks if anything is interacting within that line.
            // signature: (where you start, the direction you go, how far you should go, any of the layers you should check)
            isGrounded = Physics.Raycast(groundCheckPoint.position,
                                         Vector3.down,
                                         .25f,
                                         groundLayers);

            // Player's jumping. map to the space button on unity keyboard and to "y" in xbox controller
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                movement.y = jumpForce;
            }

            /* Player's gravity (y gravity)
            * Do NOT remove Time.deltaTime, the behavior may look right but we are basically pushing our user to move downwards if removed.
            * Add a gravityModifier to make sure jumping around makes sense in the game (is not too high and too slow when falling down).
            */
            movement.y += Physics.gravity.y * Time.deltaTime * gravityModifier;

            // remember to multiply by Time.deltaTime which is how long it takes each frame update to happen, this allow us to have a very consistent amount of movement
            // we use characterController instead of transform directly because we want to implement the physics to collide that comes with CharacterController
            characterController.Move(movement * Time.deltaTime);
        }
        /************************************************************************************************************************************************************************/
        // allow user to lock and unlock mouse when pressing the escape key and clicking back into the game screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }











    // After we deal with any kind of normal basic stuff that we are doing we can do special things in the LateUpdate to esure they happen after everything else
    private void LateUpdate()
    {
        /************************************************************************************************************************************************************************/
        // when character dies, the respawn object is a new one, so we don't want to manually have to assign the camera to that player, cuz the game is already running
        // we want to update the position of the camera, ensure the player has move before we update the camera

        camera.transform.position = viewPoint.position;
        camera.transform.rotation = viewPoint.rotation;

        /************************************************************************************************************************************************************************/


    }

}