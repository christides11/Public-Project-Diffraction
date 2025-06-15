namespace TightStuff.Rollback
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Unity.Collections;
    using SharedGame;
    
    [Serializable]
    public class DIFGame : IGame
    {
        public int Framenumber { get; private set; }
    
        public int Checksum => GetHashCode();
    
        public MatchManager match;
        public int playerID;
    
        public List<Frame> savedFrames;
    
        public DIFGame(MatchManager m, int i)
        {
            match = m;
            Framenumber = 0;
            playerID = i;
    
            savedFrames = new List<Frame>();
        }
    
        public void Update(long[] inputs, int disconnectFlags)
        {
            match.UpdateFrame(inputs);
            Framenumber = MatchManager.FrameNum;
        }
    
        public void FromBytes(NativeArray<byte> data)
        {
            using (var memoryStream = new MemoryStream(data.ToArray()))
            {
                var reader = new BinaryFormatter();
                memoryStream.Seek(0, SeekOrigin.Begin);
                Framenumber = (int)reader.Deserialize(memoryStream);
            }
    
            var frame = savedFrames[0];
            foreach (Frame f in savedFrames)
                if (f.frameNum == Framenumber)
                {
                    frame = f;
                    break;
                }
    
            match.Rollback(frame);
    
            //if (match.replayer != null)
                //match.replayer.RollbackAndDiscardAll(Framenumber - 1);
    
            savedFrames.Clear();
        }
    
    
        public NativeArray<byte> ToBytes()
        {
            savedFrames.Add(match.GetFrame());
            using (var memoryStream = new MemoryStream())
            {
                var writer = new BinaryFormatter();
                writer.Serialize(memoryStream, Framenumber);
                return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
            }
        }
    
        public long ReadInputs(int i)
        {
            int id = i + playerID;
            long input = 0;
            if (match.controllers.Count <= id)
                return 0;
            if (match.controllers[id].jumpButtonRaw)
                input |= DIFConstants.JUMP;
            if (match.controllers[id].shieldButtonRaw)
                input |= DIFConstants.SHIELD;
            if (match.controllers[id].attackButtonRaw)
                input |= DIFConstants.ATTACK;
            if (match.controllers[id].specialButtonRaw)
                input |= DIFConstants.SPECIAL;
            if (match.controllers[id].assistRaw)
                input |= DIFConstants.ASSIST;
            if (match.controllers[id].grabButtonRaw)
                input |= DIFConstants.GRAB;
            if (match.controllers[id].neutralLockRaw)
                input |= DIFConstants.NEUTRAL;
            if (match.controllers[id].dashToggleButtonRaw)
                input |= DIFConstants.DASHTOGGLE;
            if (match.controllers[id].airdashToggleButtonRaw)
                input |= DIFConstants.AIRDASHTOGGLE;
            if (match.controllers[id].jumpToggleButtonRaw)
                input |= DIFConstants.JUMPTOGGLE;
            if (match.controllers[id].smashToggleButtonRaw)
                input |= DIFConstants.SMASHTOGGLE;
            if (match.controllers[id].specialToggleRaw)
                input |= DIFConstants.SPECTOGGLE;
            if (Math.Sign(match.controllers[id].moveIntXRaw) == -1)
                input |= DIFConstants.STICK_MOVE_SIGN_X;
            if (Math.Sign(match.controllers[id].moveIntYRaw) == -1)
                input |= DIFConstants.STICK_MOVE_SIGN_Y;
            if (Math.Sign(match.controllers[id].cIntXRaw) == -1)
                input |= DIFConstants.STICK_C_SIGN_X;
            if (Math.Sign(match.controllers[id].cIntYRaw) == -1)
                input |= DIFConstants.STICK_C_SIGN_Y;
            if (match.controllers[id].upButtonRaw)
                input |= DIFConstants.UP;
            if (match.controllers[id].downButtonRaw)
                input |= DIFConstants.DOWN;
            if (match.controllers[id].leftButtonRaw)
                input |= DIFConstants.LEFT;
            if (match.controllers[id].rightButtonRaw)
                input |= DIFConstants.RIGHT;
            if (match.controllers[id].tauntButtonRaw)
                input |= DIFConstants.TAUNT;
    
            input |= ((long)(Math.Abs(match.controllers[id].moveIntXRaw)) << 21);
            input |= ((long)(Math.Abs(match.controllers[id].moveIntYRaw)) << 31);
            input |= ((long)(Math.Abs(match.controllers[id].cIntXRaw)) << 41);
            input |= ((long)(Math.Abs(match.controllers[id].cIntYRaw)) << 51);
    
            return input;
        }
    
    
        public void LogInfo(string filename)
        {
    
        }
    
        public void FreeBytes(NativeArray<byte> data)
        {
            if (data.IsCreated)
            {
                data.Dispose();
            }
        }
    
        public override int GetHashCode()
        {
            int hashCode = -1214587014;
            hashCode = hashCode * -1521134295 + Framenumber.GetHashCode();
            return hashCode;
        }
    }
}
