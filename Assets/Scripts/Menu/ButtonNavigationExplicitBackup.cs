namespace TightStuff.Menu
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class ButtonNavigationExplicitBackup : UpdateAbstract
    {
        private Selectable _selectable;
    
        public Selectable ogUp;
        public Selectable ogDown;
        public Selectable ogLeft;
        public Selectable ogRight;
    
        [SerializeField]
        private bool _auto = false;
    
        [SerializeField]
        private List<Selectable> _backupUp;
        [SerializeField]
        private List<Selectable> _backupDown;
        [SerializeField]
        private List<Selectable> _backupLeft;
        [SerializeField]
        private List<Selectable> _backupRight;
    
        // Start is called before the first frame update
        void Start()
        {
            _selectable = GetComponent<Selectable>();
            ogUp = _selectable.navigation.selectOnUp;
            ogDown = _selectable.navigation.selectOnDown;
            ogLeft = _selectable.navigation.selectOnLeft;
            ogRight = _selectable.navigation.selectOnRight;
        }
    
        public void Update()
        {
            ImplementBackup(ogUp, _backupUp, 0);
            ImplementBackup(ogDown, _backupDown, 1);
            ImplementBackup(ogLeft, _backupLeft, 2);
            ImplementBackup(ogRight, _backupRight, 3);
        }
    
        private void ImplementBackup(Selectable og, List<Selectable> backup, int i)
        {
            if (og == null)
                return;
            if ((backup == null || backup.Count <= 0) && !_auto)
                return;
    
            if (og.gameObject.activeInHierarchy)
            {
                var temp = _selectable.navigation;
                if (i == 0)
                    temp.selectOnUp = og;
                if (i == 1)
                    temp.selectOnDown = og;
                if (i == 2)
                    temp.selectOnLeft = og;
                if (i == 3)
                    temp.selectOnRight = og;
                _selectable.navigation = temp;
                return;
            }
    
            if (_auto)
            {
                var temp = _selectable.navigation;
                var currentSelected = og;
                var j = 0;
                while (j < 5)
                {
    
                    if (i == 0 && currentSelected.navigation.selectOnUp == null)
                        break;
                    if (i == 1 && currentSelected.navigation.selectOnDown == null)
                        break;
                    if (i == 2 && currentSelected.navigation.selectOnLeft == null)
                        break;
                    if (i == 3 && currentSelected.navigation.selectOnRight == null)
                        break;
    
                    if (i == 0)
                    {
                        currentSelected = currentSelected.navigation.selectOnUp;
                        if (currentSelected.gameObject.activeInHierarchy)
                            break;
                    }
                    if (i == 1)
                    {
                        currentSelected = currentSelected.navigation.selectOnDown;
                        if (currentSelected.gameObject.activeInHierarchy)
                            break;
                    }
                    if (i == 2)
                    {
                        currentSelected = currentSelected.navigation.selectOnLeft;
                        if (currentSelected.gameObject.activeInHierarchy)
                            break;
                    }
                    if (i == 3)
                    {
                        currentSelected = currentSelected.navigation.selectOnRight;
                        if (currentSelected.gameObject.activeInHierarchy)
                            break;
                    }
                    j++;
                }
                if (i == 0)
                    temp.selectOnUp = currentSelected;
                if (i == 1)
                    temp.selectOnDown = currentSelected;
                if (i == 2)
                    temp.selectOnLeft = currentSelected;
                if (i == 3)
                    temp.selectOnRight = currentSelected;
                _selectable.navigation = temp;
                return;
            }
    
            foreach (Selectable o in backup)
            {
                if (o.gameObject.activeInHierarchy)
                {
                    var temp = _selectable.navigation;
                    if (i == 0)
                        temp.selectOnUp = o;
                    if (i == 1)
                        temp.selectOnDown = o;
                    if (i == 2)
                        temp.selectOnLeft = o;
                    if (i == 3)
                        temp.selectOnRight = o;
                    _selectable.navigation = temp;
                    break;
                }
            }
        }
    }
}
