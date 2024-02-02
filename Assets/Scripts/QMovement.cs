#define USE_CROUCH
#define MOUSE_SMOOTHING
using UnityEngine;

public class QMovement : MonoBehaviour
{
    [Header("Player Components")]
    public Camera playerCamera;
    public GameObject playerHands;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 3.0f;

    [Header("Headbobbing")]
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float bobbingSpeed = 10;

    private const float FRICTION = 5.0f;
    private const float DECC_SPEED = 3.5f;
    private const float ACC_SPEED = 10.0f;
    private const float ACC_SPEED_AIR = 1.5f;
    private const float JUMP_VEL = 5.8f;
    private const float JUMP_ACC = 1.42f;
    private const float GRAVITY_ADD = 17.0f;
    private const float CAMERA_OFFSET = 0.72f;
    private const float PLAYER_HEIGHT = 1.8f;
    private const float PLAYER_TILT = 8;
    private const float PLAYER_TILTSPEED = 3;

#if MOUSE_SMOOTHING
    [RangeAttribute(0.0f, 1.0f)] public float mouseSmoothing = 0.1f;
    private Quaternion targetRotation;
#endif
    private CharacterController controller;
    private Vector3 move_input;
    private Vector3 move_direction;
    private Vector3 move_vector;
    private Vector3 vector_down;
    private Vector3 surface_normal;
    private Vector3 camera_offset;
    private float camera_zoffset;
    private Quaternion player_rotation;
    private RaycastHit hit_surface;
    private float frame_time = 0.0f;
    private float rotation_input;
    private float look_input;
    private float look_y = 0.0f;
    private float move_speed;
    private float dot;
    private float vel_add;
    private float vel_mul;
    private float speed;
    private float speed_mul;
    private float  yOffset;
    private float starterAmp;
#if USE_CROUCH
	private float crouch_value = 0;
	private float crouch_value_s = 0;
	private Vector3 center_offset;
#endif

    void Start()
    {
        player_rotation = transform.rotation;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        controller.skinWidth = 0.03f;
        controller.height = PLAYER_HEIGHT;
        controller.radius = 0.35f;
        controller.minMoveDistance = 0; // This is required for CharacterController.isGrounded to always work.
        move_vector = new Vector3(0, -0.5f, 0);
        vector_down = new Vector3(0, -1.0f, 0);
        surface_normal = new Vector3(0, 1.0f, 0);
        move_input = new Vector3(0, 0, 0);
        camera_offset = new Vector3(0, CAMERA_OFFSET, 0);
        starterAmp = amplitude;
#if USE_CROUCH
		center_offset = new Vector3(0, 0, 0);
#endif
    }

    void TiltCamera()
    {
        camera_zoffset = Mathf.Lerp(camera_zoffset, -Input.GetAxis("Horizontal") * PLAYER_TILT, PLAYER_TILTSPEED * Time.deltaTime);
    }

    void HeadBob()
    {
        if (controller.velocity.magnitude > 0 && controller.isGrounded)
        {

                amplitude = starterAmp;

        }
        else
        {
            amplitude = 0;
        }

        if (crouch_value > 0)
        {
            return;
        }

    // Calculate the new yOffset using Mathf.Sin and amplitude
    float targetYOffset = CAMERA_OFFSET + Mathf.Sin(frequency * Time.time) * amplitude;

        // Use Mathf.Lerp to smoothly interpolate between current yOffset and targetYOffset
        yOffset = Mathf.Lerp(yOffset, targetYOffset, Time.deltaTime * bobbingSpeed);

        // Set the new camera offset
        camera_offset.y = yOffset;
    }


    void Update()
    {
        TiltCamera();
        HeadBob();
        frame_time = Time.deltaTime;
        move_input.x = Input.GetAxisRaw("Horizontal");
        move_input.z = Input.GetAxisRaw("Vertical");
        rotation_input = Input.GetAxis("Mouse X") * rotationSpeed;
        look_input = Input.GetAxis("Mouse Y") * rotationSpeed * 0.9f; // Make vertical mouse look less sensitive.
        look_y -= look_input;
        look_y = Mathf.Clamp(look_y, -90.0f, 90.0f);
        player_rotation *= Quaternion.Euler(0, rotation_input, 0);
        move_direction = player_rotation * move_input;
        if (controller.isGrounded)
        {
            if (Physics.Raycast(transform.position, vector_down, out hit_surface, 1.5f))
            {
                surface_normal = hit_surface.normal;
                move_direction = ProjectOnPlane(move_direction, surface_normal); // Stick to the ground on slopes.
            }
            move_direction.Normalize();
#if USE_CROUCH
			move_speed = move_direction.magnitude * (moveSpeed - crouch_value_s * 3.0f);
#else
            move_speed = move_direction.magnitude * moveSpeed;
#endif
            dot = move_vector.x * move_direction.x + move_vector.y * move_direction.y + move_vector.z * move_direction.z;
            speed = (float)System.Math.Sqrt(move_vector.x * move_vector.x + move_vector.z * move_vector.z);
            speed_mul = speed - (speed < DECC_SPEED ? DECC_SPEED : speed) * FRICTION * frame_time;
            if (speed_mul < 0) speed_mul = 0;
            if (speed > 0) speed_mul /= speed;
            move_vector *= speed_mul;
            vel_add = move_speed - dot;
            vel_mul = ACC_SPEED * frame_time * move_speed;
            if (vel_mul > vel_add) vel_mul = vel_add;
            move_vector += move_direction * vel_mul;
            if (move_vector.y > -0.5f) move_vector.y = -0.5f; // Make sure there is always a little gravity to keep the character on the ground.
            if (Input.GetButtonDown("Jump"))
            {
                if (surface_normal.y > 0.5f) // Do not jump on high slopes.
                {
                    move_vector *= JUMP_ACC;
                    move_vector.y = JUMP_VEL;
                }
            }
        }
        else // In Air
        {
            move_direction.Normalize();
            move_speed = move_direction.magnitude * moveSpeed;
            dot = move_vector.x * move_direction.x + move_vector.y * move_direction.y + move_vector.z * move_direction.z;
            vel_add = move_speed - dot;
            vel_mul = ACC_SPEED_AIR * frame_time * move_speed;
            if (vel_mul > vel_add) vel_mul = vel_add;
            if (vel_mul > 0) move_vector += move_direction * vel_mul;
            move_vector.y -= GRAVITY_ADD * frame_time;
        }
#if USE_CROUCH
		if (Input.GetButton("Crouch"))
		{
			if (crouch_value < 1.0f)
			{
				crouch_value += frame_time * 5.7f;
				crouch_value_s = Mathf.Clamp01(crouch_value);
				center_offset.y = crouch_value_s * -0.5f;
				controller.height = PLAYER_HEIGHT - crouch_value_s;
				controller.center = center_offset;
				camera_offset.y = CAMERA_OFFSET - crouch_value_s * 0.9f;
			}
		}
		else
		{
			if (crouch_value > 0)
			{
				RaycastHit hit_up;
				if (!Physics.SphereCast(playerCamera.transform.position + vector_down * 0.25f, 0.3f, new Vector3(0, 1.0f, 0), out hit_up, 0.3f)) // Check if there is a space for player to raise
				{
					crouch_value -= frame_time * 5.0f;
					crouch_value_s = Mathf.Clamp01(crouch_value);
					center_offset.y = crouch_value_s * -0.5f;
					controller.height = PLAYER_HEIGHT - crouch_value_s;
					controller.center = center_offset;
					camera_offset.y = CAMERA_OFFSET - crouch_value_s * 0.9f;
				}
			}
		}
#endif
        controller.Move(move_vector * frame_time);
#if MOUSE_SMOOTHING
        targetRotation = player_rotation * Quaternion.Euler(look_y, 0, camera_zoffset);
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, frame_time * (1.0f - mouseSmoothing) * 50.0f);
#else
		playerCamera.transform.rotation = player_rotation * Quaternion.Euler(look_y, 0, 0);
#endif
        playerCamera.transform.position = transform.position + camera_offset;
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    }

    Vector3 ProjectOnPlane(Vector3 vector, Vector3 normal)
    {
        return vector - normal * (vector.x * normal.x + vector.y * normal.y + vector.z * normal.z);
    }
}