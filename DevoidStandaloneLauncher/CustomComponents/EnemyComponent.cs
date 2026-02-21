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

        public override void OnFixedUpdate(float fixedDt)
        {
            if (player == null) return;

            var rb = gameObject.GetComponent<RigidBodyComponent>();
            if (rb == null) return;

            Vector3 direction = player.Position - gameObject.transform.Position;
            direction.Y = 0;

            if (direction.LengthSquared() > 0.1f)
            {
                direction = Vector3.Normalize(direction);

                Vector3 velocity = rb.LinearVelocity;
                velocity.X = direction.X * MoveSpeed;
                velocity.Z = direction.Z * MoveSpeed;

                rb.LinearVelocity = velocity;
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