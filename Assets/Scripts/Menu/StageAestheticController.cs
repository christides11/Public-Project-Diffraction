namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class StageAestheticController : MonoBehaviour
    {
        public List<Text> texts;
        public List<SpriteRenderer> images;
    
        public Image stageImage;
        public Text stageName;
    
        public void ChangeImageAndText()
        {
            foreach (var image in images)
                image.sprite = stageImage.sprite;
            foreach (var text in texts)
                text.text = stageName.text;
        }
    }
}
