using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PacMan
{
    
    public enum EventType {None, PlayerDied, ButtonPressed, MapComplete, MapNameChanged, ScoreChanged, LivesChanged, CherryEaten, StateEnd, Blink, NameEntered,NewRecord};

    public delegate void HandleMouseEvent(SceneType type, MouseEventArgs em);
    public delegate void HandleKeyboardEvent(SceneType type, KeyEventArgs ek);
    public delegate void HandleEvent(SceneType type, int arg, string string_args);

    
    struct EventArgs
    {
        public SceneType type;
        public EventType e;
        public int int_arg;
        public string string_args;

        public EventArgs(SceneType type, EventType e, int int_arg, string string_args)
        {
            this.type = type;
            this.e = e;
            this.int_arg = int_arg;
            this.string_args = string_args;
        }
    }

    
    class Game
    {
        public static event HandleEvent PlayerDied;
        public static event HandleEvent ButtonPressed;
        public static event HandleEvent MapComplete;
        public static event HandleEvent MapNameChanged;
        public static event HandleEvent ScoreChanged;
        public static event HandleEvent LivesChanged;
        public static event HandleEvent CherryEaten;
        public static event HandleEvent StateEnd;
        public static event HandleEvent Blink;
        public static event HandleEvent NameEntered;
        public static event HandleEvent NewRecord;

        private List<EventArgs> events = new List<EventArgs>();

        SceneManager sceneManager;

        private static Game instance;
        public static Game GetInstance()
        {
            if (instance == null)
                instance = new Game();

            return instance;
        }
        private Game()
        {

          sceneManager = SceneManager.GetInstance();
          sceneManager.ChangeSceneTo(SceneType.Menu);

        }
        public void Update(long dT)
        {
            sceneManager.Update(dT);

            if (Form.ActiveForm != null)
                Form.ActiveForm.Invalidate();

            ResolveEvents();
        }

        public void Draw(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);
            sceneManager.Draw(e);
        }


        public void AddEvent(SceneType type,EventType e,int int_arg = 0,string string_args = "")
        {
            events.Add(new EventArgs(type,e,int_arg,string_args));
        }

        private void ResolveEvents()
        {
            for (int i = 0; i < events.Count; ++i)
            {
                var e = events[i];

                switch (e.e)
                {
                    case EventType.PlayerDied:
                        PlayerDied(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.LivesChanged:
                        LivesChanged(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.ButtonPressed:
                        ButtonPressed(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.MapComplete:
                        MapComplete(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.MapNameChanged:
                        MapNameChanged(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.NameEntered:
                        NameEntered(e.type, e.int_arg, e.string_args);
                        break;

                    case EventType.ScoreChanged:
                        ScoreChanged(e.type, e.int_arg, e.string_args);
                        break;
                    case EventType.CherryEaten:
                        CherryEaten(e.type, e.int_arg, e.string_args);
                        break;
                    case EventType.StateEnd:
                        StateEnd(e.type, e.int_arg, e.string_args);
                        break;
                    case EventType.Blink:
                        Blink(e.type, e.int_arg, e.string_args);
                        break;
                    case EventType.NewRecord:
                        NewRecord(e.type, e.int_arg, e.string_args);
                        break;
                }
            }

            events.Clear();
        }
    }
}
