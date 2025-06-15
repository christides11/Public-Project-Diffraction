using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TightStuff.Menu
{
    public class ProfileManager : MonoBehaviour
    {
        public PlayerTag currentProfile;
        public List<PlayerTag> profileList;

        public GameObject profileDisplay;
        public RectTransform profileListGroup;
        public GroupMenuBehaviour profileGroup;

        [SerializeField]
        private LerpToColor _tagBrush;

        public UnityEvent<PlayerTag> UpdateElements;
        public UnityEvent OnSelectingFavouriteFighterEvent;
        public UnityEvent OnDeselectingFavouriteFighterEvent;

        public UnityEvent OnPopulateList;

        public TMP_InputField profileName;
        public Text profileNameNormalText;
        public Slider fighterSlider;
        public Slider paletteSlider;
        public Image tagColor;

        public UnityEvent OnProfileEdit;
        public UnityEvent OnProfileDelete;

        public GridSlider gridSlider;

        public InputActionAsset defaultControls;
        public InputActionAsset twilightControls;

        public PlayerControlledCanvas playercanvas;

        public UnityEvent<PlayerTag> OnProfileChange;

        public TextAsset defaultProfile;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void UpdateBrushColor()
        {
            _tagBrush.SetTargetColor(Color.black * 0.8f);
        }

        public void UpdateElementsToFirstFavouriteFighter()
        {
            UpdateElements?.Invoke(currentProfile);
        }
        public void UpdateSliders()
        {
            if (currentProfile.fighterInUse != null)
            {
                fighterSlider.value = currentProfile.fighterInUse.fighterID;
                paletteSlider.value = currentProfile.fighterInUse.favouriteCostumeID + 1;
            }
            if (currentProfile.favouriteFighters != null && currentProfile.favouriteFighters.Count != 0)
            {
                fighterSlider.value = currentProfile.favouriteFighters[0].fighterID;
                paletteSlider.value = currentProfile.favouriteFighters[0].favouriteCostumeID + 1;
            }

        }

        public void UpdateFavoriteFighter()
        {
            if (currentProfile.favouriteFighters.Count <= 0)
            {
                currentProfile.favouriteFighters.Add(new PlayerTag.FavouriteFighter());
            }
            currentProfile.favouriteFighters[0].fighterID = (int)fighterSlider.value;
            currentProfile.favouriteFighters[0].favouriteCostumeID = (int)paletteSlider.value - 1;
            currentProfile.favouriteFighters[0].tagColor = tagColor.color;
            currentProfile.favouriteFighters[0].colorID = ColorAndPaletteAssigner.instance.GetColorIdBasedOnColor(tagColor.color);
        }

        public void OnSelectingFavouriteFighter(Single val)
        {
            if (currentProfile == null || currentProfile.favouriteFighters == null || currentProfile.favouriteFighters.Count == 0)
                return;
            if (val == currentProfile.favouriteFighters[0].fighterID)
            {
                paletteSlider.value = currentProfile.favouriteFighters[0].favouriteCostumeID + 1;
                OnSelectingFavouriteFighterEvent?.Invoke();
            }
            else
                OnDeselectingFavouriteFighterEvent?.Invoke();
        }
        public void OnSelectingFavouriteFighter()
        {
            if (currentProfile == null || currentProfile.favouriteFighters == null || currentProfile.favouriteFighters.Count == 0)
                return;
            if (fighterSlider.value == currentProfile.favouriteFighters[0].fighterID)
            {
                paletteSlider.value = currentProfile.favouriteFighters[0].favouriteCostumeID + 1;
                OnSelectingFavouriteFighterEvent?.Invoke();
            }
            else
                OnDeselectingFavouriteFighterEvent?.Invoke();
        }

        public PlayerTag SetProfile(PlayerTag tag)
        {
            currentProfile = tag;
            OnProfileChange?.Invoke(currentProfile);
            return currentProfile;
        }
        public void SetProfileName()
        {
            DeleteFile();
            currentProfile.tag = profileName.text;
            SaveProfile();
        }
        public void ResetProfileNameView()
        {
            profileName.text = currentProfile.tag;
        }
        public void ResetProfileNameViewNormalText()
        {
            profileNameNormalText.text = currentProfile.tag;
        }
        public void NewProfile()
        {
            currentProfile = new PlayerTag();
            currentProfile.favouriteFighters = new List<PlayerTag.FavouriteFighter>();

            currentProfile.tapOptions = new List<PlayerTag.TapToggles>();
            PlayerTag.TapToggles keytap = new PlayerTag.TapToggles();

            keytap.inputDevice = "Keyboard";
            keytap.tapDash = true;
            keytap.tapJump = true;
            keytap.tapSmash = true;
            keytap.tapAirdash = true;
            keytap.tapDrop = true;
            keytap.doubleTap = true;

            PlayerTag.TapToggles gamepadtap = new PlayerTag.TapToggles();

            gamepadtap.inputDevice = "Gamepad";
            gamepadtap.tapDash = true;
            gamepadtap.tapJump = true;
            gamepadtap.tapSmash = true;
            gamepadtap.tapAirdash = false;
            gamepadtap.tapDrop = true;
            gamepadtap.doubleTap = false;

            currentProfile.tapOptions.Add(keytap);
            currentProfile.tapOptions.Add(gamepadtap);

            currentProfile.autoLedgeGrab = true;

            currentProfile.bufferWindow = 10;
            currentProfile.stickSensitivity = 5;

            defaultControls.RemoveAllBindingOverrides();
            currentProfile.rebind = defaultControls;
            currentProfile.rebindOverrides = defaultControls.SaveBindingOverridesAsJson();

            if (profileList.Count <= 0)
                currentProfile.id = profileList.Count;
            else
            {
                var biggest = 0;
                for (int i = 0; i < profileList.Count; i++)
                {
                    if (profileList[i].id > profileList[biggest].id)
                        biggest = i;
                }
                currentProfile.id = profileList[biggest].id + 1;
            }
        }
        public void SaveProfile()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            var fileName = currentProfile.id.ToString();
            if (!string.IsNullOrEmpty(currentProfile.tag))
                fileName = rgx.Replace(currentProfile.tag, "_") + currentProfile.id.ToString();
            currentProfile.SaveToJson(fileName + ".json", "PlayerProfiles");
        }
        public void DeleteFile()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            if (currentProfile == null)
                return;
            var fileName = currentProfile.id.ToString();
            if (!string.IsNullOrEmpty(currentProfile.tag))
                fileName = rgx.Replace(currentProfile.tag, "_") + currentProfile.id.ToString();

            PlayerTag.DeleteFile("PlayerProfiles/" + fileName + ".json");
        }

        public void PopulateProfileList()
        {
            if (profileList.Count == profileListGroup.childCount && profileList.Count != 0)
            {
                return;
            }
            profileList.Clear();
            GetJsonFiles("PlayerProfiles");
            if (profileList.Count <= 0)
            {
                Debug.Log("No Profiles");
                return;
            }
            var i = 0;
            foreach(var profile in profileList)
            {
                var display = Instantiate(profileDisplay).GetComponent<ProfileDisplay>();
                display.profile = profile;
                display.profile.fighterInUse = null;
                display.transform.SetParent(profileListGroup);
                display.transform.localScale = Vector3.one;
                display.profileManager = this;
                display.profileOrder = i;
                i++;

                GroupMenuBehaviour groupMenuBehaviour = profileGroup.GetComponent<GroupMenuBehaviour>();
                display.GetComponent<Button>().onClick.AddListener(groupMenuBehaviour.PeekState);
                display.GetComponent<ButtonExtension>().OnSelectWithObjectEvent.AddListener(gridSlider.OnOutsideView);
                display.deleteButton.onClick.AddListener(OnProfileDelete.Invoke);
                display.editButton.onClick.AddListener(OnProfileEdit.Invoke);
                display.subMenu.OnCancel.AddListener(groupMenuBehaviour.ActivateState);
                display.deleteButton.onClick.AddListener(groupMenuBehaviour.ActivateState);
            }
            StartCoroutine(OnFinishPopulating());
        }

        public void PopulateProfileListPreFight()
        {
            if (profileList.Count == profileListGroup.childCount && profileList.Count != 0)
            {
                return;
            }
            profileList.Clear();
            profileList.Add(PlayerTag.FromJson(defaultProfile.text));
            GetJsonFiles("PlayerProfiles");
            if (profileList.Count <= 0)
            {
                Debug.Log("No Profiles");
                return;
            }
            var i = 0;
            ProfileDisplay tagToMove = null;

            foreach (var profile in profileList)
            {
                var display = Instantiate(profileDisplay).GetComponent<ProfileDisplay>();
                if (tagToMove == null)
                {
                    tagToMove = display;
                }

                if (profile.tag + profile.id == currentProfile.tag + currentProfile.id)
                    tagToMove = display;

                display.profile = profile;
                if (string.IsNullOrEmpty(display.profile.tag))
                    tagToMove.profile.tag = "P" + (playercanvas.GetComponent<PlayerSelectAestheticController>().quadrant.currentQuardrant + 1);
                display.profile.fighterInUse = null;
                display.transform.SetParent(profileListGroup);
                display.transform.localScale = Vector3.one;
                display.profileManager = this;
                display.profileOrder = i;
                i++;

                GroupMenuBehaviour groupMenuBehaviour = profileGroup.GetComponent<GroupMenuBehaviour>();
                display.GetComponent<Button>().onClick.AddListener(groupMenuBehaviour.DisableState);
                display.GetComponent<ButtonExtension>().OnCancelEvent.AddListener(groupMenuBehaviour.DisableState);
                display.GetComponent<ButtonExtension>().OnSelectWithObjectEvent.AddListener(gridSlider.OnOutsideView);

            }
            tagToMove.transform.SetAsFirstSibling();
            if (playercanvas != null && tagToMove != null)
            {
                Debug.Log("oh no");
                playercanvas.Select(tagToMove.gameObject);
            }
            StartCoroutine(OnFinishPopulating());
        }

        public IEnumerator OnFinishPopulating()
        {
            yield return new WaitForEndOfFrame();
            OnPopulateList?.Invoke();
        }

        public void ClearProfileList()
        {
            foreach (Transform child in profileListGroup)
            {
                Destroy(child.gameObject);
            }
            profileList.Clear();
        }

        public string[] GetJsonFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");


                foreach (string filePath in jsonFiles)
                {
                    string json = File.ReadAllText(filePath);
                    PlayerTag playerTag = PlayerTag.FromJson(json);

                    if (playerTag != null)
                    {
                        profileList.Add(playerTag);
                    }
                    else
                    {
                        Debug.LogWarning("Failed to load PlayerTag from: " + filePath);
                    }
                }
                return jsonFiles;
            }
            else
            {
                Debug.LogWarning("Directory not found: " + folderPath);
                return new string[0];  // Return an empty array if the folder doesn't exist
            }
        }
    }
}
