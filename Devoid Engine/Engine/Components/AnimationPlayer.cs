using DevoidEngine.Engine.Animation;
using DevoidEngine.Engine.Core;
using System.Collections.Generic;

namespace DevoidEngine.Engine.Components
{
    public class AnimationComponent : Component
    {
        public override string Type => nameof(AnimationComponent);

        public List<AnimationPlayer> Players = new();

        public bool Playing = true;

        public void AddPlayer(AnimationPlayer player)
        {
            if (player == null) return;
            Players.Add(player);
        }

        public void RemovePlayer(AnimationPlayer player)
        {
            Players.Remove(player);
        }

        public void ClearPlayers()
        {
            Players.Clear();
        }

        public override void OnUpdate(float dt)
        {
            if (!Playing) return;

            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Update(dt);
                Console.WriteLine("Updating Animation");
            }
        }
    }
}