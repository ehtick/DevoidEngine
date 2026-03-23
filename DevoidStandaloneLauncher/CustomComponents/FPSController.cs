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
        public float MouseSensitivity = 1.2f;
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

        private Vector2 mouseDelta;

        public event Action OnDeath;

        public float InteractionDistance = 20f;

        public CanvasComponent UICanvas;
        //public LabelNode UIInteractionText;

        private PortalCubeComponent heldCube;




        public Transform GunTransform;   // assign from gun object

        public float RecoilAngle = 25f;
        public float RecoilSpeed = 14f;
        public float RecoilReturnSpeed = 10f;

        public float ReloadSpinSpeed = 1500f; // degrees per second

        private float recoilCurrent = 0f;
        private float recoilTarget = 0f;
        private float reloadSpin = 0f;

        private Quaternion gunBaseRotation;

        public Vector3 PlayerRecoilForce = new Vector3(0f, 0f, 0f);


        // ===============================
        // Setup
        // ===============================

        private Vector2 lastMouseDelta;
        private Vector2 mouseVelocity;
        private Vector2 frameMouseDelta;
        private Vector2 fixedMouseDelta;

        private Vector2 moveInput;
        private bool jumpRequested;
        private bool shootRequested;
        private bool reloadRequested;

        public Vector2 GetMouseVelocity() => mouseVelocity;
        public Transform GetCameraPivot() => cameraPivot;
        public Vector2 GetMouseDelta() => lastMouseDelta;
        public Vector3 GetLinearVelocity()
        {
            return rb != null ? rb.LinearVelocity : Vector3.Zero;
        }

        public void SetGunTransform(Transform gun)
        {
            GunTransform = gun;

            // store the gun's original orientation
            gunBaseRotation = gun.LocalRotation;
        }

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

            //UIInteractionText = new LabelNode("T to interact", FontLibrary.LoadFont("Engine/Content/Fonts/JetBrainsMono-Regular.ttf", 32), 21)
            //{

            //};
            //ROOT.Add(UIInteractionText);
            UICanvas.Canvas.Add(ROOT);

            projectileMesh = new Mesh();
            projectileMesh.SetVertices(Primitives.GetCubeVertex());

            currentAmmo = MaxAmmo;

            rb = gameObject.GetComponent<RigidBodyComponent>();
            if (rb == null)
                return;

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

        // ===============================
        // Update
        // ===============================

        float totalTime = 0f;

        public override void OnUpdate(float dt)
        {
            // === SAMPLE INPUT (ONCE PER FRAME) ===

            frameMouseDelta = new Vector2(
                InputSystem.Input.GetAction("LookX"),
                InputSystem.Input.GetAction("LookY")
            );

            moveInput = new Vector2(
                InputSystem.Input.GetAction("Left"),
                InputSystem.Input.GetAction("Forward") - InputSystem.Input.GetAction("Backward")
            );

            if (InputSystem.Input.GetActionDown("Jump"))
                jumpRequested = true;

            if (InputSystem.Input.GetActionDown("Shoot"))
                shootRequested = true;

            if (Input.GetKey(Keys.R))
                reloadRequested = true;

            HandleRotation(dt);
        }

        private void UpdateGunAnimation(float dt)
        {
            if (GunTransform == null)
                return;

            // Smooth recoil motion
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

            // Reload spinning
            if (isReloading)
                reloadSpin += ReloadSpinSpeed * dt;
            else
                reloadSpin = 0f;

            // Gun local axes from base rotation
            Vector3 gunRight = Vector3.Normalize(
                Vector3.Transform(Vector3.UnitX, gunBaseRotation)
            );

            Vector3 gunForward = Vector3.Normalize(
                Vector3.Transform(Vector3.UnitZ, gunBaseRotation)
            );

            Quaternion recoilRot =
                Quaternion.CreateFromAxisAngle(
                    gunForward,
                    MathHelper.DegToRad(-recoilCurrent)
                );

            Quaternion reloadRot =
                Quaternion.CreateFromAxisAngle(
                    gunForward,
                    MathHelper.DegToRad(-reloadSpin)
                );

            GunTransform.LocalRotation =
                gunBaseRotation *
                recoilRot *
                reloadRot;
        }

        private void TryInteract()
        {
            if (cameraPivot == null)
                return;

            Vector3 origin = cameraPivot.Position;

            Vector3 forward =
                Vector3.Normalize(
                    Vector3.Transform(Vector3.UnitZ, cameraPivot.Rotation)
                );

            if (InputSystem.Input.GetActionDown("Interact"))
            {
                // If holding cube -> drop
                if (heldCube != null)
                {
                    heldCube.Drop();
                    heldCube = null;
                    return;
                }

                // Try pickup
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
        }

        // ===============================
        // Shooting
        // ===============================

        private void HandleShooting(float fixedDt)
        {
            fireTimer -= fixedDt;

            if (!shootRequested)
                return;

            if (fireTimer > 0f || isReloading)
                return;

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

            Vector3 forward =
                Vector3.Normalize(
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

            // recoil impulse
            if (rb != null)
            {
                Vector3 recoilImpulse =
                    Vector3.Transform(PlayerRecoilForce, rotation);

                Vector3 velocity = rb.LinearVelocity;

                velocity.X += recoilImpulse.X;
                velocity.Z += recoilImpulse.Z;
                velocity.Y = MathF.Max(velocity.Y, recoilImpulse.Y);

                rb.LinearVelocity = velocity;
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
            if (!isReloading)
                return;

            reloadTimer -= dt;

            if (reloadTimer <= 0f)
            {
                isReloading = false;
                currentAmmo = MaxAmmo;
            }
        }

        // ===============================
        // Health + Damage
        // ===============================

        private void HandleDamage(float dt)
        {
            if (!inDamageZone)
                return;

            damageTimer += dt;

            if (!delayPassed && damageTimer >= DamageDelay)
                delayPassed = true;

            if (delayPassed)
                ApplyDamage(DamagePerSecond * dt);
        }

        private void HandleRegen(float dt)
        {
            if (inDamageZone)
                return;

            if (TimeSinceLastDamage() < RegenDelay)
                return;

            if (Health < MaxHealth)
            {
                Health += RegenPerSecond * dt;
                Health = Math.Min(Health, MaxHealth);
            }
        }

        private float TimeSinceLastDamage()
        {
            return (float)(totalTime - lastDamageTime);
        }

        private void ApplyDamage(float amount)
        {
            Health -= amount;
            lastDamageTime = (float)totalTime;

            if (Health <= 0f)
            {
                Health = 0f;
                OnDeath?.Invoke();
            }
        }

        // ===============================
        // Physics
        // ===============================

        public override void OnFixedUpdate(float fixedDt)
        {
            if (rb == null) return;

            // === LATCH INPUT INTO FIXED STEP ===
            fixedMouseDelta = frameMouseDelta;

            HandleMovement(fixedDt);
            HandleShooting(fixedDt);
            HandleReload(fixedDt);
            HandleDamage(fixedDt);
            HandleRegen(fixedDt);

            // reset one-shot inputs
            jumpRequested = false;
            shootRequested = false;
            reloadRequested = false;
        }

        private void HandleRotation(float fixedDt)
        {
            yaw += fixedMouseDelta.X * MouseSensitivity * fixedDt * 100;
            pitch -= fixedMouseDelta.Y * MouseSensitivity * fixedDt * 100;

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

        private void HandleMovement(float fixedDt)
        {
            Vector3 forward = Vector3.Transform(Vector3.UnitZ, rb.Rotation);
            Vector3 right = Vector3.Transform(-Vector3.UnitX, rb.Rotation);

            forward.Y = 0;
            right.Y = 0;

            forward = Vector3.Normalize(forward);
            right = Vector3.Normalize(right);

            Vector3 desiredMove =
                forward * moveInput.Y +
                right * moveInput.X;

            if (desiredMove.LengthSquared() > 1f)
                desiredMove = Vector3.Normalize(desiredMove);

            Vector3 currentVelocity = rb.LinearVelocity;

            Vector3 horizontalVelocity =
                new Vector3(currentVelocity.X, 0, currentVelocity.Z);

            Vector3 targetVelocity = desiredMove * MoveSpeed;

            bool grounded = IsGrounded();
            float control = grounded ? 1f : AirControl;

            horizontalVelocity = Vector3.Lerp(
                horizontalVelocity,
                targetVelocity,
                Acceleration * control * fixedDt
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
        // Collision Events
        // ===============================

        public void OnCollisionEnter(GameObject other)
        {
            if (other.Name == "Ground") return;
            //Console.WriteLine("ENTER");

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