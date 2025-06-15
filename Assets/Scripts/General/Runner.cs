namespace TightStuff
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System.Linq;
    using System;
    using UnityEngine.Profiling;
    
    public class Runner : MonoBehaviour
    {
        public List<UpdateAbstract> updates;
        public List<UpdateAbstract> lateUpdates;
    
        public static event Action OnFrameStart;
        public static event Action OnFrameEnd;
    
        public static int order;
    
        public void Start()
        {
            order = 0;
            updates.Clear();
            lateUpdates.Clear();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    
            foreach (var rootGameObject in rootGameObjects)
            {
                UpdateAbstract[] us = rootGameObject.GetComponentsInChildren<UpdateAbstract>();
                foreach(UpdateAbstract u in us)
                {
                    u.order = u.order * 100 + order;
                    u.lateOrder = u.lateOrder * 100 + order;
                    order++;
                    updates.Add(u);
                    lateUpdates.Add(u);
                }
            }
    
            updates.Sort((a, b) => a.order.CompareTo(b.order));
            lateUpdates.Sort((a, b) => a.lateOrder.CompareTo(b.lateOrder));
        }
    
        private void FixedUpdate()
        {
            AdvanceFrame();
        }
    
        public void AdvanceFrame()
        {
            //UnityEngine.InputSystem.InputSystem.Update();
            if (OnFrameStart != null)
                OnFrameStart();
            Physics2D.SyncTransforms();
            Physics2D.Simulate(Time.fixedDeltaTime);
            Physics2D.Simulate(0);
            foreach (UpdateAbstract u in updates)
            {
                Profiler.BeginSample(u.GetType().Name);
                u.GUpdate();
                Profiler.EndSample();
            }
            foreach (UpdateAbstract u in lateUpdates)
            {
                Profiler.BeginSample(u.GetType().Name);
                u.LateGUpdate();
                Profiler.EndSample();
            }
            if (OnFrameEnd != null)
                OnFrameEnd();
        }
    }
}
