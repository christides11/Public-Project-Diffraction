using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class RebindManager : MonoBehaviour
    {
        [SerializeField]
        private List<RebindUIToName> _gamepadRebinds;
        [SerializeField]
        private List<RebindUIToName> _keyboardRebinds;

        public InputActionAsset actionAsset;
        public string currentRebindOverrides;

        public Toggle tapDash;
        public Toggle tapJump;
        public Toggle tapSmash;
        public Toggle tapAirdash;
        public Toggle autoLedgeGrab;
        public Toggle tapDrop;
        public Toggle doubleTap;

        public Toggle keytapDash;
        public Toggle keytapJump;
        public Toggle keytapSmash;
        public Toggle keytapAirdash;
        public Toggle keytapDrop;
        public Toggle keydoubleTap;

        public Slider bufferWindow;
        public Slider stickSensitivity;

        public ProfileManager profileManager;

        public TextAsset heavenBinds;
        public TextAsset twilightBinds;

        public int currentInputDeviceOffset;

        public string currentInputDevice;

        // Start is called before the first frame update
        void Awake()
        {
            profileManager = FindObjectOfType<ProfileManager>();        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetActionAsset(InputActionAsset asset)
        {
            actionAsset = asset;
            foreach (var bind in _gamepadRebinds)
            {
                SetBind(bind);
            }
        }
        public void SetPlayerTag(PlayerTag profile)
        {
            SetActionAsset(profile.rebind);
            currentRebindOverrides = profile.rebindOverrides;
            actionAsset.LoadBindingOverridesFromJson(currentRebindOverrides);
            UpdateMiscToCurrentProfile();
        }

        public void UpdateMiscToCurrentProfile()
        {
            UpdateToggles();
            autoLedgeGrab.isOn = profileManager.currentProfile.autoLedgeGrab;

            bufferWindow.value = profileManager.currentProfile.bufferWindow;
            stickSensitivity.value = profileManager.currentProfile.stickSensitivity - 3;
        }

        private void UpdateToggles()
        {
            if (profileManager.currentProfile.tapOptions.Count == 0)
            {
                Debug.Log("no tap options");
                return;
            }

            tapDash.isOn = profileManager.currentProfile.tapOptions[0].tapDash;
            tapJump.isOn = profileManager.currentProfile.tapOptions[0].tapJump;
            tapSmash.isOn = profileManager.currentProfile.tapOptions[0].tapSmash;
            tapAirdash.isOn = profileManager.currentProfile.tapOptions[0].tapAirdash;
            tapDrop.isOn = profileManager.currentProfile.tapOptions[0].tapDrop;
            doubleTap.isOn = profileManager.currentProfile.tapOptions[0].doubleTap;

            keytapDash.isOn = profileManager.currentProfile.tapOptions[1].tapDash;
            keytapJump.isOn = profileManager.currentProfile.tapOptions[1].tapJump;
            keytapSmash.isOn = profileManager.currentProfile.tapOptions[1].tapSmash;
            keytapAirdash.isOn = profileManager.currentProfile.tapOptions[1].tapAirdash;
            keytapDrop.isOn = profileManager.currentProfile.tapOptions[1].tapDrop;
            keydoubleTap.isOn = profileManager.currentProfile.tapOptions[1].doubleTap;
        }

        private void SetBind(RebindUIToName rebind)
        {
            var inputAction = actionAsset.FindActionMap("Gameplay").FindAction(rebind.name, true);
            if (inputAction != null)
            {
                // Create a new InputActionReference and assign it
                InputActionReference inputActionReference = ScriptableObject.CreateInstance<InputActionReference>();
                inputActionReference.Set(inputAction);

                // Assign it to the RebindActionUI
                rebind.RebindActionUI.actionReference = inputActionReference;
                var id = rebind.bindingID + currentInputDeviceOffset * rebind.offsetLength;
                rebind.RebindActionUI.bindingId = inputAction.bindings[id].id.ToString();

                Debug.Log("Input action assigned successfully!");
            }
        }

        public void SaveMiscBinds()
        {
            var tap = 0;
            for (int i = 0; i < profileManager.currentProfile.tapOptions.Count; i++)
            {
                PlayerTag.TapToggles x = profileManager.currentProfile.tapOptions[i];
                if (x.inputDevice == currentInputDevice)
                    tap = i;
            }

            profileManager.currentProfile.rebindOverrides = profileManager.currentProfile.rebind.SaveBindingOverridesAsJson();
            profileManager.currentProfile.tapOptions[0].tapDash = tapDash.isOn;
            profileManager.currentProfile.tapOptions[0].tapJump = tapJump.isOn;
            profileManager.currentProfile.tapOptions[0].tapSmash = tapSmash.isOn;
            profileManager.currentProfile.tapOptions[0].tapAirdash = tapAirdash.isOn;
            profileManager.currentProfile.tapOptions[0].tapDrop = tapDrop.isOn;
            profileManager.currentProfile.tapOptions[0].doubleTap = doubleTap.isOn;

            profileManager.currentProfile.tapOptions[1].tapDash = keytapDash.isOn;
            profileManager.currentProfile.tapOptions[1].tapJump = keytapJump.isOn;
            profileManager.currentProfile.tapOptions[1].tapSmash = keytapSmash.isOn;
            profileManager.currentProfile.tapOptions[1].tapAirdash = keytapAirdash.isOn;
            profileManager.currentProfile.tapOptions[1].tapDrop = keytapDrop.isOn;
            profileManager.currentProfile.tapOptions[1].doubleTap = keydoubleTap.isOn;
            profileManager.currentProfile.autoLedgeGrab = autoLedgeGrab.isOn;

            profileManager.currentProfile.bufferWindow = (int)bufferWindow.value;
            profileManager.currentProfile.stickSensitivity  = (int)stickSensitivity.value + 3;
        }

        public void HeavensPreset()
        {
            keytapDash.isOn = true;
            keytapJump.isOn = true;
            keytapSmash.isOn = true;
            keytapAirdash.isOn = true;
            keytapDrop.isOn = true;
            keydoubleTap.isOn = true;

            tapDash.isOn = true;
            tapJump.isOn = true;
            tapSmash.isOn = true;
            tapAirdash.isOn = false;
            autoLedgeGrab.isOn = true;
            tapDrop.isOn = true;
            doubleTap.isOn = false;
            bufferWindow.value = 10;
            stickSensitivity.value = 2;

            currentRebindOverrides = heavenBinds.text;
            actionAsset.LoadBindingOverridesFromJson(currentRebindOverrides);
        }
        public void TwilightsPreset()
        {
            tapDash.isOn = true;
            tapJump.isOn = true;
            tapSmash.isOn = true;
            tapAirdash.isOn = true;
            autoLedgeGrab.isOn = true;
            tapDrop.isOn = true;
            doubleTap.isOn = true;

            keytapDash.isOn = true;
            keytapJump.isOn = true;
            keytapSmash.isOn = true;
            keytapAirdash.isOn = true;
            keytapDrop.isOn = true;
            keydoubleTap.isOn = true;

            bufferWindow.value = 10;
            stickSensitivity.value = 2;

            currentRebindOverrides = twilightBinds.text;
            actionAsset.LoadBindingOverridesFromJson(currentRebindOverrides);
        }
        public void NativesPreset()
        {
            tapDash.isOn = false;
            tapJump.isOn = false;
            tapSmash.isOn = false;
            tapAirdash.isOn = false;
            autoLedgeGrab.isOn = false;
            tapDrop.isOn = false;
            doubleTap.isOn = false;

            keytapDash.isOn = false;
            keytapJump.isOn = false;
            keytapSmash.isOn = false;
            keytapAirdash.isOn = false;
            keytapDrop.isOn = false;
            keydoubleTap.isOn = false;

            bufferWindow.value = 10;
            stickSensitivity.value = 2;

            currentRebindOverrides = "";
            actionAsset.LoadBindingOverridesFromJson(currentRebindOverrides);
        }

        public void OnChangeInputDevice(PlayerInput input)
        {
            currentInputDevice = input.currentControlScheme;
            if (input.currentControlScheme == "Keyboard")
            {
                currentInputDeviceOffset = 1;

                tapDash.gameObject.SetActive(false);
                tapJump.gameObject.SetActive(false);
                tapSmash.gameObject.SetActive(false);
                tapAirdash.gameObject.SetActive(false);
                tapDrop.gameObject.SetActive(false);
                doubleTap.gameObject.SetActive(false);

                keytapDash.gameObject.SetActive(true);
                keytapJump.gameObject.SetActive(true);
                keytapSmash.gameObject.SetActive(true);
                keytapAirdash.gameObject.SetActive(true);
                keytapDrop.gameObject.SetActive(true);
                keydoubleTap.gameObject.SetActive(true);
            }
            else
            {
                currentInputDeviceOffset = 0;

                tapDash.gameObject.SetActive(true);
                tapJump.gameObject.SetActive(true);
                tapSmash.gameObject.SetActive(true);
                tapAirdash.gameObject.SetActive(true);
                tapDrop.gameObject.SetActive(true);
                doubleTap.gameObject.SetActive(true);

                keytapDash.gameObject.SetActive(false);
                keytapJump.gameObject.SetActive(false);
                keytapSmash.gameObject.SetActive(false);
                keytapAirdash.gameObject.SetActive(false);
                keytapDrop.gameObject.SetActive(false);
                keydoubleTap.gameObject.SetActive(false);
            }

            autoLedgeGrab.isOn = profileManager.currentProfile.autoLedgeGrab;

            foreach (var bind in _gamepadRebinds)
            {
                SetBind(bind);
            }
        }

        [System.Serializable]
        private class RebindUIToName
        {
            public string name;
            public RebindActionUI RebindActionUI;
            public int bindingID;
            public int offsetLength;
        }

        [System.Serializable]
        private class ToggleProperties
        {

        }
    }
}
