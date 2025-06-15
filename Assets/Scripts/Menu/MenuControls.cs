namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;
    
    public class MenuControls : MonoBehaviour
    {
        public UnityEvent OnLeft;
        public UnityEvent OnRight;
        public UnityEvent OnUp;
        public UnityEvent OnDown;
    
        public UnityEvent OnCancelButton;
        public UnityEvent OnCancelButtonRelease;
        public UnityEvent OnConfirmButton;
        public UnityEvent OnStartButton;
        public UnityEvent OnLButton;
        public UnityEvent OnRButton;
        public UnityEvent OnBigLButton;
        public UnityEvent OnBigRButton;

        public UnityEvent OnXButton;
        public UnityEvent OnYButton;

        public float cancelButtonHoldTimer;


        private bool _movePerformed;
    
        private void OnDisable()
        {
            OnLeft.RemoveAllListeners();
            OnRight.RemoveAllListeners();
            OnUp.RemoveAllListeners();
            OnDown.RemoveAllListeners();
            OnCancelButton.RemoveAllListeners();
            OnCancelButtonRelease.RemoveAllListeners();
            OnConfirmButton.RemoveAllListeners();


            OnStartButton.RemoveAllListeners();
            OnLButton.RemoveAllListeners();
            OnRButton.RemoveAllListeners();
            OnBigLButton.RemoveAllListeners();
            OnBigRButton.RemoveAllListeners();
            OnXButton.RemoveAllListeners();
            OnYButton.RemoveAllListeners();
        }
    
        public void OnCancel(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnCancelButton?.Invoke();
            if (ctx.performed && !ctx.ReadValueAsButton())
                OnCancelButtonRelease?.Invoke();
        }
        public void OnConfirm(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnConfirmButton?.Invoke();
        }

        public void OnStart(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnStartButton?.Invoke();
        }
        public void OnL(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnLButton?.Invoke();
        }
        public void OnR(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnRButton?.Invoke();
        }
        public void OnBigL(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnBigLButton?.Invoke();
        }
        public void OnBigR(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnBigRButton?.Invoke();
        }
        public void OnX(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnXButton?.Invoke();
        }
        public void OnY(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && ctx.ReadValueAsButton())
                OnYButton?.Invoke();
        }
        public void OnMove(InputAction.CallbackContext ctx)
        {
            var val = ctx.ReadValue<Vector2>();
            if (val.magnitude < 0.75f)
            {
                _movePerformed = false;
                return;
            }
    
            if (_movePerformed)
                return;
    
            if (val.normalized.y > 0.5f)
                OnUp?.Invoke();
            if (val.normalized.y < -0.5f)
                OnDown?.Invoke();
            if (val.normalized.x > 0.5f)
                OnRight?.Invoke();
            if (val.normalized.x < -0.5f)
                OnLeft?.Invoke();
        }
    }
}
