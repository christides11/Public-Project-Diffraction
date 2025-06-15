namespace TightStuff
{
    using UnityEngine;
    
    public abstract class UpdateAbstract : MonoBehaviour
    {
        public int order;
        public int lateOrder;
    
        public virtual void GUpdate() { }
        public virtual void LateGUpdate() { }
    }
}
