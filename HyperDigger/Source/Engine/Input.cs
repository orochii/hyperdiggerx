using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HyperDigger
{
    class Input
    {
        const float AXIS_THRESHOLD = 0.5f;
        public enum Button { UP, DOWN, LEFT, RIGHT, 
            OK, CANCEL, MENU, ESCAPE, 
            JUMP, ATTACK, SKILL, DASH,
            SYS_SCALE
        }
        public enum MappingType
        {
            KEYBOARD, BUTTON, AXIS, MOUSE
        }

        private struct ButtonMapping
        {
            public MappingType Type; // keyboard, gpad button, gpad axis, mouse
            public uint Id;
            public int AxisDir;
        }
        private struct ButtonState
        {
            public bool CurrState;
            public bool PreviousState;
            public float CurrValue;
            public float PreviousValue;
            public float RepeatTimer;
            //
            public List<ButtonMapping> Mappings;
        }

        private ButtonState[] _buttonStates;

        // TODO: Load remapping
        public Input() {
            _buttonStates = new ButtonState[Enum.GetValues<Button>().Length];
            for (int i = 0; i < _buttonStates.Length; i++) { 
                _buttonStates[i] = new ButtonState();
                _buttonStates[i].Mappings = new List<ButtonMapping>();
            }
            // Movement
            AddMapping(Button.LEFT, MappingType.KEYBOARD, (int)Keys.Left);
            AddMapping(Button.RIGHT, MappingType.KEYBOARD, (int)Keys.Right);
            AddMapping(Button.UP, MappingType.KEYBOARD, (int)Keys.Up);
            AddMapping(Button.DOWN, MappingType.KEYBOARD, (int)Keys.Down);
            // Menus
            AddMapping(Button.OK, MappingType.KEYBOARD, (int)Keys.Space);
            AddMapping(Button.OK, MappingType.KEYBOARD, (int)Keys.Enter);
            AddMapping(Button.CANCEL, MappingType.KEYBOARD, (int)Keys.Escape);
            AddMapping(Button.CANCEL, MappingType.KEYBOARD, (int)Keys.Back);
            AddMapping(Button.MENU, MappingType.KEYBOARD, (int)Keys.Tab);
            AddMapping(Button.ESCAPE, MappingType.KEYBOARD, (int)Keys.Escape);
            // Gameplay
            AddMapping(Button.JUMP, MappingType.KEYBOARD, (int)Keys.Space);
            AddMapping(Button.ATTACK, MappingType.KEYBOARD, (int)Keys.Z);
            AddMapping(Button.SKILL, MappingType.KEYBOARD, (int)Keys.X);
            AddMapping(Button.DASH, MappingType.KEYBOARD, (int)Keys.LeftShift);
            // System
            AddMapping(Button.SYS_SCALE, MappingType.KEYBOARD, (int)Keys.F5);
        }

        public bool AddMapping(Button button, MappingType type, uint id, int dir = 1)
        {
            // Check if the mapping is already there
            for (int i = 0; i < _buttonStates[(int)button].Mappings.Count; i++)
            {
                if (_buttonStates[(int)button].Mappings[i].Type == type 
                    && _buttonStates[(int)button].Mappings[i].Id == id
                    && _buttonStates[(int)button].Mappings[i].AxisDir == dir)
                    return false;
            }
            // Add mapping
            var newMapping = new ButtonMapping();
            newMapping.Type = type;
            newMapping.Id = id;
            newMapping.AxisDir = dir;
            _buttonStates[(int)button].Mappings.Add(newMapping);
            return true;
        }

        public bool RemoveMapping(Button button, MappingType type, uint id, int dir = 1)
        {
            var found = -1;
            for (int i = 0; i < _buttonStates[(int)button].Mappings.Count; i++)
            {
                if (_buttonStates[(int)button].Mappings[i].Type == type
                    && _buttonStates[(int)button].Mappings[i].Id == id
                    && _buttonStates[(int)button].Mappings[i].AxisDir == dir)
                {
                    found = i;
                    break;
                }
            }
            if (found != -1)
            {
                _buttonStates[(int)button].Mappings.RemoveAt(found);
                return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            //Console.WriteLine(gameTime.ToString());
            var keyboard = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One);

            for (int i = 0; i < _buttonStates.Length; i++)
            {
                _buttonStates[i].PreviousState = _buttonStates[i].CurrState;
                _buttonStates[i].PreviousValue = _buttonStates[i].CurrValue;
                bool _curr = false;
                float _value = 0.0f;
                foreach (var m in _buttonStates[i].Mappings)
                {
                    switch (m.Type)
                    {
                        case MappingType.KEYBOARD:
                            var k = keyboard.IsKeyDown((Keys)m.Id);
                            _curr |= k;
                            if (k) _value = 1.0f;
                            break;
                        case MappingType.BUTTON:
                            var b = gamepad.IsButtonDown((Buttons)m.Id);
                            _curr |= b;
                            if (b) _value = 1.0f;
                            break;
                        case MappingType.AXIS:
                            float a = 0;
                            switch(m.Id)
                            {
                                case 0: // Left Thumb X
                                    a = gamepad.ThumbSticks.Left.X;
                                    break;
                                case 1: // Left Thumb Y
                                    a = gamepad.ThumbSticks.Left.Y;
                                    break;
                                case 2: // Right Thumb X
                                    a = gamepad.ThumbSticks.Right.X;
                                    break;
                                case 3: // Right Thumb Y
                                    a = gamepad.ThumbSticks.Right.Y;
                                    break;
                                case 4: // Left Trigger
                                    a = gamepad.Triggers.Left;
                                    break;
                                case 5: // Right Trigger
                                    a = gamepad.Triggers.Right;
                                    break;
                            }
                            if (a * m.AxisDir > AXIS_THRESHOLD) _curr = true;
                            if (Math.Abs(a) > Math.Abs(_value)) _value = a * m.AxisDir;
                            break;
                        case MappingType.MOUSE:
                            // TODO
                            break;
                    }
                }
                _buttonStates[i].CurrState = _curr;
                _buttonStates[i].CurrValue = _value;
            }

            CheckSystemInput();
        }

        private void CheckSystemInput()
        {
            if (IsTriggered(Input.Button.SYS_SCALE))
            {
                Global.Graphics.Scale = (((Global.Graphics.Scale - 1) + 1) % 3) + 1;
            }
        }
        
        public float GetHorz()
        {
            return GetValue(Button.RIGHT) - GetValue(Button.LEFT);
        }
        public float GetVert()
        {
            return GetValue(Button.DOWN) - GetValue(Button.UP);
        }
        public int GetDir4()
        {
            if (IsPressed(Input.Button.LEFT)) return 4;
            if (IsPressed(Input.Button.RIGHT)) return 6;
            if (IsPressed(Input.Button.UP)) return 8;
            if (IsPressed(Input.Button.DOWN)) return 2;
            return 0;
        }
        public int GetDir8()
        {
            if (IsPressed(Input.Button.LEFT))
            {
                if (IsPressed(Input.Button.UP)) return 7;
                if (IsPressed(Input.Button.DOWN)) return 1;
                return 4;
            }
            if (IsPressed(Input.Button.RIGHT))
            {
                if (IsPressed(Input.Button.UP)) return 9;
                if (IsPressed(Input.Button.DOWN)) return 3;
                return 6;
            }
            if (IsPressed(Input.Button.UP)) return 8;
            if (IsPressed(Input.Button.DOWN)) return 2;
            return 0;
        }
        
        public float GetValue(Button b)
        {
            return _buttonStates[(int)b].CurrValue;
        }
        public bool IsTriggered(Button b)
        {
            return _buttonStates[(int)b].CurrState && !_buttonStates[(int)b].PreviousState;
        }
        public bool IsPressed(Button b) {
            return _buttonStates[(int)b].CurrState;
        }
        public bool IsReleased(Button b)
        {
            return _buttonStates[(int)b].PreviousState && !_buttonStates[(int)b].CurrState;
        }
    }
}
