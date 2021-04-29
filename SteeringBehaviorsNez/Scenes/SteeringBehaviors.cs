using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.ImGuiTools;
using Nez.Sprites;
using SteeringBehaviorsNez.Steering;
using SteeringBehaviorsNez.Steering.Behaviors;
using Random = Nez.Random;

namespace SteeringBehaviorsNez.Scenes
{
    public class SteeringBehaviors : RenderableComponent, IUpdatable
    {
        public override float Width => 1360;
        public override float Height => 768;

        private Texture2D _defaultTexture;
        private Texture2D _obstacleTexture;
        private BitmapFont _defaultFont;

        private PathComponent _pathBuilder;

        private SteeringPhysicalParams _params = SteeringPhysicalParams.Defaults();

        [Inspectable]
        public PathFollowingMode PathFollowingMode = PathFollowingMode.OneWay;

        private int _imGuiMode;

        public SteeringBehaviors(PathComponent pathBuilder)
        {
            _pathBuilder = pathBuilder;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            Core.GetGlobalManager<ImGuiManager>().RegisterDrawCommand(UiLayout);

            _defaultTexture = Core.Content.Load<Texture2D>("steering/YellowArrow");
            _obstacleTexture = Core.Content.Load<Texture2D>("steering/GreenSquare");
            _defaultFont = Core.Content.Load<BitmapFont>("NezDefaultBMFont");


            Debug.DrawTextFromBottom = true;
        }

        void IUpdatable.Update()
        {
            if (Input.LeftMouseButtonPressed)
            {
                AddObstacle(Input.MousePosition);
            }
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            UiLayout();

            batcher.DrawString(_defaultFont, "Press Left Mouse Button to add obstacles\n Right Mouse Button to add path nodes", Vector2.One * 16, Color.Black);
        }

        public void UiLayout()
        {
            ImGui.Begin("Steering Behaviors");

            ImGui.TextWrapped("Choose a steering behavior to add:");

            if (ImGui.Button("DESTROY ALL"))
            {
                var entities = Core.Scene.FindEntitiesWithTag(123);
                foreach (var entity in entities)
                {
                    entity.Destroy();
                }
            }
            if (ImGui.Button("DELETE OBSTACLES"))
            {
                var entities = Core.Scene.FindEntitiesWithTag(111);
                foreach (var entity in entities)
                {
                    entity.Destroy();
                }
            }
            if (ImGui.Button("DELETE PATH NODES"))
            {
                _pathBuilder.Path.Clear();
            }

            if (ImGui.CollapsingHeader("Default Parameters"))
            {
                var mass = _params.Mass.Value;
                var maxForce = _params.MaxForce.Value;
                var maxVelocity = _params.MaxVelocity.Value;

                ImGui.DragFloat("Mass", ref mass, 0.1f);
                ImGui.DragFloat("Max Force", ref maxForce, 0.1f);
                ImGui.DragFloat("Max Velocity", ref maxVelocity, 0.1f);

                if (Math.Abs(mass - _params.Mass.Value) > 0.001f)
                    _params.Mass = mass;
                if (Math.Abs(maxForce - _params.MaxForce.Value) > 0.001f)
                    _params.MaxForce = maxForce;
                if (Math.Abs(maxVelocity - _params.MaxVelocity.Value) > 0.001f)
                    _params.MaxVelocity = maxVelocity;
            }
            //ImGui.Separator();

            if (ImGui.CollapsingHeader("Seek##1"))
            {
                ImGui.TextWrapped("Seek: the most simple steering behavior, only seeking the target");
                if (ImGui.Button("Add Seek"))
                {
                    AddSeek();
                }
            }
            //ImGui.Separator();

            if (ImGui.CollapsingHeader("Flee##1"))
            {
                ImGui.TextWrapped("Flee: the counter of Seek - flee away from the target");
                if (ImGui.Button("Add Flee"))
                {
                    AddFlee();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Arrival##1"))
            {
                ImGui.TextWrapped(
                    "Arrival: a seeker with arrival radius which the more slows down the nearer to the target");
                if (ImGui.Button("Add Arrival"))
                {
                    AddArrival();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Wander##1"))
            {
                ImGui.TextWrapped("Wander: just wanders around in random manner");
                if (ImGui.Button("Wander x1"))
                {
                    AddWander();
                }

                ImGui.SameLine();
                if (ImGui.Button("Wander x10"))
                {
                    AddWander(10);
                }

                ImGui.SameLine();
                if (ImGui.Button("Wander x20"))
                {
                    AddWander(20);
                }

                ImGui.SameLine();
                if (ImGui.Button("Wander x100"))
                {
                    AddWander(100);
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Pursuit##1"))
            {
                ImGui.TextWrapped("Pursuit: hunting the target down with kind of prediction");
                if (ImGui.Button("Pursuit"))
                {
                    AddPursuit();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Evade##1"))
            {
                ImGui.TextWrapped(
                    "Evade: the opposite of pursuit - just like Flee - getting away from the target with some kind of prediction");
                if (ImGui.Button("Evade"))
                {
                    AddEvade();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Collision Avoidance##1"))
            {
                ImGui.TextWrapped("Collision Avoidance: trying to avoid obstacles");
                ImGui.TextWrapped("Pro tip: use left mouse button to place obstacles");
                if (ImGui.Button("Collision Avoidance"))
                {
                    AddCollisionAvoidance();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Path Following##1"))
            {
                ImGui.TextWrapped("Path Following: just as simple as that, following the path with 'natural' movement");
                if (ImGui.Button("Path Following"))
                {
                    AddPathFollowing();
                }
                int newMode = _imGuiMode;
                ImGui.Text("Path Following Mode:");
                ImGui.ListBox("", ref newMode, new[] {"One Way", "Circular", "Patrol"}, 3);

                if (newMode != _imGuiMode)
                {
                    _imGuiMode = newMode;
                    PathFollowingMode = (PathFollowingMode)_imGuiMode;
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Leader Following##1"))
            {
                ImGui.TextWrapped(
                    "Leader Following: a composition of other steering forces, all arranged to make a group of characters follow a specific character");
                if (ImGui.Button("Leader Following"))
                {
                    AddLeaderFollowing();
                }
            }
            ImGui.Separator();

            if (ImGui.CollapsingHeader("Queue##1"))
            {
                ImGui.TextWrapped(
                    "Queue: process of standing in line, forming a row of characters that are patiently waiting to arrive somewhere");
                if (ImGui.Button("Queue"))
                {
                    AddQueue();
                }
            }

            ImGui.End();
        }

        private void AddSeek()
        {
            Debug.Log("Adding Seek");

            var entity = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddBehavior(new Seek(), (args) =>
                    {
                        var length = (args.Target.Position  -
                                      (args.Entity.Position + args.Entity.Velocity * args.Entity.Mass)).Length();
                        return length > 5f;
                    })
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();
            
            Core.Scene.AddEntity(entity);
        }

        private void AddFlee()
        {
            Debug.Log("Adding Flee");

            var entity = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddBehavior(new Flee(), (args) => (args.Target.Position - args.Entity.Position).Length() <= 400f)
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(entity);
        }
        
        private void AddArrival()
        {
            Debug.Log("Adding Arrival");
            
            var entity = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddBehavior(new Arrival(128f), args => (args.Target.Position - args.Entity.Position).Length() > 5f)
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(entity);
        }
        
        private void AddWander(int count = 1)
        {
            Debug.Log("Adding Wander x" + count);

            for (int i = 0; i < count; i++)
            {
                var entity = new SteeringBuilder(new Vector2(500, 400) + Random.RNG.NextVector2(-100, 100))
                    .SetPhysicalParams(_params)
                    .AddBehavior(new Wander(6, 8, 0, 0.5f))
                    .UseDefaultRenderer(_defaultTexture)
                    .AddCollider(12f)
                    .Build();

                Core.Scene.AddEntity(entity);    
            }
            
        }
        
        private void AddPursuit()
        {
            Debug.Log("Adding Pursuit");

            var seek = new SteeringBuilder(new Vector2(128, 128))
                .SetPhysicalParams(_params)
                .AddBehavior(new Seek())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            SteeringEntity pursuiter = null;

            var evade = new SteeringBuilder(new Vector2(128, 128), pursuiter)
                .SetPhysicalParams(_params)
                .AddBehavior(new Evade())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            pursuiter = new SteeringBuilder(new Vector2(512, 512), seek)
                .SetPhysicalParams(_params)
                .AddBehavior(new Pursuit())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(seek);
            // Core.Scene.AddEntity(evade);
            Core.Scene.AddEntity(pursuiter);
        }
        
        private void AddEvade()
        {
            Debug.Log("Adding Evade");

            var seek = new SteeringBuilder(new Vector2(128, 128))
                .SetPhysicalParams(_params)
                .AddBehavior(new Seek())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();


            var entity = new SteeringBuilder(new Vector2(512, 512), seek)
                .SetPhysicalParams(_params)
                .AddBehavior(new Evade())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(seek);
            Core.Scene.AddEntity(entity);
        }

        private void AddObstacle(Vector2 position)
        {
            var ent = Core.Scene.CreateEntity("obstacle-" + position, position);
            ent.Tag = 111;

            ent.AddComponent(new SpriteRenderer(_obstacleTexture))
                .AddComponent(new BoxCollider(-32, -32, 64, 64))
                .PhysicsLayer = 2;
        }
        
        private void AddCollisionAvoidance()
        {
            Debug.Log("Adding Collision Avoidance");

            var entity = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddCollider(12f)
                .AddBehavior(new Seek())
                .AddBehavior(new CollisionAvoidance(300f, 800f))
                .UseDefaultRenderer(_defaultTexture)
                .Build();

            Core.Scene.AddEntity(entity);
        }
        
        private void AddPathFollowing()
        {
            Debug.Log("Adding Path Following");

            var entity = new SteeringBuilder(new Vector2(512, 512), _pathBuilder)
                .SetPhysicalParams(_params)
                .AddBehavior(new PathFollowing(new Seek(), PathFollowingMode))
                .AddBehavior(new CollisionAvoidance(100f, 800f))
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(entity);
        }
        private void AddLeaderFollowing()
        {
            Debug.Log("Adding Leader Following");

            // The Leader itself
            var leader = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddBehavior(new Arrival(64f))
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();


            var leaderFollowingBehavior1 = new LeaderFollowing(leader, 64f, 64f, 30);
            var leaderFollowingBehavior2 = new LeaderFollowing(leader, 64f, 64f, 30);
            var leaderFollowingBehavior3 = new LeaderFollowing(leader, 64f, 64f, 30);

            // If IsOnLeaderSight => Evade(leader) -> Arrive -> Separation
            var entity = new SteeringBuilder(new Vector2(128, 128), leader)
                .SetPhysicalParams(_params)
                .AddBehavior(leaderFollowingBehavior1)
                .AddBehavior(new Evade(), args => IsOnLeaderSight(leader, args.Entity,
                        GetLeaderAhead(leader, leaderFollowingBehavior1.LeaderBehindDist),
                        leaderFollowingBehavior1.LeaderSightRadius))
                .AddBehavior(new Separation(CheckNearestFunc, 50f, 2f) {IsAdditive = true})
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            var entity2 = new SteeringBuilder(new Vector2(256, 128), leader)
                .SetPhysicalParams(_params)
                .AddBehavior(leaderFollowingBehavior2)
                .AddBehavior(new Evade(), args => IsOnLeaderSight(leader, args.Entity, GetLeaderAhead(leader, leaderFollowingBehavior2.LeaderBehindDist), leaderFollowingBehavior2.LeaderSightRadius))
                .AddBehavior(new Separation(CheckNearestFunc, 50f, 2f) { IsAdditive = true })
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            var entity3 = new SteeringBuilder(new Vector2(256, 256), leader)
                .SetPhysicalParams(_params)
                .AddBehavior(leaderFollowingBehavior3)
                .AddBehavior(new Evade(), args => IsOnLeaderSight(leader, args.Entity, GetLeaderAhead(leader, leaderFollowingBehavior3.LeaderBehindDist), leaderFollowingBehavior3.LeaderSightRadius))
                .AddBehavior(new Separation(CheckNearestFunc, 50f, 2f) { IsAdditive = true })
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(leader);
            Core.Scene.AddEntity(entity);
            Core.Scene.AddEntity(entity2);
            Core.Scene.AddEntity(entity3);
        }

        private Vector2 GetLeaderAhead(ISteeringEntity leader, float leaderBehindDist)
        {
            var dv = leader.Velocity;
            dv.Normalize();
            dv *= leaderBehindDist;
            return leader.Position + dv;
        }

        private bool IsOnLeaderSight(ISteeringEntity leader, ISteeringEntity entity, Vector2 leaderAhead, float leaderSightRadius)
        {
            return Vector2.Distance(leaderAhead, entity.Position) <= leaderSightRadius ||
                   Vector2.Distance(leader.Position, entity.Position) <= leaderSightRadius;
        }

        private IEnumerable<ISteeringEntity> CheckNearestFunc(Separation arg)
        {
            var entities = Core.Scene.Entities.EntitiesWithTag(123);
            var self = arg.Entity;

            foreach (var entity in entities)
            {
                if (entity != self && Vector2.Distance(entity.Position, self.Position) <= arg.SeparationRadius)
                    yield return entity as ISteeringEntity;
            }
        }

        private void AddQueue()
        {
            Debug.Log("Adding Queue");
            
            var entity = new SteeringBuilder(new Vector2(512, 512))
                .SetPhysicalParams(_params)
                .AddBehavior(new Queue())
                .UseDefaultRenderer(_defaultTexture)
                .AddCollider(12f)
                .Build();

            Core.Scene.AddEntity(entity);
        }
        
    }
}