using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.UI.Nodes;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class FPSController : Component, ICollisionListener
    {
        public override string Type => nameof(FPSController);

        // ===============================
        // Movement
        // ===============================

        public float MoveSpeed = 6f;
        public float Acceleration = 20f;
        public float AirControl = 0.3f;
        public float JumpForce = 100f;
        public float MouseSensitivity = 0.12f;
        public float MinPitch = -89f;
        public float MaxPitch = 89f;
        public float GroundCheckDistance = 1f;

        // ===============================
        // Shooting / Ammo
        // ===============================

        public float FireRate = 0.5f;
        public float ProjectileSpeed = 50f;
        public float ProjectileMass = 10f;
        public Vector3 ProjectileScale = new Vector3(0.2f);

        public int MaxAmmo = 6;
        public float ReloadTime = 1f;

        public int currentAmmo;
        public bool isReloading = false;
        private float reloadTimer = 0f;
        private float fireTimer = 0f;

        private Mesh projectileMesh;

        // ===============================
        // Health + Regen
        // ===============================

        public float MaxHealth = 100f;
        public float Health = 100f;

        public float EnterDamage = 15f;
        public float DamagePerSecond = 25f;
        public float DamageDelay = 1f;

        public float RegenPerSecond = 10f;
        public float RegenDelay = 3f;

        private float lastDamageTime = 0f;
        private float damageTimer = 0f;
        private bool inDamageZone = false;
        private bool delayPassed = false;

        // ===============================
        // Internal
        // ===============================

        private RigidBodyComponent rb;
        private Transform cameraPivot;

        private float yaw;
        private float pitch;

        public event Action OnDeath;

        public float InteractionDistance = 20f;

        public CanvasComponent UICanvas;
        private PortalCubeComponent heldCube;

        public Transform GunTransform;

        public float RecoilAngle = 25f;
        public float RecoilSpeed = 14f;
        public float RecoilReturnSpeed = 10f;
        public float ReloadSpinSpeed = 1500f;

        private float recoilCurrent = 0f;
        private float recoilTarget = 0f;
        private float reloadSpin = 0f;

        private Quaternion gunBaseRotation;

        public Vector3 PlayerRecoilForce = new Vector3(0f, 0f, 0f);

        // ===============================
        // Input State (Fixed Only)
        // ===============================

        private Vector2 moveInput;
        private Vector2 mouseDelta;
        private bool jumpRequested;
        private bool shootRequested;
        private bool reloadRequested;
        private bool interactRequested;

        float totalTime = 0f;

        // ===============================
        // Setup
        // ===============================

        public override void OnStart()
        {
            GameObject canvas = gameObject.Scene.AddGameObject("InteractionObject");
            UICanvas = canvas.AddComponent<CanvasComponent>();

            FlexboxNode ROOT = new FlexboxNode()
            {
                Size = new Vector2(200, 600),
                Justify = JustifyContent.Center,
                Align = AlignItems.End
            };

            UICanvas.Canvas.Add(ROOT);

            projectileMesh = new Mesh();
            projectileMesh.SetVertices(Primitives.GetCubeVertex());

            currentAmmo = MaxAmmo;

            rb = gameObject.GetComponent<RigidBodyComponent>();
            if (rb == null) return;

            rb.FreezeRotationX = true;
            rb.FreezeRotationY = true;
            rb.FreezeRotationZ = true;

            yaw = MathHelper.RadToDeg(
                MathF.Atan2(
                    gameObject.Transform.Forward.X,
                    gameObject.Transform.Forward.Z
                )
            );
        }

        public void SetCameraPivot(Transform pivot)
        {
            cameraPivot = pivot;
        }

        public Transform GetCameraPivot() => cameraPivot;

        public void SetGunTransform(Transform gun)
        {
            GunTransform = gun;
            gunBaseRotation = gun.LocalRotation;
        }

        public Vector3 GetLinearVelocity()
        {
            return rb != null ? rb.LinearVelocity : Vector3.Zero;
        }

        // ===============================
        // Fixed Update ONLY
        // ===============================

        public override void OnFixedUpdate(float dt)
        {
            if (rb == null) return;

            totalTime += dt;

            SampleInput();

            HandleRotation(dt);
            HandleMovement(dt);
            HandleShooting(dt);
            HandleReload(dt);
            HandleDamage(dt);
            HandleRegen(dt);
            TryInteract();
            UpdateGunAnimation(dt);

            // reset one-shot inputs
            jumpRequested = false;
            shootRequested = false;
            reloadRequested = false;
            interactRequested = false;
        }

        // ===============================
        // Input
        // ===============================

        private void SampleInput()
        {
            mouseDelta = new Vector2(
                InputSystem.Input.GetAction("LookX"),
                InputSystem.Input.GetAction("LookY")
            );

            moveInput = new Vector2(
                InputSystem.Input.GetAction("Left"),
                InputSystem.Input.GetAction("Forward") - InputSystem.Input.GetAction("Backward")
            );

            jumpRequested = InputSystem.Input.GetActionDown("Jump");
            shootRequested = InputSystem.Input.GetActionDown("Shoot");
            interactRequested = InputSystem.Input.GetActionDown("Interact");
            reloadRequested = Input.GetKey(Keys.R);
        }

        // ===============================
        // Rotation
        // ===============================

        private void HandleRotation(float dt)
        {
            yaw += mouseDelta.X * MouseSensitivity;
            pitch -= mouseDelta.Y * MouseSensitivity;

            pitch = Math.Clamp(pitch, MinPitch, MaxPitch);

            rb.Rotation =
                Quaternion.CreateFromAxisAngle(
                    -Vector3.UnitY,
                    MathHelper.DegToRad(yaw)
                );

            if (cameraPivot != null)
            {
                cameraPivot.LocalRotation =
                    Quaternion.CreateFromAxisAngle(
                        -Vector3.UnitX,
                        MathHelper.DegToRad(pitch)
                    );
            }
        }

        // ===============================
        // Movement
        // ===============================

        private void HandleMovement(float dt)
        {
            Vector3 forward = Vector3.Transform(Vector3.UnitZ, rb.Rotation);
            Vector3 right = Vector3.Transform(-Vector3.UnitX, rb.Rotation);

            forward.Y = 0;
            right.Y = 0;

            forward = Vector3.Normalize(forward);
            right = Vector3.Normalize(right);

            Vector3 desiredMove = forward * moveInput.Y + right * moveInput.X;

            if (desiredMove.LengthSquared() > 1f)
                desiredMove = Vector3.Normalize(desiredMove);

            Vector3 currentVelocity = rb.LinearVelocity;
            Vector3 horizontalVelocity = new Vector3(currentVelocity.X, 0, currentVelocity.Z);
            Vector3 targetVelocity = desiredMove * MoveSpeed;

            bool grounded = IsGrounded();
            float control = grounded ? 1f : AirControl;

            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                Acceleration * control * dt
            );

            rb.LinearVelocity = new Vector3(
                horizontalVelocity.X,
                currentVelocity.Y,
                horizontalVelocity.Z
            );

            if (jumpRequested && grounded)
            {
                rb.LinearVelocity = new Vector3(
                    rb.LinearVelocity.X,
                    JumpForce,
                    rb.LinearVelocity.Z
                );
            }
        }

        private bool IsGrounded()
        {
            Vector3 origin = gameObject.Transform.Position;

            return gameObject.Scene.Physics.Raycast(
                new Ray(origin - Vector3.UnitY, -Vector3.UnitY),
                GroundCheckDistance,
                out RaycastHit hit
            );
        }

        // ===============================
        // Shooting / Reload
        // ===============================

        private void HandleShooting(float dt)
        {
            fireTimer -= dt;

            if (!shootRequested) return;
            if (fireTimer > 0f || isReloading) return;

            if (currentAmmo <= 0)
            {
                StartReload();
                return;
            }

            fireTimer = FireRate;
            currentAmmo--;

            recoilTarget += RecoilAngle;

            Vector3 spawnPosition = cameraPivot != null
                ? cameraPivot.Position
                : gameObject.Transform.Position;

            Quaternion rotation = cameraPivot != null
                ? cameraPivot.Rotation
                : rb.Rotation;

            Vector3 forward = Vector3.Normalize(
                Vector3.Transform(Vector3.UnitZ, rotation)
            );

            BulletComponent.Spawn(
                gameObject.Scene,
                projectileMesh,
                spawnPosition + forward * 2f,
                rotation,
                ProjectileScale,
                ProjectileSpeed,
                ProjectileMass
            );

            if (rb != null)
            {
                Vector3 recoilImpulse =
                    Vector3.Transform(PlayerRecoilForce, rotation);

                var v = rb.LinearVelocity;

                v.X += recoilImpulse.X;
                v.Z += recoilImpulse.Z;
                v.Y = MathF.Max(v.Y, recoilImpulse.Y);

                rb.LinearVelocity = v;
            }
        }

        private void StartReload()
        {
            if (isReloading || currentAmmo == MaxAmmo)
                return;

            isReloading = true;
            reloadTimer = ReloadTime;
        }

        private void HandleReload(float dt)
        {
            if (!isReloading) return;

            reloadTimer -= dt;

            if (reloadTimer <= 0f)
            {
                isReloading = false;
                currentAmmo = MaxAmmo;
            }
        }

        // ===============================
        // Interaction
        // ===============================

        private void TryInteract()
        {
            if (!interactRequested || cameraPivot == null)
                return;

            Vector3 origin = cameraPivot.Position;

            Vector3 forward = Vector3.Normalize(
                Vector3.Transform(Vector3.UnitZ, cameraPivot.Rotation)
            );

            if (heldCube != null)
            {
                heldCube.Drop();
                heldCube = null;
                return;
            }

            if (gameObject.Scene.Physics.Raycast(
                new Ray(origin + forward, forward),
                InteractionDistance,
                out RaycastHit hit))
            {
                var cube = hit.HitObject?.GetComponent<PortalCubeComponent>();

                if (cube != null && !cube.IsHeld)
                {
                    heldCube = cube;
                    cube.PickUp(this);
                }
            }
        }

        // ===============================
        // Gun Animation
        // ===============================

        private void UpdateGunAnimation(float dt)
        {
            if (GunTransform == null) return;

            recoilCurrent = MathHelper.Lerp(
                recoilCurrent,
                recoilTarget,
                RecoilSpeed * dt
            );

            recoilTarget = MathHelper.Lerp(
                recoilTarget,
                0f,
                RecoilReturnSpeed * dt
            );

            if (isReloading)
                reloadSpin += ReloadSpinSpeed * dt;
            else
                reloadSpin = 0f;

            Vector3 forward = Vector3.Normalize(
                Vector3.Transform(Vector3.UnitZ, gunBaseRotation)
            );

            Quaternion recoilRot =
                Quaternion.CreateFromAxisAngle(forward, MathHelper.DegToRad(-recoilCurrent));

            Quaternion reloadRot =
                Quaternion.CreateFromAxisAngle(forward, MathHelper.DegToRad(-reloadSpin));

            GunTransform.LocalRotation =
                gunBaseRotation *
                recoilRot *
                reloadRot;
        }

        // ===============================
        // Health
        // ===============================

        private void HandleDamage(float dt)
        {
            if (!inDamageZone) return;

            damageTimer += dt;

            if (!delayPassed && damageTimer >= DamageDelay)
                delayPassed = true;

            if (delayPassed)
                ApplyDamage(DamagePerSecond * dt);
        }

        private void HandleRegen(float dt)
        {
            if (inDamageZone) return;

            if ((totalTime - lastDamageTime) < RegenDelay)
                return;

            if (Health < MaxHealth)
            {
                Health += RegenPerSecond * dt;
                Health = Math.Min(Health, MaxHealth);
            }
        }

        private void ApplyDamage(float amount)
        {
            Health -= amount;
            lastDamageTime = totalTime;

            if (Health <= 0f)
            {
                Health = 0f;
                OnDeath?.Invoke();
            }
        }

        // ===============================
        // Collision
        // ===============================

        public void OnCollisionEnter(GameObject other)
        {
            if (other.Name == "Ground") return;

            inDamageZone = true;
            damageTimer = 0f;
            delayPassed = false;

            ApplyDamage(EnterDamage);
        }

        public void OnCollisionStay(GameObject other) { }

        public void OnCollisionExit(GameObject other)
        {
            if (other.Name == "Ground") return;

            inDamageZone = false;
            damageTimer = 0f;
            delayPassed = false;
        }
    }
}