using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		
		[Header("Ability Inputs")]
		public bool path;           
		public bool freezeTriggered;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		
		[Header("Menu Inputs")]
		public bool pause;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnPath(InputValue value)
		{
			path = value.isPressed;
		}

		public void OnFreeze(InputValue value)
		{
			if (value.isPressed)
			{
				freezeTriggered = true;
			}
		}
    
		public void ConsumeFreezeInput()
		{
			freezeTriggered = false;
		}
		
		public void OnPause(InputValue value)
		{
			if (value.isPressed)
			{
				pause = true;
			}
		}
       
		public void ConsumePauseInput()
		{
			pause = false;
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (PauseMenu.IsPaused) return;

			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			if (PauseMenu.IsPaused)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				return;
			}

			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}