namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public abstract class Controllable : UpdateAbstract
    {
        public Entity entity;
        public Controller controlling;
    }
}
