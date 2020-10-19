using UnityEngine;

namespace SA.Foundation.Editor
{
    public class SA_InputEvent
    {
        readonly Event m_sourceEvent;

        public SA_InputEvent(Event sourceEvent)
        {
            m_sourceEvent = sourceEvent;
        }

        public int displayIndex => m_sourceEvent.displayIndex;

        public Vector2 mousePosition => m_sourceEvent.mousePosition;

        public Vector2 delta => m_sourceEvent.delta;

        public bool shift => m_sourceEvent.shift;

        public bool control => m_sourceEvent.control;

        public bool command => m_sourceEvent.command;

        public KeyCode keyCode => m_sourceEvent.keyCode;

        public bool capsLock => m_sourceEvent.capsLock;

        public bool numeric => m_sourceEvent.numeric;

        public bool functionKey => m_sourceEvent.functionKey;

        public bool isKey => m_sourceEvent.isKey;

        public bool alt => m_sourceEvent.alt;

        public string commandName => m_sourceEvent.commandName;

        public int clickCount => m_sourceEvent.clickCount;

        public bool isMouse => m_sourceEvent.isMouse;

        public float pressure => m_sourceEvent.pressure;

        public EventModifiers modifiers => m_sourceEvent.modifiers;

        public int button => m_sourceEvent.button;

        public EventType type => m_sourceEvent.type;

        public EventType rawType => m_sourceEvent.rawType;

        public char character => m_sourceEvent.character;

        public bool isScrollWheel
        {
            get
            {
#if !UNITY_5
                return m_sourceEvent.isScrollWheel;
#else
                return true;
#endif
            }
        }

        public Event SourceEvent => m_sourceEvent;

        public EventType GetTypeForControl(int controlID)
        {
            return m_sourceEvent.GetTypeForControl(controlID);
        }

        public override string ToString()
        {
            return m_sourceEvent.ToString();
        }

        //public override bool Equals(object obj);
        //public override int GetHashCode();
    }
}
