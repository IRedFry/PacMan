using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PacMan
{
    using Scenes = List<KeyValuePair<SceneType,Scene>>;
    using SceneFactory = Dictionary<SceneType, Func<Scene>>;
    
    public enum SceneType { None = -1, Any = 0, Menu = 1, Main_Game, Pause, GameOver, Highscores, About};

    
    class SceneManager
    {
        private Scenes scenes = new Scenes();
        private SceneFactory factory = new SceneFactory();
        private bool isSer = false;
        private static SceneManager instance;

        private SceneManager()
        {
            RegisterScene<Scene_Game>(SceneType.Main_Game);
            RegisterScene<Scene_Menu>(SceneType.Menu);
            RegisterScene<Scene_GameOver>(SceneType.GameOver);
            RegisterScene<Scene_Pause>(SceneType.Pause);
            RegisterScene<Scene_HighScores>(SceneType.Highscores);
            RegisterScene<Scene_About>(SceneType.About);
        }

        public static SceneManager GetInstance()
        {
            if (instance == null)
                instance = new SceneManager();

            return instance;
        }

        public void SerOn()
        {
            isSer = true;
        }

        public void Serialization()
        {
            Stream SaveFileStream = File.Create("save.bin");
            BinaryFormatter serializer = new BinaryFormatter();

            Scene game = null;

            foreach (var s in scenes)
            {
                if (s.Key == SceneType.Main_Game)
                {
                    game = s.Value;
                    break;
                }
            }

            serializer.Serialize(SaveFileStream, game);
            SaveFileStream.Close();
        }

        private void RegisterScene<T> (SceneType type) where T: new()
        {
            factory[type] = () =>
            {
                return (Scene)Activator.CreateInstance(typeof(T));
            };
        }

        private bool CreateScene(SceneType type)
        {
            if (type == SceneType.Main_Game && isSer)
            {
                Console.WriteLine("Reading saved file");
                Stream openFileStream = File.OpenRead("save.bin");
                BinaryFormatter deserializer = new BinaryFormatter();
                Scene scene = (Scene_Game)deserializer.Deserialize(openFileStream);
                openFileStream.Close();

                scenes.Add(new KeyValuePair<SceneType, Scene>(SceneType.Main_Game, scene));
                ((Scene_Game)scene).OnSerialization();
                scene = null;

                isSer = false;
                return true;
            }

            if (factory.TryGetValue(type, out var scene_create))
            {
                Scene scene = scene_create();
                scenes.Add(new KeyValuePair<SceneType, Scene>(type,scene));
                scene = null;
                return true;
            }
            else
                return false;
        }

        public void Update(long dT)
        {

            if (scenes.Count != 0)
            {
                var sc = scenes.Last();
                if (sc.Value.GetTransUpdate() && scenes.Count > 1)
                {
                    int s = scenes.Count - 2;
                    for (; s != 0; --s)
                    {
                        if (!scenes[s].Value.GetTransUpdate())
                            break;
                    }

                    for (; s < scenes.Count; ++s)
                        scenes[s].Value.Update(dT);
                }
                else
                    sc.Value.Update(dT);
            }
        }

        public void Draw(System.Windows.Forms.PaintEventArgs e)
        { 
            if (scenes.Count != 0)
            {
                var sc = scenes.Last();
                if (sc.Value.GetTransDraw() && scenes.Count > 1)
                {
                    int s = scenes.Count - 2;
                    for (; s != 0; --s)
                    {
                        if (!scenes[s].Value.GetTransDraw())
                            break;
                    }

                    for (; s < scenes.Count; ++s)
                        scenes[s].Value.Draw(e);
                }
                else
                    sc.Value.Draw(e);
            }
        }

        public void ChangeSceneTo(SceneType type)
        {
            if (HasScene(type))
            {
                foreach (var s in scenes)
                {
                    if (s.Key == type)
                    {
                        Scene scene = s.Value;
                        scenes.Remove(s);
                        scenes.Add(new KeyValuePair<SceneType, Scene>(type, scene));
                        return;
                    }
                }
            }
            else
            {
                CreateScene(type);
            }
        }

        public bool HasScene(SceneType type)
        {
            foreach (var s in scenes)
            {
                if (s.Key == type)
                    return true;
            }

            return false;
        }

        public void Remove(SceneType type)
        {
            if (HasScene(type))
            {
                foreach (var s in scenes)
                {
                    if (s.Key == type)
                    {
                        scenes.Remove(s);
                        s.Value.Destroy();
                        return;
                    }
                }
            }
        }

        public SceneType GetCurrentScene()
        {
            if (scenes.Count != 0)
                return scenes[scenes.Count - 1].Key;
            else
                return SceneType.None;
        }

    }
}
