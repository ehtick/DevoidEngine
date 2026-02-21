using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class Enemy : Component, ICollisionListener
    {
        public override string Type => nameof(Enemy);

        // ===============================
        // Health
        // ===============================

        public float MaxHealth = 100f;
        private float currentHealth;

        // ===============================
        // Optional simple movement
        // ===============================

        public float MoveSpeed = 3f;
        private Transform player;

        public override void OnStart()
        {
            currentHealth = MaxHealth;

            var playerObject = gameObject.Scene.GetGameObject("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        public override void OnUpdate(float dt)
        {
            if (player == null) return;

            // Simple chase movement
            Vector3 direction = player.Position - gameObject.transform.Position;
            direction.Y = 0f;

            if (direction.LengthSquared() > 0.01f)
            {
                direction = Vector3.Normalize(direction);
                gameObject.transform.Position += direction * MoveSpeed * dt;
            }
        }

        // ===============================
        // Collision Handling
        // ===============================

        public void OnCollisionEnter(GameObject other)
        {
            Console.WriteLine("Name: " + other.Name);
            if (other.Name == "Projectile")
            {
                TakeDamage(100);

                // Destroy the projectile
                gameObject.OnDestroy();
            }
        }

        public void OnCollisionStay(GameObject other) { }

        public void OnCollisionExit(GameObject other) { }

        // ===============================
        // Damage Logic
        // ===============================

        private void TakeDamage(float amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            gameObject.OnDestroy();
        }
    }
}