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

    public CharacterController characterController;

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
    }








    // Update is called once per frame
    void Update()
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
          Player's movement
          In unity we move the x axis for side to side movement and z axis for forward back movement. y controls up and down movement.
          for the moment we define 0f to vertical y axis because we don't take jumping/up/down movement
        */
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        /* Player's speed
           while leftShift key is pressed down (GetKey method) make player run otherwise walk
        */
        if (Input.GetKey(KeyCode.LeftShift)) {
            activeMoveSpeed = runSpeed;
        } else 
        {
            activeMoveSpeed = walkSpeed;
        }

        // basing movement based on z value and rotation of the player
        // it allows movement in all directions by adding both axes
        // use .normalized to prevent user from traveling at a faster speed within diagonal movement
        // use activeMoveSpeed to get player's speed
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeMoveSpeed;

        // remember to multiply by Time.deltaTime which is how long it takes each frame update to happen, this allow us to have a very consistent amount of movement
        // we use characterController instead of transform directly because we want to implement the physics to collide that comes with CharacterController
        characterController.Move(movement * Time.deltaTime);
    }
}
