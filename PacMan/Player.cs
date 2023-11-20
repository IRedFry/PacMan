using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    [Serializable]
    class Player : Character
    {

        private int score, lives;
        [NonSerialized]
        System.Diagnostics.Stopwatch predTimer = new System.Diagnostics.Stopwatch();
        private long innerTimer;
        private int invincibleCounter;
        public Player() : base()
        {
            innerTimer = 0;
            invincibleCounter = 0;
            score = 0;
            lives = 1;
        }

        public override void OnSerialization()
        {
            predTimer = new System.Diagnostics.Stopwatch();
        }

        public void SetDir(Direction di)
        {
            dirOld = dir;
            dirPred = dir = di;
        }
        public void ChangeScore(int s)
        {
            score += s;
        }

        public void ChangeLives(int l)
        {
            lives += l;
        }

        public int GetScore()
        {
            return score;
        }
        public int GetLives()
        {
            return lives;
        }
        public override void Update(long dT)
        {
            if (invincibleCounter < 16)
            {
                innerTimer += dT;

                if (innerTimer >= 100.0f)
                {
                    if (sprite.GetOpacity() == 255)
                        sprite.SetOppacity(150);
                    else
                        sprite.SetOppacity(255);

                    innerTimer = 0;
                    invincibleCounter++;
                }
            }

            if (predTimer.IsRunning)
            {
                if (predTimer.ElapsedMilliseconds >= 1000.0f)
                {
                    dirPred = dir;
                    predTimer.Reset();
                }
            }

            if (CheckDirection(dirPred))
            {
                Go(dirPred, speed * (float)dT / 1000.0f);
                dir = dirOld = dirPred;
            }
            else
            {
                if (CheckDirection(dir))
                {
                    Go(dir, speed * (float)dT / 1000.0f);
                }
                else if (CheckDirection(dirOld))
                {
                    dir = dirOld;
                    Go(dir, speed * (float)dT / 1000.0f);
                }
            }

            UpdateAABB();

            if (Map.GetInstance().GetStar(position))
            {
                Map.GetInstance().EatStar(position);
                ChangeScore(10);

                Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.ScoreChanged, score);
            }
        }

        public override void ReactOnCollision(GameEntity collider)
        {
            if (collider.GetName() == "Blinky" || collider.GetName() == "Pinky" || collider.GetName() == "Inky" || collider.GetName() == "Clyde")
            {
                if (!EntityManager.GetInstance().GetGameState())
                {
                    if (invincibleCounter > 15)
                    {
                        ChangeLives(-1);

                        if (lives <= 0)
                        {
                            Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.MapComplete, score);
                            Game.GetInstance().AddEvent(SceneType.GameOver, EventType.MapComplete, score);
                        }

                        Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.PlayerDied, score);
                        Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.LivesChanged, lives);

                        EntityManager.GetInstance().EraseEntity(0);
                    }
                }
                else
                {
                    score += 200;
                    Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.ScoreChanged, score);
                }
            }
            else if (collider.GetName() == "Cherry")
            {
                score += 1000;
                Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.ScoreChanged, score);

                Game.GetInstance().AddEvent(SceneType.Main_Game, EventType.CherryEaten);

            }
        }

    }
}
