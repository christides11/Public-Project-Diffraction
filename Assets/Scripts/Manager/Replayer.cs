namespace TightStuff
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System;
    using UnityEngine.InputSystem;
    
    public class Replayer : UpdateAbstract
    {
        private MatchManager _match;
        private Runner _runner;
        private StageBuilder _builder;
    
        private List<Frame> _savedFrames;
        private List<long[]> _savedInputs;
    
        public static bool replaying;
    
        [SerializeField]
        private int maxRecordTimeInSeconds = 600;
    
        private void Start()
        {
            _match = GetComponent<MatchManager>();
            _runner = GetComponent<Runner>();
            _builder = FindObjectOfType<StageBuilder>();
    
            _savedFrames = new List<Frame>();
            if (_savedInputs == null)
                _savedInputs = new List<long[]>();
        }
    
        public void RecordFrame(long[] input)
        {
            if (MatchManager.FrameNum > maxRecordTimeInSeconds * 60)
                return;
    
            _savedFrames.Add(_match.GetFrame());
    
            if (!replaying)
            {
                if (MatchManager.FrameNum - 1 >= _savedInputs.Count)
                    _savedInputs.Add(input);
                else
                    _savedInputs[MatchManager.FrameNum - 1] = input;
            }
    
            if (_savedInputs.Count <= MatchManager.FrameNum)
                replaying = false;
        }
    
        public void ReplayInputOnly()
        {
            for (int i = 0; i < _savedInputs[MatchManager.FrameNum - 1].Length; i++)
                _match.parseInputs(_savedInputs[MatchManager.FrameNum - 1][i], i);
        }
    
        public void RollbackAndReplayToCurrentFrame(int frameNum)
        {
            RollbackAndDiscardSavedFrames(frameNum);
    
            for (int i = frameNum; i < _savedInputs.Count; i++)
            {
                MatchManager.FrameNum++;
                for (int j = 0; j < _savedInputs[i].Length; j++)
                    _match.parseInputs(_savedInputs[i][j], j);
                _runner.AdvanceFrame();
                _savedFrames.Add(_match.GetFrame());
            }
        }
    
        public void RollbackAndDiscardSavedFrames(int frameNum)
        {
            int rollbackFrames = _savedFrames.Count - frameNum;
            _match.Rollback(_savedFrames[frameNum - 1]);
            _savedFrames.RemoveRange(frameNum, rollbackFrames);
        }
    
        public void RollbackAndDiscardAll(int frameNum)
        {
            int rollbackFrames = _savedFrames.Count - frameNum;
            _savedFrames.RemoveRange(frameNum, rollbackFrames);
            _savedInputs.RemoveRange(frameNum, rollbackFrames);
        }
        public void SaveReplay()
        {
            if (_builder == null)
                return;
            if (_builder.stageInfo == null)
                return;
            if (_savedInputs == null)
                return;
    
            string file = "save";
            string format = ".replay";
            string fileName = file + format;
            int i = 1;
            while (File.Exists(fileName))
            {
                file = "save" + i;
                fileName = file + format;
                i++;
            }
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, _savedInputs);
            Debug.Log("saved replay with " + _savedInputs.Count + " number of frames to " + fileName);
            //TemporaryReplayDisplay.instance.ChangeText("Last replay: " + fileName);
            fs.Close();
    
            string scenefileName = file + ".scenario";
            FileStream bfs = new FileStream(scenefileName, FileMode.Create);
            BinaryFormatter bbf = new BinaryFormatter();
            bbf.Serialize(bfs, _builder.stageInfo);
            bfs.Close();
        }
        public void LoadReplay()
        {
            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.replay");
            if (filePaths.Length <= 0)
                return;
            try
            {
                using (Stream stream = File.Open(filePaths[0], FileMode.Open))
                {
                    var bformatter = new BinaryFormatter();
    
                    _savedInputs = (List<long[]>)bformatter.Deserialize(stream);
                    replaying = true;
                    Debug.Log(_savedInputs.Count);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Ah");
            }
        }
        [System.Serializable]
        public class SerializableColor
        {
            public float _r;
            public float _g;
            public float _b;
            public float _a;
    
            public Color Color
            {
                get
                {
                    return new Color(_r, _g, _b, _a);
                }
                set
                {
                    _r = value.r;
                    _g = value.g;
                    _b = value.b;
                    _a = value.a;
                }
            }
    
            public SerializableColor()
            {
                // (Optional) Default to white with an empty initialisation
                _r = 1f;
                _g = 1f;
                _b = 1f;
                _a = 1f;
            }
    
            public SerializableColor(float r, float g, float b, float a = 0f)
            {
                _r = r;
                _g = g;
                _b = b;
                _a = a;
            }
    
            public SerializableColor(Color color)
            {
                _r = color.r;
                _g = color.g;
                _b = color.b;
                _a = color.a;
            }
        }
    }
}
